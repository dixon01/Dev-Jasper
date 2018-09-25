// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteResourceService type.
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

    using NLog;

    /// <summary>
    /// Defines a service that wraps an <see cref="IResourceService"/> for remote usage (WCF).
    /// </summary>
    /// <remarks>
    /// The service is exposed with ConcurrencyMode.Multiple, assuming that the internal service is thread-safe.
    /// </remarks>
    [ErrorHandler]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteResourceService : IResourceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IResourceService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteResourceService"/> class.
        /// </summary>
        /// <param name="resourceService">
        /// The resource service.
        /// </param>
        public RemoteResourceService(IResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        /// <summary>
        /// Gets the <see cref="Resource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>The resource with the given hash.</returns>
        public async Task<Resource> GetAsync(string hash)
        {
            try
            {
                return await this.resourceService.GetAsync(hash);
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
        public async Task<ResourceUploadResult> UploadAsync(ResourceUploadRequest uploadRequest)
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
        public async Task<ResourceDownloadResult> DownloadAsync(ResourceDownloadRequest downloadRequest)
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
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        public Task<bool> TestResourceAsync(string hash)
        {
            try
            {
                return this.resourceService.TestResourceAsync(hash);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error during test");
                throw new FaultException("Error during test");
            }
        }
    }
}