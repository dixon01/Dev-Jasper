// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHeader.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The peripheral header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The peripheral header.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = HeaderSize)]
    [Serializable]
    public class PeripheralHeader : IPeripheralHeader
    {
        /// <summary>The header size.</summary>
        public const int HeaderSize = 6;

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="systemMessageType">The system message type.</param>
        /// <param name="address">The address.</param>
        /// <param name="length">The message length header plus payload and without the checksum byte.</param>
        public PeripheralHeader(byte messageType = 0, byte systemMessageType = 0, ushort address = 0, int length = HeaderSize)
        {
            this.Address = address;
            this.Length = (ushort)length;
            this.MessageType = messageType;
            this.SystemMessageType = systemMessageType;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="networkToHostUseNetworkByteOrder">The network to host use network byte order.</param>
        /// <exception cref="NotSupportedException"></exception>
        public PeripheralHeader(byte[] bytes, bool networkToHostUseNetworkByteOrder = false)
        {
            if (bytes != null && bytes.Length >= HeaderSize)
            {
                bytes = bytes.SkipWhile(m => m.Equals(DimmerConstants.PeripheralFramingByte)).Take(HeaderSize).ToArray();
                var tempHeader = bytes.FromBytes<PeripheralHeader>();

                // Swap the Length & Address if going from network byte order (a RX from hardware) to Host.
                if (networkToHostUseNetworkByteOrder)
                {
                    tempHeader = tempHeader.NetworkByteOrderToHost();
                }

                if (tempHeader != null && tempHeader.Length > 0)
                {
                    this.Length = tempHeader.Length;
                    this.Address = tempHeader.Address;
                    this.MessageType = tempHeader.MessageType;
                    this.SystemMessageType = tempHeader.SystemMessageType;
                }
                else
                {
                    throw new NotSupportedException("Invalid Bytes for Header, Length is invalid");
                }
            }
            else
            {
                throw new NotSupportedException("Invalid Bytes for Header");
            }
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader" /> class.</summary>
        public PeripheralHeader()
        {
            this.Length = HeaderSize;
            this.Address = DimmerConstants.DimmerAddress;
            this.MessageType = (byte)DimmerMessageType.Unknown;
            this.SystemMessageType = DimmerConstants.DimmerSystemMessageType;
        }

        /// <summary>Gets or sets the length.</summary>
        public ushort Length { get; set; }

        /// <summary>Gets or sets the address.</summary>
        public ushort Address { get; set; }

        /// <summary>Gets or sets the system message type.</summary>
        public byte SystemMessageType { get; set; }

        /// <summary>Gets or sets the message type.</summary>
        public byte MessageType { get; set; }
    
        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return
                $"Length=0x{this.Length:X}, Address=0x{this.Address:X}, MessageType=0x{this.MessageType:X}, SystemMessageType=0x{this.SystemMessageType:X}";
        }
    }
}