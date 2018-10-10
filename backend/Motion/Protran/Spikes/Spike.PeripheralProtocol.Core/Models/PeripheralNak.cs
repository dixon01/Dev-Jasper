// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralNak.cs">
//   Copyright © 2011-2065 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral nak.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralNak : PeripheralBaseMessage<PeripheralMessageType>
    {
        /// <summary>Initializes a new instance of the <see cref="PeripheralNak"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system message type.</param>
        public PeripheralNak(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Unknown)
            : base(PeripheralMessageType.Nak, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        public PeripheralNak()
        {
            this.Header.MessageType = PeripheralMessageType.Nak;
        }
    }
}