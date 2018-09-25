// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Criteria evaluation for <see cref="NotEval"/>.
    /// This inverts the value of the <see cref="NotEval.Evaluation"/>.
    /// </summary>
    public partial class NotEvaluator
    {
        /// <summary>
        /// Evaluation method. Subclasses must implement this method so
        /// it returns the evaluated value.
        /// </summary>
        /// <returns>
        /// the evaluated value.
        /// </returns>
        protected override object CalculateValue()
        {
            return !this.InnerEvaluator.BoolValue;
        }
    }
}