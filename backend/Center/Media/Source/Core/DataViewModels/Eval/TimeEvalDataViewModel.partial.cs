// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The time evaluation data view model.
    /// </summary>
    public partial class TimeEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var beginValue = string.Empty;
            if (this.Begin != null)
            {
                beginValue = string.Format(CultureInfo.InvariantCulture, "{0:hh\\:mm\\:ss}", this.Begin.Value);
            }

            var endValue = string.Empty;
            if (this.End != null)
            {
                endValue = string.Format(CultureInfo.InvariantCulture, "{0:hh\\:mm\\:ss}", this.End.Value);
            }

            var formulatext = string.Format(CultureInfo.InvariantCulture, "Time ( '{0}'; '{1}' )", beginValue, endValue);
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
    }
}
