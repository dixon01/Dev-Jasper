// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOperatorEvaluatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BinaryOperatorEvaluatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    /// <summary>
    /// The base class for binary operator evaluators.
    /// </summary>
    public partial class BinaryOperatorEvaluatorBase
    {
        /// <summary>
        /// Gets the left dynamic property.
        /// </summary>
        protected DynamicPropertyHandler Left
        {
            get
            {
                return this.HandlerLeft;
            }
        }

        /// <summary>
        /// Gets the right dynamic property.
        /// </summary>
        protected DynamicPropertyHandler Right
        {
            get
            {
                return this.HandlerRight;
            }
        }

        /// <summary>
        /// Calculates the boolean value of this operator.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> value.
        /// </returns>
        protected abstract bool CalculateValue();

        partial void UpdateValue()
        {
            this.Value = this.CalculateValue();
        }
    }
}
