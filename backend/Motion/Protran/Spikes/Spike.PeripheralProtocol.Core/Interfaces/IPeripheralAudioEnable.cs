// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralAudioEnable.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralAudioEnable interface.</summary>
    public interface IPeripheralAudioEnable : IPeripheralAudioSwtichBaseMessage
    {
        #region Public Properties

        /// <summary>Gets or sets the active speaker.</summary>
        ActiveSpeakerZone ActiveSpeakerZone { get; set; }

        #endregion
    }
}