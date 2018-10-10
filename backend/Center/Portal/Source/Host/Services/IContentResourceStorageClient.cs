// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContentResourceStorageClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IContentResourceStorageClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Services
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface of the client to get the content of resources.
    /// </summary>
    public interface IContentResourceStorageClient
    {
        /// <summary>
        /// Gets the content with the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the resource to get the stream for.
        /// </param>
        /// <returns>
        /// The stream of the resource with the given hash.
        /// </returns>
        Task<Stream> GetContentAsync(string hash);
    }
}