// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsSatelliteState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The GPS satellite state.
    /// </summary>
    public enum GpsSatelliteState
    {
        /// <summary>
        /// The sat 02.
        /// </summary>
        Sat02 = 0x00,

        /// <summary>
        /// The sat 34.
        /// </summary>
        Sat34 = 0x01,

        /// <summary>
        /// The sat 56.
        /// </summary>
        Sat56 = 0x02,

        /// <summary>
        /// The sat sup 7.
        /// </summary>
        SatSup7 = 0x03
    }
}