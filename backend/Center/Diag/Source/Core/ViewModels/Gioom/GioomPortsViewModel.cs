// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomPortsViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomPortsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// The view model representing the list of GIOoM ports.
    /// </summary>
    public sealed class GioomPortsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GioomPortsViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit to which the ports belong.
        /// </param>
        public GioomPortsViewModel(UnitViewModelBase unit)
        {
            this.Unit = unit;
            this.Ports = new ObservableCollection<GioomPortViewModelBase>();
        }

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
        /// Gets the unit to which the ports belong.
        /// </summary>
        public UnitViewModelBase Unit { get; private set; }

        /// <summary>
        /// Gets the list of ports.
        /// </summary>
        public ObservableCollection<GioomPortViewModelBase> Ports { get; private set; }
    }
}