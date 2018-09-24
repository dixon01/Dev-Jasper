// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockMessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MockMessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transport.Stream
{
    using System;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Message codec that just provides a codec ID, but doesn't actually do en/decoding.
    /// </summary>
    internal class MockMessageCodec : MessageCodec<CodecConfig>
    {
        private readonly CodecIdentification id;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockMessageCodec"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public MockMessageCodec(CodecIdentification id)
        {
            this.id = id;
        }

        /// <summary>
        /// Gets the identification of this codec.
        /// </summary>
        public override CodecIdentification Identification
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Configures this codec with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public override void Configure(CodecConfig config)
        {
        }

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
        public override IMessageBufferProvider Encode(MediMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a buffer from a certain session to a message.
        /// </summary>
        /// <param name="buffer">
        ///   The buffer. The bytes consumed by this method are removed from the message buffer, but
        ///   the message buffer might not be empty after this method returns.
        /// </param>
        /// <param name="readResult">
        /// The read result including the session which received the message buffer.
        /// </param>
        /// <returns>
        /// the decoded message or null if the buffer was only a part of 
        /// a message. 
        /// </returns>
        public override MediMessage Decode(MessageBuffer buffer, MessageReadResult readResult)
        {
            throw new NotImplementedException();
        }
    }
}