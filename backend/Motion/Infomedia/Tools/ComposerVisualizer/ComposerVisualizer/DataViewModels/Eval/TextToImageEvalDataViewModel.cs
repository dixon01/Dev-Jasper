// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToImageEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Text to image evaluation data view model
    /// </summary>
    public class TextToImageEvalDataViewModel : ContainerEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the file patterns.
        /// </summary>
        [ReadOnly(true)]
        public string FilePatterns { get; set; }
    }
}
