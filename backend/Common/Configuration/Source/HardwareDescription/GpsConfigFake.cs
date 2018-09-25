// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsConfigFake.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsConfigFake type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The GPS config fake until the correct configuration is found.
    /// </summary>
    [Serializable]
    public class GpsConfigFake
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpsConfigFake"/> class.
        /// </summary>
        public GpsConfigFake()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpsConfigFake"/> class.
        /// </summary>
        /// <param name="gpsType">
        /// The GPS type.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public GpsConfigFake(string gpsType, int index)
        {
            this.GpsType = gpsType; // some fake attributes
            this.Index = index;
        }

        /// <summary>
        /// Gets or sets the GPS type, FAKE.
        /// </summary>
        [XmlAttribute]
        public string GpsType { get; set; }

        /// <summary>
        /// Gets or sets the index, FAKE.
        /// </summary>
        [XmlAttribute]
        public int Index { get; set; }
    }
}