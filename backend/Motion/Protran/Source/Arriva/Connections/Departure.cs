// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Departure.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about a departure.
    /// </summary>
    public class Departure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Departure"/> class.
        /// </summary>
        public Departure()
        {
            this.Departuretime = string.Empty;
            this.Delay = string.Empty;
            this.Platform = string.Empty;
            this.Destination = string.Empty;
            this.Pto = string.Empty;
            this.StopCode = string.Empty;
            this.Line = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether departure time.
        /// </summary>
        [XmlAttribute("departuretime")]
        public string Departuretime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Delay.
        /// </summary>
        [XmlAttribute("delay")]
        public string Delay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Platform.
        /// </summary>
        [XmlAttribute("platform")]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Destination.
        /// </summary>
        [XmlAttribute("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Pto.
        /// </summary>
        [XmlAttribute("pto")]
        public string Pto { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether StopCode.
        /// </summary>
        [XmlAttribute]
        public string StopCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Line.
        /// </summary>
        [XmlAttribute("line")]
        public string Line { get; set; }
    }
}
