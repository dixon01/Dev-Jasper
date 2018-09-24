// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreaterThanEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GreaterThanEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The greater than ">" evaluator.
    /// </summary>
    public partial class GreaterThanEvaluator
    {
        private readonly StringComparer comparer = new LogicalStringComparer();

        /// <summary>
        /// Calculates the boolean value of this operator.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> value.
        /// </returns>
        protected override bool CalculateValue()
        {
            return this.comparer.Compare(this.Left.StringValue, this.Right.StringValue) > 0;
        }
    }
}