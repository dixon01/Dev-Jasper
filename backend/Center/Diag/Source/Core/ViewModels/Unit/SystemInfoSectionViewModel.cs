// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemInfoSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemInfoSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// The view model for the section that shows system information.
    /// </summary>
    public class SystemInfoSectionViewModel : InfoSectionViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemInfoSectionViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public SystemInfoSectionViewModel(UnitViewModelBase unit)
            : base(unit)
        {
            this.CpuUsage = new GaugeViewModel { Label = "CPU" };
            this.RamUsage = new GaugeViewModel { Label = "RAM" };
            this.DiskUsages = new ObservableCollection<GaugeViewModel>();
        }

        /// <summary>
        /// Gets the CPU usage.
        /// </summary>
        public GaugeViewModel CpuUsage { get; private set; }

        /// <summary>
        /// Gets the RAM usage.
        /// </summary>
        public GaugeViewModel RamUsage { get; private set; }

        /// <summary>
        /// Gets the disk usages, one for each disk available on the Unit.
        /// </summary>
        public ObservableCollection<GaugeViewModel> DiskUsages { get; private set; }
    }
}