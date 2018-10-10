// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.StatusInfo
{
    /// <summary>
    /// The drive type.
    /// </summary>
    public enum DriveType
    {
        /// <summary>
        /// Nothing selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// The special destination drive mode.
        /// </summary>
        SpecialDestination = 1,

        /// <summary>
        /// The block drive mode.
        /// </summary>
        Block = 2,
    }
}