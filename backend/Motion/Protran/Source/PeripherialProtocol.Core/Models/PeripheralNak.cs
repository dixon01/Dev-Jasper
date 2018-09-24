// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralNak.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral nak.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralNak : PeripheralBaseMessage
    {
        /// <summary>The Expected size.</summary>
        public new const int Size = PeripheralBaseMessage.Size;

        /// <summary>Initializes a new instance of the <see cref="PeripheralNak"/> class.</summary>
        public PeripheralNak() : base(PeripheralMessageType.Nak)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralNak"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">Default System Message Type</param>
        public PeripheralNak(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Default) : base(PeripheralMessageType.Nak, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}