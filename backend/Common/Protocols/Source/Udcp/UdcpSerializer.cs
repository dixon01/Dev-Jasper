// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System.IO;

    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;

    /// <summary>
    /// Class to serialize and deserialize UDCP datagrams.
    /// </summary>
    public class UdcpSerializer
    {
        private static readonly byte[] Magic = { (byte)'U', (byte)'D', (byte)'C', (byte)'P' };

        /// <summary>
        /// Serializes the given datagram to a byte array.
        /// </summary>
        /// <param name="datagram">
        /// The datagram to be serialized.
        /// </param>
        /// <returns>
        /// The byte array.
        /// </returns>
        public byte[] Serialize(UdcpDatagram datagram)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(Magic);
                    datagram.Header.WriteTo(writer);
                    foreach (var field in datagram.Fields)
                    {
                        field.WriteTo(writer);
                    }

                    writer.Flush();
                    var result = ms.ToArray();
                    return result;
                }
            }
        }

        /// <summary>
        /// Deserializes the given byte array into a <see cref="UdcpDatagram"/>.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// An object of a subclass of <see cref="UdcpDatagram"/>.
        /// </returns>
        public UdcpDatagram Deserialize(byte[] input)
        {
            return this.Deserialize(new MemoryStream(input));
        }

        /// <summary>
        /// Deserializes an entire <see cref="UdcpDatagram"/> from the given stream.
        /// </summary>
        /// <param name="input">
        /// The input stream.
        /// </param>
        /// <returns>
        /// An object of a subclass of <see cref="UdcpDatagram"/>.
        /// </returns>
        public UdcpDatagram Deserialize(Stream input)
        {
            var reader = new BinaryReader(input);

            foreach (var magicChar in Magic)
            {
                var read = reader.ReadByte();
                if (read != magicChar)
                {
                    throw new UdcpException(
                        string.Format("Bad magic, expected {0:X2} but got {1:X2}", magicChar, read));
                }
            }

            var header = new Header();
            header.ReadFrom(reader);

            UdcpDatagram datagram;
            if ((header.Flags & HeaderFlags.Response) == 0)
            {
                datagram = new UdcpRequest(header);
            }
            else
            {
                datagram = new UdcpResponse(header);
            }

            while (reader.PeekChar() >= 0)
            {
                datagram.Fields.Add(Field.CreateFrom(reader));
            }

            return datagram;
        }
    }
}