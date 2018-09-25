// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EciPositionBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   EciPositionBase Frames.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Eci.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The eci position base class.
    /// </summary>
    public abstract class EciPositionBase : EciMessageBase
    {
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets number of visible GPS satellites.
        /// </summary>
        public int GpsNumberSats { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the speed in m/s .
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// Gets or sets the speed in Km/s.
        /// </summary>
        [XmlIgnore]
        public double SpeedKmS
        {
            get
            {
                return this.Speed / 3.6;
            }

            set
            {
                this.Speed = value * 3.6;
            }
        }
    }
}
