// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The not evaluation data view model.
    /// </summary>
    public partial class NotEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var evaluationValue = string.Empty;
            if (this.Evaluation != null)
            {
                evaluationValue = this.Evaluation.HumanReadable();
            }

            return "Not ( " + evaluationValue + " )";
        }

        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas()
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.Evaluation != null)
            {
                result.AddRange(this.Evaluation.GetContainedPredefinedFormulas());
            }

            return result;
        }

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.Evaluation != null)
            {
                if (this.Evaluation is EvaluationEvalDataViewModel && this.Evaluation.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Evaluation).Reference;

                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Evaluation.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }
    }
}
