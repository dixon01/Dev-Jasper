// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPeripheralAudioStatus.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The PeripheralAudioStatus interface.</summary>
    public interface IPeripheralAudioStatus : IPeripheralAudioSwtichBaseMessage
    {
        #region Public Properties

        /// <summary>Gets or sets the active source.</summary>
        AudioSourcePriority ActiveSource { get; set; } // see PCP_07_PRIORITY_IDS values

        /// <summary>Gets or sets the active speaker.</summary>
        ActiveSpeakerZone ActiveSpeaker { get; set; } // output enable bitmap 0x1 = interior 0x02 = exterior

        /// <summary>Gets or sets the exterior gain.</summary>
        byte ExteriorGain { get; set; } // Exterior Gain Level (0-10)

        /// <summary>Gets or sets the exterior noise level.</summary>
        byte ExteriorNoiseLevel { get; set; } // Exterior Noise 0 - 16 noise levels 0 = no compensation

        /// <summary>Gets or sets the exterior volume.</summary>
        byte ExteriorVolume { get; set; } // Exterior Volume 0 - 100

        /// <summary>Gets or sets the interior gain.</summary>
        byte InteriorGain { get; set; } // Interior Gain Level (0-10)

        /// <summary>Gets or sets the interior noise level.</summary>
        byte InteriorNoiseLevel { get; set; } // Interior Noise 0 - 16 noise levels 0 = no compensation

        /// <summary>Gets or sets the interior volume.</summary>
        byte InteriorVolume { get; set; } // Interior Volume 0 - 100

        /// <summary>Gets or sets the ptt lockout time.</summary>
        int PttLockoutTime { get; set; } // uint_32 time in seconds since PTT lockout activated 

        #endregion
    }
}