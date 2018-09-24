// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cu5Config.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Representation in XML style of an object CU5.
    /// </summary>
    [Serializable]
    public class Cu5Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cu5Config"/> class.
        /// </summary>
        public Cu5Config()
        {
            this.Port = 32021;
            this.IpAddress = "127.0.0.1";
        }

        /// <summary>
        /// Gets or sets the IP address (IP v4 only)
        /// of the Cu5 device.
        /// </summary>
        [XmlElement("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the IP port of the Cu5 device.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the IP address (IP v4 only) of Protran.
        /// This should usually not be set and is mostly used for testing.
        /// </summary>
        public string LocalAddress { get; set; }
    }
}
