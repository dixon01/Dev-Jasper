// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralRequestVersion.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
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

    /// <summary>The peripheral audio switch request version.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioSwitchRequestVersion : PeripheralAudioSwtichBaseMessage, IPeripheralRequestVersion<PeripheralAudioSwitchMessageType>
    {
        /// <summary>The size.</summary>
        public new const int Size = PeripheralHeaderAudioSwitch.HeaderSize + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioSwitchRequestVersion"/> class. 
        ///     Initializes a new instance of the <see cref="PeripheralRequestVersion{TMessageType}"/> class. Initializes a new instance of
        ///     the <see cref="PeripheralVersionInfo"/> class.</summary>
        public PeripheralAudioSwitchRequestVersion()
            : base(PeripheralAudioSwitchMessageType.RequestVersion, PeripheralSystemMessageType.AudioGeneration3, Size)
        {
        }
    }
}