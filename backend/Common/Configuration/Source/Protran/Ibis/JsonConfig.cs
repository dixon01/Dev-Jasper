// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for IBIS over JSON interface
    /// </summary>
    [Serializable]
    public class JsonConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfig"/> class.
        /// </summary>
        public JsonConfig()
        {
            this.IpAddress = "127.0.0.1";
            this.Port = 3011;
        }

        /// <summary>
        /// Gets or sets the ip address of the JSON server.
        /// </summary>
        [XmlElement("IPAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the port of the JSON interface.
        /// </summary>
        public int Port { get; set; }
    }
}
