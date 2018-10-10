// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDriverAlarm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDriverAlarm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System.Xml.Serialization;

    /// <summary>
    /// The driver alarm event.
    /// </summary>
    public class evDriverAlarm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDriverAlarm"/> class.
        /// </summary>
        public evDriverAlarm()
        {
            this.AlarmID = 0;
            this.State = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDriverAlarm"/> class.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="alarmID">
        /// The alarm id.
        /// </param>
        public evDriverAlarm(AlarmState state, int alarmID)
        {
            this.State = state;
            this.AlarmID = alarmID;
        }

        /// <summary>
        /// Gets or sets the current state of Alarm
        /// </summary>
        public AlarmState State { get; set; }

        /// <summary>
        /// Gets or sets the specific alarm ID. This ID is defined in the data\alarm.csv file on the mobile device.
        /// If no specific alarm ID is defined set this to -1!
        /// </summary>
        public int AlarmID { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evDriverAlarm. AlarmState : " + this.State + ", alarmID: " + this.AlarmID;
        }
    }
}