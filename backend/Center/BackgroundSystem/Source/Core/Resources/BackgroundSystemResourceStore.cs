// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemResourceStore.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemResourceStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System.IO;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files.Writable;

    using IResourceService = Gorba.Center.Common.ServiceModel.IResourceService;

    /// <summary>
    /// The <see cref="IResourceStore"/> implementation for the background system.
    /// This resource service tries to find all resources in the <see cref="IResourceService"/>.
    /// If new resources have to be stored (temporarily), they are stored in a local store.
    /// </summary>
    public class BackgroundSystemResourceStore : IResourceStore
    {
        private readonly FileResourceStore localStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemResourceStore"/> class.
        /// </summary>
        public BackgroundSystemResourceStore()
        {
            // this local store is used for all temporary resources
            // that are transferred through BGS but are not found in the resource service.
            this.localStore = new FileResourceStore();
        }

        private IResourceService ResourceService
        {
            get
            {
                return DependencyResolver.Current.Get<IResourceService>();
            }
        }

        /// <summary>
        /// Initializes the resource store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base directory for local file operations.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDirectory)
        {
            this.localStore.Initialize(baseDirectory);
        }

        /// <summary>
        /// Checks if the given resource is available in this store.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// True if it is available, otherwise false.
        /// </returns>
        public bool IsAvailable(IStoredResource resource)
        {
            if (this.localStore.IsAvailable(resource))
            {
                return true;
            }

            return this.ResourceService.GetAsync(resource.Id.Hash).Result != null;
        }

        /// <summary>
        /// Adds the given <paramref name="localFileInfo"/> to this store.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// This method might modify this parameter, so it should afterwards be updated in the data store.
        /// </param>
        /// <param name="localFileInfo">
        /// The file information for the file that is to be copied to the store.
        /// This method might move or delete the file referenced in this parameter,
        /// so it should no longer be used after calling this method.
        /// </param>
        /// <param name="deleteLocal">
        /// A flag indicating whether the <paramref name="localFileInfo"/> should be deleted after being stored.
        /// </param>
        public void Add(IStoredResource resource, IWritableFileInfo localFileInfo, bool deleteLocal)
        {
            this.localStore.Add(resource, localFileInfo, deleteLocal);
        }

        /// <summary>
        /// Removes the given resource from this store.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// </param>
        public void Remove(IStoredResource resource)
        {
            this.localStore.Remove(resource);
        }

        /// <summary>
        /// Requests read access to the contents of the given resource.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// The <see cref="IResourceAccess"/> that can be used for reading the resource.
        /// </returns>
        public IResourceAccess GetResourceAccess(IStoredResource resource)
        {
            if (BackgroundSystemResourceDataStore.ServiceStoreReference.Equals(resource.StoreReference))
            {
                return new ProviderResourceAccess(resource.Id);
            }

            return this.localStore.GetResourceAccess(resource);
        }

        /// <summary>
        /// Gets a local copy of a resource.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// This method might modify this parameter, so it should afterwards be updated in the data store.
        /// </param>
        /// <param name="localFile">
        /// The local file to which the resource will be written.
        /// If the file exists, it will be overwritten.
        /// </param>
        /// <param name="keepTracking">
        /// A flag indicating if the resource service should continue tracking the
        /// copied file.
        /// </param>
        /// <returns>
        /// True if the resource was copied successfully.
        /// </returns>
        public bool GetLocalCopy(IStoredResource resource, string localFile, bool keepTracking)
        {
            return this.localStore.GetLocalCopy(resource, localFile, keepTracking);
        }

        /// <summary>
        /// Returns a local copy retrieved by <see cref="IResourceStore.GetLocalCopy"/>.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// This method might modify this parameter, so it should afterwards be updated in the data store.
        /// </param>
        /// <param name="localFile">
        /// The local file to return.
        /// </param>
        public void ReturnLocalCopy(IStoredResource resource, IWritableFileInfo localFile)
        {
            this.localStore.ReturnLocalCopy(resource, localFile);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.localStore.Dispose();
        }

        private class ProviderResourceAccess : IResourceAccess
        {
            private readonly ResourceId id;

            public ProviderResourceAccess(ResourceId id)
            {
                this.id = id;
            }

            public void CopyTo(string newFileName)
            {
                var resource = this.GetResource();
                resource.CopyTo(newFileName);
            }

            public Stream OpenRead()
            {
                var resource = this.GetResource();
                return resource.OpenRead();
            }

            public override string ToString()
            {
                return this.id.ToString();
            }

            private IResource GetResource()
            {
                var resourceProvider = DependencyResolver.Current.Get<IResourceProvider>();
                return resourceProvider.GetResource(this.id.Hash);
            }
        }
    }
}
