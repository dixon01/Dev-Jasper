// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadMessageRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadMessageRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using System;

    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// An <see cref="IAsyncResult"/> implementation used for reading messages.
    /// </summary>
    internal class ReadMessageRequest : AsyncResultBase
    {
        private readonly IReadBufferProvider bufferProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMessageRequest"/> class.
        /// </summary>
        /// <param name="bufferProvider">
        /// The provider for the buffer that will be filled.
        /// </param>
        /// <param name="callback">
        /// The callback that will be called once the message was read.
        /// </param>
        /// <param name="state">
        /// The user state.
        /// </param>
        public ReadMessageRequest(IReadBufferProvider bufferProvider, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.bufferProvider = bufferProvider;
        }

        /// <summary>
        /// Gets the read result including the session from which the message was received.
        /// </summary>
        public MessageReadResult ReadResult { get; private set; }

        /// <summary>
        /// Completes this request.
        /// </summary>
        /// <param name="readResult">
        /// The read result including the session from which the message was received.
        /// </param>
        /// <param name="buffer">
        /// A buffer that will be appended to the buffer provided when the
        /// request was started.
        /// </param>
        /// <param name="synchronously">
        /// A flag indicating if the request was completed synchronously.
        /// </param>
        public void Complete(MessageReadResult readResult, MessageBuffer buffer, bool synchronously)
        {
            this.ReadResult = readResult;
            this.bufferProvider.GetReadBuffer(readResult.Session).Append(buffer);

            this.Complete(synchronously);
        }
    }
}