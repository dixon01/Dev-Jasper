// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessageBuffer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SendMessageBuffer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// A <see cref="MessageBuffer"/> that has also a frame id.
    /// This is used when encoding and sending messages to track their frame number.
    /// </summary>
    internal class SendMessageBuffer : MessageBuffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageBuffer"/> class.
        /// </summary>
        /// <param name="frameId">
        /// The frame id.
        /// </param>
        public SendMessageBuffer(uint frameId)
        {
            this.FrameId = frameId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageBuffer"/> class.
        /// </summary>
        /// <param name="frameId">
        /// The frame id.
        /// </param>
        /// <param name="size">
        /// The size of this buffer.
        /// </param>
        public SendMessageBuffer(uint frameId, int size)
            : base(size)
        {
            this.FrameId = frameId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageBuffer"/> class.
        /// </summary>
        /// <param name="frameId">
        /// The frame id.
        /// </param>
        /// <param name="buffer">
        /// A buffer that will be used in this object.
        /// </param>
        /// <param name="offset">
        /// The offset into the buffer where data can be found.
        /// </param>
        /// <param name="count">
        /// The number of bytes in the buffer, starting from offset.
        /// </param>
        public SendMessageBuffer(uint frameId, byte[] buffer, int offset, int count)
            : base(buffer, offset, count)
        {
            this.FrameId = frameId;
        }

        /// <summary>
        /// Gets the frame id.
        /// </summary>
        public uint FrameId { get; private set; }
    }
}