// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayConnectionType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayConnectionType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    /// <summary>
    /// The type of connection to the display.
    /// </summary>
    public enum DisplayConnectionType
    {
        /// <summary>
        /// The display is connected (remotely) through DVI.
        /// </summary>
        Dvi,

        /// <summary>
        /// The display is connected locally through LVDS.
        /// </summary>
        Lvds
    }
}