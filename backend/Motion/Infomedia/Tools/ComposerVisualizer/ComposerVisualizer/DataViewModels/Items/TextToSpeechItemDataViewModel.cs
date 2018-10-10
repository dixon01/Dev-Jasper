// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechItemDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Data view model of the text to speech item
    /// </summary>
    [DisplayName(@"Text to Speech Properties")]
    public class TextToSpeechItemDataViewModel : PlaybackItemBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextToSpeechItemDataViewModel"/> class.
        /// </summary>
        public TextToSpeechItemDataViewModel()
        {
            this.Voice = new EvaluatorBaseDataViewModel();
            this.Value = new EvaluatorBaseDataViewModel();
        }

        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Voice*")]
        public EvaluatorBaseDataViewModel Voice { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Category("Content")]
        [ReadOnly(true)]
        [ExpandableObject]
        [DisplayName(@"Value*")]
        public EvaluatorBaseDataViewModel Value { get; set; }
    }
}
