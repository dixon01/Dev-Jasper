// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaintenanceModeSettings.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MaintenanceModeSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Settings
{
    using System.Xml.Serialization;

    /// <summary>
    /// The maintenance mode. When enabled it will block all access to the background system
    /// except for the administrator. All other users will get an exception containing the reason string.
    /// </summary>
    public class MaintenanceModeSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceModeSettings"/> class.
        /// </summary>
        public MaintenanceModeSettings()
        {
            this.Reason = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether maintenance is enabled.
        /// </summary>
        [XmlAttribute("IsEnabled")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the reason for the current maintenance.
        /// </summary>
        [XmlElement("Reason")]
        public string Reason { get; set; }
    }
}