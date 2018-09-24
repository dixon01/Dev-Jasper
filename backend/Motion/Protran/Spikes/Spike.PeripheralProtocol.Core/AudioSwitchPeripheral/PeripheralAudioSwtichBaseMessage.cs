// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralAudioSwtichBaseMessage.cs">
//   Copyright © 2011-2065 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral audio switch base message.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioSwtichBaseMessage : IPeripheralAudioSwtichBaseMessage
    {
        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeaderAudioSwitch.HeaderSize + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioSwtichBaseMessage"/> class.</summary>
        public PeripheralAudioSwtichBaseMessage()
            : this(PeripheralAudioSwitchMessageType.Unknown, PeripheralSystemMessageType.AudioGeneration3)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioSwtichBaseMessage"/> class.</summary>
        /// <param name="header">The header.</param>
        /// <param name="checksum">The checksum.</param>
        public PeripheralAudioSwtichBaseMessage(PeripheralHeaderAudioSwitch header, byte checksum)
        {
            this.Header = header;
            this.Checksum = checksum;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioSwtichBaseMessage"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="systemMessageType">The system message type.</param>
        /// <param name="size">The message size/length.</param>
        public PeripheralAudioSwtichBaseMessage(
            PeripheralAudioSwitchMessageType messageType,
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.Unknown,
            int size = Size)
        {
            this.Header = new PeripheralHeaderAudioSwitch(messageType, size) { SystemType = systemMessageType };
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Gets or sets the header.</summary>
        [Order]
        public PeripheralHeader<PeripheralAudioSwitchMessageType> Header { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public byte Checksum { get; set; } 
    }
}