// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayStatusCode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplayStatusCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Notifications
{
    /// <summary>
    /// The display status code for the <see cref="DisplayStatus"/> triplet.
    /// </summary>
    public enum DisplayStatusCode
    {
        /// <summary>
        /// The backlight is not working correctly.
        /// </summary>
        BacklightError = -3,

        /// <summary>
        /// The display is not working correctly.
        /// </summary>
        DisplayError = -2,

        /// <summary>
        /// No connection to the display
        /// </summary>
        NoConnection = -1,

        /// <summary>
        /// Everything OK
        /// </summary>
        Ok = 0,

        /// <summary>
        /// The display or the system is currently initializing.
        /// </summary>
        Initializing = 1,
    }
}