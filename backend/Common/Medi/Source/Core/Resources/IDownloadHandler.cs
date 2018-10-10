// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDownloadHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A handler that downloads a stream message from a peer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// A handler that downloads a stream message from a peer.
    /// </summary>
    internal interface IDownloadHandler
    {
        /// <summary>
        /// Downloads the contents of the message synchronously.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="session">
        /// The session from which the message came.
        /// </param>
        void Download(StreamMessage message, ITransportSession session);
    }
}