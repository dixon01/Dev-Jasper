// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.App;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.Gioom;

    /// <summary>
    /// Base for all view models representing a unit.
    /// </summary>
    public abstract class UnitViewModelBase : ViewModelBase
    {
        private string name;

        private bool isFavorite;

        private ConnectionState connectionState;

        private bool fileSystemIsDownloading;

        private string description;

        private ConnectionMode connectionMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitViewModelBase"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        protected UnitViewModelBase(IDiagShell shell)
        {
            this.Shell = shell;
            this.Applications = new ObservableCollection<RemoteAppViewModel>();
            this.FileSystemRoots = new ObservableCollection<FileSystemItemViewModelBase>();
            this.GioomPorts = new GioomPortsViewModel(this);
        }

        /// <summary>
        /// Gets or sets the name of the unit.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.SetProperty(ref this.name, value, () => this.Name))
                {
                    this.RaisePropertyChanged(() => this.DisplayName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the manual description of the unit.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets the name to display when showing this unit.
        /// Depending on the type of unit, this can differ from <see cref="Name"/>.
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                return this.Name;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this unit is a favorite.
        /// </summary>
        public bool IsFavorite
        {
            get
            {
                return this.isFavorite;
            }

            set
            {
                this.SetProperty(ref this.isFavorite, value, () => this.IsFavorite);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the file system is busy.
        /// </summary>
        public bool FileSystemIsDownloading
        {
            get
            {
                return this.fileSystemIsDownloading;
            }

            set
            {
                this.SetProperty(ref this.fileSystemIsDownloading, value, () => this.FileSystemIsDownloading);
            }
        }

        /// <summary>
        /// Gets or sets the connection state for this unit.
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                return this.connectionState;
            }

            set
            {
                this.SetProperty(ref this.connectionState, value, () => this.ConnectionState);
            }
        }

        /// <summary>
        /// Gets or sets the way Diag can connect to this unit.
        /// </summary>
        public ConnectionMode ConnectionMode
        {
            get
            {
                return this.connectionMode;
            }

            set
            {
                this.SetProperty(ref this.connectionMode, value, () => this.ConnectionMode);
            }
        }

        /// <summary>
        /// Gets the applications of this unit.
        /// </summary>
        public ObservableCollection<RemoteAppViewModel> Applications { get; private set; }

        /// <summary>
        /// Gets the shell
        /// </summary>
        public IDiagShell Shell { get; private set; }

        /// <summary>
        /// Gets the folders
        /// </summary>
        public ObservableCollection<FileSystemItemViewModelBase> FileSystemRoots { get; private set; }

        /// <summary>
        /// Gets the GIOoM ports of this unit.
        /// </summary>
        public GioomPortsViewModel GioomPorts { get; private set; }

        /// <summary>
        /// Gets the current group member.
        /// </summary>
        public string GroupMember
        {
            get
            {
                if (this.IsFavorite)
                {
                    return DiagStrings.AllUnits_Favorites;
                }

                return (this.ConnectionState == ConnectionState.Disconnected)
                                          ? DiagStrings.AllUnits_Unconnected
                                          : DiagStrings.AllUnits_Connected;
            }
        }
    }
}
