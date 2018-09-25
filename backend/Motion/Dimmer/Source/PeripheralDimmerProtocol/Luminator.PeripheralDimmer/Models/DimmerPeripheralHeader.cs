// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerPeripheralHeader.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
//   The Dimmer peripheral header.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer peripheral header.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class DimmerPeripheralHeader : PeripheralHeader, IDimmerPeripheralHeader
    {
        /// <summary>Initializes a new instance of the <see cref="DimmerPeripheralHeader"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="length">The length.</param>
        public DimmerPeripheralHeader(DimmerMessageType messageType, int length = HeaderSize)
            : base((byte)messageType, DimmerConstants.DimmerSystemMessageType, DimmerConstants.DimmerAddress, (ushort)length)
        {
        }

        /// <summary>Gets or sets the dimmer message type.</summary>
        public DimmerMessageType DimmerMessageType
        {
            get
            {
                return (DimmerMessageType)this.MessageType;
            }

            set
            {
                this.MessageType = (byte)value;
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return $"Length=0x{this.Length:X}, Address=0x{this.Address:X}, MessageType=0x{this.DimmerMessageType:X}, SystemMessageType=0x{this.SystemMessageType:X}";
        }
    }
}