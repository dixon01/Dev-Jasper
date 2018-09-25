// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates all available values indicating in details how handle the display on/off.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates all available values indicating in details how handle the display on/off.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// None (Legacy code = DISP_MODE_NONE) 
        /// </summary>
        None = -1,

        /// <summary>
        /// Normal display - all columns are displayed should be display on activity. (Legacy code = DISP_MODE_NORMAL) 
        /// </summary>
        Normal = 0,

        /// <summary>
        /// All columns are not displayed, should be display off activity. (Legacy code = DISP_MODE_ALL_OFF)
        /// </summary> 
        AllOff,

        /// <summary>
        /// Only time column is not displayed (off), all other columns are(on). (Legacy code = DISP_MODE_TIME_OFF)
        /// </summary>
        OnlyTimeOff,

        /// <summary>
        /// Displays the special text instead of destination text. (Legacy code = DISP_MODE_SPEC_TEXT)
        /// </summary>
        ReplaceDestWithSpecialText,

        /// <summary>
        /// Max code number. (Legacy code = DISP_MODE_MAX)
        /// </summary>
        Max
    }
}
