// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    /// <summary>
    /// Evaluator for <see cref="DateEval"/>.
    /// </summary>
    public partial class DateEvaluator
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
            if (today < this.DateEval.Begin)
            {
                this.Value = false;
                this.SetNextCheckTime(this.DateEval.Begin);
            }
            else if (today <= this.DateEval.End)
            {
                this.Value = true;
                this.SetNextCheckTime(this.DateEval.End.AddMilliseconds(1));
            }
            else
            {
                this.Value = false;

                // don't set the next time check since this won't become valid again later
            }
        }
    }
}