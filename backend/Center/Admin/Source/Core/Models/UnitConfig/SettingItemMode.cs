// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingItemMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SettingItemMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models.UnitConfig
{
    /// <summary>
    /// Way a single system setting is configured.
    /// </summary>
    public enum SettingItemMode
    {
        /// <summary>
        /// The setting is not configured at all.
        /// The user can't change it and it won't be set on the system.
        /// </summary>
        NotConfigured = 0,

        /// <summary>
        /// The global setting is used (no per-I/O setting).
        /// </summary>
        UseGlobal = 1,

        /// <summary>
        /// The setting must be defined for each I/O separately.
        /// </summary>
        UseSpecific = 2
    }
}
