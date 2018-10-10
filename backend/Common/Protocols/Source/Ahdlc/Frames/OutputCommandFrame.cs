// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputCommandFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OutputCommandFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// The output command frame (0x02).
    /// This frame is sent from the master to the slave.
    /// </summary>
    public class OutputCommandFrame : LongFrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandFrame"/> class.
        /// </summary>
        public OutputCommandFrame()
            : base(FunctionCode.OutputCommand)
        {
        }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public int BlockNumber { get; set; }

        /// <summary>
        /// Gets or sets the bitmap or text data (everything after the block number).
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Reads the payload of this frame (without the command byte) from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal override void ReadPayload(FrameReader reader)
        {
            if (reader.IsHighSpeed)
            {
                var b1 = reader.ReadByte();
                var b2 = reader.ReadByte();
                this.BlockNumber = (b1 << 8) | b2;
            }
            else
            {
                this.BlockNumber = reader.ReadByte();
            }

            this.Data = reader.ReadAll();
        }

        /// <summary>
        /// Writes the payload of this frame (without the command byte) to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal override void WritePayload(FrameWriter writer)
        {
            if (writer.IsHighSpeed)
            {
                writer.WriteByte((byte)(this.BlockNumber >> 8));
            }

            writer.WriteByte((byte)(this.BlockNumber & 0xFF));
            writer.WriteBytes(this.Data);
        }
    }
}