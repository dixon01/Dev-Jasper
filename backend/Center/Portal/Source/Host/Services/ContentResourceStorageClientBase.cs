// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentResourceStorageClientBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Portal.Host.Services
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// The content resource storage client.
    /// </summary>
    public abstract class ContentResourceStorageClientBase : IContentResourceStorageClient
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
        public virtual Task<Stream> GetContentAsync(string hash)
        {
            return this.RetrieveContentAsync(hash);
        }

        /// <summary>
        /// The retrieve content async.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected abstract Task<Stream> RetrieveContentAsync(string hash);
    }
}