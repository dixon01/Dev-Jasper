// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The base class for resource services.
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
    /// Base class that defines a service to upload and download <see cref="Resource"/>s with their content.
    /// </summary>
    public abstract class ResourceServiceBase : ConcurrentServiceBase, IResourceService, IExtendedResourceProvider
    {
        private int currentSetSize;

        /// <summary>
        /// Gets the <see cref="Resource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<Resource> GetAsync(string hash)
        {
            using (await this.AcquireReaderLockAsync())
            {
                return await GetResourceEntityAsync(hash, DependencyResolver.Current.Get<IResourceDataService>());
            }
        }

        /// <summary>
        /// Uploads a resource.
        /// If the resource already exists, the existing resource info will be returned.
        /// </summary>
        /// <param name="uploadRequest">The info for the upload.</param>
        /// <returns>The result of the upload.</returns>
        public async Task<ResourceUploadResult> UploadAsync(ResourceUploadRequest uploadRequest)
        {
            var currentUserInfo = CurrentContextUserInfoProvider.Current.GetUserInfo();
            using (await this.AcquireReaderLockAsync())
            {
                if (await this.CheckResourceAsync(uploadRequest.Resource.Hash))
                {
                    this.Logger.Trace("The resource already exists");
                    return null;
                }
            }

            await this.StoreContentAsync(uploadRequest.Resource.Hash, uploadRequest.Content);
            var userDataService = DependencyResolver.Current.Get<IUserDataService>();
            var userFilter = UserQuery.Create().WithUsername(currentUserInfo.Username);
            var uploadingUser = (await userDataService.QueryAsync(userFilter)).SingleOrDefault();
            var resourceDataService = DependencyResolver.Current.Get<IResourceDataService>();
            using (await this.AcquireWriterLockAsync())
            {
                var existingResource = await GetResourceEntityAsync(uploadRequest.Resource.Hash, resourceDataService);
                if (existingResource == null)
                {
                    uploadRequest.Resource.UploadingUser = uploadingUser;
                    var addedResource = await resourceDataService.AddAsync(uploadRequest.Resource);
                    this.Logger.Trace("The resource '{0}' doesn't exit. Creating it", uploadRequest.Resource.Hash);
                    return new ResourceUploadResult { Resource = addedResource };
                }

                existingResource.OriginalFilename = uploadRequest.Resource.OriginalFilename;
                existingResource.UploadingUser = uploadingUser;
                this.Logger.Trace(
                    "The resource '{0}' exists (Id: {1}). Creating it",
                    uploadRequest.Resource.Hash,
                    existingResource.Id);
                existingResource = await resourceDataService.UpdateAsync(existingResource);

                return new ResourceUploadResult { Resource = existingResource };
            }
        }

        /// <summary>
        /// Downloads a resource.
        /// If the resource doesn't exist an exception (if locally, a fault when requesting to a remote service) is
        /// thrown.
        /// </summary>
        /// <param name="downloadRequest">the info for the download.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<ResourceDownloadResult> DownloadAsync(ResourceDownloadRequest downloadRequest)
        {
            var resource = await this.GetAsync(downloadRequest.Hash);
            var content = this.GetContent(downloadRequest.Hash);
            return new ResourceDownloadResult { Content = content, Resource = resource };
        }

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content). Otherwise,
        /// <c>false</c>.
        /// </returns>
        public async Task<bool> TestResourceAsync(string hash)
        {
            return await this.CheckResourceAsync(hash);
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
        public virtual IResource GetResource(string hash)
        {
            if (!this.ContentExists(hash))
            {
                throw new UpdateException("Couldn't find resource " + hash);
            }

            return new ResourceWrapper(hash, this);
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
        public abstract void AddResource(string hash, string resourceFile, bool deleteFile);

        /// <summary>
        /// The begin current set.
        /// </summary>
        public void BeginCurrentSet()
        {
            /*
            1- Start purging older resources if the reaching the limits.
            2- Use Priority Queue to arrange the resources
            3- Delete resources that are not in the current set
            */
            this.currentSetSize++;
        }

        /// <summary>
        /// The end current set.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int EndCurrentSet()
        {
            return this.currentSetSize;
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
        public abstract bool TryGetResource(string hash, out IResource resource);

        /// <summary>
        /// Checks if content exists for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the content.
        /// </param>
        /// <returns>
        /// <c>true</c> if the content exists; <c>false</c> otherwise.
        /// </returns>
        public abstract bool ContentExists(string hash);

        /// <summary>
        /// Deletes the resource with the given <paramref name="hash"/>, if found. If the resource doesn't exist,
        /// nothing will be done.
        /// </summary>
        /// <param name="hash">The hash of the resource to delete.</param>
        /// <exception cref="UpdateException">
        /// Any error occurred while removing the resource from the provider.
        /// </exception>
        public abstract void DeleteResource(string hash);

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
        protected abstract Task StoreContentAsync(string hash, Stream stream);

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

        private static async Task<Resource> GetResourceEntityAsync(
          string hash,
          IResourceDataService resourceDataService)
        {
            var query = ResourceQuery.Create().WithHash(hash);
            return (await resourceDataService.QueryAsync(query)).SingleOrDefault();
        }

        private async Task<bool> CheckResourceAsync(string hash)
        {
            var resourceDataService = DependencyResolver.Current.Get<IResourceDataService>();
            var query = ResourceQuery.Create().WithHash(hash);
            var existingResource = (await resourceDataService.QueryAsync(query)).SingleOrDefault();
            if (existingResource == null)
            {
                this.Logger.Trace("The resource '{0}' doesn't exist in database yet", hash);
                return false;
            }

            return this.ContentExists(existingResource.Hash);
        }

        private class ResourceWrapper : IResource
        {
            private readonly ResourceServiceBase owner;

            public ResourceWrapper(string hash, ResourceServiceBase owner)
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
