// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerCompareEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Integer compare evaluation data view model
    /// </summary>
    public class IntegerCompareEvalDataViewModel : ContainerEvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the begin.
        /// </summary>
        [ReadOnly(true)]
        public int Begin { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        [ReadOnly(true)]
        public int End { get; set; }
    }
}
