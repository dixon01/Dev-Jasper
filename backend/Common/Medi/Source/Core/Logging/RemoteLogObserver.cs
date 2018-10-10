// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteLogObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteLogObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Log observer implementation that listens to log events from a remote Medi node.
    /// </summary>
    internal sealed class RemoteLogObserver : ILogObserver
    {
        /// <summary>
        /// The request timeout. After this time log messages are no more
        /// forwarded if the request is not renewed.
        /// </summary>
        public static readonly TimeSpan RequestTimeout = TimeSpan.FromMinutes(10);

        private readonly ITimer renewTimer = TimerFactory.Current.CreateTimer("RemoteLogRenew");

        private readonly IMessageDispatcher messageDispatcher;

        private readonly MediAddress remoteAddress;

        private EventHandler<LogEventArgs> messageLogged;

        private LogLevel minLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLogObserver"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        public RemoteLogObserver(IMessageDispatcher messageDispatcher, MediAddress remoteAddress)
        {
            this.messageDispatcher = messageDispatcher;
            this.remoteAddress = remoteAddress;
            this.MinLevel = LogLevel.Info;

            this.renewTimer.Interval = TimeSpan.FromMilliseconds(RequestTimeout.TotalMilliseconds * 0.9);
            this.renewTimer.Elapsed += this.RenewTimerOnElapsed;
        }

        /// <summary>
        /// Event that is risen whenever a log message has arrived.
        /// </summary>
        public event EventHandler<LogEventArgs> MessageLogged
        {
            add
            {
                lock (this)
                {
                    var connect = this.messageLogged == null;
                    this.messageLogged = (EventHandler<LogEventArgs>)Delegate.Combine(this.messageLogged, value);

                    if (connect)
                    {
                        this.Connect();
                    }
                }
            }

            remove
            {
                lock (this)
                {
                    this.messageLogged = (EventHandler<LogEventArgs>)Delegate.Remove(this.messageLogged, value);

                    if (this.messageLogged == null)
                    {
                        this.Disconnect();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum level of this observer.
        /// </summary>
        public LogLevel MinLevel
        {
            get
            {
                return this.minLevel;
            }

            set
            {
                lock (this)
                {
                    if (value.Equals(this.minLevel))
                    {
                        return;
                    }

                    this.minLevel = value;
                    if (this.messageLogged != null)
                    {
                        this.SendLogRequest(value);
                    }
                }
            }
        }

        private void Connect()
        {
            this.messageDispatcher.Subscribe<LogEventArgs>(this.OnMessageReceived);
            this.SendLogRequest(this.MinLevel);
            this.renewTimer.Enabled = true;
        }

        private void Disconnect()
        {
            this.renewTimer.Enabled = false;
            this.SendLogRequest(LogLevel.Off);
            this.messageDispatcher.Unsubscribe<LogEventArgs>(this.OnMessageReceived);
        }

        private void SendLogRequest(LogLevel level)
        {
            this.messageDispatcher.Send(this.remoteAddress, new LogRequest { MinLevel = level });
        }

        private void RenewTimerOnElapsed(object sender, EventArgs e)
        {
            this.SendLogRequest(this.MinLevel);
        }

        private void OnMessageReceived(object sender, MessageEventArgs<LogEventArgs> e)
        {
            if (e.Source.Equals(this.remoteAddress))
            {
                this.RaiseMessageLogged(e.Message);
                Console.WriteLine("Natraj" + e.Message);
            }
        }

        private void RaiseMessageLogged(LogEventArgs args)
        {
            var handler = this.messageLogged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}