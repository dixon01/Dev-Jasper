// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayOfWeekEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The day of week evaluation data view model.
    /// </summary>
    public partial class DayOfWeekEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var result = new StringBuilder("DayOfWeek ( '");
            var hasPreviousDay = false;

            if (this.Monday != null && this.Monday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Mon");
            }

            if (this.Tuesday != null && this.Tuesday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Tue");
            }

            if (this.Wednesday != null && this.Wednesday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Wed");
            }

            if (this.Thursday != null && this.Thursday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Thu");
            }

            if (this.Friday != null && this.Friday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Fri");
            }

            if (this.Saturday != null && this.Saturday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Sat");
            }

            if (this.Sunday != null && this.Sunday.Value)
            {
                SetDayString(ref hasPreviousDay, result, "Sun");
            }

            result.Append("' )");
            return result.ToString();
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
        /// Resets all day values to false;
        /// </summary>
        public void ClearValues()
        {
            this.Monday.Value = false;
            this.Tuesday.Value = false;
            this.Wednesday.Value = false;
            this.Thursday.Value = false;
            this.Friday.Value = false;
            this.Saturday.Value = false;
            this.Sunday.Value = false;
        }

        private static void SetDayString(ref bool hasPreviousDay, StringBuilder result, string day)
        {
            if (hasPreviousDay)
            {
                result.Append(", ");
            }

            result.Append(day);
            hasPreviousDay = true;
        }
    }
}
