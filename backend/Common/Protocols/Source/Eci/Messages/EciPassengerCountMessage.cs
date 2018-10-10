// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciPassengerCountMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    /// <summary>
    /// The passenger count message.
    /// </summary>
    public class EciPassengerCountMessage : EciPositionBase
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
        /// Gets or sets the passenger count.
        /// </summary>
        public int NumberOfPassengers { get; set; }

        /// <summary>
        /// Gets or sets the driver id.
        /// </summary>
        public int DriverId { get; set; }

        /// <summary>
        /// Gets or sets the path id.
        /// </summary>
        public int PathId { get; set; }

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
    }
}
