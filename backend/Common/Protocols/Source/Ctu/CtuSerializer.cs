// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtuSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu
{
    using System.IO;

    using Gorba.Common.Protocols.Ctu.Datagram;

    /// <summary>
    /// Object tasket to serialize a CtuDatagrm object into a buffer of bytes
    /// and to deserialize a buffer of bytes to a CtuDatagram object.
    /// </summary>
    public class CtuSerializer
    {
        /// <summary>
        /// Serializes a CtuDatagram object into a right buffer of bytes (if possible).
        /// </summary>
        /// <param name="datagram">The datagram to be serialized.</param>
        /// <returns>The serialization's result or null in case of error.</returns>
        public byte[] Serialize(CtuDatagram datagram)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new BinaryWriter(ms))
                {
                    // Attention:
                    // hereafter don't remove the casts otherwise
                    // .NET will use the int type as default, and the
                    // resulting array will have a bigger size than the correct one.
                    sw.Write((byte)datagram.Header.VersionNumber);
                    sw.Write((byte)datagram.Header.Flags);
                    sw.Write((ushort)datagram.Header.SequenceNumber);
                    foreach (var triplet in datagram.Payload.Triplets)
                    {
                        triplet.WriteTo(sw);
                    }

                    sw.Flush();
                    var result = ms.ToArray();
                    return result;
                }
            }
        }

        /// <summary>
        /// Deserializes a buffer of bytes to a CtuDatagram object (if possible).
        /// </summary>
        /// <param name="input">The bytes from which perform the deserialization.</param>
        /// <returns>The deserialization's result.</returns>
        public CtuDatagram Deserialize(byte[] input)
        {
            return this.Deserialize(new MemoryStream(input));
        }

        /// <summary>
        /// Deserializes a buffer of bytes to a CtuDatagram object (if possible).
        /// </summary>
        /// <param name="input">The stream from which perform the deserialization.</param>
        /// <returns>The deserialization's result.</returns>
        public CtuDatagram Deserialize(Stream input)
        {
            var reader = new BinaryReader(input);

            var header = new Header();
            header.VersionNumber = reader.ReadByte();
            header.Flags = (HeaderFlags)reader.ReadByte();
            header.SequenceNumber = reader.ReadUInt16();

            var payload = new Payload();
            while (reader.PeekChar() >= 0)
            {
                payload.Triplets.Add(Triplet.CreateFrom(reader));
            }

            return new CtuDatagram(header, payload);
        }
    }
}
