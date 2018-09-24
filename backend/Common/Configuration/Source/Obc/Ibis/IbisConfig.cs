// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The IBIS config file.
    /// </summary>
    [Serializable]
    public class IbisConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisConfig"/> class.
        /// </summary>
        public IbisConfig()
        {
            this.InterfaceSettings = new InterfaceSettings();
            this.Functionality = new Functionality();
            this.Devices = new IbisDevices();
            this.IbisTelegrams = new IbisTelegrams();
        }

        /// <summary>
        /// Gets or sets the devices configuration.
        /// </summary>
        [XmlElement(ElementName = "Devices")]
        public IbisDevices Devices { get; set; }

        /// <summary>
        /// Gets or sets the IBIS telegrams configuration.
        /// </summary>
        [XmlElement(ElementName = "IBIS_Telegrams")]
        public IbisTelegrams IbisTelegrams { get; set; }

        /// <summary>
        /// Gets or sets the IBIS functionality configuration.
        /// </summary>
        [XmlElement(ElementName = "Functionality")]
        public Functionality Functionality { get; set; }

        /// <summary>
        /// Gets or sets the IBIS interface settings.
        /// </summary>
        [XmlElement(ElementName = "Interface")]
        public InterfaceSettings InterfaceSettings { get; set; }
    }
}