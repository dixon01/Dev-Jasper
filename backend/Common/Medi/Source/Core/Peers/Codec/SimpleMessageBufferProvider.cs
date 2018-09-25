// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMessageBufferProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleMessageBufferProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// <see cref="IMessageBufferProvider"/> that just provides a single
    /// <see cref="MessageBuffer"/>, independent of the session querying
    /// this provider.
    /// </summary>
    internal class SimpleMessageBufferProvider : IMessageBufferProvider
    {
        private readonly SendMessageBuffer buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMessageBufferProvider"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to be returned by <see cref="GetMessageBuffers"/>
        /// </param>
        public SimpleMessageBufferProvider(SendMessageBuffer buffer)
        {
            this.buffer = buffer;
        }

        /// <summary>
        /// Returns exactly one <see cref="MessageBuffer"/>, the one given in the constructor.
        /// </summary>
        /// <param name="destination">
        ///   This parameter is ignored.
        /// </param>
        /// <param name="codecId">
        /// The codec identification to use for encoding the message.
        /// </param>
        /// <returns>
        /// Exactly one <see cref="MessageBuffer"/>.
        /// </returns>
        public IEnumerable<SendMessageBuffer> GetMessageBuffers(
            ITransportSession destination, CodecIdentification codecId)
        {
            // ignore the destination, we just return the given buffer
            yield return this.buffer;
        }
    }
}
