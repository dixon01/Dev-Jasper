// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Meta
{
    using System;
    using System.Windows.Input;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The FTP server writable data view model.
    /// </summary>
    public class FtpServerDataViewModel : DataViewModelBase
    {
        private CompressionAlgorithm compression;

        private string host;

        private int port;

        private string username;

        private string password;

        private string repositoryBasePath;

        private TimeSpan pollInterval;

        private VerificationState verificationState;

        private string verificationMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerDataViewModel"/> class.
        /// </summary>
        /// <param name="dataViewModel">
        /// The read-only data view model.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        public FtpServerDataViewModel(FtpServerReadOnlyDataViewModel dataViewModel, DataViewModelFactory factory)
            : base(dataViewModel, factory)
        {
            if (dataViewModel == null)
            {
                return;
            }

            this.Id = dataViewModel.Id;
            this.Compression = dataViewModel.Compression;
            this.Host = dataViewModel.Host;
            this.Password = dataViewModel.Password;
            this.PollInterval = dataViewModel.PollInterval;
            this.Port = dataViewModel.Port;
            this.RepositoryBasePath = dataViewModel.RepositoryBasePath;
            this.Username = dataViewModel.Username;
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
        /// Gets or sets the compression mode to be used by this provider.
        /// </summary>
        public CompressionAlgorithm Compression
        {
            get
            {
                return this.compression;
            }

            set
            {
                this.SetProperty(ref this.compression, value, () => this.Compression);
            }
        }

        /// <summary>
        /// Gets or sets the FTP server host name.
        /// </summary>
        public string Host
        {
            get
            {
                return this.host;
            }

            set
            {
                this.SetProperty(ref this.host, value, () => this.Host);
            }
        }

        /// <summary>
        /// Gets or sets the FTP server TCP port.
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                this.SetProperty(ref this.port, value, () => this.Port);
            }
        }

        /// <summary>
        /// Gets or sets the FTP server login username.
        /// </summary>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.SetProperty(ref this.username, value, () => this.Username);
            }
        }

        /// <summary>
        /// Gets or sets the FTP server login password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.SetProperty(ref this.password, value, () => this.Password);
            }
        }

        /// <summary>
        /// Gets or sets the source path of the update information.
        /// </summary>
        public string RepositoryBasePath
        {
            get
            {
                return this.repositoryBasePath;
            }

            set
            {
                this.SetProperty(ref this.repositoryBasePath, value, () => this.RepositoryBasePath);
            }
        }

        /// <summary>
        /// Gets or sets the poll interval at which the repository is
        /// scanned.
        /// </summary>
        public TimeSpan PollInterval
        {
            get
            {
                return this.pollInterval;
            }

            set
            {
                this.SetProperty(ref this.pollInterval, value, () => this.PollInterval);
            }
        }

        /// <summary>
        /// Gets or sets the verification state.
        /// </summary>
        public VerificationState VerificationState
        {
            get
            {
                return this.verificationState;
            }

            set
            {
                this.SetProperty(ref this.verificationState, value, () => this.VerificationState);
            }
        }

        /// <summary>
        /// Gets or sets the verification message.
        /// </summary>
        public string VerificationMessage
        {
            get
            {
                return this.verificationMessage;
            }

            set
            {
                this.SetProperty(ref this.verificationMessage, value, () => this.VerificationMessage);
            }
        }

        /// <summary>
        /// Gets or sets the command to verify the FTP server settings.
        /// </summary>
        public ICommand VerifyFtpServerCommand { get; set; }

        /// <summary>
        /// Creates a new config object from this data view model.
        /// </summary>
        /// <returns>
        /// The <see cref="FtpUpdateProviderConfig"/>.
        /// </returns>
        public FtpUpdateProviderConfig CreateConfig()
        {
            return new FtpUpdateProviderConfig
                       {
                           Compression = this.Compression,
                           Host = this.Host,
                           Name = "FTP Provider",
                           Password = this.Password,
                           PollInterval = this.PollInterval,
                           Port = this.Port,
                           RepositoryBasePath = this.RepositoryBasePath,
                           Username = this.Username
                       };
        }

        /// <summary>
        /// Implementation of the disposing.
        /// </summary>
        /// <param name="disposing">
        /// A flag indicating if the object is being disposed or finalized.
        /// </param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}