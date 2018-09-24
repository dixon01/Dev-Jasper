// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestPacketLength.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The LGR type request.
    /// </summary>
    public enum RequestPacketLength
    {
        /// <summary>
        /// The time.
        /// </summary>
        Time = 7,

        /// <summary>
        /// The reboot.
        /// </summary>
        Reboot = 7,

        /// <summary>
        /// The update.
        /// </summary>
        Update = 7,

        /// <summary>
        /// The initialize duty.
        /// </summary>
        InitDuty = 7,

        /// <summary>
        /// The initialize alarm.
        /// </summary>
        InitAlarm = 7
    }
}
