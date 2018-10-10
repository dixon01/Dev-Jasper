// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DayOfWeekEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DayOfWeekEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    /// <summary>
    /// Evaluator for <see cref="DayOfWeekEval"/>.
    /// </summary>
    public partial class DayOfWeekEvaluator
    {
        /// <summary>
        /// Checks the given time and if necessary updates the <see cref="EvaluatorBase.Value"/>.
        /// </summary>
        /// <param name="now">
        /// The current time.
        /// </param>
        protected override void CheckTime(DateTime now)
        {
            now = now.ToLocalTime();
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    this.Value = this.DayOfWeekEval.Monday;
                    break;
                case DayOfWeek.Tuesday:
                    this.Value = this.DayOfWeekEval.Tuesday;
                    break;
                case DayOfWeek.Wednesday:
                    this.Value = this.DayOfWeekEval.Wednesday;
                    break;
                case DayOfWeek.Thursday:
                    this.Value = this.DayOfWeekEval.Thursday;
                    break;
                case DayOfWeek.Friday:
                    this.Value = this.DayOfWeekEval.Friday;
                    break;
                case DayOfWeek.Saturday:
                    this.Value = this.DayOfWeekEval.Saturday;
                    break;
                case DayOfWeek.Sunday:
                    this.Value = this.DayOfWeekEval.Sunday;
                    break;
            }

            // check again at midnight local time
            this.SetNextCheckTime(now.AddDays(1).Date);
        }
    }
}