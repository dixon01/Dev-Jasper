// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceStore.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;

    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Interface of the resource store responsible to store resources.
    /// </summary>
    public interface IResourceStore : IDisposable
    {
        /// <summary>
        /// Initializes the resource store by loading all necessary data.
        /// </summary>
        /// <param name="baseDirectory">
        /// The base directory for local file operations.
        /// </param>
        void Initialize(IWritableDirectoryInfo baseDirectory);

        /// <summary>
        /// Checks if the given resource is available in this store.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// True if it is available, otherwise false.
        /// </returns>
        bool IsAvailable(IStoredResource resource);

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
        void Add(IStoredResource resource, IWritableFileInfo localFileInfo, bool deleteLocal);

        /// <summary>
        /// Removes the given resource from this store.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// </param>
        void Remove(IStoredResource resource);

        /// <summary>
        /// Requests read access to the contents of the given resource.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// The <see cref="IResourceAccess"/> that can be used for reading the resource.
        /// </returns>
        IResourceAccess GetResourceAccess(IStoredResource resource);

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
        bool GetLocalCopy(IStoredResource resource, string localFile, bool keepTracking);

        /// <summary>
        /// Returns a local copy retrieved by <see cref="GetLocalCopy"/>.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// This method might modify this parameter, so it should afterwards be updated in the data store.
        /// </param>
        /// <param name="localFile">
        /// The local file to return.
        /// </param>
        void ReturnLocalCopy(IStoredResource resource, IWritableFileInfo localFile);
    }
}