// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The alarm state.
    /// </summary>
    public enum AlarmState
    {
        /// <summary>
        /// no alarm.
        /// </summary>
        Inactive,   // no alarm

        /// <summary>
        /// VT3 Alarm send to iCenter.
        /// </summary>
        Reported,

        /// <summary>
        /// alarm (SW) ACK from iCenter.
        /// </summary>
        Received,

        /// <summary>
        /// User ACK iCenter.
        /// </summary>
        Confirmed,

        /// <summary>
        /// User Ends alarm iCenter but button is still on.
        /// </summary>
        Ended,
    }
}