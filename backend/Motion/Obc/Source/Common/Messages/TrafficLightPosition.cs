// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrafficLightPosition.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TrafficLightPosition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The traffic light position.
    /// Same values as in poi.h
    /// </summary>
    public enum TrafficLightPosition
    {
        /// <summary>
        /// Entering the interaction zone of the traffic light.
        /// </summary>
        POINT_SSTYPE_TRAFICLIGHT_ENTRY = 1,

        /// <summary>
        /// Control point of the traffic light.
        /// </summary>
        POINT_SSTYPE_TRAFICLIGHT_CHECKPOINT = 2,

        /// <summary>
        /// The traffic light position.
        /// </summary>
        POINT_SSTYPE_TRAFICLIGHT_POS = 4,

        /// <summary>
        /// Exiting the interaction zone of the traffic light.
        /// </summary>
        POINT_SSTYPE_TRAFICLIGHT_EXIT = 3
    }
}