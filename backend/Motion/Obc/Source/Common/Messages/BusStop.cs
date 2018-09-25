// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusStop.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BusStop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The bus stop information.
    /// </summary>
    public class BusStop
    {
        private string name1Tts = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusStop"/> class.
        /// </summary>
        public BusStop()
        {
            this.Direction = 0;
            this.Announce = 0;
            this.DruckName = string.Empty;
            this.Didok = 0;
            this.SignCode = 0;
            this.Zone = 0;
            this.Name2 = string.Empty;
            this.Name1 = string.Empty;
            this.OrtOrigin = 0;
            this.Ort = 0;
            this.SecondsFromDeparture = 0;
        }

        /// <summary>
        /// Gets or sets the time in seconds between departure and this stop left
        /// </summary>
        [XmlElement("SecDep")]
        public int SecondsFromDeparture { get; set; }

        /// <summary>
        /// Gets the local time to leave this stop
        /// </summary>
        [XmlIgnore]
        public DateTime DateTimetoLeave
        {
            get
            {
                return RemoteEventHandler.CurrentTrip.DateTimeStart.AddSeconds(this.SecondsFromDeparture);
            }
        }

        /// <summary>
        /// Gets or sets the Stop ID
        /// </summary>
        public int Ort { get; set; }

        /// <summary>
        /// Gets or sets the Customer Stop ID
        /// </summary>
        public int OrtOrigin { get; set; }

        /// <summary>
        /// Gets or sets the commercial stop name
        /// </summary>
        public string Name1 { get; set; }

        /// <summary>
        /// Gets or sets the alternative commercial stop name
        /// </summary>
        public string Name2 { get; set; }

        /// <summary>
        /// Gets or sets the ticketing zone
        /// </summary>
        public short Zone { get; set; }

        /// <summary>
        /// Gets or sets the Sign Destination code
        /// </summary>
        public int SignCode { get; set; }

        /// <summary>
        /// Gets or sets the Didok number (Atron/Biel)
        /// </summary>
        public int Didok { get; set; }

        /// <summary>
        /// Gets or sets the Didok number (Atron/Biel)
        /// </summary>
        public string DruckName { get; set; }

        /// <summary>
        /// Gets or sets the announcement code
        /// </summary>
        public short Announce { get; set; }

        /// <summary>
        /// Gets or sets the direction code
        /// </summary>
        public short Direction { get; set; }

        /// <summary>
        /// Gets or sets the name 1 TTS.
        /// </summary>
        public string Name1TTS
        {
            // si TTS n'est pas renseigné, utiliser le nom text.
            get
            {
                return !string.IsNullOrEmpty(this.name1Tts) ? this.name1Tts : this.Name1;
            }

            set
            {
                this.name1Tts = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "BusStop. N: " + this.Name1 + ", Sgn: " + this.SignCode + ", Zo: " + this.Zone + ", Dep: "
                   + this.DateTimetoLeave;
        }
    }
}