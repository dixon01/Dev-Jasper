// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationsDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationsDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections.ObjectModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The presentations data view model.
    /// </summary>
    public class PresentationsDataViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationsDataViewModel"/> class.
        /// </summary>
        public PresentationsDataViewModel()
        {
            this.PhysicalScreens = new ObservableCollection<PhysicalScreenDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the master presentations.
        /// </summary>
        public ObservableCollection<PhysicalScreenDataViewModel> PhysicalScreens { get; set; }
    }
}