// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;

    /// <summary>
    /// Container of all configuration of stop related information
    /// </summary>
    [Serializable]
    public abstract class StopConfigBase
    {
        /// <summary>
        /// Gets or sets the stop number config.
        /// </summary>
        public DataItemConfig StopNumber { get; set; }

        /// <summary>
        /// Gets or sets the stop name config.
        /// </summary>
        public DataItemConfig StopName { get; set; }

        /// <summary>
        /// Gets or sets the stop description config.
        /// </summary>
        public DataItemConfig StopDescription { get; set; }

        /// <summary>
        /// Gets or sets the stop symbol config.
        /// </summary>
        public DataItemConfig StopSymbol { get; set; }

        /// <summary>
        /// Gets or sets the stop arrival time config.
        /// </summary>
        public TimeItemConfig StopArrivalTime { get; set; }

        /// <summary>
        /// Gets or sets the stop departure time config.
        /// </summary>
        public TimeItemConfig StopDepartureTime { get; set; }
    }
}
