// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Arriva
{
    /// <summary>
    /// Arriva FTP configuration.
    /// </summary>
    public class FtpConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpConfig"/> class.
        /// </summary>
        public FtpConfig()
        {
            this.PollingEnabled = false;
            this.SourceDirectory = string.Empty;
            this.Filename = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Polling.
        /// </summary>
        public bool PollingEnabled { get; set; }

        /// <summary>
        /// Gets or sets SourceDirectory.
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets Filename.
        /// </summary>
        public string Filename { get; set; }
    }
}
