// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegexReplaceEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegexReplaceEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Evaluator for <see cref="RegexReplaceEval"/>.
    /// </summary>
    public partial class RegexReplaceEvaluator
    {
        private Regex regex;

        /// <summary>
        /// Evaluation method. Subclasses must implement this method so
        /// it returns the evaluated value.
        /// </summary>
        /// <returns>
        /// the evaluated value.
        /// </returns>
        protected override object CalculateValue()
        {
            return this.regex.Replace(this.InnerEvaluator.StringValue, this.RegexReplaceEval.Replacement);
        }

        partial void Initialize()
        {
            this.regex = new Regex(this.RegexReplaceEval.Pattern);
        }
    }
}
