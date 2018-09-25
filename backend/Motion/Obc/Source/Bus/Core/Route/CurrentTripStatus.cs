// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentTripStatus.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CurrentTripStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;

    /// <summary>
    /// Information about the current trio. This object is used for persistence.
    /// </summary>
    [Serializable]
    public class CurrentTripStatus
    {
        /// <summary>
        /// Gets or sets the date and time when this status was last updated.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the course id of the current candidate.
        /// </summary>
        public int CandidateCourseId { get; set; }

        /// <summary>
        /// Gets or sets the current stop index.
        /// </summary>
        public int StopIndex { get; set; }
    }
}