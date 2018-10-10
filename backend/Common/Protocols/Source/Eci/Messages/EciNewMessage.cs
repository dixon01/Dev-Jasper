// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciNewMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The ECI new message format
    /// </summary>
    public class EciNewMessage : EciMessageBase
    {
        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.GeneralMessageNewFormat;
            }
        }

        /// <summary>
        /// Gets or sets the service number.
        /// </summary>
        public int ServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the path id.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        // [ABA] TODO: type should be changed from int to VehicleTypeEnum

        /// <summary>
        /// Gets or sets the vehicle type.
        /// </summary>
        public int VehicleType { get; set; }

        /// <summary>
        /// Gets or sets the position id.
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the position type.
        /// </summary>
        public char PositionType { get; set; }
    }
}
