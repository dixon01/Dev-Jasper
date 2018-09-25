// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringCompareEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// String compare evaluation data view model
    /// </summary>
    public class StringCompareEvalDataViewModel : ContainerEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether ignore case.
        /// </summary>
        [ReadOnly(true)]
        public bool IgnoreCase { get; set; }
    }
}
