// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerCompareEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The integer compare evaluation data view model.
    /// </summary>
    public partial class IntegerCompareEvalDataViewModel
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

            var beginValue = string.Empty;
            if (this.Begin != null)
            {
                beginValue = this.Begin.Value.ToString(CultureInfo.InvariantCulture);
            }

            var endValue = string.Empty;
            if (this.End != null)
            {
                endValue = this.End.Value.ToString(CultureInfo.InvariantCulture);
            }

            return string.Format(
                "IntegerCompare ( {0}; {1}; {2} )", evaluationValue, beginValue, endValue);
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
