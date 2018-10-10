// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Common.Configuration.Infomedia.Eval;

    using NLog;

    /// <summary>
    /// The csv mapping evaluation data view model.
    /// </summary>
    public partial class CsvMappingEvalDataViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var fileName = string.Empty;
            if (this.FileName != null)
            {
                fileName = this.FileName.Value;
            }

            var outputFormat = string.Empty;
            if (this.OutputFormat != null)
            {
                outputFormat = this.OutputFormat.Value;
            }

            var defaultValue = string.Empty;
            if (this.DefaultValue != null)
            {
                defaultValue = this.DefaultValue.HumanReadable();
            }

            var cases = string.Empty;
            if (this.matches != null)
            {
                cases = string.Join("; ", this.Matches.Select(o => o.HumanReadable()));
            }

            var result = string.Format(
                "CsvMapping ( '{0}'; '{1}'; {2}; {3} )",
                fileName,
                outputFormat,
                defaultValue,
                cases);
            return result;
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

            foreach (var match in this.Matches)
            {
                if (match.Evaluation != null)
                {
                    result.AddRange(match.Evaluation.GetContainedPredefinedFormulas());
                }
            }

            if (this.DefaultValue != null)
            {
                result.AddRange(this.DefaultValue.GetContainedPredefinedFormulas());
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

            foreach (var match in this.Matches)
            {
                if (match != null)
                {
                    if (match.Evaluation is EvaluationEvalDataViewModel && match.Evaluation.ClonedFrom != 0)
                    {
                        var predefinedFormula = ((EvaluationEvalDataViewModel)match.Evaluation).Reference;
                        if (predefinedFormula != null)
                        {
                            predefinedFormula.ReferencesCount--;
                            result.Add(predefinedFormula);
                        }
                    }
                    else
                    {
                        if (match.Evaluation != null)
                        {
                            match.Evaluation.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                        }
                    }
                }
            }

            if (this.DefaultValue != null)
            {
                if (this.DefaultValue is EvaluationEvalDataViewModel && this.DefaultValue.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.DefaultValue).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.DefaultValue.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }

        partial void ExportNotGeneratedValues(CsvMappingEval model, object exportParameters)
        {
            if (string.IsNullOrWhiteSpace(model.FileName))
            {
                Logger.Debug("No filename set.");
                return;
            }

            // written file will get an extension
            model.FileName += Settings.Default.CsvMapping_DefaultExtension;

            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            var defaultEval = model.DefaultValue.Evaluation as CodeConversionEval;
            if (defaultEval != null)
            {
                model.DefaultValue.Evaluation = this.ConvertCodeConversionToCsvMapping(defaultEval);
            }
        }
    }
}
