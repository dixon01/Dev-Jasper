// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemSettings.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BackgroundSystemSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Settings
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// Defines the internal configuration of the BackgroundSystem.
    /// </summary>
    [XmlRoot("BackgroundSystemSettings")]
    public class BackgroundSystemSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemSettings"/> class.
        /// </summary>
        public BackgroundSystemSettings()
        {
            this.MaintenanceMode = new MaintenanceModeSettings();
        }

        /// <summary>
        /// Gets or sets the Azure update provider configuration.
        /// </summary>
        [XmlElement("AzureUpdateProvider")]
        public AzureUpdateProviderConfig AzureUpdateProvider { get; set; }

        /// <summary>
        /// Gets or sets the FTP update providers.
        /// </summary>
        [XmlElement("FtpUpdateProvider")]
        public List<FtpUpdateProviderConfig> FtpUpdateProviders { get; set; }

        /// <summary>
        /// Gets or sets the maintenance mode.
        /// </summary>
        [XmlElement("MaintenanceMode")]
        public MaintenanceModeSettings MaintenanceMode { get; set; }

        /// <summary>
        /// Gets or sets the path to uploaded files.
        /// </summary>
        [XmlElement("UploadsPath")]
        public string UploadsPath { get; set; }
    }
}