// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointSubTypeTrafficLight.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointSubTypeTrafficLight type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// Same values as <see cref="TrafficLightPosition"/>
    /// </summary>
    public enum PointSubTypeTrafficLight
    {
        /// <summary>
        /// Entering the interaction zone of the traffic light.
        /// </summary>
        Entry = 1,

        /// <summary>
        /// Control point of the traffic light.
        /// </summary>
        CheckPoint = 2,

        /// <summary>
        /// The traffic light position.
        /// </summary>
        Position = 4,

        /// <summary>
        /// Exiting the interaction zone of the traffic light.
        /// </summary>
        Exit = 3
    }
}