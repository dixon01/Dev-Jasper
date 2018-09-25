// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicMessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicMessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Dynamic
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// This message codec dynamically switches between supported codecs
    /// </summary>
    internal class DynamicMessageCodec : MessageCodec<DynamicCodecConfig>, IManageable
    {
        private readonly List<IMessageCodec> codecs = new List<IMessageCodec>();

        private readonly Dictionary<ISessionId, IMessageCodec> codecMapping =
            new Dictionary<ISessionId, IMessageCodec>();

        /// <summary>
        /// Gets the identification of this codec.
        /// This method always throws a NotSupportedException
        /// since it can't be used as a codec in a client, it is only
        /// supported for servers that have some kind of handshake with
        /// the client, where the Codec is defined.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Always throws a NotSupportedException.
        /// </exception>
        public override CodecIdentification Identification
        {
            get
            {
                throw new NotSupportedException("DynamicMessageCodec is only supported for TransportServers.");
            }
        }

        /// <summary>
        /// Configures this codec with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public override void Configure(DynamicCodecConfig config)
        {
            this.codecs.Clear();

            foreach (var codecInfo in config.SupportedCodecs)
            {
                var codec = Activator.CreateInstance(codecInfo.CodecType) as IMessageCodec;
                if (codec == null)
                {
                    // todo: log error
                    continue;
                }

                foreach (var iface in codecInfo.CodecType.GetInterfaces())
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IConfigurable<>))
                    {
                        var configType = iface.GetGenericArguments()[0];
                        if (configType.IsInstanceOfType(codecInfo.Config))
                        {
                            codecInfo.CodecType.GetMethod("Configure", new[] { configType }).Invoke(
                                codec, new object[] { codecInfo.Config });
                        }

                        break;
                    }
                }

                this.codecs.Add(codec);
            }
        }

        /// <summary>
        /// Checks if this codec supports the given identification for the given session ID.
        /// This method checks all supported Codecs and returns the identification of the codec that
        /// supports the given identification.
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
        public override CodecIdentification CheckSupport(ISessionId sessionId, CodecIdentification identification)
        {
            foreach (var codec in this.codecs)
            {
                var id = codec.CheckSupport(sessionId, identification);
                if (id == null)
                {
                    continue;
                }

                if (!this.codecMapping.ContainsKey(sessionId))
                {
                    this.codecMapping[sessionId] = codec;
                }

                return id;
            }

            return null;
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
            return new DynamicMessageBufferProvider(this, message);
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
            IMessageCodec codec;
            if (!this.codecMapping.TryGetValue(readResult.Session.SessionId, out codec))
            {
                return null;
            }

            return codec.Decode(buffer, readResult);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var codec in this.codecs)
            {
                yield return parent.Factory.CreateManagementProvider(
                    codec.GetType().Name, parent, codec);
            }
        }

        private class DynamicMessageBufferProvider : IMessageBufferProvider
        {
            private readonly DynamicMessageCodec dynamicCodec;

            private readonly MediMessage message;

            public DynamicMessageBufferProvider(DynamicMessageCodec codec, MediMessage message)
            {
                this.dynamicCodec = codec;
                this.message = message;
            }

            public IEnumerable<SendMessageBuffer> GetMessageBuffers(
                ITransportSession destination, CodecIdentification codecId)
            {
                IMessageCodec codec;
                if (!this.dynamicCodec.codecMapping.TryGetValue(destination.SessionId, out codec))
                {
                    return null;
                }

                var provider = codec.Encode(this.message);
                if (provider == null)
                {
                    return null;
                }

                return provider.GetMessageBuffers(destination, codecId);
            }
        }
    }
}
