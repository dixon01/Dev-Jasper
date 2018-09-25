// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralPollAudioStatus.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral poll audio status.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralPoll : PeripheralBaseMessage
    {
        public new const int Size = PeripheralBaseMessage.Size;

        /// <summary>Initializes a new instance of the <see cref="PeripheralPoll"/> class. Initializes a new instance of the <see cref="PeripheralAck"/> class.</summary>
        public PeripheralPoll()
            : base(PeripheralMessageType.Poll)
        {
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralPoll"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system Message Type.</param>
        public PeripheralPoll(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Default) : base(PeripheralMessageType.Poll, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }
    }
}