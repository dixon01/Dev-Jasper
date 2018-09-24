// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryFileReader.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BinaryFileReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// Helper class that allows to read data from an NTD, font or bitmap file.
    /// </summary>
    internal class BinaryFileReader : IDisposable
    {
        private const int HeaderSize = 200;

        private readonly Stream input;

        private readonly bool closeStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileReader"/> class.
        /// </summary>
        /// <param name="input">
        /// The underlying input stream.
        /// </param>
        /// <param name="closeStream">
        /// A flag indicating whether the <see cref="input"/> should be closed/disposed when this
        /// object is closed/disposed.
        /// </param>
        public BinaryFileReader(Stream input, bool closeStream)
        {
            if (!input.CanSeek)
            {
                throw new ArgumentException("Input stream must support seeking");
            }

            if (!input.CanRead)
            {
                throw new ArgumentException("Input stream must support reading");
            }

            this.input = input;
            this.closeStream = closeStream;
        }

        /// <summary>
        /// Gets the length of the underlying stream.
        /// </summary>
        public int Length
        {
            get
            {
                return (int)this.input.Length;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the file is in extended format
        /// (e.g. extended format uses 2-byte values for width and height).
        /// </summary>
        public bool ExtendedFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the file has colors (i.e. color signs).
        /// </summary>
        public bool HasColors { get; set; }

        /// <summary>
        /// Sets the position in the underlying stream at which data should be read next.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        public void SetPosition(IntPtr offset)
        {
            this.input.Seek(offset.ToInt32(), SeekOrigin.Begin);
        }

        /// <summary>
        /// Reads one byte from the underlying stream.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/> read.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method is called after the end of the stream is reached.
        /// </exception>
        public byte ReadByte()
        {
            var read = this.input.ReadByte();
            if (read < 0)
            {
                throw new EndOfStreamException();
            }

            return (byte)read;
        }

        /// <summary>
        /// Reads the given number of bytes from the underlying stream.
        /// </summary>
        /// <param name="length">
        /// The number of bytes to read.
        /// </param>
        /// <returns>
        /// The <see cref="IByteAccess"/> to get the values of the individual bytes.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method tries to read past the end of the stream.
        /// </exception>
        public IByteAccess ReadBytes(int length)
        {
            var data = new byte[length];
            var offset = 0;
            int r;
            while ((r = this.input.Read(data, offset, length - offset)) > 0 & (offset += r) < length)
            {
            }

            if (offset != length)
            {
                throw new EndOfStreamException();
            }

            return new ArrayByteAccess(data);
        }

        /// <summary>
        /// Reads an <see cref="ushort"/> from the underlying stream.
        /// This method reads first the LSB, then the MSB.
        /// </summary>
        /// <returns>
        /// The <see cref="ushort"/> read.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method tries to read past the end of the stream.
        /// </exception>
        public ushort ReadLsbUInt16()
        {
            var b1 = this.input.ReadByte();
            var b2 = this.input.ReadByte();
            if (b1 < 0 || b2 < 0)
            {
                throw new EndOfStreamException();
            }

            return (ushort)(b1 | (b2 << 8));
        }

        /// <summary>
        /// Reads an <see cref="ushort"/> from the underlying stream.
        /// This method reads first the MSB, then the LSB.
        /// </summary>
        /// <returns>
        /// The <see cref="ushort"/> read.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method tries to read past the end of the stream.
        /// </exception>
        public ushort ReadMsbUInt16()
        {
            var b1 = this.input.ReadByte();
            var b2 = this.input.ReadByte();
            if (b1 < 0 || b2 < 0)
            {
                throw new EndOfStreamException();
            }

            return (ushort)((b1 << 8) | b2);
        }

        /// <summary>
        /// Reads a 3 byte <see cref="uint"/> from the underlying stream.
        /// This method reads first the least significant byte.
        /// </summary>
        /// <returns>
        /// The <see cref="ushort"/> read.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method tries to read past the end of the stream.
        /// </exception>
        public int ReadLsbUInt24()
        {
            var b1 = this.input.ReadByte();
            var b2 = this.input.ReadByte();
            var b3 = this.input.ReadByte();
            if (b1 < 0 || b2 < 0 || b3 < 0)
            {
                throw new EndOfStreamException();
            }

            return b1 | (b2 << 8) | (b3 << 16);
        }

        /// <summary>
        /// Reads a 3 byte address from the underlying stream.
        /// This method reads first the least significant byte.
        /// </summary>
        /// <returns>
        /// The address read.
        /// </returns>
        /// <exception cref="EndOfStreamException">
        /// if this method tries to read past the end of the stream.
        /// </exception>
        public IntPtr ReadLsbUInt24Address()
        {
            var address = new IntPtr(this.ReadLsbUInt24());
            this.VerifyAdress(address);

            return address;
        }

        /// <summary>
        /// Verifies if the given address is valid inside this file.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// if the <see cref="address"/> is invalid.
        /// </exception>
        public void VerifyAdress(IntPtr address)
        {
            if (address != IntPtr.Zero && (address.ToInt32() < HeaderSize || address.ToInt64() >= this.input.Length))
            {
                throw new IndexOutOfRangeException("Address is incorrect: 0x" + address.ToInt32().ToString("X8"));
            }
        }

        /// <summary>
        /// Reads a single byte from the underlying stream and
        /// verifies that it matches the given <see cref="identifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <exception cref="FileLoadException">
        /// if the read identifier is incorrect.
        /// </exception>
        public void ExpectIdentifier(char identifier)
        {
            var id = this.ReadByte();
            if (id != identifier)
            {
                throw new FileFormatException(
                    string.Format("Expected identifier '{0}' but got '{1}'", identifier, id));
            }
        }

        /// <summary>
        /// Closes this object and the underlying stream if configured.
        /// </summary>
        public void Close()
        {
            if (this.closeStream)
            {
                this.input.Close();
            }
        }

        /// <summary>
        /// Disposes this object and the underlying stream if configured.
        /// </summary>
        public void Dispose()
        {
            if (this.closeStream)
            {
                ((IDisposable)this.input).Dispose();
            }
        }

        private class ArrayByteAccess : IByteAccess
        {
            private readonly byte[] data;

            public ArrayByteAccess(byte[] data)
            {
                this.data = data;
                this.Length = this.data.Length;
            }

            public int Length { get; private set; }

            public byte this[int index]
            {
                get
                {
                    return this.data[index];
                }
            }

            public byte[] ToArray()
            {
                return this.data;
            }
        }
    }
}