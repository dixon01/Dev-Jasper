// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evTrafficLight.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evTrafficLight type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// This event tells the state of the traffic light controller (priority requested or not)
    /// mainly for the display on the driver terminal.
    /// </summary>
    public class evTrafficLight
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evTrafficLight"/> class.
        /// </summary>
        public evTrafficLight()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTrafficLight"/> class.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        public evTrafficLight(TrafficLightState state)
            : this(state, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evTrafficLight"/> class.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="approachTime">
        /// The approach time.
        /// </param>
        public evTrafficLight(TrafficLightState state, int approachTime)
        {
            this.State = state;
            this.ApproachTime = approachTime;
        }

        /// <summary>
        /// Gets or sets the current state of traffic light.
        /// </summary>
        public TrafficLightState State { get; set; }

        /// <summary>
        /// Gets or sets the time calculated for the approach
        /// </summary>
        public int ApproachTime { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evTrafficLight .TrafficLightState: " + this.State + " .approachTime: " + this.ApproachTime;
        }
    }
}