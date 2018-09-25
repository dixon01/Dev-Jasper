// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusRequestFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusRequestFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// The status request frame (0x00).
    /// This frame is sent from the master to the slave.
    /// </summary>
    public class StatusRequestFrame : LongFrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusRequestFrame"/> class.
        /// </summary>
        public StatusRequestFrame()
            : base(FunctionCode.StatusRequest)
        {
        }

        /// <summary>
        /// Reads the payload of this frame (without the command byte) from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal override void ReadPayload(FrameReader reader)
        {
            // we have no payload
        }

        /// <summary>
        /// Writes the payload of this frame (without the command byte) to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal override void WritePayload(FrameWriter writer)
        {
            // we have no payload
        }
    }
}