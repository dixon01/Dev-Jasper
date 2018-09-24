// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycleBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCycleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;

    /// <summary>
    /// Base class for all event cycles (master and package event cycles).
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside this cycle's sections. To be defined by subclasses.
    /// </typeparam>
    public abstract class EventCycleBase<T> : CycleBase<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCycleBase{T}"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="factory">
        /// The section factory, to be provided by the subclass.
        /// </param>
        protected EventCycleBase(EventCycleConfigBase config, IPresentationContext context, ISectionFactory<T> factory)
            : base(config, context, factory)
        {
            this.TriggerHandler = new GenericTriggerHandler(config.Trigger, context);
            this.TriggerHandler.ValueChanged += this.TriggerHandlerOnValueChanged;
        }

        /// <summary>
        /// Event that is fired whenever the <see cref="EventCycleConfigBase.Trigger"/> changes
        /// and this cycle is enabled.
        /// </summary>
        public event EventHandler Triggered;

        /// <summary>
        /// Gets the trigger handler.
        /// </summary>
        public GenericTriggerHandler TriggerHandler { get; private set; }

        /// <summary>
        /// Gets a value indicating whether should loop.
        /// </summary>
        protected override bool ShouldLoop
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.TriggerHandler.ValueChanged -= this.TriggerHandlerOnValueChanged;
            this.TriggerHandler.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Raises the <see cref="Triggered"/> event.
        /// </summary>
        protected virtual void RaiseTriggered()
        {
            var handler = this.Triggered;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void TriggerHandlerOnValueChanged(object sender, EventArgs e)
        {
            if (this.IsEnabled())
            {
                this.RaiseTriggered();
            }
        }
    }
}