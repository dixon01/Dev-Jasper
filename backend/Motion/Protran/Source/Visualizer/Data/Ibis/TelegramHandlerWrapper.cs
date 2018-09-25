// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramHandlerWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.Visualizer.Data.Ibis
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.Ibis.Handlers;

    /// <summary>
    /// Wrapper for <see cref="ITelegramHandler"/> that
    /// allows logging of everything that happens in a handler.
    /// </summary>
    public class TelegramHandlerWrapper : ITelegramHandler
    {
        private readonly ITelegramHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramHandlerWrapper"/> class.
        /// </summary>
        /// <param name="handler">
        /// The wrapped handler.
        /// </param>
        public TelegramHandlerWrapper(ITelegramHandler handler)
        {
            this.handler = handler;
            this.handler.XimpleCreated += this.OnXimpleCreated;
            this.handler.StatusChanged += this.OnStatusChanged;
        }

        /// <summary>
        /// Event that is fired if <see cref="ITelegramHandler.HandleInput"/> created
        /// some data. In rare cases like DS021a, this event might also be
        /// fired asynchronously through a timer that elapsed.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired whenever an handler changes its status
        /// depending on the content of the telegrams that it has to parse.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// Event that is fired if a handler starts handling a telegram.
        /// </summary>
        public event EventHandler HandlingTelegram;

        /// <summary>
        /// Event that is fired if a handler has finished handling a telegram.
        /// </summary>
        public event EventHandler HandledTelegram;

        /// <summary>
        /// Gets the priority, the lower the number the higher the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return this.handler.Priority;
            }
        }

        /// <summary>
        /// Gets the status of this handler. By default this should return
        /// <see cref="Gorba.Common.Configuration.Protran.Ibis.Telegrams.Status.Ok"/>.
        /// </summary>
        public Status Status
        {
            get
            {
                return this.handler.Status;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this handler is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.handler.Enabled;
            }
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public void Configure(TelegramConfig config, IIbisConfigContext configContext)
        {
            this.handler.Configure(config, configContext);
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// Implementations of this method usually check the type of the event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public bool Accept(Telegram telegram)
        {
            return this.handler.Accept(telegram);
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public void HandleInput(Telegram telegram)
        {
            var eventHandler = this.HandlingTelegram;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }

            this.handler.HandleInput(telegram);

            eventHandler = this.HandledTelegram;
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// To start the handler and check for persistence. Update status of handler
        /// based on persistence.
        /// </summary>
        public void StartCheck()
        {
            this.handler.StartCheck();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.handler.ToString();
        }

        private void OnXimpleCreated(object sender, XimpleEventArgs e)
        {
            var eventHandler = this.XimpleCreated;
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            var eventHandler = this.StatusChanged;
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }
    }
}