// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciRequestCode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The eci sub command code.
    /// </summary>
    public enum EciRequestCode
    {
        /// <summary>
        /// The position.
        /// </summary>
        Position = 2,

        /// <summary>
        /// The request reboot.
        /// </summary>
        RequestReboot = 'r',

        /// <summary>
        /// The request update.
        /// </summary>
        RequestUpdate = 'u',

        /// <summary>
        /// The request time.
        /// </summary>
        RequestTime = 'h',

        /// <summary>
        /// The initialize duty.
        /// </summary>
        InitDuty = 'd',

        /// <summary>
        /// The initialize alarm.
        /// </summary>
        InitAlarm = 'a',

        /// <summary>
        /// The info time.
        /// </summary>
        InfoTime = 'i'
    }
}
