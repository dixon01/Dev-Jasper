// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisDevices.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisDevices type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration about IBIS devices connected to the OBU.
    /// </summary>
    [Serializable]
    public class IbisDevices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisDevices"/> class.
        /// </summary>
        public IbisDevices()
        {
            this.GorbaTft = new GorbaTftUnit();
            this.TicketingConfig = new TicketingConfig();
            this.PassengerCountConfig = new PassengerCountConfig();
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlAttribute(AttributeName = "Description")]
        public string Description
        {
            get
            {
                return "If the IBIS_Address is -1, then the device is disabled";
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the ticketing config.
        /// </summary>
        [XmlElement(ElementName = "TicketingConfig")]
        public TicketingConfig TicketingConfig { get; set; }

        /// <summary>
        /// Gets or sets the Gorba TFT configuration.
        /// </summary>
        [XmlElement(ElementName = "GorbaTFT")]
        public GorbaTftUnit GorbaTft { get; set; }

        /// <summary>
        /// Gets or sets the passenger counting config.
        /// </summary>
        [XmlElement(ElementName = "PassengerCountingConfig")]
        public PassengerCountConfig PassengerCountConfig { get; set; }
    }
}