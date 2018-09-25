// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvalBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Eval
{
    using System.ComponentModel;

    /// <summary>
    /// Evaluation base data view model
    /// </summary>
    public class EvalBaseDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvalBaseDataViewModel"/> class.
        /// </summary>
        public EvalBaseDataViewModel()
        {
            this.EvalValue = string.Empty;
        }

        /// <summary>
        /// Gets or sets the value of the evaluator.
        /// </summary>
        [Browsable(false)]
        public object EvalValue { get; set; }

        /// <summary>
        /// Converts the value to a string
        /// </summary>
        /// <returns>
        /// The value<see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.EvalValue.ToString();
        }
    }
}
