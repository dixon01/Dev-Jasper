// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlphaAnimator.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AlphaAnimator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager.Animation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// An animator that handles two items with their alpha values.
    /// The alpha values can be between 0 (transparent) and 1.0 (opaque).
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items to be animated.
    /// </typeparam>
    public class AlphaAnimator<T>
        where T : class
    {
        private DualValueAnimatorStrategy strategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaAnimator{T}"/> class.
        /// </summary>
        /// <param name="value">
        /// The initial value.
        /// </param>
        public AlphaAnimator(T value)
        {
            this.NewValue = value;
            this.NewAlpha = 1;
        }

        /// <summary>
        /// A task with an alpha value.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        public delegate void Task(T item, double alpha);

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public T NewValue { get; private set; }

        /// <summary>
        /// Gets the old alpha.
        /// </summary>
        public double OldAlpha { get; private set; }

        /// <summary>
        /// Gets the new alpha.
        /// </summary>
        public double NewAlpha { get; private set; }

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
        public void Animate(PropertyChangeAnimation animation, T newValue)
        {
            this.ReleaseOld();

            this.OldValue = this.NewValue;
            this.NewValue = newValue;

            this.strategy = DualValueAnimatorStrategy.Create(animation, 1, 0, 0, 1);
        }

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
            this.OldAlpha = (double)this.strategy.OldValue;
            this.NewAlpha = (double)this.strategy.NewValue;

            if (this.strategy.IsDone)
            {
                this.strategy = null;
                this.ReleaseOld();
            }
        }

        /// <summary>
        /// Convenience method to execute a task with both
        /// the old and the new value (if they are available).
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        public void DoWithValues(Task task)
        {
            if (this.OldValue != null)
            {
                task(this.OldValue, this.OldAlpha);
            }

            if (this.NewValue != null)
            {
                task(this.NewValue, this.NewAlpha);
            }
        }

        /// <summary>
        /// Releases the old and the new values.
        /// </summary>
        public void Release()
        {
            this.ReleaseOld();

            var newValue = this.NewValue as IDisposable;
            if (newValue != null)
            {
                newValue.Dispose();
            }

            this.NewValue = null;
        }

        private void ReleaseOld()
        {
            var oldValue = this.OldValue as IDisposable;
            if (oldValue != null)
            {
                oldValue.Dispose();
            }

            this.OldValue = null;
        }
    }
}
