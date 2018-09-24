// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoapIbisPlusConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoapIbisPlusConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// The configuration of the SOAP for IBIS plus protocol.
    /// </summary>
    [Serializable]
    [XmlRoot("SoapIbisPlus")]
    public class SoapIbisPlusConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoapIbisPlusConfig"/> class.
        /// </summary>
        public SoapIbisPlusConfig()
        {
            this.Service = new ServiceConfig();
            this.TripInfo = new TripInfoConfig();
            this.TransferInformation = new TransferInformationConfig();
            this.PassengerInfo = new PassengerInfoConfig();
            this.Transformations = new List<Chain>();
        }

        /// <summary>
        /// Gets or sets the Web Service config.
        /// </summary>
        public ServiceConfig Service { get; set; }

        /// <summary>
        /// Gets or sets the time sync config.
        /// </summary>
        public TimeSyncConfig TimeSync { get; set; }

        /// <summary>
        /// Gets or sets the trip info handling config.
        /// </summary>
        public TripInfoConfig TripInfo { get; set; }

        /// <summary>
        /// Gets or sets the transfer information config.
        /// </summary>
        public TransferInformationConfig TransferInformation { get; set; }

        /// <summary>
        /// Gets or sets the passenger info config.
        /// </summary>
        public PassengerInfoConfig PassengerInfo { get; set; }

        /// <summary>
        /// Gets or sets the data transformations.
        /// </summary>
        public List<Chain> Transformations { get; set; }
    }
}
