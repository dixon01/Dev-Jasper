// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameReader.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System.IO;

    /// <summary>
    /// Class for reading frames from a stream.
    /// </summary>
    internal class FrameReader
    {
        private readonly Stream input;

        private byte checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameReader"/> class.
        /// </summary>
        /// <param name="input">
        /// The input stream to read from.
        /// This stream must support getting the <see cref="Stream.Position"/> and <see cref="Stream.Length"/>.
        /// </param>
        /// <param name="isHighSpeed">
        /// Flag indicating if we are reading high-speed frames.
        /// </param>
        public FrameReader(Stream input, bool isHighSpeed)
        {
            this.input = input;
            this.IsHighSpeed = isHighSpeed;

            this.checksum = 0xFF;
        }

        /// <summary>
        /// Gets a value indicating whether we are reading high-speed frames.
        /// </summary>
        public bool IsHighSpeed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there is more data available.
        /// </summary>
        public bool HasMore
        {
            get
            {
                return this.input.Position < this.input.Length;
            }
        }

        /// <summary>
        /// Reads a single byte from the underlying stream.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        /// <exception cref="FrameDecodingException">
        /// if the end of the stream (EOS) was reached.
        /// To check if you can read a byte, please use <see cref="HasMore"/>.
        /// </exception>
        public byte ReadByte()
        {
            var read = this.input.ReadByte();
            if (read == -1)
            {
                throw new FrameDecodingException("Reached unexpected end of telegram");
            }

            var b = (byte)read;
            this.checksum ^= b;
            return b;
        }

        /// <summary>
        /// Reads all remaining bytes from the underlying stream.
        /// </summary>
        /// <returns>
        /// All bytes read.
        /// </returns>
        public byte[] ReadAll()
        {
            var data = new byte[this.input.Length - this.input.Position];
            var read = this.input.Read(data, 0, data.Length);
            if (read != data.Length)
            {
                throw new FrameDecodingException("Couldn't read all data");
            }

            foreach (var b in data)
            {
                this.checksum ^= b;
            }

            return data;
        }

        /// <summary>
        /// Verifies that the calculated checksum matches the <see cref="expected"/>.
        /// This method should only be called by the <see cref="FrameDecoder"/> when
        /// it finished decoding the message.
        /// </summary>
        /// <param name="expected">
        /// The expected checksum.
        /// </param>
        /// <exception cref="FrameDecodingException">
        /// If the checksums don't match.
        /// </exception>
        public void VerifyChecksum(byte expected)
        {
            if (this.checksum == expected)
            {
                return;
            }

            var msg = string.Format(
                "Wrong checksum: expected {0:X2}, but calculated {1:X2}", expected, this.checksum);
            throw new FrameDecodingException(msg);
        }
    }
}