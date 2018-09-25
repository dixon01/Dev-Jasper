// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralAck.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral ack.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAck : PeripheralBaseMessage, IPeripheralBaseMessage
    {
        /// <summary>The Expected size.</summary>
        public new const int Size = PeripheralBaseMessage.Size;

        /// <summary>Initializes a new instance of the <see cref="PeripheralAck"/> class.</summary>
        public PeripheralAck() : 
            base(PeripheralMessageType.Ack)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralAck"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">Default System Message Type</param>
        public PeripheralAck(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Default) : base(PeripheralMessageType.Ack, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}