// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IXimpleConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Protran.XimpleProtocol
{
    /// <summary>The XimpleConfig interface.</summary>
    public interface IXimpleConfig
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this serial port is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>Gets or sets a value indicating whether enable response.</summary>
        bool EnableResponse { get; set; }

        /// <summary>Gets or sets a value indicating whether enable ximple only response.</summary>
        bool EnableXimpleOnlyResponse { get; set; }

        /// <summary>Gets or sets the infotainment audio status table index.</summary>
        int InfoTainmentAudioStatusTableIndex { get; set; }

        /// <summary>Gets or sets the infotainment canned msg play table index.</summary>
        int InfoTainmentCannedMsgPlayTableIndex { get; set; }

        /// <summary>Gets or sets the audio settings table index in the dictionary.xml.</summary>
        int InfoTainmentVolumeSettingsTableIndex { get; set; }

        /// <summary>Gets or sets the network connection table index in the dictionary.xml.</summary>
        int NetworkChangedMessageTableIndex { get; set; }

        /// <summary>Gets or sets the network shared folder for Prosys table index in the dictionary.xml.</summary>
        int NetworkFileAccessSettingsTableIndex { get; set; }

        /// <summary>Gets or sets the infotainment system status.</summary>
        int InfoTainmentSystemStatusTableIndex { get; set; }

        /// <summary>Gets or sets the port.</summary>
        int Port { get; set; }

        /// <summary>Gets or sets the audio zone presentation values.</summary>
        AudioZonePresentationValues AudioZonePresentationValues { get; set; }

        /// <summary>Gets or sets the ximple server monitor timer interval.</summary>
        int XimpleServerMonitorTimerInterval { get; set; }

        int MaxXimpleServerFailuresBeforeRestart { get; set; }

        #endregion
    }
}