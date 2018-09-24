// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralBaseMessage.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;


    /// <summary>The peripheral simple message.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralBaseMessage : IPeripheralBaseMessage
    {
        /// <summary>The expected size, header and checksum.</summary>
        public const int Size = PeripheralHeader.Size + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage"/> class. Initializes a new instance of the<see cref="PeripheralBaseMessage"/> struct. Initializes a new instance of
        ///     the <see cref="PeripheralBaseMessage"/> class. Initializes a new instance of the <see cref="PeripheralNak"/>
        ///     class.</summary>
        /// <param name="peripheralMessageType">The peripheral Message Type.</param>
        /// <param name="systemMessageType">The peripheral Message System Message Type.</param>
        protected PeripheralBaseMessage(
            PeripheralMessageType peripheralMessageType, 
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Default)
        {
            this.Header = new PeripheralHeader
                              {
                                  MessageType = peripheralMessageType, 
                                  SystemType = systemMessageType, 
                                  Length = (ushort)Marshal.SizeOf(this)
                              };
            this.Checksum = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage"/> class.</summary>
        /// <param name="header">The header.</param>
        protected PeripheralBaseMessage(PeripheralHeader header)
        {
            this.Header = header;
            this.Checksum = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralBaseMessage" /> class.</summary>
        public PeripheralBaseMessage()
            : this(PeripheralMessageType.Unknown, PeripheralSystemMessageType.Default)
        {
        }

        /// <summary>Gets or sets the header.</summary>
        [Order]
        public PeripheralHeader Header { get; set; }

        /// <summary>The is message.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsMessage(PeripheralMessageType type)
        {
            return type.Equals(this.Header.MessageType);
        }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public byte Checksum { get; set; }
    }
}