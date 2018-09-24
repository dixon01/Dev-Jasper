// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// Config base data view model
    /// </summary>
    public class ConfigBaseDataViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigBaseDataViewModel"/> class.
        /// </summary>
        public ConfigBaseDataViewModel()
        {
            this.ImagePath = string.Empty;
        }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        [Browsable(false)]
        public string ImagePath { get; set; }
    }
}
