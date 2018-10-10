// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySettingResults.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// The possible return values of ChangeDisplaySettings API
    /// </summary>
    public enum DisplaySettingResults
    {
        /// <summary>
        /// The settings change was successful.
        /// </summary>
        Successful = 0,

        /// <summary>
        /// The settings change was unsuccessful because the system is DualView capable.
        /// </summary>
        BadDualView = -6,

        /// <summary>
        /// An invalid set of flags was passed in.
        /// </summary>
        BadFlags = -4,

        /// <summary>
        /// The graphics mode is not supported.
        /// </summary>
        BadMode = -2,

        /// <summary>
        /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
        /// </summary>
        BadParam = -5,

        /// <summary>
        /// The display driver failed the specified graphics mode.
        /// </summary>
        Failed = -1,

        /// <summary>
        /// Unable to write settings to the registry.
        /// </summary>
        NotUpdated = -3,

        /// <summary>
        /// The computer must be restarted for the graphics mode to work.
        /// </summary>
        Restart = 1,
    }
}
