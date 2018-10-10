// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Busdepartures.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about a bus departure.
    /// </summary>
    public class Busdepartures
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Busdepartures"/> class.
        /// </summary>
        public Busdepartures()
        {
            this.Departures = new List<Departure>();
        }

        /// <summary>
        /// Gets or sets Departure.
        /// </summary>
        [XmlElement("departure")]
        public List<Departure> Departures { get; set; }
    }
}
