// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TripInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Data
{
    using System;

    /// <summary>
    /// Information about a trip.
    /// </summary>
    [Serializable]
    public class TripInfo
    {
        /// <summary>
        /// Gets or sets the service number.
        /// </summary>
        public int ServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the route id (used to be <c>m_sItineraire</c> in the old code).
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets or sets the trip id (used to be <c>m_lCourseId</c> in the old code).
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// Gets or sets the customer trip id.
        /// </summary>
        public int CustomerTripId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the trip since midnight.
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time group used by this trip (indexed from 1).
        /// </summary>
        public int TimeGroup { get; set; }

        /// <summary>
        /// Gets or sets the index of the first stop to take into account.
        /// </summary>
        public int FirstStopIndex { get; set; }
    }
}