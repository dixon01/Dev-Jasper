// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DualValueAnimatorStrategy.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Strategy pattern for dual value animations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager.Animation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// Strategy pattern for dual value animations.
    /// </summary>
    internal abstract class DualValueAnimatorStrategy
    {
        private PropertyChangeAnimation animation;
        private decimal startOld;
        private decimal endOld;
        private decimal startNew;
        private decimal endNew;

        private DualValueAnimatorStrategy()
        {
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public decimal OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public decimal NewValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the animation is done.
        /// </summary>
        public bool IsDone
        {
            get
            {
                return this.OldValue == this.endOld && this.NewValue == this.endNew;
            }
        }

        /// <summary>
        /// Creates a new strategy for the given animation.
        /// </summary>
        /// <param name="animation">
        /// The animation.
        /// </param>
        /// <param name="startOld">
        /// The start value of the old value.
        /// </param>
        /// <param name="endOld">
        /// The end value of the old value.
        /// </param>
        /// <param name="startNew">
        /// The start value of the new value.
        /// </param>
        /// <param name="endNew">
        /// The end value of the new value.
        /// </param>
        /// <returns>
        /// The new <see cref="DualValueAnimatorStrategy"/>. Never null.
        /// </returns>
        public static DualValueAnimatorStrategy Create(
            PropertyChangeAnimation animation, decimal startOld, decimal endOld, decimal startNew, decimal endNew)
        {
            var strategy = animation == null ? new Null() : Create(animation.Type);
            strategy.animation = animation;
            strategy.startOld = startOld;
            strategy.endOld = endOld;
            strategy.startNew = startNew;
            strategy.endNew = endNew;
            return strategy;
        }

        /// <summary>
        /// Updates this animation strategy with the given context (i.e. timestamp).
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public abstract void Update(IRenderContext context);

        private static DualValueAnimatorStrategy Create(PropertyChangeAnimationType type)
        {
            switch (type)
            {
                case PropertyChangeAnimationType.Linear:
                    return new Linear();
                case PropertyChangeAnimationType.FadeThroughNothing:
                    return new FadeThroughNothing();
                default:
                    return new Null();
            }
        }

        private class Null : DualValueAnimatorStrategy
        {
            public override void Update(IRenderContext context)
            {
                this.OldValue = this.endOld;
                this.NewValue = this.endNew;
            }
        }

        private class Linear : DualValueAnimatorStrategy
        {
            private long startTime = -1;

            public override void Update(IRenderContext context)
            {
                if (this.startTime == -1)
                {
                    this.startTime = context.MillisecondsCounter;
                }

                var duration = (int)this.animation.Duration.TotalMilliseconds;
                var deltaTime = Math.Min(context.MillisecondsCounter - this.startTime, duration);
                this.OldValue = this.startOld + ((this.endOld - this.startOld) * deltaTime / duration);
                this.NewValue = this.startNew + ((this.endNew - this.startNew) * deltaTime / duration);
            }
        }

        private class FadeThroughNothing : DualValueAnimatorStrategy
        {
            private int halfTime;
            private long startTime = -1;

            public override void Update(IRenderContext context)
            {
                if (this.startTime == -1)
                {
                    this.startTime = context.MillisecondsCounter;
                    this.halfTime = (int)this.animation.Duration.TotalMilliseconds / 2;
                }

                var deltaTime = Math.Min(context.MillisecondsCounter - this.startTime, this.halfTime);
                this.OldValue = this.startOld + ((this.endOld - this.startOld) * deltaTime / this.halfTime);

                deltaTime =
                    Math.Max(Math.Min(context.MillisecondsCounter - this.startTime - this.halfTime, this.halfTime), 0);
                this.NewValue = this.startNew + ((this.endNew - this.startNew) * deltaTime / this.halfTime);
            }
        }
    }
}