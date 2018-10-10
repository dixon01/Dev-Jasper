// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceServiceImpl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceServiceImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System;

    /// <summary>
    /// Internal interface to be implemented by all implementations of
    /// <see cref="IResourceService"/>. Provides methods not supposed for
    /// public use.
    /// </summary>
    internal interface IResourceServiceImpl : IResourceService
    {
        /// <summary>
        /// Begins adding a resource to the service being received over a Medi connection.
        /// This method can be asynchronous, but you won't be notified when the announcement was handled completely.
        /// </summary>
        /// <param name="source">
        ///     The original source of the announcement or <see cref="MediAddress.Empty"/> if unknown.
        /// </param>
        /// <param name="destination">
        ///     The original destination of the announcement or <see cref="MediAddress.Empty"/> if unknown.
        /// </param>
        /// <param name="announcement">
        ///     The resource announcement with the information about the resource.
        /// </param>
        void AnnounceResource(MediAddress source, MediAddress destination, ResourceAnnouncement announcement);

        /// <summary>
        /// Begins to get the status of a resource asynchronously.
        /// </summary>
        /// <param name="id">
        /// The resource id.
        /// </param>
        /// <param name="callback">
        /// The async callback.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// An async result to be used with <see cref="EndGetResourceStatus"/>.
        /// </returns>
        IAsyncResult BeginGetResourceStatus(ResourceId id, AsyncCallback callback, object state);

        /// <summary>
        /// Completes the asynchronous call started by <see cref="BeginGetResourceStatus"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginGetResourceStatus"/>.
        /// </param>
        /// <returns>
        /// The status of the resource.
        /// </returns>
        ResourceStatus EndGetResourceStatus(IAsyncResult result);

        /// <summary>
        /// Creates a download handler for the given id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IDownloadHandler"/>.
        /// </returns>
        IDownloadHandler CreateDownloadHandler(ResourceId id);

        /// <summary>
        /// Creates an upload handler for the given id.
        /// </summary>
        /// <param name="id">
        /// The resource id.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <returns>
        /// The <see cref="IUploadHandler"/>.
        /// </returns>
        IUploadHandler CreateUploadHandler(ResourceId id, MediAddress destination);
    }
}