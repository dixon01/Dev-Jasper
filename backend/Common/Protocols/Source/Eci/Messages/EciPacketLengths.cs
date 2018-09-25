// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciPacketLengths.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The eci packet lengths.
    /// </summary>
    public enum EciPacketLengths
    {
        /// <summary>
        /// The info time.
        /// </summary>
        InfoTime = 13,

        /// <summary>
        /// The alarm.
        /// </summary>
        Alarm = 21,

        /// <summary>
        /// The position v 3.
        /// </summary>
        PositionV3 = 29,

        /// <summary>
        /// The acknowledge.
        /// </summary>
        Ack = 9,

        /// <summary>
        /// The duty.
        /// </summary>
        Duty = 21,

        /// <summary>
        /// The traffic light entry.
        /// </summary>
        TrafficLightEntry = 30,

        /// <summary>
        /// The traffic light check point.
        /// </summary>
        TrafficLightCheckPoint = 24,

        /// <summary>
        /// The traffic light exit.
        /// </summary>
        TrafficLightExit = 21,

        /// <summary>
        /// The traffic light acknowledge.
        /// </summary>
        TrafficLightAck = 17,

        /// <summary>
        /// The keep alive.
        /// </summary>
        KeepAlive = 6,

        /// <summary>
        /// The util.
        /// </summary>
        Util = 7,

        /// <summary>
        /// The text.
        /// </summary>
        Text = 24
    }
}
