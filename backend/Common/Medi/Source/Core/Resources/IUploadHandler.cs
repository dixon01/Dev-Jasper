// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUploadHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A handler that uploads a stream message to a peer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Resources
{
    using System.IO;

    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// A handler that uploads a stream message to a peer.
    /// </summary>
    internal interface IUploadHandler
    {
        /// <summary>
        /// Gets the total bytes uploaded until now.
        /// </summary>
        int TotalBytesUploaded { get; }

        /// <summary>
        /// Writes the given message to the given stream.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="output">
        /// The output to write to.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        void Upload(StreamMessage message, Stream output, ITransportSession session);

        /// <summary>
        /// Tells the upload handler that this upload has completed
        /// (either successfully or not).
        /// </summary>
        /// <param name="successful">
        /// A flag telling whether the upload was successful.
        /// </param>
        void Complete(bool successful);
    }
}