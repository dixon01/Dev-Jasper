// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteContentResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines a service that wraps an <see cref="IContentResourceService" /> for remote usage (WCF).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Resources
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Utility;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Resources;

    using NLog;

    /// <summary>
    /// Defines a service that wraps an <see cref="IContentResourceService"/> for remote usage (WCF).
    /// </summary>
    /// <remarks>
    /// The service is exposed with ConcurrencyMode.Multiple, assuming that the internal service is thread-safe.
    /// </remarks>
    [ErrorHandler]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteContentResourceService : IContentResourceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IContentResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteContentResourceService"/> class.
        /// </summary>
        /// <param name="resourceService">
        /// The resource service.
        /// </param>
        public RemoteContentResourceService(IContentResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        /// <summary>
        /// Gets the <see cref="ContentResource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<ContentResource> GetAsync(string hash, HashAlgorithmTypes hashType)
        {
            try
            {
                return await this.resourceService.GetAsync(hash, hashType);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while getting the resource");
                throw new FaultException("Error while getting the resource");
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
            try
            {
                return await this.resourceService.UploadAsync(uploadRequest);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during upload");
                throw new FaultException("Error during upload");
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
            try
            {
                return await this.resourceService.DownloadAsync(downloadRequest);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during download");
                throw new FaultException("Error during download");
            }
        }

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        public Task<bool> TestContentResourceAsync(string hash, HashAlgorithmTypes hashType)
        {
            try
            {
                return this.resourceService.TestContentResourceAsync(hash, hashType);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during test");
                throw new FaultException("Error during test");
            }
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
        public IResource GetResource(string hash, HashAlgorithmTypes hashType)
        {
            try
            {
                return this.resourceService.GetResource(hash, hashType);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error getting the content resource");
                var message = string.Format("Error getting content resource '{0}'", hash);
                throw new FaultException(message);
            }
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
        public void AddResource(string hash, HashAlgorithmTypes hashType, string resourceFile, bool deleteFile)
        {
            try
            {
                this.resourceService.AddResource(hash, hashType, resourceFile, deleteFile);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error adding a content resource");
                var message = string.Format("Error adding content resource with hash '{0}'", hash);
                throw new FaultException(message);
            }
        }
    }
}
