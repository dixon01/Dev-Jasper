// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageReadResult.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageReadResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// Result of the asynchronous <see cref="IMessageTransport.BeginReadMessage"/>.
    /// </summary>
    internal class MessageReadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReadResult"/> class.
        /// </summary>
        /// <param name="bytesRead">
        /// The number of bytes read.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="codecId">
        /// The codec id.
        /// </param>
        public MessageReadResult(int bytesRead, ITransportSession session, CodecIdentification codecId)
        {
            this.CodecId = codecId;
            this.Session = session;
            this.BytesRead = bytesRead;
        }

        /// <summary>
        /// Gets the number of bytes read.
        /// </summary>
        public int BytesRead { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public ITransportSession Session { get; private set; }

        /// <summary>
        /// Gets the codec id.
        /// </summary>
        public CodecIdentification CodecId { get; private set; }
    }
}