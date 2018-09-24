// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisStream.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Channels
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream that calculates the VDV 300 checksum when reading from or writing to it.
    /// </summary>
    public class IbisStream : Stream
    {
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisStream"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public IbisStream(Stream stream)
        {
            this.stream = stream;
            this.ResetChecksum();
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        public byte Checksum { get; private set; }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream supports reading; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream supports seeking; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>
        /// A long value representing the length of the stream in bytes.
        /// </returns>
        /// <exception cref="NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        /// <filterpriority>1</filterpriority>
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Resets the checksum.
        /// </summary>
        public void ResetChecksum()
        {
            this.Checksum = 0x7F;
        }

        /// <summary>
        /// Writes an ASCII character to this stream.
        /// </summary>
        /// <param name="value">
        /// The ASCII character value.
        /// </param>
        public void WriteAsciiChar(char value)
        {
            this.WriteByte((byte)(value & 0x7F));
        }

        /// <summary>
        /// Writes an ASCII character to this stream.
        /// </summary>
        /// <param name="value">
        /// The decimal value.
        /// </param>
        /// <param name="digitCount">
        /// The number of digits.
        /// </param>
        public void WriteDigits(int value, int digitCount)
        {
            var multiplier = 1;
            for (var i = 1; i < digitCount; i++)
            {
                multiplier *= 10;
            }

            for (var i = digitCount; i > 0; i--)
            {
                this.WriteHexValue((value / multiplier) % 10);
                multiplier /= 10;
            }
        }

        /// <summary>
        /// Writes an IBIS hex value ('0'...'?') to this stream.
        /// </summary>
        /// <param name="value">
        /// The decimal value between 0..15.
        /// </param>
        public void WriteHexValue(int value)
        {
            this.WriteAsciiChar((char)('0' + value));
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
            this.Checksum ^= value;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte,
        /// or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        /// </returns>
        public override int ReadByte()
        {
            var read = this.stream.ReadByte();
            if (read != -1)
            {
                this.Checksum ^= (byte)read;
            }

            return read;
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and
        /// causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            this.stream.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <param name="offset">
        /// A byte offset relative to the <paramref name="origin"/> parameter.
        /// </param><param name="origin">
        /// A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.
        /// </param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">
        /// The desired length of the current stream in bytes.
        /// </param>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support both writing and seeking, such as if the
        /// stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the
        /// current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than
        /// the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains the
        /// specified byte array with the values between <paramref name="offset"/>
        /// and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
        /// by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin
        /// storing the data read from the current stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="NotSupportedException">The stream does not support reading. </exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = this.stream.Read(buffer, offset, count);
            for (var i = 0; i < read; i++)
            {
                this.Checksum ^= buffer[offset + i];
            }

            return read;
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and
        /// advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies <paramref name="count"/> bytes from
        /// <paramref name="buffer"/> to the current stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in <paramref name="buffer"/> at which to begin
        /// copying bytes to the current stream.
        /// </param>
        /// <param name="count">
        /// The number of bytes to be written to the current stream.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The sum of <paramref name="offset"/> and <paramref name="count"/> is greater than the buffer length.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="count"/> is negative.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
            for (var i = 0; i < count; i++)
            {
                this.Checksum ^= buffer[offset + i];
            }
        }
    }
}