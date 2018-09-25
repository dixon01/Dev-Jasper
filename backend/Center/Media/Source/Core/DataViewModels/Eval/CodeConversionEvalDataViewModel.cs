// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeConversionEvalDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CodeConversionEvalDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// The code conversion evaluation data view model.
    /// </summary>
    public partial class CodeConversionEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var useImage = false;
            if (this.UseImage != null)
            {
                useImage = this.UseImage.Value;
            }

            var result = string.Format(
                "CodeConversion ('{0}')",
                useImage);
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
            return new List<EvaluationConfigDataViewModel>();
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
            return new List<EvaluationConfigDataViewModel>();
        }

        partial void ExportNotGeneratedValues(CodeConversionEval model, object exportParameters)
        {
            if (model.FileName != null)
            {
                model.FileName = "codeconversion.csv";
            }
        }
    }
}
