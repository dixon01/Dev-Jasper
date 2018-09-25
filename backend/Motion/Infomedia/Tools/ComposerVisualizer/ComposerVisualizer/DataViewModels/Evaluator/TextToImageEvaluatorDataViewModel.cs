// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToImageEvaluatorDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval;

    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    /// <summary>
    /// Text to image evaluator data view model
    /// </summary>
    [DisplayName(@"Text To Image Evaluator")]
    public class TextToImageEvaluatorDataViewModel : ContainerEvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the text to image evaluation.
        /// </summary>
        [ExpandableObject]
        [ReadOnly(true)]
        public TextToImageEvalDataViewModel TextToImageEval { get; set; }
    }
}
