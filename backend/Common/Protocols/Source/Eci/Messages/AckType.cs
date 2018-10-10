// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AckType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AckType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The type of ECI acknowledgment.
    /// </summary>
    public enum AckType
    {
        /// <summary>
        /// Position query (ACK_QUERYPOS)
        /// </summary>
        QueryPos = 0x10,

        /// <summary>
        /// Position information (ACK_POSITION)
        /// </summary>
        Position = 0x11,

        /// <summary>
        /// Intervention (ACK_INTERVENTION)
        /// </summary>
        Intervention = 0x12,

        /// <summary>
        /// Intervention state (ACK_STATEINTERV)
        /// </summary>
        InterventionState = 0x13,

        /// <summary>
        /// Intervention message (ACK_MESSAGE_INTERV)
        /// </summary>
        InterventionMessage = 0x14,

        /// <summary>
        /// Alarm received (ACK_RX_ALARME)
        /// </summary>
        AlarmReceived = 0x30,

        /// <summary>
        /// Alarm confirmed (ACK_CTRL_ALARME)
        /// </summary>
        AlarmConfirmed = 0x40,

        /// <summary>
        /// Alarm ended (ACK_END_ALARME)
        /// </summary>
        AlarmEnd = 0x50,

        /// <summary>
        /// Duty (ACK_DUTY)
        /// </summary>
        Duty = 0x85,

        /// <summary>
        /// Display a delay (ACK_AFF_RETARD)
        /// </summary>
        DisplayDelay = 0x86,

        /// <summary>
        /// Display a message (ACK_AFF_MESSAGE)
        /// </summary>
        DisplayMessage = 0x88,

        /// <summary>
        /// Keep-alive response (ACK_KEEPALIVE)
        /// </summary>
        KeepAlive = 0x0C,

        /// <summary>
        /// Message (ACK_MESSAGE)
        /// </summary>
        Message = 0x0E
    }
}