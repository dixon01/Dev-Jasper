// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    /// <summary>
    /// The display mode of the screens on hardware.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Clone the two display screens.
        /// </summary>
        Clone,

        /// <summary>
        /// Extend the main display screen to the second one.
        /// </summary>
        Extend
    }
}