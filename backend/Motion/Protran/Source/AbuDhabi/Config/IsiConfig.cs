// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Representation in XML style of an object ISI.
    /// </summary>
    [Serializable]
    public class IsiConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsiConfig"/> class.
        /// </summary>
        public IsiConfig()
        {
            this.Port = 51001;
            this.IpAddress = "127.0.0.1";
            this.LogToFile = string.Empty;
        }

        /// <summary>
        /// Gets or sets the IP address (IP v4 only)
        /// of the ISI TCP/IP server.
        /// </summary>
        [XmlElement("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the IP port of the ISI TCP/IP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the path of the file to log to.
        /// If the value is null or empty, logging is disabled.
        /// </summary>
        [DefaultValue("")]
        public string LogToFile { get; set; }
    }
}
