// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecReader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.IO;

    /// <summary>
    /// Reads from a stream using BEC (Binary Enhanced Coding).
    /// </summary>
    public class BecReader
    {
        private readonly BinaryReader input;

        private readonly IMapper<string> stringMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BecReader"/> class.
        /// </summary>
        /// <param name="input">
        /// The input stream to be read from.
        /// </param>
        /// <param name="stringMapper">
        /// The string mapper used for reading strings.
        /// </param>
        internal BecReader(Stream input, IMapper<string> stringMapper)
        {
            this.input = new BinaryReader(input);
            this.stringMapper = stringMapper;
        }

        /// <summary>
        /// Reads an array of bytes.
        /// The method reads an int first and then reads that number of
        /// bytes.
        /// </summary>
        /// <returns>
        /// a new byte array read from the underlying stream.
        /// </returns>
        public byte[] ReadBytes()
        {
            var count = (int)this.ReadUInt32();
            return this.input.ReadBytes(count);
        }

        /// <summary>
        /// Reads a signed byte from the underlying stream.
        /// </summary>
        /// <returns>
        /// the singed byte read.
        /// </returns>
        public sbyte ReadSByte()
        {
            return this.input.ReadSByte();
        }

        /// <summary>
        /// Reads a signed short from the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="ReadInt64"/>.
        /// </summary>
        /// <returns>
        /// the read signed short.
        /// </returns>
        public short ReadInt16()
        {
            return (short)this.ReadInt64();
        }

        /// <summary>
        /// Reads a signed integer from the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="ReadInt64"/>.
        /// </summary>
        /// <returns>
        /// the read signed integer.
        /// </returns>
        public int ReadInt32()
        {
            return (int)this.ReadInt64();
        }

        /// <summary>
        /// <para>
        /// Reads a signed long integer from the underlying stream.
        /// The format tries to save as much space as possible.
        /// The highest bit of every byte read defines if there
        /// are more bytes coming.
        /// The second highest bit of the first byte is set if the
        /// value is to be inverted (2's complement) to get a negative
        /// number.
        /// </para>
        /// <para>
        /// All read bits are the OR-ed together shifting each bit group
        /// by its position in the stream.
        /// </para>
        /// <para>
        /// For examples see <see cref="BecWriter.WriteInt64"/>
        /// </para>
        /// </summary>
        /// <returns>
        /// the read signed long integer.
        /// </returns>
        public long ReadInt64()
        {
            byte b = this.input.ReadByte();

            bool negative = (b & 0x40) > 0;
            long result = b & 0x3F;

            int shift = 6;
            while ((b & 0x80) > 0)
            {
                b = this.input.ReadByte();

                result |= ((long)b & 0x7F) << shift;
                shift += 7;
            }

            if (negative)
            {
                // inverted two's complement
                result = -result - 1;
            }

            return result;
        }

        /// <summary>
        /// Reads an unsigned byte from the underlying stream.
        /// </summary>
        /// <returns>
        /// the unsigned byte read.
        /// </returns>
        public byte ReadByte()
        {
            return this.input.ReadByte();
        }

        /// <summary>
        /// Reads an unsigned short from the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="ReadUInt64"/>.
        /// </summary>
        /// <returns>
        /// the unsigned short read.
        /// </returns>
        public ushort ReadUInt16()
        {
            return (ushort)this.ReadUInt64();
        }

        /// <summary>
        /// Reads an unsigned integer from the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="ReadUInt64"/>.
        /// </summary>
        /// <returns>
        /// the unsigned integer read.
        /// </returns>
        public uint ReadUInt32()
        {
            return (uint)this.ReadUInt64();
        }

        /// <summary>
        /// Reads an unsigned long integer from the underlying stream.
        /// The format is the same as for <see cref="ReadInt64"/>
        /// except that the first byte doesn't contain any sign information.
        /// </summary>
        /// <returns>
        /// the unsigned long integer read.
        /// </returns>
        public ulong ReadUInt64()
        {
            byte b;
            ulong result = 0;

            int shift = 0;
            do
            {
                b = this.input.ReadByte();

                result |= ((ulong)b & 0x7F) << shift;
                shift += 7;
            }
            while ((b & 0x80) > 0);

            return result;
        }

        /// <summary>
        /// Reads a single precision floating point number
        /// from the underlying stream. This always uses
        /// 4 bytes.
        /// </summary>
        /// <returns>
        /// the read single precision floating point number.
        /// </returns>
        public float ReadSingle()
        {
            return this.input.ReadSingle();
        }

        /// <summary>
        /// Reads a double precision floating point number
        /// from the underlying stream. This always uses
        /// 8 bytes.
        /// </summary>
        /// <returns>
        /// the read double precision floating point number.
        /// </returns>
        public double ReadDouble()
        {
            return this.input.ReadDouble();
        }

        /// <summary>
        /// Reads an unsigned 2-byte character from the
        /// underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="ReadUInt64"/>.
        /// </summary>
        /// <returns>
        /// the character read.
        /// </returns>
        public char ReadChar()
        {
            return (char)this.ReadUInt64();
        }

        /// <summary>
        /// Reads a boolean encoded as a single
        /// byte from the underlying stream.
        /// </summary>
        /// <returns>
        /// the boolean read.
        /// </returns>
        public bool ReadBool()
        {
            return this.ReadByte() != 0;
        }

        /// <summary>
        /// Reads a string ID as an integer (see <see cref="ReadInt32"/>)
        /// from the underlying stream  and maps it with the string mapper
        /// given in the constructor.
        /// An id of 0 represents null (not an empty string!).
        /// </summary>
        /// <returns>
        /// the mapped string matching the ID read from the stream or
        /// null if the ID was 0.
        /// </returns>
        public string ReadString()
        {
            int id = this.ReadInt32();
            if (id == 0)
            {
                return null;
            }

            return this.stringMapper[id];
        }
    }
}
