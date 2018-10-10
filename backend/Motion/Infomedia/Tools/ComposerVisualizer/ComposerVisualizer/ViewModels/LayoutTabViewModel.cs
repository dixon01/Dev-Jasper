// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutTabViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutTabViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels
{
    using System.Collections.ObjectModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The layout tab view model.
    /// </summary>
    public class LayoutTabViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutTabViewModel"/> class.
        /// </summary>
        public LayoutTabViewModel()
        {
            this.Layouts = new ObservableCollection<LayoutViewModel>();
        }

        /// <summary>
        /// Gets or sets the layouts.
        /// </summary>
        public ObservableCollection<LayoutViewModel> Layouts { get; set; }
    }
}
