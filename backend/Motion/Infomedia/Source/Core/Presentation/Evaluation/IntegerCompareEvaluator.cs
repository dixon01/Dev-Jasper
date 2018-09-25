// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerCompareEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerCompareEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Evaluation for an <see cref="IntegerCompareEval"/>.
    /// </summary>
    public partial class IntegerCompareEvaluator
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
            // don't just use evaluator.IntValue because this would return
            // 0 if the string is not a number, and in this case we want
            // the condition to be false even if the range includes 0.
            int number;
            return ParserUtil.TryParse(this.InnerEvaluator.StringValue, out number)
                   && number >= this.IntegerCompareEval.Begin && number <= this.IntegerCompareEval.End;
        }
    }
}