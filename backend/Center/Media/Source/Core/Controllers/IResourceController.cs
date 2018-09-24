// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the controller handling project resources and thumbnails.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Defines the controller handling project resources and thumbnails.
    /// </summary>
    public interface IResourceController
    {
        /// <summary>
        /// Gets the media shell.
        /// </summary>
        IMediaShell MediaShell { get; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Decrements the reference count for the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        void DecrementResourceReferenceCount(string hash);

        /// <summary>
        /// Increments the reference count for the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        void IncrementResourceReferenceCount(string hash);

        /// <summary>
        /// Gets the thumbnail resource of the given <paramref name="thumbnailHash"/>.
        /// </summary>
        /// <param name="thumbnailHash">
        /// The thumbnail hash.
        /// </param>
        /// <param name="thumbnailResource">
        /// The thumbnail resource.
        /// </param>
        /// <returns>
        /// <c>true</c> if the resource was found; otherwise, <c>false</c>.
        /// </returns>
        bool GetVideoThumbnail(string thumbnailHash, out IResource thumbnailResource);

        /// <summary>
        /// Ensures that the preview of the resource with the given <paramref name="resourceInfo"/> is in the local
        /// AppData. If it's not already present, it is created.
        /// </summary>
        /// <param name="resourceInfo">The resource info.</param>
        /// <param name="thumbnailResource">The resource containing the thumbnail.</param>
        /// <returns>
        /// <c>true</c> if the preview was successfully created (or was already available), and it is available as
        /// <paramref name="thumbnailResource"/>; otherwise, <c>false</c> (and the value of thumbnailResource is not
        /// meaningful.
        /// </returns>
        bool EnsurePreview(ResourceInfoDataViewModel resourceInfo, out IResource thumbnailResource);

        /// <summary>
        /// Asynchronously uploads a resource to the server.
        /// </summary>
        /// <param name="resource">
        /// The resource to upload.
        /// </param>
        /// <returns>
        /// The <see cref="Resource"/> from server.
        /// </returns>
        Task<Resource> UploadResourceAsync(Resource resource);

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        Task<bool> TestResourceAsync(string hash);

        /// <summary>
        /// The update resources led font type. This is support for old projects. New resources get this value set.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool UpdateResourcesLedFontType();
    }
}