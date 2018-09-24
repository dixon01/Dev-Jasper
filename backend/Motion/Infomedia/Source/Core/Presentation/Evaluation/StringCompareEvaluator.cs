// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringCompareEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringCompareEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    /// <summary>
    /// Criteria evaluation for a <see cref="StringCompareEval"/>.
    /// </summary>
    public partial class StringCompareEvaluator
    {
        private StringComparison comparison;

        /// <summary>
        /// Evaluation method. Subclasses must implement this method so
        /// it returns the evaluated value.
        /// </summary>
        /// <returns>
        /// the evaluated value.
        /// </returns>
        protected override object CalculateValue()
        {
            return string.Equals(this.StringCompareEval.Value, this.InnerEvaluator.StringValue, this.comparison);
        }

        partial void Initialize()
        {
            this.comparison = this.StringCompareEval.IgnoreCase
                                 ? StringComparison.InvariantCultureIgnoreCase
                                 : StringComparison.InvariantCulture;
        }
    }
}