// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralBaseMessage.cs">
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

    /// <summary>The peripheral simple message.</summary>
    /// <typeparam name="TMessageType">Enum Message Type</typeparam>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize + sizeof(byte))]
    public class PeripheralBaseMessage<TMessageType> : IPeripheralBaseMessageType<TMessageType>
    {
        /// <summary>The expected size, header and the checksum byte.</summary>
        public const int Size = Constants.PeripheralHeaderSize + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage{TMessageType}"/> class. Initializes a new
        ///     instance of the <see cref="PeripheralBaseMessage"/> class. Initializes a new instance of the<see cref="PeripheralBaseMessage"/> struct. Initializes a new instance of
        ///     the <see cref="PeripheralBaseMessage"/> class. Initializes a new instance of the <see cref="PeripheralNak"/>
        ///     class.</summary>
        /// <param name="messageType">The peripheral Message Type.</param>
        /// <param name="systemMessageType">The peripheral Message System Message Type.</param>
        /// <param name="address">The address.</param>
        public PeripheralBaseMessage(TMessageType messageType, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Unknown, ushort address = 0)
        {
            this.Header = new PeripheralHeader<TMessageType>
            {
                Length = Size,
                Address = address,
                MessageType = messageType,
                SystemType = systemMessageType,
            };
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage{TMessageType}"/> class. Initializes a new
        ///     instance of the <see cref="PeripheralBaseMessage"/> class.</summary>
        /// <param name="header">The header.</param>
        public PeripheralBaseMessage(PeripheralHeader<TMessageType> header)
        {
            this.Header = header;
            this.Checksum = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage{TMessageType}" /> class.</summary>
        public PeripheralBaseMessage()
            : this(default(TMessageType), PeripheralSystemMessageType.Unknown)
        {
        }

        /// <summary>Gets or sets the header.</summary>
        [Order]
        public PeripheralHeader<TMessageType> Header { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public byte Checksum { get; set; }

        /// <summary>The is message.</summary>
        /// <param name="messageType">The type.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsMessage(TMessageType messageType)
        {
            return messageType.Equals(this.Header.MessageType);
        }
 
        /// <summary>Gets a value indicating whether is the message type is a ACK.</summary>
        [IgnoreDataMember]
        public bool IsAck
        {
            get
            {
                return this.Header.IsAck;
            }
        }

        /// <summary>Gets a value indicating whether is the message type is a is NAK.</summary>
        [IgnoreDataMember]
        public bool IsNak
        {
            get
            {
                return this.Header.IsNak;
            }
        }
    }
}