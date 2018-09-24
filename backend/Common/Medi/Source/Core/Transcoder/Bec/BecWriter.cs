// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecWriter.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.IO;

    /// <summary>
    /// Writes to a stream using BEC (Binary Enhanced Coding).
    /// </summary>
    public class BecWriter
    {
        private readonly BinaryWriter output;

        private readonly IMapper<string> stringMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BecWriter"/> class.
        /// </summary>
        /// <param name="output">
        /// The output stream to be written to.
        /// </param>
        /// <param name="stringMapper">
        /// The string mapper used for writing strings.
        /// </param>
        internal BecWriter(Stream output, IMapper<string> stringMapper)
        {
            this.output = new BinaryWriter(output);
            this.stringMapper = stringMapper;
        }

        /// <summary>
        /// Writes an array of bytes.
        /// The method writes an integer representing the number of bytes first
        /// and then writes that number of bytes.
        /// </summary>
        /// <param name="buffer">
        /// The buffer to be written.
        /// </param>
        public void WriteBytes(byte[] buffer)
        {
            this.WriteUInt32((uint)buffer.Length);
            this.output.Write(buffer);
        }

        /// <summary>
        /// Writes a signed byte to the underlying stream.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteSByte(sbyte value)
        {
            this.output.Write(value);
        }

        /// <summary>
        /// Writes a signed short to the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="WriteInt64"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteInt16(short value)
        {
            this.WriteInt64(value);
        }

        /// <summary>
        /// Writes a signed integer to the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="WriteInt64"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteInt32(int value)
        {
            this.WriteInt64(value);
        }

        /// <summary>
        /// <para>
        /// Writes a signed long integer to the underlying stream.
        /// The format tries to save as much space as possible.
        /// The highest bit of every byte written defines if there
        /// are more bytes coming.
        /// The second highest bit of the first byte is set if the
        /// value is to be inverted (2's complement) to represent a
        /// negative number.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="table">
        /// <item>
        /// <term>19</term>
        /// <description>Binary: 0001'0011 --&gt; 0001'0011</description>
        /// </item>
        /// <item>
        /// <term>-55</term>
        /// <description>
        /// Binary of 2's complement of -55 = 54: 0011'0110 --&gt; 0111'0110
        /// (second bit from the left says it is negative)
        /// </description>
        /// </item>
        /// <item>
        /// <term>165</term>
        /// <description>
        /// Binary: 1010'0101 --&gt; 1010'0101 0000'0010
        /// (only 6 bits can be represented in the first byte, therefore the first bit is set and a second byte follows)
        /// </description>
        /// </item>
        /// <item>
        /// <term>543</term>
        /// <description>Binary: 0000'0010 0001'1111 --&gt; 1001'1111 0000'1000</description>
        /// </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteInt64(long value)
        {
            bool negative = value < 0;

            if (negative)
            {
                // inverted two's complement
                value = -(value + 1);
            }

            byte b = (byte)(value & 0x3F);

            if (negative)
            {
                // negative flag
                b |= 0x40;
            }

            value >>= 6;

            bool hasMore;
            do
            {
                if (value > 0)
                {
                    // next flag
                    b |= 0x80;
                    hasMore = true;
                }
                else
                {
                    hasMore = false;
                }

                this.output.Write(b);
                b = (byte)(value & 0x7F);
                value >>= 7;
            }
            while (hasMore);
        }

        /// <summary>
        /// Writes an unsigned byte to the underlying stream.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteByte(byte value)
        {
            this.output.Write(value);
        }

        /// <summary>
        /// Writes an unsigned short to the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="WriteUInt64"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteUInt16(ushort value)
        {
            this.WriteUInt64(value);
        }

        /// <summary>
        /// Writes an unsigned integer to the underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="WriteUInt64"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteUInt32(uint value)
        {
            this.WriteUInt64(value);
        }

        /// <summary>
        /// Writes an unsigned long integer to the underlying stream.
        /// The format is the same as for <see cref="WriteInt64"/>
        /// except that the first byte doesn't contain any sign information.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteUInt64(ulong value)
        {
            bool hasMore;
            do
            {
                byte b = (byte)(value & 0x7F);
                value >>= 7;
                if (value > 0)
                {
                    // next flag
                    b |= 0x80;
                    hasMore = true;
                }
                else
                {
                    hasMore = false;
                }

                this.output.Write(b);
            }
            while (hasMore);
        }

        /// <summary>
        /// Writes a single precision floating point number
        /// to the underlying stream. This always uses
        /// 4 bytes.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteSingle(float value)
        {
            this.output.Write(value);
        }

        /// <summary>
        /// Writes a double precision floating point number
        /// to the underlying stream. This always uses
        /// 8 bytes.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteDouble(double value)
        {
            this.output.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 2-byte character to the
        /// underlying stream.
        /// For the exact binary format, refer to the documentation of
        /// <see cref="WriteUInt64"/>.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteChar(char value)
        {
            this.WriteUInt64(value);
        }

        /// <summary>
        /// Writes a boolean encoded as a single
        /// byte to the underlying stream.
        /// </summary>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public void WriteBool(bool value)
        {
            this.WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <summary>
        /// Writes a string ID as an integer (see <see cref="WriteInt32"/>)
        /// to the underlying stream using the string mapper
        /// given in the constructor.
        /// An id of 0 represents null (not an empty string!).
        /// </summary>
        /// <param name="value">
        /// The value to be written, can be null.
        /// </param>
        public void WriteString(string value)
        {
            if (value == null)
            {
                this.WriteInt32(0);
            }
            else
            {
                this.WriteInt32(this.stringMapper[value]);
            }
        }
    }
}
