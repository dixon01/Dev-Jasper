// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHeader.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
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

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize)]
    public class PeripheralHeader : IPeripheralHeader
    {
        public PeripheralHeader(PeripheralSystemMessageType systemType, byte messageType)
        {
            this.MessageType = messageType;
            this.SystemType = systemType;
        }
        public PeripheralHeader(PeripheralSystemMessageType systemType, object messageType) : this(systemType, Convert.ToByte(messageType))
        {
        }
        public PeripheralHeader()
        {
        }

        public ushort Length { get; set; }
        public ushort Address { get; set; }

        public byte MessageType { get; set; }

        public PeripheralSystemMessageType SystemType { get; set; }
    }

    /// <summary>The Peripheral header used in the exchange with the hardware switch via RS485/RS232.</summary>
    /// <typeparam name="TMessageType"></typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize)]
    public class PeripheralHeader<TMessageType> : PeripheralHeaderType<TMessageType>
    {
        /// <summary>The default system type.</summary>
        public const PeripheralSystemMessageType DefaultSystemType = PeripheralSystemMessageType.AudioGeneration3;

        public const int HeaderSize = Constants.PeripheralHeaderSize;

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader{TMessageType}"/> class. 
        ///     Initializes a new instance of the <see cref="PeripheralHeader"/> class. Initializes a new instance of the<see cref="PeripheralHeader"/> struct. Initializes a new instance of the <see cref="PeripheralHeader"/> class.
        ///     Initializes a new instance of the <see cref="PrephialHeader"/> class.</summary>
        /// <param name="messageType">The message id.</param>
        /// <param name="length">The length which is the Header + Payload and excludes the trailing checksum octet. on TX/RX</param>
        /// <param name="systemMessage">The system id.</param>
        /// <param name="address">The address.</param>
        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader"/> class. Initializes a new instance of the<see cref="PeripheralHeader"/> struct.</summary>
        public PeripheralHeader()
        {
            this.Length = 0;
            this.Address = 0;
            this.SystemType = PeripheralSystemMessageType.Unknown;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader{TMessageType}"/> class. Initializes a new instance of the <see cref="PeripheralHeader"/> class.</summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="headerIsNetworkByteOrder">The header Is Network Byte Order.</param>
        /// <exception cref="ArgumentException">Invalid Buffer for PeripheralHeader!</exception>
        public PeripheralHeader(byte[] buffer, bool headerIsNetworkByteOrder = true)
        {
            var header = buffer?.GetValidPeripheralHeader<TMessageType>(headerIsNetworkByteOrder);
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

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeader{TMessageType}"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="length">The length.</param>
        /// <param name="systemType">The system type.</param>
        /// <param name="messageType">The message type.</param>
        public PeripheralHeader(
            ushort address = 0, 
            ushort length = Constants.PeripheralHeaderSize, 
            PeripheralSystemMessageType systemType = PeripheralSystemMessageType.Unknown, 
            TMessageType messageType = default(TMessageType))
            : base(address, length, systemType, messageType)
        {
        }

        /// <summary>The update header to network byte order.</summary>
        public new void UpdateHeaderToNetworkByteOrder()
        {
            this.Length = HostToNetworkOrder(this.Length);
            this.Address = HostToNetworkOrder(this.Address);
        }

        /// <summary>Gets a value indicating whether is ack.</summary>
        [IgnoreDataMember]
        public bool IsAck
        {
            get
            {
                return this.MessageType.ToString() == PeripheralMessageType.Ack.ToString();
            }
        }

        /// <summary>Gets a value indicating whether is nak.</summary>
        [IgnoreDataMember]
        public bool IsNak
        {
            get
            {
                return this.MessageType.ToString() == PeripheralMessageType.Nak.ToString();
            }
        }

        /// <summary>Gets a value indicating whether is unknown.</summary>
        [IgnoreDataMember]
        public bool IsUnknown
        {
            get
            {
                return this.MessageType.ToString() == PeripheralMessageType.Unknown.ToString();
            }
        }    

        [IgnoreDataMember]
        public string MessageToken
        {
            get
            {
                return string.Format("{0}-{1}", this.SystemType, this.MessageType);
            }
        }

        /// <summary>The is message tye.</summary>
        /// <param name="messageTypeEnum">The message type enum.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsMessageTye(object messageTypeEnum)
        {
            return this.MessageType.ToString() == messageTypeEnum.ToString();
        }

        /// <summary>Attempt to create from bytes.</summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="headerIsNetworkByteOrder">The header Is Network Byte Order.</param>
        /// <returns>The <see cref="PeripheralHeader"/>A header entity else null</returns>
        public static PeripheralHeader<TMessageType> Create(byte[] bytes, bool headerIsNetworkByteOrder = true)
        {
            try
            {
                return new PeripheralHeader<TMessageType>(bytes, headerIsNetworkByteOrder);
            }
            catch
            {
                return null;
            }
        }
    }
}