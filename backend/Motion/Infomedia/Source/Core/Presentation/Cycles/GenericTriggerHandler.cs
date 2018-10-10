// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTriggerHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericTriggerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Motion.Infomedia.Core.Presentation.Evaluation;

    /// <summary>
    /// Handler for <see cref="GenericTriggerConfig"/> that observes all the coordinates
    /// and triggers the <see cref="ValueChanged"/> event if one changes.
    /// </summary>
    public class GenericTriggerHandler : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTriggerHandler"/> class.
        /// </summary>
        /// <param name="trigger">
        /// The trigger.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public GenericTriggerHandler(GenericTriggerConfig trigger, IPresentationContext context)
        {
            this.Evaluators = new List<EvaluatorBase>();
            foreach (var coordinate in trigger.Coordinates)
            {
                var evaluator = EvaluatorFactory.CreateEvaluator(coordinate, context);
                evaluator.ValueChanged += this.EvaluatorOnValueChanged;
                this.Evaluators.Add(evaluator);
            }
        }

        /// <summary>
        /// Event that is fired whenever the value of any of the coordinates changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the evaluators.
        /// </summary>
        public List<EvaluatorBase> Evaluators { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var evaluator in this.Evaluators)
            {
                evaluator.ValueChanged -= this.EvaluatorOnValueChanged;
                evaluator.Dispose();
            }
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