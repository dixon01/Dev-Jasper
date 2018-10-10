// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Header.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Header type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp.Datagram
{
    using System.IO;

    /// <summary>
    /// The UDCP header which resides at position 0 inside the UDP payload.
    /// The header consists of 8 bytes (1 byte flags, 1 byte type, 6 bytes unit address).
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        internal Header()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        /// <param name="flags">
        /// The header flags.
        /// </param>
        /// <param name="type">
        /// The datagram type.
        /// </param>
        /// <param name="unitAddress">
        /// The unit address.
        /// </param>
        internal Header(HeaderFlags flags, DatagramType type, UdcpAddress unitAddress)
        {
            this.Flags = flags;
            this.Type = type;
            this.UnitAddress = unitAddress;
        }

        /// <summary>
        /// Gets the header flags (first byte in the header).
        /// </summary>
        public HeaderFlags Flags { get; private set; }

        /// <summary>
        /// Gets the type of the datagram.
        /// </summary>
        public DatagramType Type { get; private set; }

        /// <summary>
        /// Gets the MAC address of the unit.
        /// For <see cref="UdcpRequest"/> this is the address of the unit to which the message is sent,
        /// in this case it can be <see cref="UdcpAddress.BroadcastAddress"/> which means broadcast to
        /// every unit in the current subnet.
        /// For <see cref="UdcpResponse"/> this is the address of the unit from which the message comes.
        /// </summary>
        public UdcpAddress UnitAddress { get; private set; }

        /// <summary>
        /// Writes the contents of this header to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)this.Flags);
            writer.Write((byte)this.Type);
            writer.Write(this.UnitAddress.GetAddressBytes());
        }

        /// <summary>
        /// Reads from the given reader and sets the contents of this header.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal void ReadFrom(BinaryReader reader)
        {
            this.Flags = (HeaderFlags)reader.ReadByte();
            this.Type = (DatagramType)reader.ReadByte();
            this.UnitAddress = new UdcpAddress(reader.ReadBytes(6));
        }
    }
}