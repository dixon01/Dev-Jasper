// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsData.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Entities.Gps
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Information from a GPS receiver.
    /// </summary>
    public class GpsData
    {
        /// <summary>
        /// Gets or sets the time received from the satellites in UTC.
        /// This is not the local timestamp when the GPS data was received!
        /// </summary>
        public DateTime? SatelliteTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the longitude in degrees [°].
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude in degrees [°].
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the altitude in meters [m].
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        /// Gets or sets the speed in meters per second [m/s].
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle is stopped.
        /// </summary>
        public bool IsStopped { get; set; }

        /// <summary>
        /// Gets or sets the direction in degrees [°].
        /// </summary>
        public float Direction { get; set; }

        /// <summary>
        /// Gets or sets the dilution of the precision (also known as PDOP in NMEA).
        /// </summary>
        public float PrecisionDilution { get; set; }

        /// <summary>
        /// Gets or sets the local offset from UTC.
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        /// <summary>
        /// Gets or sets the number of satellites used for calculation.
        /// </summary>
        public int SatelliteCount { get; set; }

        /// <summary>
        /// Gets or sets the state of the data in this object.
        /// </summary>
        public GpsState State { get; set; }

        /// <summary>
        /// Gets a value indicating whether this data is valid.
        /// </summary>
        [XmlIgnore]
        public bool IsValid
        {
            get
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                return this.Latitude != 0 && this.Latitude != 90 && this.Longitude != 90;
                // ReSharper restore CompareOfFloatsByEqualityOperator
            }
        }
    }
}
