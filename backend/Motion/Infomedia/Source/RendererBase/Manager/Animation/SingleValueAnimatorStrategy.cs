// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleValueAnimatorStrategy.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SingleValueAnimatorStrategy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager.Animation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Animation strategy for a single value.
    /// </summary>
    internal class SingleValueAnimatorStrategy
    {
        private readonly DualValueAnimatorStrategy strategy;

        private SingleValueAnimatorStrategy(DualValueAnimatorStrategy strategy)
        {
            this.strategy = strategy;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public decimal Value
        {
            get
            {
                return this.strategy.NewValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the animation is done.
        /// </summary>
        public bool IsDone
        {
            get
            {
                return this.strategy.IsDone;
            }
        }

        /// <summary>
        /// Creates a new strategy for the given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation.
        /// </param>
        /// <param name="start">
        /// The start value of the value.
        /// </param>
        /// <param name="end">
        /// The end value of the value.
        /// </param>
        /// <returns>
        /// The new <see cref="SingleValueAnimatorStrategy"/>. Never null.
        /// </returns>
        public static SingleValueAnimatorStrategy Create(
            PropertyChangeAnimation animation, decimal start, decimal end)
        {
            if (animation != null && animation.Type == PropertyChangeAnimationType.FadeThroughNothing)
            {
                throw new NotSupportedException("Can't fade a single animated value through nothing");
            }

            return new SingleValueAnimatorStrategy(DualValueAnimatorStrategy.Create(animation, start, end, start, end));
        }

        /// <summary>
        /// Updates this animation strategy with the given context (i.e. timestamp).
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Update(IRenderContext context)
        {
            this.strategy.Update(context);
        }
    }
}