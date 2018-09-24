// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciAckTs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// ECI Acknowledge TS message.
    /// </summary>
    public class EciAckTs : EciMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EciAckTs"/> class.
        /// </summary>
        /// <param name="vehicleId">
        /// The vehicle id.
        /// </param>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        /// <param name="subType">
        /// The sub type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        public EciAckTs(int vehicleId, DateTime timeStamp, char subType, byte value, int reference)
        {
            this.VehicleId = vehicleId;
            this.TimeStamp = timeStamp;
            this.SubType = subType;
            this.Value = value;
            this.Reference = reference;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.AckTs;
            }
        }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        public int Reference { get; set; }

        /// <summary>
        /// Gets or sets the sub type.
        /// </summary>
        public char SubType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public byte Value { get; set; }
    }
}
