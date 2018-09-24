// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Constant evaluation data view model.
    /// </summary>
    public class ConstantEvalDataViewModel : EvalBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [ReadOnly(true)]
        public string Value { get; set; }
    }
}
