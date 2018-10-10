// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeDeviceSettings.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// Indicates how the graphics mode should be changed.
    /// </summary>
    public enum ChangeDeviceSettings
    {
        /// <summary>
        /// The mode is temporary in nature.
        /// If you change to and from another desktop, this mode will not be reset.
        /// </summary>
        Fullscreen = 0x4,

        /// <summary>
        /// The settings will be saved in the global settings area so that they will affect all  users on the machine.
        /// Otherwise, only the settings for the user are modified. This flag is only valid when specified with the
        /// CDS_UPDATEREGISTRY flag.
        /// </summary>
        Global = 0x8,

        /// <summary>
        /// The settings will be saved in the registry, but will not take effect. This flag is only valid when
        /// specified with the CDS_UPDATEREGISTRY flag.
        /// </summary>
        NoReset = 0x10000000,

        /// <summary>
        /// The settings should be changed, even if the requested settings are the same as the current settings.
        /// </summary>
        Reset = 0x40000000,

        /// <summary>
        /// This device will become the primary device.
        /// </summary>
        SetPrimary = 0x10,

        /// <summary>
        /// The system tests if the requested graphics mode could be set.
        /// </summary>
        Test = 0x2,

        /// <summary>
        /// The graphics mode for the current screen will be changed dynamically and the graphics mode will be updated
        /// in the registry. The mode information is stored in the USER profile.
        /// </summary>
        UpdateRegistry = 0x1
    }
}
