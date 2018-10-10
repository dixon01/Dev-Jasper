// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointStatus.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The status of a point on a route.
    /// </summary>
    public enum PointStatus
    {
        /// <summary>
        /// The point was not yet touched (i.e. the bus wasn't yet close to this point).
        /// </summary>
        NotTouched = 0,

        /// <summary>
        /// We have entered the point's buffer area.
        /// </summary>
        Entry = 1,

        /// <summary>
        /// We have stopped within the point's buffer area.
        /// </summary>
        StopIn = 2,

        /// <summary>
        /// We have regularly left the point's buffer area.
        /// </summary>
        Done = 3,

        /// <summary>
        /// The point wasn't reached after a timeout.
        /// </summary>
        Timeout = 10,

        /// <summary>
        /// The point was passed when we didn't expect it.
        /// </summary>
        OutsideSchedule = 11
    }
}