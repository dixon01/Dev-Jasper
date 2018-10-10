// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameWriter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System.IO;

    /// <summary>
    /// Class for writing frames to a stream.
    /// </summary>
    internal class FrameWriter
    {
        private readonly Stream output;

        private byte checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameWriter"/> class.
        /// </summary>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        /// <param name="isHighSpeed">
        /// Flag indicating if we are writing high-speed frames.
        /// </param>
        public FrameWriter(Stream output, bool isHighSpeed)
        {
            this.output = output;
            this.IsHighSpeed = isHighSpeed;
        }

        /// <summary>
        /// Gets a value indicating whether we are writing high-speed frames.
        /// </summary>
        public bool IsHighSpeed { get; private set; }

        /// <summary>
        /// Writes a frame boundary marker (0x7E) without escaping it.
        /// </summary>
        public void WriteFrameBoundary()
        {
            this.output.WriteByte(FrameConstants.Boundary);
            this.checksum = 0xFF;
        }

        /// <summary>
        /// Writes a single byte to the underlying stream, escaping it if necessary.
        /// </summary>
        /// <param name="b">
        /// The byte to write.
        /// </param>
        public void WriteByte(byte b)
        {
            this.checksum ^= b;
            if (b == FrameConstants.Boundary || b == FrameConstants.Escape)
            {
                this.output.WriteByte(FrameConstants.Escape);
                b ^= FrameConstants.XOr;
            }

            this.output.WriteByte(b);
        }

        /// <summary>
        /// Writes an array of bytes to the underlying stream, escaping them if necessary.
        /// </summary>
        /// <param name="data">
        /// The bytes to write to the stream.
        /// </param>
        public void WriteBytes(byte[] data)
        {
            foreach (var b in data)
            {
                this.WriteByte(b);
            }
        }

        /// <summary>
        /// Writes an array of bytes to the underlying stream, escaping them if necessary.
        /// </summary>
        /// <param name="data">
        /// The bytes to write to the stream.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        public void WriteBytes(byte[] data, int offset, int length)
        {
            foreach (var b in data)
            {
                this.WriteByte(b);
            }
        }

        /// <summary>
        /// Writes the (final) checksum byte to the underlying stream, escaping it if necessary.
        /// </summary>
        public void WriteChecksum()
        {
            // this alters the checksum, but we don't care since it is not used anymore after this
            // (there is immediately a frame boundry after this byte)
            this.WriteByte(this.checksum);
        }
    }
}