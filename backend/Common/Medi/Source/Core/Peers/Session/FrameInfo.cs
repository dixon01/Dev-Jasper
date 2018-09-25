// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    /// <summary>
    /// Frame information used as a header to verify frame sequence and acknowledges.
    /// </summary>
    public class FrameInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameInfo"/> class.
        /// </summary>
        /// <param name="sendFrameId">
        /// The send frame id.
        /// </param>
        /// <param name="ackFrameId">
        /// The acknowledge frame id.
        /// </param>
        public FrameInfo(uint sendFrameId, uint ackFrameId)
        {
            this.SendFrameId = sendFrameId;
            this.AckFrameId = ackFrameId;
        }

        /// <summary>
        /// Gets the send frame id.
        /// This is the frame counter on the sender side.
        /// </summary>
        public uint SendFrameId { get; private set; }

        /// <summary>
        /// Gets the acknowledge frame id.
        /// This is the frame counter of the last received frame.
        /// </summary>
        public uint AckFrameId { get; private set; }
    }
}