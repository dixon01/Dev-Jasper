// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciAlarmMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   GeneralMessage Frames.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// EciAlarmMessage message.
    /// </summary>
    public class EciAlarmMessage : EciPositionBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Alarm;
            }
        }

        /// <summary>
        /// Gets or sets the GPS state.
        /// </summary>
        public GpsState GpsState { get; set; }

        /// <summary>
        /// Gets or sets the alarm state.
        /// </summary>
        public EciAlarmState AlarmState { get; set; }
    }
}
