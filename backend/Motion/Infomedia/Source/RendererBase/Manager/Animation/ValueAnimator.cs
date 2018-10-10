// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnimator.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValueAnimator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager.Animation
{
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Animator for a single value.
    /// </summary>
    public class ValueAnimator
    {
        private SingleValueAnimatorStrategy strategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAnimator"/> class.
        /// </summary>
        /// <param name="value">
        /// The initial value.
        /// </param>
        public ValueAnimator(decimal value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public decimal Value { get; private set; }

        /// <summary>
        /// Updates this animator with the given context (i.e. timestamp).
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Update(IRenderContext context)
        {
            if (this.strategy == null)
            {
                return;
            }

            this.strategy.Update(context);
            this.Value = this.strategy.Value;
            if (this.strategy.IsDone)
            {
                this.strategy = null;
            }
        }

        /// <summary>
        /// Starts the animation from the current value to a new value with
        /// the given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public void Animate(PropertyChangeAnimation animation, decimal newValue)
        {
            this.strategy = SingleValueAnimatorStrategy.Create(animation, this.Value, newValue);
        }
    }
}