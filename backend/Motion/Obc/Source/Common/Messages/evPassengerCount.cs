// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evPassengerCount.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evPassengerCount type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The passenger count event.
    /// </summary>
    public class evPassengerCount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evPassengerCount"/> class.
        /// </summary>
        public evPassengerCount()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evPassengerCount"/> class.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="blockNumber">
        /// The block number.
        /// </param>
        /// <param name="routePathNumber">
        /// The route path number.
        /// </param>
        /// <param name="stopNumber">
        /// The stop number.
        /// </param>
        /// <param name="driverNumber">
        /// The driver number.
        /// </param>
        /// <param name="passengerCount">
        /// The passenger count.
        /// </param>
        public evPassengerCount(
            int lineNumber,
            int blockNumber,
            int routePathNumber,
            int stopNumber,
            int driverNumber,
            int passengerCount)
        {
            this.LineNumber = lineNumber;
            this.BlockNumber = blockNumber;
            this.RoutePathNumber = routePathNumber;
            this.StopNumber = stopNumber;
            this.DriverNumber = driverNumber;
            this.PassengerCount = passengerCount;
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        public int BlockNumber { get; set; }

        /// <summary>
        /// Gets or sets the route path number.
        /// </summary>
        public int RoutePathNumber { get; set; }

        /// <summary>
        /// Gets or sets the stop number.
        /// </summary>
        public int StopNumber { get; set; }

        /// <summary>
        /// Gets or sets the driver number.
        /// </summary>
        public int DriverNumber { get; set; }

        /// <summary>
        /// Gets or sets the passenger count.
        /// </summary>
        public int PassengerCount { get; set; }
    }
}