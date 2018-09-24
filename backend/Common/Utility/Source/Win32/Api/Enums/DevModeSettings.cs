// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DevModeSettings.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// The type of information to be retrieved. This value can be a graphics mode index or one of the following values.
    /// </summary>
    public enum DevModeSettings
    {
        /// <summary>
        /// Retrieve the current settings for the display device.
        /// </summary>
        EnumCurrentSettings = -1,

        /// <summary>
        /// Retrieve the settings for the display device that are currently stored in the registry.
        /// </summary>
        EnumRegistrySettings = -2
    }
}
