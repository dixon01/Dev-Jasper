// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Input handler base class that provides some helpful
    /// methods for implementing input handlers.
    /// </summary>
    /// <typeparam name="T">
    /// The type of input that is handled by this class.
    /// </typeparam>
    public abstract class InputHandler<T> : IInputHandler
        where T : Telegram
    {
        private Status status;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandler{T}"/> class.
        /// </summary>
        /// <param name="priority">
        /// The priority, the lower the number the higher the priority.
        /// </param>
        protected InputHandler(int priority)
        {
            this.Priority = priority;
            this.status = Status.Ok;
        }

        /// <summary>
        /// Event that is fired if <see cref="HandleInput"/> created
        /// some data. In rare cases like DS021a, this event might also be
        /// fired asynchronously through a timer that elapsed.
        /// </summary>
        public virtual event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired whenever an handler changes its status
        /// depending on the content of the telegrams that it has to parse.
        /// </summary>
        public virtual event EventHandler StatusChanged;

        /// <summary>
        /// Gets the priority, the lower the number the higher the priority.
        /// </summary>
        [XmlIgnore]
        public int Priority { get; private set; }

        /// <summary>
        /// Gets or sets the status of this handler. By default this should return
        /// <see cref="Gorba.Common.Configuration.Protran.Ibis.Telegrams.Status.Ok"/>.
        /// Setting the status will raise the <see cref="StatusChanged"/> event.
        /// </summary>
        [XmlIgnore]
        public Status Status
        {
            get
            {
                return this.status;
            }

            protected set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;

                var handler = this.StatusChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        protected Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public virtual bool Accept(Telegram telegram)
        {
            return telegram is T;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public virtual void HandleInput(Telegram telegram)
        {
            this.HandleInput((T)telegram);
        }

        /// <summary>
        /// To start the handler and check for persistence. Update status of handler
        /// based on persistence.
        /// </summary>
        public virtual void StartCheck()
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (this.GetType().IsGenericType)
            {
                return this.GetType().Name + "<" + typeof(T).Name + ">";
            }

            return this.GetType().Name;
        }

        /// <summary>
        /// Configures this handler with the given generic view dictionary.
        /// </summary>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        protected void Configure(IIbisConfigContext configContext)
        {
            this.Dictionary = configContext.Dictionary;
        }

        /// <summary>
        /// Handles the input event and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given input event.
        /// </summary>
        /// <param name="input">
        /// The input event.
        /// </param>
        protected abstract void HandleInput(T input);

        /// <summary>
        /// Fires the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="args">
        /// The event args.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs args)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}