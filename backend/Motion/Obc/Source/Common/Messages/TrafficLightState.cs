// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficLightState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TrafficLightState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The traffic light state.
    /// </summary>
    public enum TrafficLightState
    {
        /// <summary>
        /// Not in the zone of a traffic light, or exit
        /// </summary>
        Inactive,

        /// <summary>
        /// The request for traffic light priority has been sent
        /// </summary>
        Requested,

        /// <summary>
        /// The request has been received and acknowledged
        /// </summary>
        Received,
    }
}