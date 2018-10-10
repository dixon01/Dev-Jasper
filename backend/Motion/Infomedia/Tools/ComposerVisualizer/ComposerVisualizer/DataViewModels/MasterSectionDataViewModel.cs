// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterSectionDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterSectionDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// The master section data view model.
    /// </summary>
    [DisplayName(@"Master Section Properties")]
    public class MasterSectionDataViewModel : SectionDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSectionDataViewModel"/> class.
        /// </summary>
        public MasterSectionDataViewModel()
        {
            this.MasterLayouts = new ObservableCollection<MasterLayoutDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the master layouts in the master section.
        /// </summary>
        [Browsable(false)]
        public ObservableCollection<MasterLayoutDataViewModel> MasterLayouts { get; set; }
    }
}