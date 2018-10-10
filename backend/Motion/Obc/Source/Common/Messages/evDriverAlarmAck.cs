// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDriverAlarmAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDriverAlarmAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The driver alarm acknowledge.
    /// </summary>
    public class evDriverAlarmAck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDriverAlarmAck"/> class.
        /// </summary>
        public evDriverAlarmAck()
        {
            this.State = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDriverAlarmAck"/> class.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        public evDriverAlarmAck(AlarmAck state)
        {
            this.State = state;
        }

        /// <summary>
        /// Gets or sets the current state of Alarm
        /// </summary>
        public AlarmAck State { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evDriverAlarmAck. State: " + this.State;
        }
    }
}