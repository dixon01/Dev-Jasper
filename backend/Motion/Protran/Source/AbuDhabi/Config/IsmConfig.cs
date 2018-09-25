// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsmConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// /// Representation in XML style of an object ISI.
    /// </summary>
    [Serializable]
    public class IsmConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsmConfig"/> class.
        /// </summary>
        public IsmConfig()
        {
            this.Port = 21;
            this.IpAddress = "127.0.0.1";
            this.Behaviour = new IsmBehaviour();
        }

        /// <summary>
        /// Gets or sets the IP address (IP v4 only)
        /// of the FTP server device.
        /// </summary>
        [XmlElement("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the IP port of the FTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets general configurations to respect dealing
        /// with the FTP server.
        /// </summary>
        public IsmBehaviour Behaviour { get; set; }
    }
}
