// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToImageEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The text to image evaluation data view model.
    /// </summary>
    public partial class TextToImageEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var evaluationString = string.Empty;
            if (this.Evaluation != null)
            {
                evaluationString = this.Evaluation.HumanReadable();
            }

            var filePatternsString = string.Empty;
            if (this.FilePatterns != null)
            {
                filePatternsString = this.FilePatterns.Value;
            }

            return string.Format(
                "TextToImage ( {0}; '{1}' )", evaluationString, filePatternsString);
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
