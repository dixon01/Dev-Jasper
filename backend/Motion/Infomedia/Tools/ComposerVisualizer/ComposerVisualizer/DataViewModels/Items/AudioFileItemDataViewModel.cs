// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the audio file item
    /// </summary>
    [DisplayName(@"Audio File Properties")]
    public class AudioFileItemDataViewModel : PlaybackItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileItemDataViewModel"/> class.
        /// </summary>
        public AudioFileItemDataViewModel()
        {
            this.Filename = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the filename of the audio file.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Filename*")]
        public EvaluatorBaseDataViewModel Filename { get; set; }
    }
}
