// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerStageViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerStageViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages.Meta
{
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The stage view model for FTP servers from the <see cref="BackgroundSystemSettings"/>.
    /// </summary>
    public class FtpServerStageViewModel : EntityStageViewModelBase
    {
        private FtpServerReadOnlyDataViewModel selectedFtpServer;

        private IReadOnlyEntityCollection<FtpServerReadOnlyDataViewModel> ftpServers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerStageViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public FtpServerStageViewModel(ICommandRegistry commandRegistry)
            : base(commandRegistry)
        {
            this.Name = this.EntityName = "FtpServer";
            this.PluralDisplayName = "FTP Servers";
            this.SingularDisplayName = "FTP Server";

            this.IsLoading = true;
        }

        /// <summary>
        /// Gets or sets the list of FTP servers.
        /// </summary>
        public IReadOnlyEntityCollection<FtpServerReadOnlyDataViewModel> FtpServers
        {
            get
            {
                return this.ftpServers;
            }

            set
            {
                var old = this.ftpServers;
                if (!this.SetProperty(ref this.ftpServers, value, () => this.FtpServers))
                {
                    return;
                }

                if (old != null)
                {
                    old.PropertyChanged -= this.AllOnPropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += this.AllOnPropertyChanged;
                    this.IsLoading = value.IsLoading;
                }
                else
                {
                    this.IsLoading = true;
                }

                this.RaisePropertyChanged(() => this.Instances);
            }
        }

        /// <summary>
        /// Gets or sets the selected FTP server.
        /// </summary>
        public FtpServerReadOnlyDataViewModel SelectedFtpServer
        {
            get
            {
                return this.selectedFtpServer;
            }

            set
            {
                var model = this.FtpServers.FirstOrDefault(m => m.Equals(value));
                if (this.SetProperty(ref this.selectedFtpServer, model, () => this.SelectedFtpServer))
                {
                    this.RaisePropertyChanged(() => this.SelectedInstance);
                }
            }
        }

        /// <summary>
        /// Gets the list of instances.
        /// </summary>
        public override IList Instances
        {
            get
            {
                return this.FtpServers;
            }
        }

        /// <summary>
        /// Gets or sets the selected instance from the <see cref="EntityStageViewModelBase.Instances"/> collection.
        /// </summary>
        public override ReadOnlyDataViewModelBase SelectedInstance
        {
            get
            {
                return this.SelectedFtpServer;
            }

            set
            {
                this.SelectedFtpServer = (FtpServerReadOnlyDataViewModel)value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type of entity has details.
        /// </summary>
        public override bool HasDetails
        {
            get
            {
                return false;
            }
        }

        private void AllOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ftpServers != null && e.PropertyName == "IsLoading")
            {
                this.IsLoading = this.ftpServers.IsLoading;
            }
        }
    }
}
