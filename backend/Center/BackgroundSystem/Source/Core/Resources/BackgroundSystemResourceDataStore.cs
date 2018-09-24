// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemResourceDataStore.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemResourceDataStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Utility.Files.Writable;

    using IResourceService = Gorba.Center.Common.ServiceModel.IResourceService;

    /// <summary>
    /// The <see cref="IResourceDataStore"/> implementation for the background system.
    /// This resource data store simply keeps track of resources in memory.
    /// </summary>
    public class BackgroundSystemResourceDataStore : IResourceDataStore
    {
        /// <summary>
        /// This value is used for <see cref="IStoredResource.StoreReference"/> for
        /// resources from the <see cref="IResourceService"/>.
        /// This helps us identify BGS resources against local ones.
        /// </summary>
        internal static readonly string ServiceStoreReference = "n/a";

        private readonly Dictionary<ResourceId, StoredResource> resources =
            new Dictionary<ResourceId, StoredResource>();

        private int currentSetIndex;

        private IResourceService ResourceService
        {
            get
            {
                return DependencyResolver.Current.Get<IResourceService>();
            }
        }

        private IResourceDataService ResourceDataService
        {
            get
            {
                return DependencyResolver.Current.Get<IResourceDataService>();
            }
        }

        /// <summary>
        /// Initializes the data store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base directory.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDirectory)
        {
            this.Initialize(baseDirectory, 0);
        }

        /// <summary>
        /// Initializes the store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base Directory.
        /// </param>
        /// <param name="maxSizeMb">
        /// The max Size Mb.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDirectory, int maxSizeMb)
        {
        }

        /// <summary>
        /// Creates a new <see cref="StoredResource"/>, but doesn't add it to the store yet.
        /// Use this method instead of creating <see cref="StoredResource"/> manually.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <returns>A new <see cref="StoredResource"/> that can be used with this store.</returns>
        public IStoredResource Create(ResourceId id)
        {
            return new StoredResource(id);
        }

        /// <summary>
        /// Gets the resource for a given id or null if the resource is not found.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="StoredResource"/> for the given id or null if the resource is not found.
        /// </returns>
        public IStoredResource Get(ResourceId id)
        {
            StoredResource storedResource;
            if (this.resources.TryGetValue(id, out storedResource))
            {
                return storedResource;
            }

            Resource resource;
            try
            {
                resource = this.ResourceService.GetAsync(id.Hash).Result;
            }
            catch (KeyNotFoundException)
            {
                // assume the resource doesn't exist since the data service is not yet available
                return null;
            }

            if (resource == null)
            {
                this.resources[id] = null;
                return null;
            }

            return this.AddResource(resource);
        }

        /// <summary>
        /// Adds the given resource to the store.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// if a resource with the given <see cref="StoredResource.Id"/>
        /// already exists.
        /// </exception>
        public void Add(IStoredResource resource)
        {
            StoredResource found;
            if (this.resources.TryGetValue(resource.Id, out found) && found != null)
            {
                throw new ArgumentException("Resource with the same ID exists already");
            }

            this.resources[resource.Id] = (StoredResource)resource;
        }

        /// <summary>
        /// Updates the information about the given resource in the database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        public void Update(IStoredResource resource)
        {
            // nothing to do since we keep everything in memory
        }

        /// <summary>
        /// Removes the given resource from this database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// True if the resource was successfully removed.
        /// </returns>
        public bool Remove(IStoredResource resource)
        {
            return this.resources.Remove(resource.Id);
        }

        /// <summary>
        /// Gets all resources registered in this store.
        /// </summary>
        /// <returns>
        /// All resources.
        /// </returns>
        public IEnumerable<IStoredResource> GetAll()
        {
            try
            {
                foreach (var resource in this.ResourceDataService.QueryAsync().Result)
                {
                    if (!this.resources.ContainsKey(new ResourceId(resource.Hash)))
                    {
                        this.AddResource(resource);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                // ignore this, the data service is not yet available
            }

            return this.resources.Values.Where(r => r != null).ToList();
        }

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        public void BeginSet()
        {
            this.currentSetIndex++;
        }

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        public void EndSet()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.resources.Clear();
        }

        private IStoredResource AddResource(Resource resource)
        {
            var id = new ResourceId(resource.Hash);
            var storedResource = new StoredResource(id)
            {
                IsTemporary = false,
                OriginalFileName = resource.OriginalFilename,
                Size = resource.Length,
                State = ResourceState.Available,
                StoreReference = ServiceStoreReference
            };
            this.resources[id] = storedResource;
            return storedResource;
        }
    }
}