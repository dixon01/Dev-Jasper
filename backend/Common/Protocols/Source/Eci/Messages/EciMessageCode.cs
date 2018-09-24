// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciMessageCode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The message type.
    /// </summary>
    public enum EciMessageCode
    {
        /// <summary>
        /// The position.
        /// </summary>
        Position = 'p',

        /// <summary>
        /// The position v 2.
        /// </summary>
        PositionV2 = 'P',

        /// <summary>
        /// The position v3.
        /// </summary>
        PositionV3 = 'G',

        /// <summary>
        /// The alarm.
        /// </summary>
        Alarm = 'E',

        /// <summary>
        /// The acknowledge.
        /// </summary>
        Ack = 'A',

        /// <summary>
        /// The traffic light.
        /// </summary>
        TrafficLight = 'F',

        /// <summary>
        /// The keep alive.
        /// </summary>
        KeepAlive = 'K',

        /// <summary>
        /// The log.
        /// </summary>
        Log = 'Z',

        /// <summary>
        /// The util.
        /// </summary>
        Util = 'U',

        /// <summary>
        /// The passenger count.
        /// </summary>
        PassengerCount = 'C',

        /// <summary>
        /// The delay.
        /// </summary>
        Delay = 'R',

        /// <summary>
        /// The general message.
        /// </summary>
        GeneralMessage = 'M',

        // VM.x frames

        /// <summary>
        /// The general message new format.
        /// </summary>
        GeneralMessageNewFormat = 'm',

        /// <summary>
        /// The text message.
        /// </summary>
        TextMessage = 't',

        /// <summary>
        /// The duty.
        /// </summary>
        Duty = 'b',

        /// <summary>
        /// The Acknowledge.
        /// </summary>
        AckTs = 'a',
    }
}