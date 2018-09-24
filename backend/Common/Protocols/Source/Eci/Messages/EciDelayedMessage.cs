// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciDelayedMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;

    /// <summary>
    /// The eci delayed packet message.
    /// </summary>
    public class EciDelayedMessage : EciMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EciDelayedMessage"/> class.
        /// </summary>
        /// <param name="vehicleId">
        /// The device id.
        /// </param>
        /// <param name="serviceNumber">
        /// The service number.
        /// </param>
        /// <param name="routeId">
        /// The route id.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="stopId">
        /// The stop id.
        /// </param>
        /// <param name="estimatedDelay">
        /// The estimated delay.
        /// </param>
        public EciDelayedMessage(
            int vehicleId,
            int serviceNumber,
               int routeId,
               int lineNumber,
               int stopId,
               TimeSpan estimatedDelay)
        {
            this.VehicleId = vehicleId;
            this.ServiceNumber = serviceNumber;
            this.RouteId = routeId;
            this.LineNumber = lineNumber;
            this.StopId = stopId;
            this.EstimatedDelay = estimatedDelay;
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public override EciMessageCode MessageType
        {
            get
            {
                return EciMessageCode.Delay;
            }
        }

        /// <summary>
        /// Gets or sets the service number.
        /// </summary>
        public int ServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the stop id.
        /// </summary>
        public int StopId { get; set; }

        /// <summary>
        /// Gets or sets the path id.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets or sets the estimated delay.
        /// </summary>
        public TimeSpan EstimatedDelay { get; set; }
    }
}
