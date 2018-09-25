// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpUpdateProviderConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpUpdateProviderConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Providers
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration of FTP update provider.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Ftp.FtpUpdateProvider, Gorba.Common.Update.Ftp")]
    public class FtpUpdateProviderConfig : UpdateProviderConfigBase, IFtpUpdateConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpUpdateProviderConfig"/> class.
        /// </summary>
        public FtpUpdateProviderConfig()
        {
            this.Compression = CompressionAlgorithm.None;
            this.Port = 21;
            this.RepositoryBasePath = string.Empty;
            this.PollInterval = TimeSpan.FromMinutes(5);
            this.Name = "FtpProvider";
        }

        /// <summary>
        /// Gets or sets the compression mode to be used by this provider.
        /// </summary>
        public CompressionAlgorithm Compression { get; set; }

        /// <summary>
        /// Gets or sets the FTP server host name.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the FTP server TCP port.
        /// </summary>
        [DefaultValue(21)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the FTP server login username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the FTP server login password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the source path of the update information.
        /// </summary>
        public string RepositoryBasePath { get; set; }

        /// <summary>
        /// Gets or sets the poll interval at which the repository is
        /// scanned.
        /// </summary>
        [XmlIgnore]
        public TimeSpan PollInterval { get; set; }

        /// <summary>
        /// Gets or sets the poll interval as an XML serializable string.
        /// </summary>
        [XmlElement("PollInterval")]
        public string PollIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.PollInterval);
            }

            set
            {
                this.PollInterval = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}