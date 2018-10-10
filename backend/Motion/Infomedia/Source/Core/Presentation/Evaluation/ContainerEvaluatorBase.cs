// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerEvaluatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContainerEvaluatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Base class for evaluators that have a <see cref="ContainerEvalBase"/>
    /// as configuration.
    /// </summary>
    public abstract partial class ContainerEvaluatorBase
    {
        /// <summary>
        /// Gets the evaluator for the contained item.
        /// </summary>
        protected EvaluatorBase InnerEvaluator
        {
            get
            {
                return this.EvalEvaluation;
            }
        }

        /// <summary>
        /// Evaluation method. Subclasses must implement this method so
        /// it returns the evaluated value.
        /// </summary>
        /// <returns>
        /// the evaluated value.
        /// </returns>
        protected abstract object CalculateValue();

        partial void UpdateValue()
        {
            this.Value = this.CalculateValue();
        }
    }
}