// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The GPS state.
    /// </summary>
    public enum GpsState
    {
        /// <summary>
        /// Searching for GPS satellites.
        /// </summary>
        SearchingSat = 0x00,

        /// <summary>
        /// The bad geo.
        /// </summary>
        BadGeo       = 0x40,

        /// <summary>
        /// The mode 2D.
        /// </summary>
        Mode2D       = 0x80,

        /// <summary>
        /// The mode 3D.
        /// </summary>
        Mode3D       = 0xC0
    }
}
