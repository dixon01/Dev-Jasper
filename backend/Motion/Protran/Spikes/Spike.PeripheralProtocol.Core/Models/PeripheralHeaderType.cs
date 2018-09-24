// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHeaderType.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralHeaderType interface.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    public interface IPeripheralHeaderType<TMessageType>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the address. The address field byte order is Most significant byte
        ///     followed byte Least significant byte.
        /// </summary>
        ushort Address { get; set; }

        /// <summary>
        ///     Gets or sets the packet length which is the header plus payload and excludes the terminating checksum byte.
        ///     Packets with no payload will have a fixed length of size bytes. The length field byte order is Most significant
        ///     byte
        ///     followed byte Least significant byte. Max packet length is 0x6FFF
        ///     The Length property of the packet header is the header size and payload (not including checksum)
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">Value exceeds max value {0} for property: Length of {1}</exception>
        ushort Length { get; set; }

        /// <summary>
        ///     Gets or sets the message type. The Message ID indicates the packet type being transmitted.
        ///     The Message ID is specific to the System ID given.
        ///     Thus System ID 0x07 and Message ID 0x03 is a different packet type than System ID 0x08 and Message ID 0x03.
        /// </summary>
        TMessageType MessageType { get; set; }

        /// <summary>
        ///     Gets or sets the system type. The System ID specifies the specific peripheral system interface for the packet.
        ///     By supporting multiple system interface types, the PCP can be extended as required for new peripheral and/or
        ///     controller systems.
        ///     Common packet types such as POLL, DATA, ACK, etc. will contain a System ID of 0x00
        /// </summary>
        PeripheralSystemMessageType SystemType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The update header to network byte order.</summary>
        void UpdateHeaderToNetworkByteOrder();

        #endregion
    }

    /// <summary>The peripheral header type.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize)]
    public abstract class PeripheralHeaderType<TMessageType> : IPeripheralHeaderType<TMessageType>
    {
        #region Notes
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
        #endregion

        /// <summary>The expected header size.</summary>
        //public const int Size = Constants.PeripheralHeaderSize;

        /// <summary>The max length.</summary>
        public const int MaxLength = 0x6FFF;

        /// <summary>The smallest message size.</summary>
        public const int SmallestMessageSize = Constants.PeripheralHeaderSize + 1; // Frame byte + Checksum that is not include in Length

        private ushort length;

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderType{TMessageType}"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="length">The length.</param>
        /// <param name="systemType">The system type.</param>
        /// <param name="messageType">The message type.</param>
        protected PeripheralHeaderType(
            ushort address = 0, 
            ushort length = Constants.PeripheralHeaderSize, 
            PeripheralSystemMessageType systemType = PeripheralSystemMessageType.Unknown, 
            TMessageType messageType = default(TMessageType))
        {
            this.Address = address;
            this.Length = length;
            this.SystemType = systemType;
            this.MessageType = messageType;
        }


        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderType{TMessageType}"/> class. Initializes a new
        ///     instance of the <see cref="PeripheralHeaderType{T}"/> class.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="headerIsNetworkByteOrder">The header is network byte order.</param>
        /// <exception cref="ArgumentException"></exception>
        protected PeripheralHeaderType(byte[] buffer, bool headerIsNetworkByteOrder = true)
        {
            var header = buffer.GetValidPeripheralHeader<TMessageType>(headerIsNetworkByteOrder);
            if (header != null)
            {
                this.Address = header.Address;
                this.Length = header.Length;
                this.MessageType = header.MessageType;
                this.SystemType = header.SystemType;
            }
            else
            {
                throw new ArgumentException("Invalid Buffer for PeripheralHeader!", nameof(buffer));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderType{TMessageType}" /> class.</summary>
        protected PeripheralHeaderType()
        {
            this.Address = 0;
            this.SystemType = PeripheralSystemMessageType.Unknown;
            this.Length = Constants.PeripheralHeaderSize;
            this.MessageType = default(TMessageType);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderType{TMessageType}"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="length">The length.</param>
        /// <param name="address">The address.</param>
        /// <param name="systemType">The system type.</param>
        protected PeripheralHeaderType(TMessageType messageType, ushort length, ushort address, PeripheralSystemMessageType systemType)
        {
            this.Address = address;
            this.length = length;
            this.MessageType = messageType;
            this.SystemType = systemType;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderType{TMessageType}"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="length">The length.</param>
        /// <param name="address">The address.</param>
        /// <param name="systemType">The system type.</param>
        protected PeripheralHeaderType(byte messageType, ushort length, ushort address, PeripheralSystemMessageType systemType)
        {
            this.Address = address;
            this.length = length;
            this.SystemType = systemType;
            var m = Enum.Parse(typeof(TMessageType), messageType.ToString());
            this.MessageType = (TMessageType)m;
        }

        /// <summary>
        ///     Gets or sets the packet length which is the header plus payload and excludes the terminating checksum byte.
        ///     Packets with no payload will have a fixed length of size bytes. The length field byte order is Most significant
        ///     byte
        ///     followed byte Least significant byte. Max packet length is 0x6FFF
        ///     The Length property of the packet header is the header size and payload (not including checksum)
        /// </summary>
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

        /// <summary>
        ///     Gets or sets the address. The address field byte order is Most significant byte
        ///     followed byte Least significant byte.
        /// </summary>
        [Order(1)]
        public ushort Address { get; set; }

        /// <summary>
        ///     Gets or sets the system type. The System ID specifies the specific peripheral system interface for the packet.
        ///     By supporting multiple system interface types, the PCP can be extended as required for new peripheral and/or
        ///     controller systems.
        ///     Common packet types such as POLL, DATA, ACK, etc. will contain a System ID of 0x00
        /// </summary>
        [Order(2)]
        public PeripheralSystemMessageType SystemType { get; set; }

        /// <summary>
        ///     Gets or sets the message type. The Message ID indicates the packet type being transmitted.
        ///     The Message ID is specific to the System ID given.
        ///     Thus System ID 0x07 and Message ID 0x03 is a different packet type than System ID 0x08 and Message ID 0x03.
        /// </summary>
        [Order(3)]
        public TMessageType MessageType { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format(
                "PeripheralHeader: Length={0}, Address=0x{1:X}, SystemType={2}, MessageType={3}", 
                this.Length, 
                this.Address, 
                this.SystemType, 
                this.MessageType);
        }

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Equals(object obj)
        {
            var dest = obj as PeripheralHeaderType<TMessageType>;
            return this.Address == dest.Address && this.Length == dest.Length && this.SystemType == dest.SystemType
                   && this.MessageType.ToString() == dest.MessageType.ToString();
        }

        /// <summary>The update header to network byte order.</summary>
        public void UpdateHeaderToNetworkByteOrder()
        {
            this.Length = HostToNetworkOrder(this.Length);
            this.Address = HostToNetworkOrder(this.Address);
        }

        /// <summary>The host to network order.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="ushort"/>.</returns>
        public static ushort HostToNetworkOrder(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class.</summary>
        // <param name="buffer">The buffer.</param>
        /// <param name="headerIsNetworkByteOrder">The header Is Network Byte Order.</param>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        // public static PeripheralHeaderType<T>(byte[] buffer, bool headerIsNetworkByteOrder = true)
        // {
        // // TODO KSH REFACTOR
        // PeripheralHeaderType<T> header = default(T);
        // return header;

        // //var header = buffer.GetValidPeripheralHeader(headerIsNetworkByteOrder);
        // //if (header != null)
        // //{
        // //    this.Address = header.Address;
        // //    this.Length = header.length;
        // //    this.MessageType = header.MessageType;
        // //    this.SystemType = header.SystemType;
        // //}
        // //else
        // //{
        // //    throw new ArgumentException("Invalid Buffer for PeripheralHeader!", nameof(buffer));
        // //}
        // }
    }
}