// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeEvaluatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeEvaluatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    /// <summary>
    /// Base class for all evaluators that check the current time.
    /// </summary>
    public abstract partial class DateTimeEvaluatorBase
    {
        private DateTime nextCheckTime;

        /// <summary>
        /// Checks the given time and if necessary updates the <see cref="EvaluatorBase.Value"/>.
        /// This method should always call <see cref="SetNextCheckTime"/>
        /// to re-enable the update timer for the next time this evaluator's value is changing.
        /// </summary>
        /// <param name="now">
        /// The current time.
        /// </param>
        protected abstract void CheckTime(DateTime now);

        /// <summary>
        /// Set the time at which <see cref="CheckTime"/> should be called the next time.
        /// </summary>
        /// <param name="dateTime">
        /// The date and time at which <see cref="CheckTime"/> will be called the next time.
        /// </param>
        protected void SetNextCheckTime(DateTime dateTime)
        {
            this.nextCheckTime = dateTime;
            this.Context.Time.AddTimeReachedHandler(this.nextCheckTime, this.CheckTime);
        }

        partial void UpdateValue()
        {
            this.CheckTime(this.Context.Time.UtcNow);
        }

        partial void Deinitialize()
        {
            this.Context.Time.RemoveTimeReachedHandler(this.nextCheckTime, this.CheckTime);
        }
    }
}