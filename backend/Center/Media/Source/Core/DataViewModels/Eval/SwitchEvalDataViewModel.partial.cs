// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The switch evaluation data view model.
    /// </summary>
    public partial class SwitchEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var valueString = string.Empty;
            if (this.Value != null)
            {
                valueString = this.Value.HumanReadable();
            }

            var casesString = new StringBuilder();
            if (this.Cases != null)
            {
                foreach (var caseProperty in this.Cases)
                {
                    casesString.AppendFormat("{0}; ", caseProperty.HumanReadable());
                }
            }

            var defaultString = string.Empty;
            if (this.Default != null)
            {
                defaultString = this.Default.HumanReadable();
            }

            return string.Format(
                "Switch ( {0}; {1}{2} )", valueString, casesString, defaultString);
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

            if (this.Value != null)
            {
                result.AddRange(this.Value.GetContainedPredefinedFormulas());
            }

            foreach (var switchCase in this.cases)
            {
                if (switchCase.Evaluation != null)
                {
                    result.AddRange(switchCase.Evaluation.GetContainedPredefinedFormulas());
                }
            }

            if (this.Default != null)
            {
                result.AddRange(this.Default.GetContainedPredefinedFormulas());
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

            if (this.Value != null)
            {
                if (this.Value is EvaluationEvalDataViewModel && this.Value.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Value).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Value.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            for (var i = 0; i < this.cases.Count; i++)
            {
                if (this.cases[i].Evaluation != null)
                {
                    if (this.cases[i].Evaluation is EvaluationEvalDataViewModel
                        && this.cases[i].Evaluation.ClonedFrom != 0)
                    {
                        var predefinedFormula = ((EvaluationEvalDataViewModel)this.cases[i].Evaluation).Reference;
                        if (predefinedFormula != null)
                        {
                            predefinedFormula.ReferencesCount--;
                            result.Add(predefinedFormula);
                        }
                    }
                    else
                    {
                        this.cases[i].Evaluation.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                    }
                }
            }

            if (this.Default != null)
            {
                if (this.Default is EvaluationEvalDataViewModel && this.Default.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.Default).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.Default.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }

        /// <summary>
        /// The clear case if value or case is empty or null
        /// </summary>
        public void ClearEmptyCases()
        {
            for (var i = this.Cases.Count - 1; 0 <= i; i--)
            {
                if (this.Cases[i].Value == null
                    || this.Cases[i].Value.Value.Equals(string.Empty)
                    || this.Cases[i].Evaluation == null
                    || this.Cases[i].Evaluation.HumanReadable().Equals(string.Empty))
                {
                    this.Cases.RemoveAt(i);
                }
            }
        }

        partial void ExportNotGeneratedValues(SwitchEval model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            var defaultEval = model.Default == null ? null : model.Default.Evaluation as CodeConversionEval;
            if (defaultEval != null)
            {
                model.Default.Evaluation = this.ConvertCodeConversionToCsvMapping(defaultEval);
            }

            var valueEval = model.Value == null ? null : model.Value.Evaluation as CodeConversionEval;
            if (valueEval != null)
            {
                model.Value.Evaluation = this.ConvertCodeConversionToCsvMapping(valueEval);
            }
        }
    }
}
