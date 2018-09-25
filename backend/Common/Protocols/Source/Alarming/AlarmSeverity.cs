// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmSeverity.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmSeverity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Alarming
{
    /// <summary>
    /// Defines the possible values for the severity of an alarm.
    /// </summary>
    public enum AlarmSeverity
    {
        /// <summary>
        /// Critical alarm.
        /// Example: the entire system is not working.
        /// </summary>
        Critical = 0,

        /// <summary>
        /// Severe alarm.
        /// Example: a unit is not working.
        /// </summary>
        Severe = 1,

        /// <summary>
        /// Error alarm.
        /// Example: a unit restarted unexpectedly.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Warning alarm.
        /// Example: battery temperature is above a certain level
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Informational message (not really an alarm).
        /// Example: the unit has restarted because of a timer.
        /// </summary>
        Info = 4,
    }
}