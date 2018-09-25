// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageTransport.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageTransport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using System;

    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Streams;

    /// <summary>
    /// Abstract implementation of <see cref="IMessageTransport"/> that
    /// can be used as a base class for transport implementations.
    /// </summary>
    internal abstract class MessageTransport : IMessageTransport
    {
        /// <summary>
        /// Stops this transport.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Asynchronously reads a message from the transport.
        /// </summary>
        /// <param name="bufferProvider">
        /// The provider for the buffer to be filled.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// An async result that can be used in <see cref="EndReadMessage"/>.
        /// </returns>
        public abstract IAsyncResult BeginReadMessage(
            IReadBufferProvider bufferProvider, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the async request issued by <see cref="BeginReadMessage"/>.
        /// </summary>
        /// <param name="result">
        ///   The result returned by <see cref="BeginReadMessage"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public abstract MessageReadResult EndReadMessage(IAsyncResult result);

        /// <summary>
        /// Sends messages to the remote party.
        /// </summary>
        /// <param name="bufferProvider">
        /// The buffer provider which will be queried for message buffers.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given buffers must be written.
        /// </param>
        public abstract void WriteMessage(IMessageBufferProvider bufferProvider, ISessionId destinationSessionId);

        /// <summary>
        /// Asynchronously reads a stream from the transport.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// An async result that can be used in <see cref="IMessageTransport.EndReadStream"/>.
        /// </returns>
        public abstract IAsyncResult BeginReadStream(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the async request issued by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IMessageTransport.BeginReadStream"/>.
        /// </param>
        /// <returns>
        /// The read result including the session from which the data was received.
        /// </returns>
        public abstract StreamReadResult EndReadStream(IAsyncResult result);

        /// <summary>
        /// Sends a stream to the remote party.
        /// </summary>
        /// <param name="message">
        /// The stream message to send.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given stream message must be written.
        /// </param>
        public abstract void WriteStream(StreamMessage message, ISessionId destinationSessionId);

        /// <summary>
        /// Previews a received and decoded message.
        /// This can be used to handle and/or change any received message.
        /// If you set <see cref="message"/> to null, handling of the message
        /// will be stopped.
        /// </summary>
        /// <param name="session">
        /// The session through which the message was received.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public abstract void PreviewDecodedMessage(ITransportSession session, ref MediMessage message);
    }
}