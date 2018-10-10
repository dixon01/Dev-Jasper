// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CandidateStatus.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CandidateStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The trip candidate status.
    /// </summary>
    public enum CandidateStatus
    {
        /// <summary>
        /// The candidate must be loaded.
        /// </summary>
        ToLoad = 0,

        /// <summary>
        /// The candidate was loaded but not yet validated.
        /// </summary>
        Loaded = 1,

        /// <summary>
        /// The candidate is valid.
        /// </summary>
        Valid = 2,

        /// <summary>
        /// The candidate is outside of the schedule.
        /// </summary>
        OutsideSchedule = 3
    }
}