// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexReplaceEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Regex replace evaluation data view model
    /// </summary>
    public class RegexReplaceEvalDataViewModel : ContainerEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        [ReadOnly(true)]
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the replacement.
        /// </summary>
        [ReadOnly(true)]
        public string Replacement { get; set; }
    }
}
