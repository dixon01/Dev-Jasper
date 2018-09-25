// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDataResourceProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDataResourceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.IO;

    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Defines an <see cref="IResourceProvider"/> using the AppData as local storage for resources.
    /// </summary>
    public class AppDataResourceProvider : IExtendedResourceProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWritableFileSystem fileSystem;

        private readonly string resourcesRelativePath;

        private string resourcesRootPath;

        private string resourcesDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDataResourceProvider"/> class.
        /// </summary>
        /// <param name="resourcesRelativePath">Path relative to local AppData where resources should be stored.</param>
        public AppDataResourceProvider(string resourcesRelativePath)
        {
            if (resourcesRelativePath == null)
            {
                throw new ArgumentNullException("resourcesRelativePath");
            }

            try
            {
                this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
            }
            catch (InvalidCastException exception)
            {
                throw new UpdateException(
                    "To be able to use the AppDataResourceProvider, the current Local filesystem must be writable",
                    exception);
            }

            this.resourcesRelativePath = resourcesRelativePath;
            this.resourcesRootPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            this.resourcesDirectory =
                Path.Combine(this.resourcesRootPath, resourcesRelativePath);

            this.EnsureResourcePathExists();
        }

        /// <summary>
        /// Gets a resource for a given hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        public IResource GetResource(string hash)
        {
            IResource resource;
            this.EnsureResourcePathExists();
            if (this.TryGetResource(hash, out resource))
            {
                return resource;
            }

            throw new UpdateException("Can't find resource '" + hash + "'");
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">The expected hash of the resource file (the name from where it was copied).</param>
        /// <param name="resourceFile">The full resource file path.</param>
        /// <param name="deleteFile">
        /// A flag indicating whether the <see cref="resourceFile"/> should be deleted after being registered.
        /// </param>
        /// <exception cref="UpdateException">
        /// If the resource file doesn't match the given hash.
        /// </exception>
        public void AddResource(string hash, string resourceFile, bool deleteFile)
        {
            IWritableFileInfo resourceFileInfo;
            this.EnsureResourcePathExists();
            if (!this.fileSystem.TryGetFile(resourceFile, out resourceFileInfo))
            {
                throw new UpdateException("Can't find resource file '" + resourceFile + "'");
            }

            IWritableFileInfo fileInfo;
            if (this.TryGetFile(hash, out fileInfo))
            {
                if (deleteFile)
                {
                    Logger.Debug("Deleting original resource file '{0}'", resourceFile);
                    resourceFileInfo.Delete();
                }

                Logger.Debug("Resource with has '{0}' already exists", hash);
                return;
            }

            var resourceStoreFile = this.GetFilePath(hash);
            if (deleteFile)
            {
                Logger.Debug("Original file should be delete. Moving '{0}' to '{1}'", resourceFile, resourceStoreFile);
                resourceFileInfo.MoveTo(resourceStoreFile);
                return;
            }

            Logger.Debug("Copying original file '{0}' to '{1}'", resourceFile, resourceStoreFile);

            var newResource = ((IWritableFileSystem)FileSystemManager.Local).CreateFile(resourceStoreFile);
            using (var targetStream = newResource.OpenWrite())
            {
                using (var sourceStream = resourceFileInfo.OpenRead())
                {
                    sourceStream.CopyTo(targetStream);
                }
            }
        }

        /// <summary>
        /// Changes the root directory for the local resources.
        /// </summary>
        /// <param name="path">
        /// The root path for all local resources. If the directory doesn't exists, nothing changes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="path"/> is null or white space.
        /// </exception>
        public void SetResourceDirectoryRoot(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (Directory.Exists(path))
            {
                this.resourcesDirectory = Path.Combine(path, this.resourcesRelativePath);
                this.resourcesRootPath = path;
                this.EnsureResourcePathExists();
                Logger.Debug("Local resource root changed to {0}", path);
            }
            else
            {
                Logger.Warn(
                    "The root directory '{0}' does not exist. The resource directory is still '{1}'",
                    path,
                    this.resourcesDirectory);
            }
        }

        /// <summary>
        /// Changes the resource directory for the local resources.
        /// </summary>
        /// <param name="path">
        /// The full resource path for all local resources. If the directory doesn't exist, it will be created.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="path"/> is null or white space.
        /// </exception>
        public void SetResourceDirectory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            this.resourcesDirectory = path;
            this.EnsureResourcePathExists();
            Logger.Debug("Changed local resource path to {0}", path);
        }

        /// <summary>
        /// Deletes the resource with the given <paramref name="hash"/>, if found. If the resource doesn't exist,
        /// nothing will be done.
        /// </summary>
        /// <param name="hash">The hash of the resource to delete.</param>
        /// <exception cref="UpdateException">
        /// Any error occurred while removing the resource from the provider.
        /// </exception>
        public void DeleteResource(string hash)
        {
            // TODO: implement deletion
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
        public bool TryGetResource(string hash, out IResource resource)
        {
            IWritableFileInfo fileInfo;
            this.EnsureResourcePathExists();
            if (this.TryGetFile(hash, out fileInfo))
            {
                resource = new LocalResource(fileInfo, hash);
                return true;
            }

            resource = null;
            return false;
        }

        private string GetFilePath(string hash)
        {
            return Path.Combine(this.resourcesDirectory, hash + ".rx");
        }

        private bool TryGetFile(string hash, out IWritableFileInfo fileInfo)
        {
            var filePath = this.GetFilePath(hash);
            return this.fileSystem.TryGetFile(filePath, out fileInfo);
        }

        private void EnsureResourcePathExists()
        {
            this.fileSystem.CreateDirectory(this.resourcesDirectory);
        }

        private class LocalResource : IResource
        {
            private readonly IWritableFileInfo fileInfo;

            public LocalResource(IWritableFileInfo fileInfo, string hash)
            {
                this.fileInfo = fileInfo;
                this.Hash = hash;
            }

            public string Hash { get; private set; }

            public void CopyTo(string filePath)
            {
                this.fileInfo.CopyTo(filePath);
            }

            public Stream OpenRead()
            {
                return this.fileInfo.OpenRead();
            }
        }
    }
}