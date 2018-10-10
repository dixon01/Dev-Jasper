// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteAppViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteAppViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.Gioom;
    using Gorba.Center.Diag.Core.ViewModels.Log;
    using Gorba.Center.Diag.Core.ViewModels.MediTree;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Medi.Core;

    /// <summary>
    /// View model for a remote application.
    /// </summary>
    public class RemoteAppViewModel : ViewModelBase
    {
        private string name;

        private MediAddress address;

        private ApplicationType applicationType;

        private RemoteAppStateViewModel state;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAppViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public RemoteAppViewModel(UnitViewModelBase unit)
        {
            this.Unit = unit;
            this.ApplicationType = ApplicationType.Unknown;
            this.GioomPorts = new GioomPortsViewModel(this.Unit);
            this.MediTreeRoots = new ObservableCollection<MediTreeNodeViewModel>();
            this.Logging = new LoggingViewModel();
        }

        /// <summary>
        /// Gets the unit to which this application belongs.
        /// </summary>
        public UnitViewModelBase Unit { get; private set; }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IDiagShell Shell
        {
            get
            {
                return this.Unit.Shell;
            }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the Medi address of the application.
        /// </summary>
        public MediAddress Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.SetProperty(ref this.address, value, () => this.Address);
            }
        }

        /// <summary>
        /// Gets or sets the type of application this is (this is used for showing application specific icons).
        /// </summary>
        public ApplicationType ApplicationType
        {
            get
            {
                return this.applicationType;
            }

            set
            {
                this.SetProperty(ref this.applicationType, value, () => this.ApplicationType);
            }
        }

        /// <summary>
        /// Gets or sets the state information of this application.
        /// </summary>
        public RemoteAppStateViewModel State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.SetProperty(ref this.state, value, () => this.State);
            }
        }

        /// <summary>
        /// Gets the GIOoM ports of this application.
        /// </summary>
        public GioomPortsViewModel GioomPorts { get; private set; }

        /// <summary>
        /// Gets the root nodes of the Medi tree.
        /// </summary>
        public ObservableCollection<MediTreeNodeViewModel> MediTreeRoots { get; private set; }

        /// <summary>
        /// Gets the logging view model.
        /// </summary>
        public LoggingViewModel Logging { get; private set; }
    }
}
