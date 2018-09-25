// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="FtpUpdateClientConfig.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Update.Clients
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    ///     Configuration of FTP update client.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.Ftp.FtpUpdateClient, Gorba.Common.Update.Ftp")]
    public class FtpUpdateClientConfig : UpdateClientConfigBase, IFtpUpdateConfig
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpUpdateClientConfig" /> class.
        /// </summary>
        public FtpUpdateClientConfig()
        {
            this.Port = 21;
            this.RepositoryBasePath = string.Empty;
            this.PollInterval = TimeSpan.FromMinutes(5);
            this.Name = "FtpClient";
            this.LocalFtpHomePath = @"D:\Ftproot";
            this.RequireWifiNetworkConnection = false;
#if LTG_INFOTRANSITE
            this.EnableMulticastIP = true;
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether enable of setting the FTP Config settings via multicast networking.</summary>
        public bool EnableMulticastIP { get; set; }

        /// <summary>
        ///     Gets or sets the FTP server host name.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the local Ftp Home Root on the master display to be used as an Ftp server setup for ftp with 3rdparty - Prosys/LAM
        /// </summary>
        public string LocalFtpHomePath { get; set; }

        /// <summary>
        ///     Gets or sets the FTP server login password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the poll interval at which the repository is
        ///     scanned.
        /// </summary>
        [XmlIgnore]
        public TimeSpan PollInterval { get; set; }

        /// <summary>Gets or sets a value indicating whether to require a wifi network connection on Ftp client updates.</summary>
        [XmlElement]
        public bool RequireWifiNetworkConnection { get; set; }

        /// <summary>
        ///     Gets or sets the poll interval as an XML serialize string.
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

        /// <summary>
        ///     Gets or sets the FTP server TCP port.
        /// </summary>
        [DefaultValue(21)]
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets the source path of the update information.
        /// </summary>
        public string RepositoryBasePath { get; set; }

        /// <summary>
        ///     Gets or sets the FTP server login username.
        /// </summary>
        public string Username { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Test the ftp config to be valid with a Host IP, Username and password.</summary>
        /// <param name="config">The config.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsFtpCredentialsValid(FtpUpdateClientConfig config)
        {
            if (config != null)
            {
                IPAddress ip;
                return IPAddress.TryParse(config.Host, out ip) && !string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password);
            }

            return false;
        }

        #endregion
    }
}