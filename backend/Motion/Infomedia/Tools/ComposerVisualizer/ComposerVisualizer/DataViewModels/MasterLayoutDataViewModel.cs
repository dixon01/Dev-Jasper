// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterLayoutDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterLayoutDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// The master layout data view model.
    /// </summary>
    [DisplayName(@"Master Layout Properties")]
    public class MasterLayoutDataViewModel : LayoutDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterLayoutDataViewModel"/> class.
        /// </summary>
        public MasterLayoutDataViewModel()
        {
            this.VirtualDisplays = new ObservableCollection<VirtualDisplayDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the virtual displays in the master layout.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<VirtualDisplayDataViewModel> VirtualDisplays { get; set; }
    }
}