// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferInformationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;

    /// <summary>
    /// Configuration for the handling of the <code>SetTransferInformation()</code> method,
    /// <code>UpdateTransferInformation()</code> and <code>DeleteTransferInformation()</code>
    /// </summary>
    [Serializable]
    public class TransferInformationConfig : StopConfigBase
    {
        /// <summary>
        /// Gets or sets the planned departure time config.
        /// </summary>
        public TimeItemConfig PlannedDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the calculated departure time config.
        /// </summary>
        public TimeItemConfig CalculatedDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the route number config.
        /// </summary>
        public DataItemConfig RouteNumber { get; set; }

        /// <summary>
        /// Gets or sets the destination text config.
        /// </summary>
        public DataItemConfig DestinationText { get; set; }

        /// <summary>
        /// Gets or sets the track text config.
        /// </summary>
        public DataItemConfig TrackText { get; set; }
    }
}
