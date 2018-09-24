// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrientationMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The different options for the orientation of the display screens.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    /// <summary>
    /// The different options for the orientation of the display screens.
    /// </summary>
    public enum OrientationMode
    {
        /// <summary>
        /// The landscape mode of screen display.
        /// </summary>
        Landscape = 0,

        /// <summary>
        /// The portrait mode of screen display.
        /// </summary>
        Portrait = 1,

        /// <summary>
        /// The landscape flipped mode of screen display
        /// </summary>
        LandscapeFlipped = 2,

        /// <summary>
        /// The portrait flipped mode of screen display
        /// </summary>
        PortraitFlipped = 3
    }
}