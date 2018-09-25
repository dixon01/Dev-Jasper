// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerReadOnlyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Meta
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The FTP server read-only data view model.
    /// </summary>
    public class FtpServerReadOnlyDataViewModel : ReadOnlyDataViewModelBase
    {
        private readonly FtpUpdateProviderConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerReadOnlyDataViewModel"/> class.
        /// </summary>
        /// <param name="config">
        /// The underlying <see cref="FtpUpdateProviderConfig"/>.
        /// </param>
        /// <param name="id">
        /// The id of this data view model (index in the list of FTP servers + 1).
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        public FtpServerReadOnlyDataViewModel(FtpUpdateProviderConfig config, int id, DataViewModelFactory factory)
            : base(factory)
        {
            this.config = config;
            this.Id = id;
        }

        /// <summary>
        /// Gets the display text of this data view model.
        /// </summary>
        public override string DisplayText
        {
            get
            {
                return this.Host;
            }
        }

        /// <summary>
        /// Gets the id (index in the list of FTP servers + 1).
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the FTP server host name.
        /// </summary>
        public string Host
        {
            get
            {
                return this.config.Host;
            }
        }

        /// <summary>
        /// Gets the FTP server TCP port.
        /// </summary>
        public int Port
        {
            get
            {
                return this.config.Port;
            }
        }

        /// <summary>
        /// Gets the FTP server login username.
        /// </summary>
        public string Username
        {
            get
            {
                return this.config.Username;
            }
        }

        /// <summary>
        /// Gets the FTP server login password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.config.Password;
            }
        }

        /// <summary>
        /// Gets the source path of the update information.
        /// </summary>
        public string RepositoryBasePath
        {
            get
            {
                return this.config.RepositoryBasePath;
            }
        }

        /// <summary>
        /// Gets the poll interval at which the repository is scanned.
        /// </summary>
        public TimeSpan PollInterval
        {
            get
            {
                return this.config.PollInterval;
            }
        }

        /// <summary>
        /// Gets the compression mode to be used.
        /// </summary>
        public CompressionAlgorithm Compression
        {
            get
            {
                return this.config.Compression;
            }
        }

        /// <summary>
        /// Gets the string representation of the id.
        /// </summary>
        /// <returns>
        /// The id as a <see cref="string"/>.
        /// </returns>
        public override string GetIdString()
        {
            return this.Id.ToString(CultureInfo.InvariantCulture);
        }
    }
}
