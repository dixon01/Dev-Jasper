// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamReadResult.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamReadResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Streams
{
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;

    /// <summary>
    /// Result of the asynchronous <see cref="IMessageTransport.BeginReadStream"/>.
    /// </summary>
    internal class StreamReadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamReadResult"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        public StreamReadResult(StreamMessage message, ITransportSession session)
        {
            this.Message = message;
            this.Session = session;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public StreamMessage Message { get; private set; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public ITransportSession Session { get; private set; }
    }
}