// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Traindepartures.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Arriva.Connections
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about a train departure.
    /// </summary>
    public class Traindepartures
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Traindepartures"/> class.
        /// </summary>
        public Traindepartures()
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
