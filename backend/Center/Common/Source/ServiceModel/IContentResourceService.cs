// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentResourceService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the resource service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Defines the content resource service. It provides methods to up-/download ContentResources.
    /// </summary>
    [ServiceContract]
    public interface IContentResourceService
    {
        /// <summary>
        /// Gets the <see cref="Resource"/> with the given <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>The ContentResource with the given hash.</returns>
        [OperationContract]
        Task<ContentResource> GetAsync(string hash, HashAlgorithmTypes hashType);

        /// <summary>
        /// Uploads a resource.
        /// If the resource already exists, the existing resource info will be returned.
        /// </summary>
        /// <param name="uploadRequest">The info for the upload.</param>
        /// <returns>The result of the upload.</returns>
        [OperationContract]
        Task<ContentResourceUploadResult> UploadAsync(ContentResourceUploadRequest uploadRequest);

        /// <summary>
        /// Downloads a resource.
        /// If the resource doesn't exist an exception (if locally, a fault when requesting to a remote service) is
        /// thrown.
        /// </summary>
        /// <param name="downloadRequest">the info for the download.</param>
        /// <returns>The resource with the given hash.</returns>
        [OperationContract]
        Task<ContentResourceDownloadResult> DownloadAsync(ContentResourceDownloadRequest downloadRequest);

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <param name="hashType">The used hash algorithm.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        [OperationContract]
        Task<bool> TestContentResourceAsync(string hash, HashAlgorithmTypes hashType);

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
        [OperationContract]
        IResource GetResource(string hash, HashAlgorithmTypes hashType);

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
        [OperationContract]
        void AddResource(string hash, HashAlgorithmTypes hashType, string resourceFile, bool deleteFile);
    }
}
