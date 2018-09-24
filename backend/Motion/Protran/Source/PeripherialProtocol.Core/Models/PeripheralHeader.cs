// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHeader.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The Peripheral header used in the exchange with the hardware switch via RS485.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = PeripheralHeader.Size)]
    public class PeripheralHeader : IPeripheralHeader
    {
        protected ushort length;

        // Note the Protocol dictates the header is in Network Byte Order and
        /*
             typedef struct _pcpheader_t
             {
                 uint_16         length;
                 uint_16         address;
                 uint_8          systemID;
                 uint_8          messageID;
             } PACKED(PCP_HEADER);
         */

     /* Notes

     * 5.2.1	Length
        The packet length field indicates the octet length of the packet header plus the packet payload. 
        The checksum octet is not included in the length.  Packets with no payload will have a fixed length of 0x0006
        (the length of the header).  The length field byte order is Most significant byte followed by Least significant byte. 
        The maximum packet length supported is 0x6FFF (28,671) octets.
    
        5.4	CHECKSUM
        The last octet of each packet shall be the two’s compliment of the sum of the header and payload octets.  
        Simple packet error detection can be implemented by adding the checksum to the sum of all header and payload octets. 
        The 8-bit sum will be zero (neglecting the carry) for a correctly received packet. 
        If a received packet has an invalid checksum octet, the receiver should transmit a NAK packet and discard the received packet.

     */

        public const int MaxLength = 0x6FFF;

        /// <summary>The expected header size.</summary>
        public const int Size = 6;

        /// <summary>The smallest message size.</summary>
        public const int SmallestMessageSize = Size + 1;    // Frame byte + Checksum that is not include in Length

        /// <summary>The default system type.</summary>
        public const PeripheralSystemMessageType DefaultSystemType = PeripheralSystemMessageType.AudioGeneration3;

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class. Initializes a new instance of the <see cref="PeripheralHeader"/> struct. Initializes a new instance of the <see cref="PeripheralHeader"/> class. Initializes a new instance of the <see cref="PrephialHeader"/> class.</summary>
        /// <param name="messageType">The message id.</param>
        /// <param name="length">The length which is the Header + Payload and excludes the trailing checksum octet. on TX/RX</param>
        /// <param name="systemMessage">The system id.</param>
        /// <param name="address">The address.</param>
        public PeripheralHeader(
            PeripheralMessageType messageType, 
            int length = Size,  // default size to match the class marshal size
            PeripheralSystemMessageType systemMessage = DefaultSystemType, 
            ushort address = Constants.DefaultPeripheralAddress)
        {
            this.Address = address;
            this.Length = (ushort)length;
            this.MessageType = messageType;
            this.SystemType = systemMessage;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class. Initializes a new instance of the <see cref="PeripheralHeader"/> struct.</summary>
        public PeripheralHeader()
            : this(PeripheralMessageType.Unknown, Size, DefaultSystemType, Constants.DefaultPeripheralAddress)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="headerIsNetworkByteOrder">The header Is Network Byte Order.</param>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        public PeripheralHeader(byte[] buffer, bool headerIsNetworkByteOrder = true)
        {
            var header = buffer.GetValidPeripheralHeader(headerIsNetworkByteOrder);
            if (header != null)
            {
                this.Address = header.Address;
                this.Length = header.length;
                this.MessageType = header.MessageType;
                this.SystemType = header.SystemType;
            }
            else
            {
                throw new ArgumentException("Invalid Buffer for PeripheralHeader!", nameof(buffer));
            }
        }

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Equals(object obj)
        {
            var dest = obj as PeripheralHeader;
            return
                this.Address == dest.Address && this.Length == dest.Length && this.SystemType == dest.SystemType && this.MessageType ==
                dest.MessageType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Gets or sets the packet length which is the header plus payload and excludes the terminating checksum byte. 
        /// Packets with no payload will have a fixed length of size bytes. The length field byte order is Most significant byte
        /// followed byte Least significant byte. Max packet length is 0x6FFF
        /// The Length property of the packet header is the header size and payload (not including checksum)</summary>
        /// <exception cref="InvalidOperationException" accessor="set">Value exceeds max value {0} for property: Length of {1}</exception>
        [Order(0)]
        public ushort Length
        {
            get
            {
                return this.length;
            }
            set
            {
                if (value <= MaxLength)
                {
                    this.length = value;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Value exceeds max value {0} for property: Length of {1}", value, MaxLength));
                }
            }
        }

        /// <summary>Gets or sets the address. The address field byte order is Most significant byte
        /// followed byte Least significant byte.</summary>
        [Order(1)]
        public ushort Address { get; set; }

        /// <summary>Gets or sets the system type. The System ID specifies the specific peripheral system interface for the packet. 
        ///  By supporting multiple system interface types, the PCP can be extended as required for new peripheral and/or controller systems. 
        ///  Common packet types such as POLL, DATA, ACK, etc. will contain a System ID of 0x00
        /// </summary>
        [Order(2)]
        public PeripheralSystemMessageType SystemType { get; set; }

        /// <summary>Gets or sets the message type. The Message ID indicates the packet type being transmitted. 
        ///  The Message ID is specific to the System ID given.  
        /// Thus System ID 0x07 and Message ID 0x03 is a different packet type than System ID 0x08 and Message ID 0x03.</summary>
        [Order(3)]
        public PeripheralMessageType MessageType { get; set; }

        public override string ToString()
        {
            return string.Format(
                "PeripheralHeader: Length={0}, Address=0x{1:X}, SystemType={2}, MessageType={3}",
                this.length,
                this.Address,
                this.SystemType,
                this.MessageType);
        }

        /// <summary>Attempt to create from bytes.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="PeripheralHeader"/>A header entity else null</returns>
        public static PeripheralHeader Create(byte[] bytes)
        {
            try
            {
                return new PeripheralHeader(bytes);
            }
            catch
            {
                return null;
            }
        }
    }
}