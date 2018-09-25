// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciDutyMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The eci vehicle duty message.
    /// </summary>
    public class EciDutyMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Duty;
            }
        }

        /// <summary>
        /// Gets or sets the service number.
        /// </summary>
        public int ServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the login type.
        /// </summary>
        public char LoginType { get; set; }

        /// <summary>
        /// Gets or sets the driver id.
        /// </summary>
        public int DriverId { get; set; }

        /// <summary>
        /// Gets or sets the driver pin.
        /// </summary>
        public int DriverPin { get; set; }

        /// <summary>
        /// Gets or sets the option.
        /// </summary>
        public int Option { get; set; }
    }
}
