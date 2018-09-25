// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalResourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Defines a service to upload and download <see cref="Resource"/>s with their content.
    /// </summary>
    public class LocalResourceService : ResourceServiceBase
    {
        private const int BufferSize = 2048;

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

            var path = this.GetPath(hash);

            // ReSharper disable once PossibleNullReferenceException
            if (!path.Directory.Exists)
            {
                path.Directory.Create();
            }

            if (deleteFile)
            {
                file.MoveTo(path.FullName);
            }
            else
            {
                file.CopyTo(path.FullName);
            }

            var storedHash = ResourceHash.Create(path.FullName);
            if (string.Equals(storedHash, hash))
            {
                return;
            }

            path.Delete();
            throw new UpdateException("Hash verification failed");
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
            var path = this.GetPath(hash);
            if (!File.Exists(path.FullName))
            {
                return false;
            }

            var existingHash = ResourceHash.Create(path.FullName);
            var isValidHash = string.Equals(existingHash, hash, StringComparison.InvariantCultureIgnoreCase);
            if (isValidHash)
            {
                return true;
            }

            path.Delete();
            return false;
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
            var path = this.GetPath(hash);
            if (!File.Exists(path.FullName))
            {
                throw new UpdateException("File not found");
            }

            File.Delete(path.FullName);
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
            var path = this.GetPath(hash);
            return new FileStream(path.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

            var path = this.GetPath(hash);

            // ReSharper disable once PossibleNullReferenceException
            if (!path.Directory.Exists)
            {
                path.Directory.Create();
            }

            var tempFile = GetRandomFileName(path.Directory);
            try
            {
                using (var fileStream = new FileStream(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream, BufferSize);
                }

                var storedHash = ResourceHash.Create(tempFile);
                if (!string.Equals(storedHash, hash, StringComparison.InvariantCultureIgnoreCase))
                {
                    path.Delete();
                    throw new UpdateException("Hash verification failed");
                }

                File.Move(tempFile, path.FullName);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
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
            var settings = HostingSettingsProvider.Current.GetSettings();
            var resourcesPath = string.IsNullOrEmpty(settings.ResourcesPath)
                                    ? this.GetRootPath()
                                    : new DirectoryInfo(settings.ResourcesPath);
            var fileName = hash + ".rx";
            return new FileInfo(Path.Combine(resourcesPath.FullName, fileName));
        }

        private static string GetRandomFileName(DirectoryInfo directory)
        {
            return Path.Combine(directory.FullName, string.Format("{0}.tmp", Guid.NewGuid()));
        }

        private DirectoryInfo GetRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(assembly.Location);

            // ReSharper disable once PossibleNullReferenceException
            var resourcesPath = Path.Combine(fileInfo.Directory.FullName, "Resources");
            return new DirectoryInfo(resourcesPath);
        }
    }
}