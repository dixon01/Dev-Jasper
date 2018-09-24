// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeripheralAudioEnable.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral audio enable.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioEnable : IPeripheralAudioSwtichBaseMessage
    {
        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeaderAudioSwitch.HeaderSize + (2 * sizeof(byte));

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioEnable"/> class.</summary>
        /// <param name="activeSpeakerZone">The speaker type.</param>
        public PeripheralAudioEnable(ActiveSpeakerZone activeSpeakerZone)
        {
            this.Header = new PeripheralHeaderAudioSwitch(PeripheralAudioSwitchMessageType.AudioOutputEnable, Size);
            this.ActiveSpeakerZone = activeSpeakerZone;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioEnable"/> class.</summary>
        public PeripheralAudioEnable()
            : this(ActiveSpeakerZone.None)
        {
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader<PeripheralAudioSwitchMessageType> Header { get; set; }

        /// <summary>Gets or sets the active speaker type.</summary>
        public ActiveSpeakerZone ActiveSpeakerZone { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}