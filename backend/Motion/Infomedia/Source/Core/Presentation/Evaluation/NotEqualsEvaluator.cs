// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotEqualsEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NotEqualsEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    /// <summary>
    /// The not equals "!=" (or &lt;&gt;) evaluator.
    /// </summary>
    public partial class NotEqualsEvaluator
    {
        /// <summary>
        /// Calculates the boolean value of this operator.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> value.
        /// </returns>
        protected override bool CalculateValue()
        {
            return !object.Equals(this.Left.StringValue, this.Right.StringValue);
        }
    }
}