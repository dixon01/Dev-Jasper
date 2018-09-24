// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecMessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecMessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// MessageCodec implementation for BEC (Binary Enhanced Coding)
    /// </summary>
    internal class BecMessageCodec : MessageCodec<BecCodecConfig>, IManageable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<BecMessageCodec>();

        private static readonly CodecIdentification CurrentVersion = new CodecIdentification('B', 1);

        private readonly Dictionary<ISessionId, BecSerializer> serializers =
            new Dictionary<ISessionId, BecSerializer>();

        private BecCodecConfig config;

        /// <summary>
        /// Gets the identification of this codec.
        /// </summary>
        public override CodecIdentification Identification
        {
            get { return CurrentVersion; }
        }

        /// <summary>
        /// Configures this codec with the given configuration.
        /// </summary>
        /// <param name="codecConfig">
        /// The config object.
        /// </param>
        public override void Configure(BecCodecConfig codecConfig)
        {
            this.config = codecConfig;
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
            return new BecMessageBufferProvider(message, this);
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
            var serializer = this.GetSerializer(readResult.Session, readResult.CodecId);

            try
            {
                return serializer.Deserialize(buffer) as MediMessage;
            }
            catch (EndOfStreamException)
            {
                Logger.Debug("Got incomplete message, waiting for more data");
                return null;
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var serializer in this.serializers)
            {
                yield return parent.Factory.CreateManagementProvider(
                    serializer.Key.ToString(), parent, serializer.Value);
            }
        }

        private BecSerializer GetSerializer(ITransportSession session, CodecIdentification codecId)
        {
            BecSerializer serializer;
            lock (this.serializers)
            {
                if (!this.serializers.TryGetValue(session.SessionId, out serializer))
                {
                    serializer = new BecSerializer(
                        this.config, session.FrameController, (codecId ?? CurrentVersion).Version);
                    this.serializers.Add(session.SessionId, serializer);

                    // remove serializer when the session is disconnected
                    session.Disconnected += (sender, args) =>
                    {
                        lock (this.serializers)
                        {
                            this.serializers.Remove(session.SessionId);
                        }
                    };
                }
            }

            return serializer;
        }

        private class BecMessageBufferProvider : IMessageBufferProvider
        {
            private readonly MediMessage message;

            private readonly BecMessageCodec codec;

            public BecMessageBufferProvider(MediMessage message, BecMessageCodec codec)
            {
                this.message = message;
                this.codec = codec;
            }

            public IEnumerable<SendMessageBuffer> GetMessageBuffers(
                ITransportSession destination, CodecIdentification codecId)
            {
                var serializer = this.codec.GetSerializer(destination, codecId);

                return serializer.Serialize(this.message);
            }
        }
    }
}
