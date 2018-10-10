// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceStore.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResourceStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// The file resource store.
    /// </summary>
    public class FileResourceStore : IResourceStore
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FileResourceStore>();

        private IWritableDirectoryInfo baseDirectory;

        private IWritableFileSystem fileSystem;

        /// <summary>
        /// Initializes the resource store by loading all necessary data.
        /// </summary>
        /// <param name="baseDir">
        /// The base directory for local file operations.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDir)
        {
            this.baseDirectory = baseDir;
            this.fileSystem = this.baseDirectory.FileSystem;
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
            IWritableFileInfo storeFile;
            if (string.IsNullOrEmpty(resource.StoreReference)
                || this.fileSystem.TryGetFile(resource.StoreReference, out storeFile))
            {
                return true;
            }

            Logger.Warn(
                "Resource {0} is missing its store reference {1}",
                resource.Id,
                resource.StoreReference);
            return false;
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
            var storeFilePath = this.GetStoreFilePath(resource.Id);
            IWritableFileInfo storeFile;
            if (this.fileSystem.TryGetFile(storeFilePath, out storeFile))
            {
                Logger.Warn("Found store file where there shouldn't be one, deleting it: {0}", storeFile.FullName);
                storeFile.Delete();
            }

            storeFile = deleteLocal
                            ? localFileInfo.MoveTo(storeFilePath)
                            : localFileInfo.CopyTo(storeFilePath);
            storeFile.Attributes = FileAttributes.Normal;

            resource.StoreReference = storeFile.FullName;
        }

        /// <summary>
        /// Removes the given resource from this store.
        /// </summary>
        /// <param name="resource">
        /// The resource information.
        /// </param>
        public void Remove(IStoredResource resource)
        {
            if (resource.StoreReference == null)
            {
                Logger.Warn("Resource doesn't have a store reference, can't remove it: {0}", resource.Id);
                return;
            }

            try
            {
                this.fileSystem.GetFile(resource.StoreReference).Delete();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't delete store reference file {0}", resource.StoreReference);
            }
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
            return new FileResourceAccess(this.fileSystem.GetFile(resource.StoreReference ?? resource.References[0]));
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
            IWritableFileInfo targetFile = null;
            if (resource.StoreReference != null)
            {
                if (keepTracking
                    && Path.GetPathRoot(resource.StoreReference)
                           .Equals(Path.GetPathRoot(localFile), StringComparison.InvariantCultureIgnoreCase))
                {
                    // move the store reference file out of the store
                    targetFile = this.fileSystem.GetFile(resource.StoreReference).MoveTo(localFile);
                    resource.StoreReference = null;
                }
                else
                {
                    // it's on a different drive, let's copy it (doesn't make any difference)
                    targetFile = this.fileSystem.GetFile(resource.StoreReference).CopyTo(localFile);
                }
            }
            else
            {
                foreach (var reference in resource.References)
                {
                    try
                    {
                        targetFile = this.fileSystem.GetFile(reference).CopyTo(localFile);
                        break;
                    }
                    catch (IOException ex)
                    {
                        Logger.Warn(ex, "Couldn't copy referenced resource {0}", reference);
                    }
                }
            }

            if (targetFile != null)
            {
                targetFile.Attributes = FileAttributes.Normal;
            }

            return targetFile != null;
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
            if (resource.StoreReference != null || resource.References.Count > 0)
            {
                // delete the file if we already have it in the store or still have a reference in a different place
                localFile.Attributes = FileAttributes.Normal;
                localFile.Delete();
                return;
            }

            var storePath = this.GetStoreFilePath(resource.Id);
            var storeFile = localFile.MoveTo(storePath);
            storeFile.Attributes = FileAttributes.Normal;
            resource.StoreReference = storePath;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        private string GetStoreFilePath(ResourceId id)
        {
            return Path.Combine(this.baseDirectory.FullName, id.Hash + ".rx");
        }
    }
}