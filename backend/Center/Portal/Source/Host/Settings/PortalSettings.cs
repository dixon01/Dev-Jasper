// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalSettings.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Settings
{
    /// <summary>
    /// The portal settings.
    /// </summary>
    public class PortalSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortalSettings"/> class.
        /// </summary>
        public PortalSettings()
        {
            this.HttpPort = 80;
            this.HttpsPort = 443;
            this.BackgroundSystemApiPort = 8081;
            this.BackgroundSystemSecureApiPort = 8082;
        }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        public string AppDataPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Portal should provide links to the Beta versions of the
        /// applications (<c>true</c>) or not (<c>false</c>).
        /// </summary>
        public bool ClickOnceUseBeta { get; set; }

        /// <summary>
        /// Gets or sets the http port.
        /// </summary>
        public int HttpPort { get; set; }

        /// <summary>
        /// Gets or sets the https port.
        /// </summary>
        public int HttpsPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether https is enabled or not.
        /// </summary>
        public bool EnableHttps { get; set; }

        /// <summary>
        /// Gets or sets the port used by the BackgroundSystem REST api.
        /// </summary>
        public int BackgroundSystemApiPort { get; set; }

        /// <summary>
        /// Gets or sets the port used by the BackgroundSystem REST api (secure version).
        /// </summary>
        public int BackgroundSystemSecureApiPort { get; set; }
    }
}