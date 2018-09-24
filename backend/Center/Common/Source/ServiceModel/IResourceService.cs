// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Resources;

    /// <summary>
    /// Defines the resource service.
    /// </summary>
    [ServiceContract]
    public interface IResourceService
    {
        /// <summary>
        /// Gets the <see cref="Resource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>The resource with the given hash.</returns>
        [OperationContract]
        Task<Resource> GetAsync(string hash);

        /// <summary>
        /// Uploads a resource.
        /// If the resource already exists, the existing resource info will be returned.
        /// </summary>
        /// <param name="uploadRequest">The info for the upload.</param>
        /// <returns>The result of the upload.</returns>
        [OperationContract]
        Task<ResourceUploadResult> UploadAsync(ResourceUploadRequest uploadRequest);

        /// <summary>
        /// Downloads a resource.
        /// If the resource doesn't exist an exception (if locally, a fault when requesting to a remote service) is
        /// thrown.
        /// </summary>
        /// <param name="downloadRequest">the info for the download.</param>
        /// <returns>The resource with the given hash.</returns>
        [OperationContract]
        Task<ResourceDownloadResult> DownloadAsync(ResourceDownloadRequest downloadRequest);

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        [OperationContract]
        Task<bool> TestResourceAsync(string hash);
    }
}