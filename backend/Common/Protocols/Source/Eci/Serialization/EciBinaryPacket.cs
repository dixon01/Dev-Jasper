// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciBinaryPacket.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Serialization
{
    using System;
    using System.Text;

    using Gorba.Common.Protocols.Eci.Messages;

    /// <summary>
    /// The ECI binary packet.
    /// </summary>
    public class EciBinaryPacket
    {
        // The ECI protocol is LittleEndian
        private const bool LittleEndian = true;

        // Storing the system LittleEndian value
        private static readonly bool IsSystemLittleEndian = BitConverter.IsLittleEndian;

        private readonly int packetSize;
        private readonly byte[] buffer;
        private int bufferIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="EciBinaryPacket"/> class.
        /// </summary>
        /// <param name="payloadsize">
        /// The size.
        /// </param>
        public EciBinaryPacket(int payloadsize)
        {
            this.packetSize = payloadsize + 3;
            this.bufferIndex = 1;
            this.buffer = new byte[this.packetSize];
            this.buffer[0] = (byte)'<';
            this.buffer[this.packetSize - 1] = (byte)'>';
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EciBinaryPacket"/> class.
        /// </summary>
        /// <param name="buff">
        /// The buffer.
        /// </param>
        public EciBinaryPacket(byte[] buff)
        {
            this.packetSize = buff.Length;
            this.buffer = new byte[this.packetSize];
            buff.CopyTo(this.buffer, 0);
            this.bufferIndex = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EciBinaryPacket"/> class.
        /// </summary>
        /// <param name="buff">
        /// The buff.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        public EciBinaryPacket(byte[] buff, int startIndex, int endIndex)
        {
            this.packetSize = endIndex - startIndex + 1;
            this.buffer = new byte[this.packetSize];
            Array.Copy(buff, startIndex, this.buffer, 0, this.packetSize);
            this.bufferIndex = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EciBinaryPacket"/> class.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        public EciBinaryPacket(EciBinaryPacket packet)
        {
            this.buffer = new byte[packet.buffer.Length];
            packet.buffer.CopyTo(this.buffer, 0);
            this.bufferIndex = 1;
        }

        /// <summary>
        /// Gets the type code.
        /// </summary>
        public EciMessageCode TypeCode
        {
            get
            {
                return (EciMessageCode)this.buffer[1];
            }
        }

        /// <summary>
        /// Gets the sub type code.
        /// </summary>
        public byte SubTypeCode
        {
            get
            {
                return this.buffer[2];
            }
        }

        /// <summary>
        /// Gets the packet size.
        /// </summary>
        public int PacketSize
        {
            get
            {
                return this.packetSize;
            }
        }

        /// <summary>
        /// Gets the payload size.
        /// </summary>
        public int PayloadSize
        {
            get
            {
                return this.packetSize - 3;
            }
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this.buffer;
            }
        }

        /// <summary>
        /// The compute checksum.
        /// </summary>
        /// <param name="buff">
        /// The buff.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="endIndex">
        /// The end index.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public static byte ComputeChecksum(byte[] buff, int startIndex, int endIndex)
        {
            byte cc = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                cc ^= buff[i];
            }

            return cc;
        }

        /// <summary>
        /// Returns an array of requested size from the current offset.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ParseBytes(int size)
        {
            var ba = new byte[size];
            Array.Copy(this.buffer, this.bufferIndex, ba, 0, size);
            return ba;
        }

        /// <summary>
        /// The append bytes.
        /// </summary>
        /// <param name="buff">
        /// The buff.
        /// </param>
        public void AppendBytes(byte[] buff)
        {
            Array.Copy(buff, this.buffer, buff.Length);
        }

        /// <summary>
        /// Computes and set the checksum.
        /// </summary>
        public void SetCheckSum()
        {
            this.buffer[this.packetSize - 2] = ComputeChecksum(this.buffer, 0, this.packetSize - 3);
        }

        /// <summary>
        /// Validate the packet checksum.
        /// </summary>
        /// <returns>
        /// Returns true if the checksum is valid. <see cref="bool"/>.
        /// </returns>
        public bool ValidateCheckSum()
        {
            return this.buffer[this.packetSize - 2] == ComputeChecksum(this.buffer, 0, this.packetSize - 3);
        }

        /// <summary>
        /// The append byte.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendByte(int value)
        {
            this.buffer[this.bufferIndex++] = (byte)value;
        }

        /// <summary>
        /// The append byte.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendByte(byte value)
        {
            this.buffer[this.bufferIndex++] = value;
        }

/*        public void AppendByte<T>(T val) where T: struct, IComparable, IConvertible, IFormattable
        {
            this.buffer[this.bufferIndex++] = val.ToType();
        }*/

        /// <summary>
        /// The parse byte.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte ParseByte()
        {
            return this.buffer[this.bufferIndex++];
        }

        /// <summary>
        /// The append a short value
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendUShort(int value)
        {
            byte[] shortBytes = BitConverter.GetBytes((ushort)value);
            CorrectByteOrder(shortBytes);
            shortBytes.CopyTo(this.buffer, this.bufferIndex);
            this.bufferIndex += sizeof(short);
        }

        /// <summary>
        /// The parse u short.
        /// </summary>
        /// <returns>
        /// The <see cref="ushort"/>.
        /// </returns>
        public ushort ParseUShort()
        {
            var shortBytes = this.ParseBytes(2);
            CorrectByteOrder(shortBytes);
            this.bufferIndex += sizeof(short);
            return BitConverter.ToUInt16(shortBytes, 0);
        }

        /// <summary>
        /// The append int.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendInt(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            CorrectByteOrder(intBytes);
            intBytes.CopyTo(this.buffer, this.bufferIndex);
            this.bufferIndex += sizeof(int);
        }

        /// <summary>
        /// The parse int.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ParseInt()
        {
            var intBytes = this.ParseBytes(4);
            CorrectByteOrder(intBytes);
            this.bufferIndex += sizeof(int);
            return BitConverter.ToInt32(intBytes, 0);
        }

        /// <summary>
        /// The append float.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendFloat(double value)
        {
            byte[] floatBytes = BitConverter.GetBytes((float)value);
            CorrectByteOrder(floatBytes);
            floatBytes.CopyTo(this.buffer, this.bufferIndex);
            this.bufferIndex += sizeof(float);
        }

        /// <summary>
        /// The parse float.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double ParseFloat()
        {
            var floatBytes = this.ParseBytes(4);
            CorrectByteOrder(floatBytes);
            this.bufferIndex += sizeof(float);
            return BitConverter.ToSingle(floatBytes, 0);
        }

        /// <summary>
        /// The append date time.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendDateTime(DateTime value)
        {
            // TODO: [ALM] verify the correct format for date and time
            ushort date = (ushort)((value.Day & 0x1F) << 8 |
                (value.Month & 0x0F) << 4 | (value.Year & 0x0F));
            this.AppendUShort(date);
            this.AppendByte(value.Hour);
            this.AppendByte(value.Minute);
            this.AppendByte(value.Second);
        }

        /// <summary>
        /// The parse date time.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime ParseDateTime()
        {
            // TODO: [ALM] is this correct?
            var packedDate = this.ParseUShort();
            var year  = packedDate & 0x0F;
            var month = (packedDate >> 4) & 0x0F;
            var day = (packedDate >> 8) & 0x1F;
            var hour = this.ParseByte();
            var minute = this.ParseByte();
            var second = this.ParseByte();
           return new DateTime(year + 2000, month, day, hour, minute, second);
        }

        /// <summary>
        /// The append time.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AppendTime(TimeSpan value)
        {
            this.AppendByte((byte)value.Hours);
            this.AppendByte((byte)value.Minutes);
            this.AppendByte((byte)value.Seconds);
        }

        /// <summary>
        /// The parse time.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public TimeSpan ParseTime()
        {
            return new TimeSpan(
                                this.ParseByte(),
                                this.ParseByte(),
                                this.ParseByte());
        }

        /// <summary>
        /// The append string.
        /// </summary>
        /// <param name="str">
        /// A string.
        /// </param>
        public void AppendString(string str)
        {
            Encoding.ASCII.GetBytes(str).CopyTo(this.buffer, this.bufferIndex);
            this.bufferIndex += str.Length;
        }

        /// <summary>
        /// The parse string.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ParseString(int length)
        {
            var str = Encoding.ASCII.GetString(this.buffer, this.bufferIndex, length);
            this.bufferIndex += length;
            return str;
        }

        private static void CorrectByteOrder(byte[] buff)
        {
            if (IsSystemLittleEndian != LittleEndian)
            {
                Array.Reverse(buff);
            }
        }
    }
}
