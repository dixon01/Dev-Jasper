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

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;


    /// <summary>The peripheral poll audio status.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralPoll : PeripheralBaseMessage<PeripheralMessageType>, IPeripheralBaseMessageType<PeripheralMessageType>
    {
        /// <summary>The Expected size.</summary>
        public new const int Size = PeripheralBaseMessage<PeripheralMessageType>.Size;

        /// <summary>Initializes a new instance of the <see cref="PeripheralPoll"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system message type.</param>
        public PeripheralPoll(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Unknown) :
            base(PeripheralMessageType.Poll, systemMessageType)
        {
            base.Header.Address = address;
        }

        public PeripheralPoll()
        {
            this.Header.MessageType = PeripheralMessageType.Poll;
        }
    }
}