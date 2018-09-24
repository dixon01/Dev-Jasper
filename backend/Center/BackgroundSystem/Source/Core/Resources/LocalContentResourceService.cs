// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalContentResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalContentResourceService type.
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
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Defines a service to upload and download <see cref="ContentResource"/>s with their content.
    /// </summary>
    public class LocalContentResourceService : ContentResourceServiceBase
    {
        private const int BufferSize = 2048;

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
        public override void AddResource(string hash, HashAlgorithmTypes hashType, string resourceFile, bool deleteFile)
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

            var storedHash = ContentResourceHash.Create(path.FullName, HashAlgorithmTypes.xxHash64);
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
        /// <param name="hashType">
        /// The hash Type.
        /// </param>
        /// <returns>
        /// <c>true</c> if the content exists; <c>false</c> otherwise.
        /// </returns>
        public override bool ContentExists(string hash, HashAlgorithmTypes hashType)
        {
            var path = this.GetPath(hash);
            if (!File.Exists(path.FullName))
            {
                return false;
            }

            var existingHash = ContentResourceHash.Create(path.FullName, hashType);
            var isValidHash = string.Equals(existingHash, hash, StringComparison.InvariantCultureIgnoreCase);
            if (isValidHash)
            {
                return true;
            }

            path.Delete();
            return false;
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
        /// The resource hash.
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

                var storedHash = ContentResourceHash.Create(tempFile, hashType);
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
            var resourcesPath = string.IsNullOrEmpty(settings.ContentResourcesPath)
                                    ? this.GetRootPath()
                                    : new DirectoryInfo(Path.Combine(settings.ContentResourcesPath));
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
            var resourcesPath = Path.Combine(fileInfo.Directory.FullName, "ContentResources");
            return new DirectoryInfo(resourcesPath);
        }
    }
}
