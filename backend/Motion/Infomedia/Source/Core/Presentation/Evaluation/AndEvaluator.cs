// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AndEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Criteria evaluation for an <see cref="AndEval"/>.
    /// </summary>
    public partial class AndEvaluator
    {
        /// <summary>
        /// Evaluation method. This method returns true true only if all
        /// <see cref="CollectionEvaluatorBase.Evaluators"/> are met.
        /// </summary>
        /// <returns>
        /// true if the underlying evaluation is met.
        /// </returns>
        protected override object CalculateValue()
        {
            foreach (var condition in this.Evaluators)
            {
                if (!condition.BoolValue)
                {
                    return false;
                }
            }

            return true;
        }
    }
}