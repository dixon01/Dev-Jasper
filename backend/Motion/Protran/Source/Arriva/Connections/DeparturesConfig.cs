// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeparturesConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the fields required to configure correctly a departure.
    /// </summary>
    [XmlRoot("departures")]
    public class DeparturesConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeparturesConfig"/> class.
        /// </summary>
        public DeparturesConfig()
        {
            this.Traindepartures = new Traindepartures();
            this.Busdepartures = new Busdepartures();
        }

        /// <summary>
        /// Gets or sets Train departures.
        /// </summary>
        [XmlElement("traindepartures")]
        public Traindepartures Traindepartures { get; set; }

        /// <summary>
        /// Gets or sets Bus departures.
        /// </summary>
        [XmlElement("busdepartures")]
        public Busdepartures Busdepartures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departure time.
        /// </summary>
        [XmlAttribute("device-id")]
        public string Deviceid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departure time.
        /// </summary>
        [XmlAttribute("expiration")]
        public string Expiration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departure time.
        /// </summary>
        [XmlAttribute("stationname")]
        public string Stationname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether departure time.
        /// </summary>
        [XmlAttribute("ETA")]
        public string Eta { get; set; }
    }
}
