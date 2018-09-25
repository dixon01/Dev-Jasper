// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class that defines a service to upload and download <see cref="ContentResource" />s with their content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Base class that defines a service to upload and download <see cref="ContentResource"/>s with their content.
    /// </summary>
    public abstract class ContentResourceServiceBase : ConcurrentServiceBase, IContentResourceService
    {
        /// <summary>
        /// Gets the <see cref="Resource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<ContentResource> GetAsync(string hash, HashAlgorithmTypes hashType)
        {
            using (await this.AcquireReaderLockAsync())
            {
                return await GetResourceEntityAsync(
                    hash, hashType, DependencyResolver.Current.Get<IContentResourceDataService>());
            }
        }

        /// <summary>
        /// Uploads a resource.
        /// If the resource already exists, the existing resource info will be returned.
        /// </summary>
        /// <param name="uploadRequest">The info for the upload.</param>
        /// <returns>The result of the upload.</returns>
        public async Task<ContentResourceUploadResult> UploadAsync(ContentResourceUploadRequest uploadRequest)
        {
            var currentUserInfo = CurrentContextUserInfoProvider.Current.GetUserInfo();
            using (await this.AcquireReaderLockAsync())
            {
                if (await
                    this.CheckResourceAsync(uploadRequest.Resource.Hash, uploadRequest.Resource.HashAlgorithmType))
                {
                    this.Logger.Trace("The resource already exists");
                    return null;
                }
            }

            await
                this.StoreContentAsync(
                    uploadRequest.Resource.Hash,
                    uploadRequest.Resource.HashAlgorithmType,
                    uploadRequest.Content);
            var userDataService = DependencyResolver.Current.Get<IUserDataService>();
            var userFilter = UserQuery.Create().WithUsername(currentUserInfo.Username);
            var uploadingUser = (await userDataService.QueryAsync(userFilter)).SingleOrDefault();
            var resourceDataService = DependencyResolver.Current.Get<IContentResourceDataService>();
            using (await this.AcquireWriterLockAsync())
            {
                var existingResource =
                    await
                    GetResourceEntityAsync(
                        uploadRequest.Resource.Hash,
                        uploadRequest.Resource.HashAlgorithmType,
                        resourceDataService);
                if (existingResource == null)
                {
                    uploadRequest.Resource.UploadingUser = uploadingUser;
                    var addedResource = await resourceDataService.AddAsync(uploadRequest.Resource);
                    this.Logger.Trace(
                        "The ContentResource '{0}' doesn't exit. Creating it",
                        uploadRequest.Resource.Hash);
                    return new ContentResourceUploadResult { Resource = addedResource };
                }

                existingResource.OriginalFilename = uploadRequest.Resource.OriginalFilename;
                existingResource.UploadingUser = uploadingUser;
                this.Logger.Trace(
                    "The ContentResource '{0}' exists (Id: {1}). Creating it",
                    uploadRequest.Resource.Hash,
                    existingResource.Id);
                existingResource = await resourceDataService.UpdateAsync(existingResource);

                return new ContentResourceUploadResult { Resource = existingResource };
            }
        }

        /// <summary>
        /// Downloads a resource.
        /// If the resource doesn't exist an exception (if locally, a fault when requesting to a remote service) is
        /// thrown.
        /// </summary>
        /// <param name="downloadRequest">the info for the download.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<ContentResourceDownloadResult> DownloadAsync(ContentResourceDownloadRequest downloadRequest)
        {
            var resource = await this.GetAsync(downloadRequest.Hash, downloadRequest.HashType);
            var content = this.GetContent(downloadRequest.Hash);
            return new ContentResourceDownloadResult { Content = content, Resource = resource };
        }

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content). Otherwise,
        /// <c>false</c>.
        /// </returns>
        public async Task<bool> TestContentResourceAsync(string hash, HashAlgorithmTypes hashType)
        {
            return await this.CheckResourceAsync(hash, hashType);
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
        public virtual IResource GetResource(string hash, HashAlgorithmTypes hashType)
        {
            if (!this.ContentExists(hash, hashType))
            {
                throw new UpdateException("Couldn't find resource " + hash);
            }

            return new ContentResourceServiceBase.ContentResourceWrapper(hash, this);
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
        public abstract void AddResource(
            string hash, HashAlgorithmTypes hashType, string resourceFile, bool deleteFile);

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
        public abstract bool ContentExists(string hash, HashAlgorithmTypes hashType);

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
        protected abstract Task StoreContentAsync(string hash, HashAlgorithmTypes hashType, Stream stream);

        /// <summary>
        /// Gets the content for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// The content <see cref="Stream"/>.
        /// </returns>
        protected abstract Stream GetContent(string hash);

        /// <summary>
        /// Gets the path where the content is stored.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// The <see cref="FileInfo"/> of the content.
        /// </returns>
        protected abstract FileInfo GetPath(string hash);

        private static async Task<ContentResource> GetResourceEntityAsync(
          string hash,
          HashAlgorithmTypes hashType,
          IContentResourceDataService resourceDataService)
        {
            var query = ContentResourceQuery.Create().WithHash(hash).WithHashAlgorithmType(hashType);
            return (await resourceDataService.QueryAsync(query)).SingleOrDefault();
        }

        private async Task<bool> CheckResourceAsync(string hash, HashAlgorithmTypes hashType)
        {
            var resourceDataService = DependencyResolver.Current.Get<IContentResourceDataService>();
            var query = ContentResourceQuery.Create().WithHash(hash).WithHashAlgorithmType(hashType);
            var existingResource = (await resourceDataService.QueryAsync(query)).SingleOrDefault();
            if (existingResource == null)
            {
                this.Logger.Trace(
                    "The ContentResource '{0}', hash type '{1}' doesn't exist in database yet",
                    hash,
                    hashType);
                return false;
            }

            return this.ContentExists(existingResource.Hash, existingResource.HashAlgorithmType);
        }

        private class ContentResourceWrapper : IResource
        {
            private readonly ContentResourceServiceBase owner;

            public ContentResourceWrapper(string hash, ContentResourceServiceBase owner)
            {
                this.owner = owner;

                this.Hash = hash;
            }

            public string Hash { get; private set; }

            public void CopyTo(string filePath)
            {
                this.owner.GetPath(this.Hash).CopyTo(filePath);
            }

            public Stream OpenRead()
            {
                return this.owner.GetContent(this.Hash);
            }
        }
    }
}
