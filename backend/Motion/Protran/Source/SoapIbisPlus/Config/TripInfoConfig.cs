// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TripInfoConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TripInfoConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;

    /// <summary>
    /// Configuration for the handling of the <code>SetTripInfo()</code> method.
    /// </summary>
    [Serializable]
    public class TripInfoConfig : StopConfigBase
    {
        /// <summary>
        /// Gets or sets the route number config.
        /// </summary>
        public DataItemConfig RouteNumber { get; set; }

        /// <summary>
        /// Gets or sets the run number config.
        /// </summary>
        public DataItemConfig RunNumber { get; set; }

        /// <summary>
        /// Gets or sets the block number config.
        /// </summary>
        public DataItemConfig BlockNumber { get; set; }

        /// <summary>
        /// Gets or sets the direction config.
        /// </summary>
        public DataItemConfig Direction { get; set; }

        /// <summary>
        /// Gets or sets the destination number config.
        /// </summary>
        public DataItemConfig DestinationNumber { get; set; }

        /// <summary>
        /// Gets or sets the destination name config.
        /// </summary>
        public DataItemConfig DestinationName { get; set; }

        /// <summary>
        /// Gets or sets the destination description config.
        /// </summary>
        public DataItemConfig DestinationDescription { get; set; }

        /// <summary>
        /// Gets or sets the destination symbol config.
        /// </summary>
        public DataItemConfig DestinationSymbol { get; set; }

        /// <summary>
        /// Gets or sets the destination arrival time config.
        /// </summary>
        public TimeItemConfig DestinationArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the destination departure time config.
        /// </summary>
        public TimeItemConfig DestinationDepartureTime { get; set; }
    }
}