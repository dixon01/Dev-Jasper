// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SwitchEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    /// <summary>
    /// Evaluator for <see cref="SwitchEval"/>.
    /// </summary>
    public partial class SwitchEvaluator
    {
        partial void UpdateValue()
        {
            var value = this.HandlerValue.StringValue;
            for (int i = 0; i < this.SwitchEval.Cases.Count; i++)
            {
                var @case = this.SwitchEval.Cases[i];
                if (@case.Value.Equals(value))
                {
                    this.Value = this.HandlersCases[i].StringValue;
                    return;
                }
            }

            this.Value = this.HandlerDefault.StringValue;
        }
    }
}