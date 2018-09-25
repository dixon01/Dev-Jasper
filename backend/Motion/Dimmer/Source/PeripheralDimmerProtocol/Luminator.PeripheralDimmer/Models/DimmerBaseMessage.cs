// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerBaseMessage.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer base message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer base message.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public abstract class DimmerBaseMessage : IPeripheralBaseMessage
    {
        /// <summary>The size.</summary>
        public const int Size = PeripheralHeader.HeaderSize + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="DimmerBaseMessage"/> class.</summary>
        /// <param name="dimmerMessageType">The dimmer message type.</param>
        protected DimmerBaseMessage(DimmerMessageType dimmerMessageType)
        {
            this.Header = new PeripheralHeader(
                (byte)dimmerMessageType, 
                DimmerConstants.DimmerSystemMessageType, 
                DimmerConstants.DimmerAddress, 
                Marshal.SizeOf(this));
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format("{0}, Checksum=0x{1:X}", this.Header, this.Checksum);
        }
    }
}