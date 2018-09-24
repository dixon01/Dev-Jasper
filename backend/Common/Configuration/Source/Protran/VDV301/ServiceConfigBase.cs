// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.VDV301
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for services.
    /// </summary>
    [Serializable]
    public abstract class ServiceConfigBase
    {
        /// <summary>
        /// Gets or sets the host name.
        /// If this value is not set, the service address will be searched with DNS-SD.
        /// </summary>
        [XmlAttribute("Host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the TCP port on which the service is provided.
        /// If this value is not set, the service address will be searched with DNS-SD.
        /// </summary>
        [XmlAttribute("Port")]
        [DefaultValue(0)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the path under which the service is provided.
        /// This should not contain the service name, but just the path to the root (might be empty).
        /// </summary>
        [XmlAttribute("Path")]
        public string Path { get; set; }
    }
}