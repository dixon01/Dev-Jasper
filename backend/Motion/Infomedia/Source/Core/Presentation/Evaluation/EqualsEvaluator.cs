// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualsEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EqualsEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    /// <summary>
    /// The equals "==" evaluator.
    /// </summary>
    public partial class EqualsEvaluator
    {
        /// <summary>
        /// Calculates the boolean value of this operator.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> value.
        /// </returns>
        protected override bool CalculateValue()
        {
            return object.Equals(this.Left.StringValue, this.Right.StringValue);
        }
    }
}