// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicPropertyHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicPropertyHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    /// <summary>
    /// Handles <see cref="DynamicProperty"/> inside a presentation.
    /// It wraps an <see cref="EvaluatorBase"/> if necessary.
    /// </summary>
    public class DynamicPropertyHandler : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyHandler"/> class.
        /// </summary>
        /// <param name="dynamicProperty">
        /// The dynamic property.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DynamicPropertyHandler(
            DynamicProperty dynamicProperty, string defaultValue, IPresentationContext context)
            : this(dynamicProperty, (object)defaultValue, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyHandler"/> class.
        /// </summary>
        /// <param name="dynamicProperty">
        /// The dynamic property.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DynamicPropertyHandler(DynamicProperty dynamicProperty, int defaultValue, IPresentationContext context)
            : this(dynamicProperty, (object)defaultValue, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPropertyHandler"/> class.
        /// </summary>
        /// <param name="dynamicProperty">
        /// The dynamic property.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DynamicPropertyHandler(DynamicProperty dynamicProperty, bool defaultValue, IPresentationContext context)
            : this(dynamicProperty, (object)defaultValue, context)
        {
        }

        private DynamicPropertyHandler(
            DynamicProperty dynamicProperty, object defaultValue, IPresentationContext context)
        {
            var animated = dynamicProperty as AnimatedDynamicProperty;
            if (animated != null &&
                animated.Animation != null &&
                animated.Animation.Type != PropertyChangeAnimationType.None &&
                animated.Animation.Duration > TimeSpan.Zero)
            {
                this.Animation = animated.Animation;
            }

            if (dynamicProperty != null)
            {
                this.Evaluator = EvaluatorFactory.CreateEvaluator(dynamicProperty.Evaluation, context);
            }

            if (this.Evaluator == null)
            {
                this.Evaluator = new ConstantEvaluator(defaultValue);
            }

            this.Evaluator.ValueChanged += this.EvaluatorOnValueChanged;
        }

        /// <summary>
        /// Event that is fired whenever this dynamic property changes its value.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the animation.
        /// </summary>
        public PropertyChangeAnimation Animation { get; private set; }

        /// <summary>
        /// Gets the string representation of the current value.
        /// </summary>
        public string StringValue
        {
            get
            {
                return this.Evaluator.StringValue;
            }
        }

        /// <summary>
        /// Gets the integer representation of the current value.
        /// </summary>
        public int IntValue
        {
            get
            {
                return this.Evaluator.IntValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current value is true.
        /// </summary>
        public bool BoolValue
        {
            get
            {
                return this.Evaluator.BoolValue;
            }
        }

        /// <summary>
        /// Gets the evaluator.
        /// </summary>
        public EvaluatorBase Evaluator { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Evaluator.ValueChanged -= this.EvaluatorOnValueChanged;
            this.Evaluator.Dispose();
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseValueChanged(EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void EvaluatorOnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged(e);
        }
    }
}