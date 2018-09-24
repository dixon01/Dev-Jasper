// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFtpUpdateConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFtpUpdateConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Common
{
    using System.ComponentModel;

    /// <summary>
    /// Interface to be implemented by FTP Update Client and Provider configurations.
    /// </summary>
    public interface IFtpUpdateConfig
    {
        /// <summary>
        /// Gets or sets the FTP server host name.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Gets or sets the FTP server TCP port.
        /// </summary>
        [DefaultValue(21)]
        int Port { get; set; }

        /// <summary>
        /// Gets or sets the FTP server login username.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the FTP server login password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the source path of the update information.
        /// </summary>
        string RepositoryBasePath { get; set; }
    }
}