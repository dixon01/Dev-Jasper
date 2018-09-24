// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The point type.
    /// </summary>
    public enum PointType
    {
        /// <summary>
        /// The point is a bus stop.
        /// </summary>
        Stop = 1,

        /// <summary>
        /// The point is related to traffic light management.
        /// </summary>
        TrafficLight = 5,

        /// <summary>
        /// The point defines a speed limit (used for traffic light management).
        /// </summary>
        SpeedLimit = 6
    }
}