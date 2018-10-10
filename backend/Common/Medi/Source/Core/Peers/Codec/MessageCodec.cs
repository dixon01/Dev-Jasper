// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Abstract base class for message codecs.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of the configuration.
    /// </typeparam>
    internal abstract class MessageCodec<TConfig> : IMessageCodec, IConfigurable<TConfig>
        where TConfig : CodecConfig
    {
        /// <summary>
        /// Gets the identification of this codec.
        /// </summary>
        public abstract CodecIdentification Identification { get; }

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
        public virtual CodecIdentification CheckSupport(ISessionId sessionId, CodecIdentification identification)
        {
            var id = this.Identification;
            if (id.Name != identification.Name)
            {
                return null;
            }

            if (id.Version < identification.Version)
            {
                // we were requested a higher version than supported, let's downgrade
                return new CodecIdentification(id.Name, id.Version);
            }

            return identification;
        }

        /// <summary>
        /// Configures this codec with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public abstract void Configure(TConfig config);

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
        public abstract IMessageBufferProvider Encode(MediMessage message);

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
        /// the decoded message or null if the buffer was only a part of a message.
        /// </returns>
        public abstract MediMessage Decode(MessageBuffer buffer, MessageReadResult readResult);
    }
}
