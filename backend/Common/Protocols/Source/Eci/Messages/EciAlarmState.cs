// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciAlarmState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The alarm state.
    /// </summary>
    public enum EciAlarmState
    {
        /// <summary>
        /// The free.
        /// </summary>
        Free       = 0x00,

        /// <summary>
        /// The start call.
        /// </summary>
        StartCall  = 0x20,

        /// <summary>
        /// The Acknowledge.
        /// </summary>
        Ack        = 0x50,

        /// <summary>
        /// The keep.
        /// </summary>
        Keep       = 0x60,

        /// <summary>
        /// The end.
        /// </summary>
        End        = 0x80
    }
}
