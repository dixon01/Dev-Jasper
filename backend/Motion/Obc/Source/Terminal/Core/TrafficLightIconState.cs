// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficLightIconState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TrafficLightIconState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// State of the traffic light icon.
    /// </summary>
    public enum TrafficLightIconState
    {
        /// <summary>
        /// Hides any traffic light icon.
        /// </summary>
        None,

        /// <summary>
        /// Shows a traffic light requested icon.
        /// </summary>
        Requested,

        /// <summary>
        /// Shows a traffic light received icon.
        /// </summary>
        Received
    }
}