// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollSpeed.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScrollSpeed type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    /// <summary>
    /// The scroll speed of a scroll block.
    /// </summary>
    public enum ScrollSpeed
    {
        // REMARK: all values are shifted by 1 bit to the left, that's according to the specification!

        /// <summary>
        /// The fastest speed (0).
        /// </summary>
        Fastest = 0x00,

        /// <summary>
        /// The fast speed (1).
        /// </summary>
        Fast = 0x02,

        /// <summary>
        /// The slow speed (2).
        /// </summary>
        Slow = 0x04,

        /// <summary>
        /// The slowest speed (3).
        /// </summary>
        Slowest = 0x06
    }
}