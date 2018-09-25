// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerEvalDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ContainerEvalDataViewModelBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The Container Evaluation Data View Model Base.
    /// </summary>
    public partial class ContainerEvalDataViewModelBase
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

        partial void ExportNotGeneratedValues(ContainerEvalBase model, object exportParameters)
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
    }
}