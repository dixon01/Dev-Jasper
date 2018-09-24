// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandshakeSuccessEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HandshakeSuccessEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    using Gorba.Common.Medi.Core.Peers.Codec;

    /// <summary>
    /// Event argument for the <see cref="StreamHandshake.Connected"/>
    /// event. It contains the network stream that can be used to 
    /// communicate to the peer when the handshake was successful.
    /// </summary>
    internal class HandshakeSuccessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeSuccessEventArgs"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <param name="agreedCodec">
        /// The agreed codec.
        /// </param>
        /// <param name="features">
        /// The supported features.
        /// </param>
        internal HandshakeSuccessEventArgs(IStreamConnection connection, StreamSessionId sessionId, CodecIdentification agreedCodec, StreamFeature features)
        {
            this.Connection = connection;
            this.SessionId = sessionId;
            this.AgreedCodec = agreedCodec;
            this.Features = features;

            switch (features & StreamFeature.TypeMask)
            {
                case StreamFeature.MessagesType:
                    this.ChannelType = ChannelType.Message;
                    break;
                case StreamFeature.StreamsType:
                    this.ChannelType = ChannelType.Stream;
                    break;
            }
        }

        /// <summary>
        /// Gets the network stream that can be used to 
        /// communicate to the peer when the handshake was successful.
        /// </summary>
        public IStreamConnection Connection { get; private set; }

        /// <summary>
        /// Gets agreed session id.
        /// </summary>
        public StreamSessionId SessionId { get; private set; }

        /// <summary>
        /// Gets the agreed codec identification.
        /// </summary>
        public CodecIdentification AgreedCodec { get; private set; }

        /// <summary>
        /// Gets the features agreed upon during the handshake.
        /// </summary>
        public StreamFeature Features { get; private set; }

        /// <summary>
        /// Gets the connected channel type (this is only relevant on server side).
        /// </summary>
        public ChannelType ChannelType { get; private set; }
    }
}