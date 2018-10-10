// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Message codec that transforms <see cref="MediMessage"/>s
    /// to <see cref="MessageBuffer"/>s and vice versa.
    /// </summary>
    internal interface IMessageCodec
    {
        /// <summary>
        /// Gets the identification of this codec.
        /// </summary>
        CodecIdentification Identification { get; }

        /// <summary>
        /// Checks if this codec supports the given identification for the given session ID.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <param name="identification">
        /// The identification.
        /// </param>
        /// <returns>
        /// The used codec identification (its version number can be lower or equal to
        /// the one given in the argument) or null if this codec doesn't support the
        /// given codec identification.
        /// </returns>
        CodecIdentification CheckSupport(ISessionId sessionId, CodecIdentification identification);

        /// <summary>
        /// Converts the message to buffers. More precisely creates a
        /// provider that can be queried for the message buffers.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// a message buffer provider.
        /// </returns>
        IMessageBufferProvider Encode(MediMessage message);

        /// <summary>
        /// Converts a buffer from a certain session to a message.
        /// </summary>
        /// <param name="buffer">
        /// The buffer. The bytes consumed by this method are removed from the message buffer, but
        /// the message buffer might not be empty after this method returns.
        /// </param>
        /// <param name="readResult">
        /// The read result including the session which received the message buffer.
        /// </param>
        /// <returns>
        /// the decoded message or null if the buffer was only a part of a message.
        /// </returns>
        MediMessage Decode(MessageBuffer buffer, MessageReadResult readResult);
    }
}
