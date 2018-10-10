// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models.Eval;

    /// <summary>
    /// The generic evaluation data view model.
    /// </summary>
    public partial class GenericEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            return GenericExtensions.ConvertToHumanReadable(
                this.mediaShell,
                this.Table,
                this.Column,
                this.Row,
                this.Language);
        }

        /// <summary>
        /// Searches for all contained predefined formulas.
        /// </summary>
        /// <returns>
        /// The contained predefined formulas.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> GetContainedPredefinedFormulas()
        {
            return Enumerable.Empty<EvaluationConfigDataViewModel>();
        }

        /// <summary>
        /// The set contained predefined formulas.
        /// </summary>
        /// <param name="predefinedFormulas">
        /// The predefined formulas.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public override IEnumerable<EvaluationConfigDataViewModel> ReplaceContainedPredefinedFormulasWithOriginals(
            ExtendedObservableCollection<EvaluationConfigDataViewModel> predefinedFormulas)
        {
            return new List<EvaluationConfigDataViewModel>();
        }

        /// <summary>
        /// returns human readable representation
        /// </summary>
        /// <returns>human readable representation</returns>
        public override string ToString()
        {
            return this.HumanReadable();
        }

        partial void Initialize(GenericEvalDataModel dataModel)
        {
            this.DisplayText = this.ToString();
        }

        partial void Initialize(GenericEvalDataViewModel dataViewModel)
        {
            this.DisplayText = this.ToString();
        }
    }
}
