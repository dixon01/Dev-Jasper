// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trip.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Trip type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System;
    using System.Text;

    /// <summary>
    /// The trip.
    /// </summary>
    public class Trip
    {
        private string annonceExt = string.Empty;

        private string annonceExtTts = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trip"/> class.
        /// </summary>
        public Trip()
        {
            this.CustomerTripId = 0;
            this.Id = 0;
            this.Stop = new BusStopList();
            this.RouteId = 0;
            this.LineName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the external announcement.
        /// </summary>
        public string AnnonceExt
        {
            get
            {
                // si l'annonce text n'est pas renseigné, on prend le dernier arret
                if (!string.IsNullOrEmpty(this.annonceExt))
                {
                    return this.annonceExt;
                }

                if (this.Stop.Count == 0)
                {
                    return string.Empty;
                }

                string ret = this.Stop[this.Stop.Count - 1].Name1;
                if (string.IsNullOrEmpty(this.Stop[this.Stop.Count - 1].Name2))
                {
                    ret += "/";
                    ret += this.Stop[this.Stop.Count - 1].Name2; // modif pour Bienne
                }

                return ret;
            }

            set
            {
                this.annonceExt = value;
            }
        }

        /// <summary>
        /// Gets or sets the external TTS announcement.
        /// </summary>
        public string AnnonceExtTTS
        {
            // si annonce ext tts n'est pas renseigné, on prend l'annonce text.
            get
            {
                return this.annonceExtTts != string.Empty ? this.annonceExtTts : this.AnnonceExt;
            }

            set
            {
                this.annonceExtTts = value;
            }
        }

        /// <summary>
        /// Gets or sets the current line name (for the end user)
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// Gets or sets the current Route id
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Gets or sets the current trip's bus stops list
        /// </summary>
        public BusStopList Stop { get; set; }

        /// <summary>
        /// Gets or sets the current trip's ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the current customer trip's ID
        /// </summary>
        public int CustomerTripId { get; set; }

        /// <summary>
        /// Gets or sets the current trip's start date and time
        /// </summary>
        public DateTime DateTimeStart { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var nextStops = new StringBuilder();
            foreach (BusStop busStop in this.Stop)
            {
                nextStops.Append(busStop + "\n");
            }

            return "ID: " + this.Id + ", TimeStart: " + this.DateTimeStart + "\n" + nextStops;
        }
    }
}