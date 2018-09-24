// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfEvalDataViewModel.partial.cs" company="Gorba AG">
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
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The if evaluation data view model.
    /// </summary>
    public partial class IfEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var conditionString = this.condition == null ? string.Empty : this.condition.HumanReadable();
            var thenString = this.then == null ? string.Empty : this.then.HumanReadable();
            var formulatext = "If ( " + conditionString + "; " + thenString;
            formulatext += this.Else == null ? " )" : "; " + this.Else.HumanReadable() + " )";

            return formulatext;
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

            if (this.Condition != null)
            {
                result.AddRange(this.Condition.GetContainedPredefinedFormulas());
            }

            if (this.Then != null)
            {
                result.AddRange(this.Then.GetContainedPredefinedFormulas());
            }

            if (this.Else != null)
            {
                result.AddRange(this.Else.GetContainedPredefinedFormulas());
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

            if (this.Condition != null)
            {
                if (this.Condition is EvaluationEvalDataViewModel && this.Condition.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Condition).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Condition.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            if (this.Then != null)
            {
                if (this.Then is EvaluationEvalDataViewModel && this.Then.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Then).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Then.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            if (this.Else != null)
            {
                if (this.Else is EvaluationEvalDataViewModel && this.Else.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Else).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Else.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }

        partial void ExportNotGeneratedValues(IfEval model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            var conditionEval = model.Condition == null ? null : model.Condition.Evaluation as CodeConversionEval;
            if (conditionEval != null)
            {
                model.Condition.Evaluation = this.ConvertCodeConversionToCsvMapping(conditionEval);
            }

            var thenEval = model.Then == null ? null : model.Then.Evaluation as CodeConversionEval;
            if (thenEval != null)
            {
                model.Then.Evaluation = this.ConvertCodeConversionToCsvMapping(thenEval);
            }

            var elseEval = model.Else == null ? null : model.Else.Evaluation as CodeConversionEval;
            if (elseEval != null)
            {
                model.Else.Evaluation = this.ConvertCodeConversionToCsvMapping(elseEval);
            }
        }
    }
}
