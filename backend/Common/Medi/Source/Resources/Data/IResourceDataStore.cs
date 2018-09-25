// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceDataStore.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceDataStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Interface of the resource data store responsible to store resource information.
    /// </summary>
    public interface IResourceDataStore : IDisposable
    {
        /// <summary>
        /// Initializes the data store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base directory.
        /// </param>
        void Initialize(IWritableDirectoryInfo baseDirectory);

        /// <summary>
        /// Initializes the store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base Directory.
        /// </param>
        /// <param name="maxSizeMb">
        /// The max Size Mb.
        /// </param>
        void Initialize(IWritableDirectoryInfo baseDirectory, int maxSizeMb);

        /// <summary>
        /// Creates a new <see cref="StoredResource"/>, but doesn't add it to the store yet.
        /// Use this method instead of creating <see cref="StoredResource"/> manually.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <returns>A new <see cref="StoredResource"/> that can be used with this store.</returns>
        IStoredResource Create(ResourceId id);

        /// <summary>
        /// Gets the resource for a given id or null if the resource is not found.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="StoredResource"/> for the given id or null if the resource is not found.
        /// </returns>
        IStoredResource Get(ResourceId id);

        /// <summary>
        /// Adds the given resource to the store.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if a resource with the given <see cref="StoredResource.Id"/>
        /// already exists.
        /// </exception>
        void Add(IStoredResource resource);

        /// <summary>
        /// Updates the information about the given resource in the database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        void Update(IStoredResource resource);

        /// <summary>
        /// Removes the given resource from this database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// True if the resource was successfully removed.
        /// </returns>
        bool Remove(IStoredResource resource);

        /// <summary>
        /// Gets all resources registered in this store.
        /// </summary>
        /// <returns>
        /// All resources.
        /// </returns>
        IEnumerable<IStoredResource> GetAll();

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        void BeginSet();

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        void EndSet();
    }
}