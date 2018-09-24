// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciPositionMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The position message.
    /// </summary>
    public class EciPositionMessage : EciPositionBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.PositionV3;
            }
        }

        /// <summary>
        /// Gets or sets the position event.
        /// </summary>
        public PositionEvent PositionEvent { get; set; }

        /// <summary>
        /// Gets or sets the alarm state.
        /// </summary>
        public EciAlarmState AlarmState { get; set; }

        /// <summary>
        /// Gets or sets the id of the currently active alarm.
        /// </summary>
        public int AlarmId { get; set; }

        /// <summary>
        /// Gets or sets the direction in degrees [°].
        /// </summary>
        public double Direction { get; set; }

        /// <summary>
        /// Gets or sets the block id.
        /// </summary>
        public int BlockId { get; set; }

        /// <summary>
        /// Gets or sets the line id.
        /// </summary>
        public int LineId { get; set; }

        /// <summary>
        /// Gets or sets the stop id.
        /// </summary>
        public int StopId { get; set; }

        /// <summary>
        /// Gets or sets the trip id.
        /// </summary>
        public int TripId { get; set; }
    }
}
