// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Evaluation evaluation data view model
    /// </summary>
    public class EvaluationEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        [ReadOnly(true)]
        public string Reference { get; set; }
    }
}
