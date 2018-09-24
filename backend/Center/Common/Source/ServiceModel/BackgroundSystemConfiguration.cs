// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the configuration of the BackgroundSystem.
    /// </summary>
    public class BackgroundSystemConfiguration
    {
        public const int DefaultApiHostPort = 8081;
        private int apiHostPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemConfiguration"/> class.
        /// </summary>
        public BackgroundSystemConfiguration()
        {
            this.DataServices = ServiceConfigurationDefaults.DefaultDataServicesConfiguration;
            this.FunctionalServices = ServiceConfigurationDefaults.DefaultFunctionalServicesConfiguration;
        }

        /// <summary>
        /// Gets or sets the connection string for the Service Bus.
        /// </summary>
        [XmlElement("NotificationsConnectionString")]
        public string NotificationsConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the port to use for communicating with the API host.
        /// </summary>
        [XmlElement("ApiHostPort")]
        public int ApiHostPort
        {
            get { return apiHostPort; }
            set
            {
                if (value <= 0)
                {
                    value = DefaultApiHostPort;
                }
                apiHostPort = value;
            }
        }

        /// <summary>
        /// Gets or sets the configuration for data services.
        /// </summary>
        [XmlElement("DataServices")]
        public RemoteServicesConfiguration DataServices { get; set; }

        /// <summary>
        /// Gets or sets the configuration for functional services (e.g.: membership service).
        /// </summary>
        [XmlElement("FunctionalServices")]
        public RemoteServicesConfiguration FunctionalServices { get; set; }
    }
}