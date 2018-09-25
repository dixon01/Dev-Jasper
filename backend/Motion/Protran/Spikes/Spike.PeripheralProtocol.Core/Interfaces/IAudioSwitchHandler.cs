// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IAudioSwitchHandler.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The AudioSwitchHandler interface.</summary>
    public interface IAudioSwitchHandler : IPeripheralHandler<PeripheralAudioSwitchMessageType>
    {
        #region Public Methods and Operators

        /// <summary>The ximple created.</summary>
        event EventHandler<PeripheralVersionsInfo> PeripherialVersionChangedEvent;

        /// <summary>The audio status message event.</summary>
        event EventHandler<AudioStatusMessage> AudioStatusMessageEvent;

        /// <summary>The audio status message event.</summary>
        event EventHandler<VolumeChangeRequest> VolumeChangeRequestedEvent;

        /// <summary>Handle the MEDI audio status request message</summary>
        event EventHandler AudioStatusRequested;

        /// <summary>Peripheral GPIO changed event</summary>
        event EventHandler<PeripheralGpioEventArg> GpioChanged;

        /// <summary>The volume settings changed event.</summary>
        event EventHandler<VolumeSettingsMessage> VolumeSettingsChangedEvent;

        /// <summary>The audio playback changed event.</summary>
        event EventHandler<AudioPlaybackEvent> AudioPlaybackChangedEvent;

        /// <summary>The broadcast audio status changed.</summary>
        /// <param name="audioStatusMessage">The audio status message.</param>
        void BroadcastAudioStatusChanged(AudioStatusMessage audioStatusMessage);

        void FireVolumeSettingsChanged(VolumeSettingsMessage volumeSettingsMessage);

        #endregion
    }
}