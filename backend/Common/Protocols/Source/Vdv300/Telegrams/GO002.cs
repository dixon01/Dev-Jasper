// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO002.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv300.Telegrams
{
    using System.Xml.Serialization;

    /// <summary>
    /// Representation of all the information
    /// contained into an IBIS GO001 telegram.
    /// </summary>
    public class GO002 : StringTelegram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO002"/> class.
        /// </summary>
        public GO002()
        {
            this.StopIndex = -1;
        }

        /// <summary>
        /// Gets or sets DataLength.
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// Gets or sets the stop index string.
        /// </summary>
        public int StopIndex { get; set; }

        /// <summary>
        /// Gets or sets the row number string.
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets or sets the pictogram string.
        /// </summary>
        public string Pictogram { get; set; }

        /// <summary>
        /// Gets or sets the line number  string.
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the departure time  string.
        /// </summary>
        public string DepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the track number string.
        /// </summary>
        public string TrackNumber { get; set; }

        /// <summary>
        /// Gets or sets the deviation string.
        /// </summary>
        public string Deviation { get; set; }

        /// <summary>
        /// Gets or sets the destination string.
        /// </summary>
        [XmlIgnore]
        public string Destination
        {
            get
            {
                return this.Data;
            }

            set
            {
                this.Data = value;
            }
        }
    }
}
