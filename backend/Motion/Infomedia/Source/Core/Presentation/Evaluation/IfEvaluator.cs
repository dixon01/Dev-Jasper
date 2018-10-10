// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IfEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Evaluator for <see cref="IfEval"/>.
    /// </summary>
    public partial class IfEvaluator
    {
        partial void UpdateValue()
        {
            var handler = this.HandlerCondition.BoolValue ? this.HandlerThen : this.HandlerElse;

            this.Value = handler.StringValue;
        }
    }
}