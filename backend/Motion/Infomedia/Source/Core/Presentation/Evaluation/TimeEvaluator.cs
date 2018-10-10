// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    /// <summary>
    /// Evaluator for <see cref="TimeEval"/>.
    /// </summary>
    public partial class TimeEvaluator
    {
        /// <summary>
        /// Checks the given time and if necessary updates the <see cref="EvaluatorBase.Value"/>.
        /// </summary>
        /// <param name="now">
        /// The current time.
        /// </param>
        protected override void CheckTime(DateTime now)
        {
            var today = now.ToLocalTime().Date;
            var time = now.ToLocalTime().TimeOfDay;
            if (time < this.TimeEval.Begin)
            {
                this.Value = false;
                this.SetNextCheckTime(today + this.TimeEval.Begin);
            }
            else if (time <= this.TimeEval.End)
            {
                this.Value = true;
                this.SetNextCheckTime(today.AddSeconds(1) + this.TimeEval.End);
            }
            else
            {
                this.Value = false;
                this.SetNextCheckTime(today.AddDays(1) + this.TimeEval.Begin);
            }
        }
    }
}