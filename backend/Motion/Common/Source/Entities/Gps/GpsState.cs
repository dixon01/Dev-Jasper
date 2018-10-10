// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Entities.Gps
{
    using System;

    /// <summary>
    /// The state of a <see cref="GpsData"/>.
    /// </summary>
    [Flags]
    public enum GpsState
    {
        /// <summary>
        /// Everything is OK.
        /// </summary>
        Ok = 0x00,

        /// <summary>
        /// The receiver is searching for satellites.
        /// </summary>
        Searching = 0x01,

        /// <summary>
        /// The location is invalid.
        /// </summary>
        BadLocation = 0x02,

        /// <summary>
        /// The data is missing the third dimension (i.e. only 2D mode).
        /// </summary>
        Missing3RdDimenion = 0x04,

        /// <summary>
        /// The antenna is missing.
        /// </summary>
        AntennaMissing = 0x10,

        /// <summary>
        /// The antenna has a short-circuit.
        /// </summary>
        AntennaShortCircuit = 0x20,

        /// <summary>
        /// The antenna has a power surge (consumption too high).
        /// </summary>
        AntennaPowerSurge = 0x40,
    }
}