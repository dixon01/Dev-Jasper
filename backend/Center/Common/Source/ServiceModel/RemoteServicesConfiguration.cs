// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteServicesConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteServicesConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the configuration for remote services.
    /// Addresses have the following form:
    /// {protocol}://{hostname}[:{port}][/{path}]/{serviceName}[{nameSuffix}]
    /// </summary>
    public class RemoteServicesConfiguration
    {
        /// <summary>
        /// The default port.
        /// </summary>
        public static readonly int DefaultPort = DefaultPortValue;

        private const int DefaultPortValue = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServicesConfiguration"/> class.
        /// </summary>
        public RemoteServicesConfiguration()
        {
            this.InternalPort = DefaultPort;
            this.Port = DefaultPort;
            this.Protocol = RemoveServiceProtocol.Tcp;
        }

        /// <summary>
        /// Gets or sets the path for the service.
        /// </summary>
        [XmlElement("Path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        [XmlIgnore]
        public RemoveServiceProtocol Protocol { get; set; }

        /// <summary>
        /// Gets or sets the protocol value.
        /// </summary>
        [XmlElement("Protocol")]
        public string ProtocolValue
        {
            get
            {
                return this.Protocol.ToString();
            }

            set
            {
                this.Protocol = (RemoveServiceProtocol)Enum.Parse(typeof(RemoveServiceProtocol), value);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>An integer indicating the port number.</value>
        /// <remarks>Use <see cref="DefaultPort"/> for the default port of the protocol.</remarks>
        [XmlElement("Port")]
        [DefaultValue(DefaultPortValue)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the internal port.
        /// </summary>
        /// <value>An integer indicating the port number user internally.</value>
        /// <remarks>Use <see cref="DefaultPort"/> for the default port of the protocol.</remarks>
        [XmlElement("InternalPort")]
        [DefaultValue(DefaultPortValue)]
        public int InternalPort { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [XmlElement("Host")]
        public string Host { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{{RemoteServicesConfiguration {0}://{1}:{2}}}", this.Protocol, this.Host, this.Port);
        }
    }
}