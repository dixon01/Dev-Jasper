// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evDistressAlarm.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evDistressAlarm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The distress alarm event.
    /// </summary>
    public class evDistressAlarm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evDistressAlarm"/> class.
        /// </summary>
        public evDistressAlarm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evDistressAlarm"/> class.
        /// </summary>
        /// <param name="alarmSet">
        /// The alarm set.
        /// </param>
        public evDistressAlarm(bool alarmSet)
        {
            this.AlarmSet = alarmSet;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alarm is set.
        /// </summary>
        public bool AlarmSet { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "evDistressAlarm. AlarmSet : " + this.AlarmSet;
        }
    }
}