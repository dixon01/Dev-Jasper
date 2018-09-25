// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureResourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Resources
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Defines a service to upload and download <see cref="Resource"/>s with their content from an Azure storage.
    /// </summary>
    public class AzureResourceService : ResourceServiceBase
    {
        private string storageContainerName;

        private CloudStorageAccount storageAccount;

        /// <summary>
        /// Initializes the storage account and creates the container if it doesn't already exist.
        /// </summary>
        /// <param name="containerName">
        /// The name of the container. The default value is 'resources'.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task InitializeStorageAsync(string containerName = "resources")
        {
            this.Logger.Debug("Initialize the container within the storage.");
            this.storageContainerName = containerName;
            this.storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting(
                        Common.Azure.PredefinedAzureItems.Settings.StorageConnectionString));

            var client = this.storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(this.storageContainerName);
            await container.CreateIfNotExistsAsync();

            // ensure that we can publicly access the blobs (but not the container listing)
            await container.SetPermissionsAsync(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            this.Logger.Debug("Container '{0}' in storage initialized.", this.storageContainerName);
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">
        /// The expected hash of the resource file (the name from where it was copied).
        /// </param>
        /// <param name="resourceFile">
        /// The full resource file path.
        /// </param>
        /// <param name="deleteFile">
        /// A flag indicating whether the <see cref="resourceFile"/> should be deleted after being registered.
        /// </param>
        /// <exception cref="UpdateException">
        /// If the resource file doesn't match the given hash.
        /// </exception>
        public override void AddResource(string hash, string resourceFile, bool deleteFile)
        {
            var file = new FileInfo(resourceFile);
            if (!file.Exists)
            {
                throw new FileNotFoundException("Couldn't find resource file", resourceFile);
            }

            var resourceDataService = DependencyResolver.Current.Get<IResourceDataService>();
            var query = ResourceQuery.Create().WithHash(hash);
            var existingEntity = resourceDataService.QueryAsync(query).Result.SingleOrDefault();
            if (existingEntity != null)
            {
                if (deleteFile)
                {
                    File.Delete(resourceFile);
                }

                return;
            }

            var entity = new Resource
            {
                CreatedOn = TimeProvider.Current.UtcNow,
                Hash = hash,
                Length = file.Length,
                MimeType = "application/octet-stream",
                OriginalFilename = file.Name,
            };
            resourceDataService.AddAsync(entity).Wait();

            this.Logger.Trace("Adding resource with hash '{0}' to storage.", hash);
            var blob = this.GetBlockBlobReference(hash);
            string storedHash;
            using (var fileStream = file.OpenRead())
            {
                blob.UploadFromStream(fileStream);
                storedHash = ResourceHash.Create(fileStream);
            }

            if (deleteFile)
            {
                file.Delete();
            }

            if (string.Equals(storedHash, hash))
            {
                return;
            }

            throw new UpdateException("Hash verification failed");
        }

        /// <summary>
        /// Gets a resource for a given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        public override IResource GetResource(string hash)
        {
            if (!this.ContentExists(hash))
            {
                throw new UpdateException("Couldn't find resource " + hash);
            }

            return new ResourceWrapper(hash, this);
        }

        /// <summary>
        /// Moves a resource to the storage and deletes the source file.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task MoveResourceToStorageAsync(string hash, FileInfo resource)
        {
            this.Logger.Trace("Move resource '{0}' to storage", resource.Name);
            using (var stream = resource.OpenRead())
            {
                await this.StoreContentAsync(hash, stream);
            }

            if (resource.Exists)
            {
                resource.Delete();
            }
        }

        /// <summary>
        /// Checks if content exists for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// <c>true</c> if the content exists; <c>false</c> otherwise.
        /// </returns>
        public override bool ContentExists(string hash)
        {
            return this.GetBlockBlobReference(hash).Exists();
        }

        /// <summary>
        /// Deletes the resource with the given <paramref name="hash"/>, if found. If the resource doesn't exist,
        /// nothing will be done.
        /// </summary>
        /// <param name="hash">The hash of the resource to delete.</param>
        /// <exception cref="UpdateException">
        /// Any error occurred while removing the resource from the provider.
        /// </exception>
        public override void DeleteResource(string hash)
        {
            try
            {
                var blob = this.GetBlockBlobReference(hash);
                if (blob.Exists())
                {
                    blob.Delete();
                }
            }
            catch (Exception e)
            {
                throw new UpdateException("Error while trying to delete resource " + hash, e);
            }
        }

        /// <summary>
        /// Tries to get the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>
        /// <c>true</c> if the resource was found and returned as the out parameter <paramref name="resource"/>;
        /// otherwise, <c>false</c> (and the out parameter doesn't have a meaningful value).
        /// </returns>
        public override bool TryGetResource(string hash, out IResource resource)
        {
            try
            {
                resource = this.GetResource(hash);
                return true;
            }
            catch (UpdateException exception)
            {
                this.Logger.Debug("Error while trying to get the resource Hash={0}, {1}", hash, exception);
                resource = null;
                return false;
            }
        }

        /// <summary>
        /// Asynchronously stores the content.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task StoreContentAsync(string hash, Stream stream)
        {
            if (this.ContentExists(hash))
            {
                return;
            }

            var blob = this.GetBlockBlobReference(hash);
            await blob.UploadFromStreamAsync(stream);
        }

        /// <summary>
        /// Gets the content for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// The content <see cref="Stream"/>.
        /// </returns>
        protected override Stream GetContent(string hash)
        {
            var blob = this.GetBlockBlobReference(hash);
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        /// <summary>
        /// Gets the path where the content is stored.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// The <see cref="FileInfo"/> of the content.
        /// </returns>
        protected override FileInfo GetPath(string hash)
        {
            throw new NotImplementedException();
        }

        private CloudBlockBlob GetBlockBlobReference(string hash)
        {
            var client = this.storageAccount.CreateCloudBlobClient();
            return client.GetContainerReference(this.storageContainerName).GetBlockBlobReference(hash);
        }

        private class ResourceWrapper : IResource
        {
            private readonly AzureResourceService owner;

            public ResourceWrapper(string hash, AzureResourceService owner)
            {
                this.owner = owner;

                this.Hash = hash;
            }

            public string Hash { get; private set; }

            public void CopyTo(string filePath)
            {
                var content = this.owner.GetContent(this.Hash);
                using (var file = new FileStream(filePath, FileMode.Create))
                {
                    content.CopyTo(file);
                }
            }

            public Stream OpenRead()
            {
                return this.owner.GetContent(this.Hash);
            }
        }
    }
}