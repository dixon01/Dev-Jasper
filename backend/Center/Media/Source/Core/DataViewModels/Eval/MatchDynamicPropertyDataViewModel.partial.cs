// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchDynamicPropertyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The match dynamic property data view model.
    /// </summary>
    public partial class MatchDynamicPropertyDataViewModel
    {
        /// <summary>
        /// Gets or sets a value that decreases and increases reference counts of predefined formulas
        /// </summary>
        public EvalDataViewModelBase EvaluationWithReferenceCounting
        {
            get
            {
                return this.evaluation;
            }

            set
            {
                // decrement references count for previous predefined formulas
                var previousValue = this.evaluation;
                if (previousValue != null)
                {
                    var predefinedFormulas = previousValue.GetContainedPredefinedFormulas();
                    foreach (var predefinedFormula in predefinedFormulas)
                    {
                        predefinedFormula.ReferencesCount--;
                    }
                }

                this.SetProperty(ref this.evaluation, value, () => this.Evaluation);

                // increment references count for new predefined formulas
                if (this.evaluation != null)
                {
                    var predefinedFormulas = this.evaluation.GetContainedPredefinedFormulas();
                    foreach (var predefinedFormula in predefinedFormulas)
                    {
                        predefinedFormula.ReferencesCount++;
                    }
                }

                this.RaisePropertyChanged(() => this.EvaluationWithReferenceCounting);
            }
        }

        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public string HumanReadable()
        {
            var evaluationValue = string.Empty;
            if (this.Evaluation != null)
            {
                evaluationValue = this.Evaluation.HumanReadable();
            }

            var columnValue = string.Empty;
            if (this.Column != null)
            {
                columnValue = this.Column.Value.ToString();
            }

            return string.Format("{0}:{1}", columnValue, evaluationValue);
        }

        /// <summary>
        /// Checks if a compatibility conversion for CSV mapping is required
        /// </summary>
        /// <param name="exportParameters">
        /// The export parameters.
        /// </param>
        /// <returns>
        /// True if is required.
        /// </returns>
        private static bool CsvMappingCompatibilityRequired(object exportParameters)
        {
            var parameters = exportParameters as ExportCompatibilityParameters;
            return parameters != null && parameters.CsvMappingCompatibilityRequired;
        }

        partial void ExportNotGeneratedValues(MatchDynamicProperty model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            var eval = model.Evaluation as CodeConversionEval;
            if (eval != null)
            {
                model.Evaluation = this.ConvertCodeConversionToCsvMapping(eval);
            }
        }

        private CsvMappingEval ConvertCodeConversionToCsvMapping(CodeConversionEval codeConversion)
        {
            var csvMapping = new CsvMappingEval
            {
                FileName = codeConversion.FileName,
                DefaultValue =
                    new DynamicProperty
                    {
                        Evaluation =
                            new GenericEval
                            {
                                Column = 0,
                                Language = 0,
                                Table = 10,
                                Row = 0
                            }
                    }
            };
            var match0 = new MatchDynamicProperty
            {
                Column = 0,
                Evaluation = new GenericEval
                {
                    Column = 1,
                    Language = 0,
                    Table = 10,
                    Row = 0
                }
            };
            var match1 = new MatchDynamicProperty
            {
                Column = 1,
                Evaluation =
                    new GenericEval
                    {
                        Column = 0,
                        Language = 0,
                        Table = 10,
                        Row = 0
                    }
            };

            csvMapping.Matches.Add(match0);
            csvMapping.Matches.Add(match1);
            csvMapping.OutputFormat = codeConversion.UseImage ? "{2}" : "{3}";
            return csvMapping;
        }
    }
}
