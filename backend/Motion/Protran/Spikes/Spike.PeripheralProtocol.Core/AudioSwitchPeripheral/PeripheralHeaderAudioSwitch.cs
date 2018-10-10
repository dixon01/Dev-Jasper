// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralHeaderAudioSwitch.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral header audio switch.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Constants.PeripheralHeaderSize)]
    public class PeripheralHeaderAudioSwitch : PeripheralHeader<PeripheralAudioSwitchMessageType>
    {
        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderAudioSwitch"/> class.</summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="size">The length.</param>
        public PeripheralHeaderAudioSwitch(PeripheralAudioSwitchMessageType messageType, int size)
        {
            this.Address = Constants.DefaultPeripheralAudioSwitchAddress;
            this.SystemType = PeripheralSystemMessageType.AudioGeneration3;
            this.MessageType = messageType;
            this.Length = (ushort)size;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralHeaderAudioSwitch"/> class.</summary>
        public PeripheralHeaderAudioSwitch(PeripheralAudioSwitchMessageType messageType = PeripheralAudioSwitchMessageType.Unknown)
            : base(Constants.DefaultPeripheralAudioSwitchAddress)
        {
            this.MessageType = messageType;
            this.Length = Constants.PeripheralHeaderSize;
            this.SystemType = PeripheralSystemMessageType.AudioGeneration3;
        }
    }
}