// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageBufferProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageBufferProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// A provider for message buffer lists.
    /// </summary>
    internal interface IMessageBufferProvider
    {
        /// <summary>
        /// Gets a list of message buffers specifically created for the given session.
        /// </summary>
        /// <param name="destination">
        ///   The destination session that requests the message buffers.
        /// </param>
        /// <param name="codecId">
        /// The codec identification to use for encoding the message.
        /// </param>
        /// <returns>
        /// a list of message buffers specifically created for the given session.
        /// </returns>
        IEnumerable<SendMessageBuffer> GetMessageBuffers(ITransportSession destination, CodecIdentification codecId);
    }
}
