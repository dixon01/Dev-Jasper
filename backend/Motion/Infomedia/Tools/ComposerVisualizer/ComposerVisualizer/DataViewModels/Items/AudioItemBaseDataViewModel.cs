// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioItemBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the audio item base
    /// </summary>
    public class AudioItemBaseDataViewModel : ScreenItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioItemBaseDataViewModel"/> class.
        /// </summary>
        public AudioItemBaseDataViewModel()
        {
            this.Enabled = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio item is enabled.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Enabled*")]
        public EvaluatorBaseDataViewModel Enabled { get; set; }
    }
}
