// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Channel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Ibis.Parsers;

    using NLog;

    /// <summary>
    /// This class represents the way to communicate
    /// with the remote board computer.
    /// </summary>
    public abstract class Channel : IDisposable
    {
        /// <summary>
        /// Logger for all subclasses.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        protected Channel(IIbisConfigContext configContext)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.Config = configContext.Config;

            this.IsOpen = false;

            this.Parser = Parser.Create(this.Config.Behaviour, this.Config.Telegrams);

            // now I apply the dictionary
            this.Dictionary = configContext.Dictionary;
        }

        /// <summary>
        /// Event that is fired if <see cryef="XimpleEventArgs"/> created
        /// some data. In rare cases like DS021a, this event might also be
        /// fired asynchronously through a timer that elapsed.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Handler tasked to fire the "TelegramReceived" event.
        /// </summary>
        public event EventHandler<TelegramReceivedEventArgs> TelegramReceived;

        /// <summary>
        /// Gets or sets the dictionary instance.
        /// </summary>
        public Dictionary Dictionary { get; set; }

        /// <summary>
        /// Gets a value indicating whether
        /// this channel is opened or not.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Gets the config object.
        /// </summary>
        protected IbisConfig Config { get; private set; }

        /// <summary>
        /// Gets the instance of the channel's parser.
        /// </summary>
        protected Parser Parser { get; private set; }

        /// <summary>
        /// Disposes all the resources owned by this object.
        /// </summary>
        public virtual void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Opens this channel.
        /// To see if the channel was really opened,
        /// call the method IsOpen.
        /// </summary>
        public void Open()
        {
            if (this.IsOpen)
            {
                // the channel is already opened.
                // I avoid to open it twice.
                return;
            }

            this.DoOpen();

            this.IsOpen = true;
        }

        /// <summary>
        /// Closes this channel.
        /// To see if the channel was really closed,
        /// call the method IsOpen.
        /// </summary>
        public void Close()
        {
            if (!this.IsOpen)
            {
                // the channel is already closed.
                // I avoid to close it twice.
                return;
            }

            this.DoClose();

            this.IsOpen = false;
        }

        /// <summary>
        /// Implementation of the <see cref="Open"/> method.
        /// </summary>
        protected abstract void DoOpen();

        /// <summary>
        /// Implementation of the <see cref="Close"/> method.
        /// </summary>
        protected abstract void DoClose();

        /// <summary>
        /// Notifies all the registered handlers
        /// about the reception of an input event from
        /// the remote computer.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void RaiseTelegramReceived(TelegramReceivedEventArgs e)
        {
            var handler = this.TelegramReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event
        /// </summary>
        /// <param name="args">
        /// The event arguments.
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
