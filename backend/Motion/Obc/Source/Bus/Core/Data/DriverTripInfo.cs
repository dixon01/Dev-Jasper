// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverTripInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriverTripInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Data
{
    using System;

    /// <summary>
    /// The information about a driver trip.
    /// </summary>
    public class DriverTripInfo
    {
        /// <summary>
        /// Gets or sets the relief time (was <c>ReleveTime</c> in old code).
        /// </summary>
        public TimeSpan ReliefTime { get; set; }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        /// Gets or sets the trip number.
        /// </summary>
        public int Trip { get; set; }

        /// <summary>
        /// Gets or sets the first relief index (was <c>Releve1Index</c> in old code).
        /// </summary>
        public int Relief1Index { get; set; }
    }
}