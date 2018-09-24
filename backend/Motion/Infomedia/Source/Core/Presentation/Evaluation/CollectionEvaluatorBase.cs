// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionEvaluatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CollectionEvaluatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Criteria evaluation base class for <see cref="CollectionEvalBase"/>.
    /// </summary>
    public abstract partial class CollectionEvaluatorBase
    {
        /// <summary>
        /// Gets list of all child evaluators.
        /// </summary>
        protected IEnumerable<EvaluatorBase> Evaluators
        {
            get
            {
                return this.EvalsConditions;
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