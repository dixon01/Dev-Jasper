// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LessThanEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LessThanEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The less than "&lt;" evaluator.
    /// </summary>
    public partial class LessThanEvaluator
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
            return this.comparer.Compare(this.Left.StringValue, this.Right.StringValue) < 0;
        }
    }
}