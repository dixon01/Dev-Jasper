// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluatorBaseDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator
{
    using System.ComponentModel;

    /// <summary>
    /// Evaluator base data view model
    /// </summary>
    public class EvaluatorBaseDataViewModel
    {
        /// <summary>
        /// Gets or sets the value of the evaluator.
        /// </summary>
        [Browsable(false)]
        public object Value { get; set; }

        /// <summary>
        /// Converts the value to a string
        /// </summary>
        /// <returns>
        /// The value<see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
