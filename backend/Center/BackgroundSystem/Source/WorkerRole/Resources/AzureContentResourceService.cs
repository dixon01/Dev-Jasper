// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureContentResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.BackgroundSystem.WorkerRole.Resources
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Resources;
    using Gorba.Center.Common.Azure;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The azure content resource service.
    /// </summary>
    public class AzureContentResourceService : ContentResourceServiceBase
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
        public async Task InitializeStorageAsync(string containerName = "contentresources")
        {
            this.Logger.Debug("Initialize the container within the storage.");
            this.storageContainerName = containerName;
            this.storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting(
                        PredefinedAzureItems.Settings.StorageConnectionString));

            var client = this.storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(this.storageContainerName);
            await container.CreateIfNotExistsAsync();

            // ensure that we can publicly access the blobs (but not the container listing)
            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            this.Logger.Debug("Container '{0}' in storage initialized.", this.storageContainerName);
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">
        /// The expected hash of the resource file (the name from where it was copied).
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
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
        public override void AddResource(
            string hash, HashAlgorithmTypes hashType, string resourceFile, bool deleteFile)
        {
            var file = new FileInfo(resourceFile);
            if (!file.Exists)
            {
                throw new FileNotFoundException("Couldn't find resource file", resourceFile);
            }

            var resourceDataService = DependencyResolver.Current.Get<IContentResourceDataService>();
            var query = ContentResourceQuery.Create().WithHash(hash).WithHashAlgorithmType(hashType);
            var existingEntity = resourceDataService.QueryAsync(query).Result.SingleOrDefault();
            if (existingEntity != null)
            {
                if (deleteFile)
                {
                    File.Delete(resourceFile);
                }

                return;
            }

            var entity = new ContentResource
            {
                CreatedOn = TimeProvider.Current.UtcNow,
                Hash = hash,
                Length = file.Length,
                MimeType = "application/octet-stream",
                OriginalFilename = file.Name,
                HashAlgorithmType = hashType
            };
            resourceDataService.AddAsync(entity).Wait();

            this.Logger.Trace("Adding resource with hash '{0}' to storage.", hash);
            var blob = this.GetBlockBlobReference(hash);
            string storedHash;
            using (var fileStream = file.OpenRead())
            {
                blob.UploadFromStream(fileStream);
                storedHash = ContentResourceHash.Create(fileStream, hashType);
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
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        public override IResource GetResource(string hash, HashAlgorithmTypes hashType)
        {
            if (!this.ContentExists(hash, hashType))
            {
                throw new UpdateException("Couldn't find resource " + hash);
            }

            return new ContentResourceWrapper(hash, this);
        }

        /// <summary>
        /// Checks if content exists for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// <c>true</c> if the content exists; <c>false</c> otherwise.
        /// </returns>
        public override bool ContentExists(string hash, HashAlgorithmTypes hashType)
        {
            return this.GetBlockBlobReference(hash).Exists();
        }

        /// <summary>
        /// Asynchronously stores the content.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task StoreContentAsync(string hash, HashAlgorithmTypes hashType, Stream stream)
        {
            if (this.ContentExists(hash, hashType))
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

        private class ContentResourceWrapper : IResource
        {
            private readonly AzureContentResourceService owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="ContentResourceWrapper"/> class.
            /// </summary>
            /// <param name="hash">
            /// The hash.
            /// </param>
            /// <param name="owner">
            /// The owner.
            /// </param>
            public ContentResourceWrapper(string hash, AzureContentResourceService owner)
            {
                this.owner = owner;

                this.Hash = hash;
            }

            /// <summary>
            /// Gets the hash.
            /// </summary>
            public string Hash { get; private set; }

            /// <summary>
            /// The copy to.
            /// </summary>
            /// <param name="filePath">
            /// The file path.
            /// </param>
            public void CopyTo(string filePath)
            {
                var content = this.owner.GetContent(this.Hash);
                using (var file = new FileStream(filePath, FileMode.Create))
                {
                    content.CopyTo(file);
                }
            }

            /// <summary>
            /// The open read.
            /// </summary>
            /// <returns>
            /// The <see cref="Stream"/>.
            /// </returns>
            public Stream OpenRead()
            {
                return this.owner.GetContent(this.Hash);
            }
        }
    }
}
