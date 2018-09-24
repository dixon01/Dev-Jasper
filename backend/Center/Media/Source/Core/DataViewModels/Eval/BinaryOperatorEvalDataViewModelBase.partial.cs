// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOperatorEvalDataViewModelBase.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The BinaryOperatorEvalDataViewModelBase.partial.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The Binary Operator Evaluation DataViewModel Base.
    /// </summary>
    public partial class BinaryOperatorEvalDataViewModelBase
    {
        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas()
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.left != null)
            {
                result.AddRange(this.left.GetContainedPredefinedFormulas());
            }

            if (this.right != null)
            {
                result.AddRange(this.right.GetContainedPredefinedFormulas());
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
        /// The list of predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            var result = new List<EvaluationConfigDataViewModel>();

            if (this.left != null)
            {
                if (this.left is EvaluationEvalDataViewModel && this.left.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.left).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.left.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            if (this.right != null)
            {
                if (this.right is EvaluationEvalDataViewModel && this.right.ClonedFrom != 0)
                {
                    var predefinedFormula = ((EvaluationEvalDataViewModel)this.right).Reference;
                    if (predefinedFormula != null)
                    {
                        predefinedFormula.ReferencesCount--;
                        result.Add(predefinedFormula);
                    }
                }
                else
                {
                    this.right.ReplaceContainedPredefinedFormulasWithOriginals(predefinedFormulas);
                }
            }

            return result;
        }

        partial void ExportNotGeneratedValues(BinaryOperatorEvalBase model, object exportParameters)
        {
            if (!CsvMappingCompatibilityRequired(exportParameters))
            {
                return;
            }

            var leftEval = model.Left == null ? null : model.Left.Evaluation as CodeConversionEval;
            if (leftEval != null)
            {
                model.Left.Evaluation = this.ConvertCodeConversionToCsvMapping(leftEval);
            }

            var rightEval = model.Right == null ? null : model.Right.Evaluation as CodeConversionEval;
            if (rightEval != null)
            {
                model.Right.Evaluation = this.ConvertCodeConversionToCsvMapping(rightEval);
            }
        }
    }
}