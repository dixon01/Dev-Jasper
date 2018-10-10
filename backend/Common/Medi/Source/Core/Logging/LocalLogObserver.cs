// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalLogObserver.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalLogObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Utility.Core;

    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    /// <summary>
    /// Log observer implementation that listens to local log events.
    /// This observer also handles all requests from <see cref="RemoteLogObserver"/>s.
    /// </summary>
    internal sealed class LocalLogObserver : ILogObserver
    {
        private static readonly string[] IgnoreLoggers =
            {
                typeof(MessageDispatcher).Namespace + ".",
                typeof(ProducerConsumerQueue<>).Namespace + ".ProducerConsumerQueue"
            };

        private readonly Dictionary<MediAddress, RemoteInfo> remoteObservers
            = new Dictionary<MediAddress, RemoteInfo>();

        private readonly IMessageDispatcher messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalLogObserver"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        internal LocalLogObserver(IMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
            messageDispatcher.Subscribe<LogRequest>(this.OnLogRequestReceived);

            this.MinLevel = LogLevel.Info;

            this.Register();
        }

        /// <summary>
        /// Event that is risen whenever a log message has arrived.
        /// </summary>
        public event EventHandler<LogEventArgs> MessageLogged;

        /// <summary>
        /// Gets or sets the minimum level of this observer.
        /// </summary>
        public LogLevel MinLevel { get; set; }

        private static LogEventArgs CreateLogEventArgs(LogEventInfo logEvent, Layout layout)
        {
            var e = new LogEventArgs
                        {
                            Timestamp = logEvent.TimeStamp,
                            Level = logEvent.Level,
                            LoggerName = logEvent.LoggerName,
                            Message = layout.Render(logEvent),
                            Exception = logEvent.Exception != null ? logEvent.Exception.ToString() : null
                        };
            return e;
        }

        private void WriteLogEvent(LogEventInfo logEvent, Layout layout)
        {
            var handler = this.MessageLogged;
            LogEventArgs eventArgs = null;
            if (handler != null && logEvent.Level.CompareTo(this.MinLevel) >= 0)
            {
                eventArgs = CreateLogEventArgs(logEvent, layout);
                handler(this, eventArgs);
            }

            if (this.remoteObservers.Count == 0)
            {
                return;
            }

            if (logEvent.Level.CompareTo(LogLevel.Info) < 0)
            {
                foreach (var ignoreLogger in IgnoreLoggers)
                {
                    if (logEvent.LoggerName.StartsWith(ignoreLogger))
                    {
                        // ignore all log events from certain loggers to prevent an infinite loop
                        return;
                    }
                }
            }

            var addresses = new List<MediAddress>(this.remoteObservers.Count);
            lock (this.remoteObservers)
            {
                foreach (var observer in this.remoteObservers)
                {
                    if (observer.Value.Enabled && logEvent.Level.CompareTo(observer.Value.MinLevel) >= 0)
                    {
                        addresses.Add(observer.Key);
                    }
                }
            }

            if (addresses.Count == 0)
            {
                return;
            }

            if (eventArgs == null)
            {
                eventArgs = CreateLogEventArgs(logEvent, layout);
            }

            foreach (var address in addresses)
            {
                this.messageDispatcher.Send(address, eventArgs);
            }
        }

        private void Register()
        {
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            var target = new LogTarget(this)
                             {
                                 Layout = "${message}",
                                 Name = this.GetType().Name
                             };

            config.AddTarget(target.Name, target);
            var asyncTarget = new AsyncTargetWrapper(target);
            config.AddTarget("async-" + target.Name, asyncTarget);

            var loggingRule = new LoggingRule("*", LogLevel.Trace, asyncTarget);
            config.LoggingRules.Add(loggingRule);

            if (LogManager.Configuration == null)
            {
                // NLog wasn't configured before, so we have to configure it for the first time
                LogManager.Configuration = config;
            }
            else
            {
                // NLog was configured before, so we just reload it
                LogManager.Configuration.Reload();
            }
        }

        private void OnLogRequestReceived(object sender, MessageEventArgs<LogRequest> e)
        {
            var minLevel = e.Message.MinLevel;
            lock (this.remoteObservers)
            {
                if (minLevel.Equals(LogLevel.Off))
                {
                    this.remoteObservers.Remove(e.Source);
                }
                else
                {
                    this.remoteObservers[e.Source] = new RemoteInfo(minLevel);
                }
            }
        }

        private class RemoteInfo
        {
            private readonly long expiration;

            public RemoteInfo(LogLevel logLevel)
            {
                this.MinLevel = logLevel;
                this.expiration =
                    (long)(TimeProvider.Current.TickCount + RemoteLogObserver.RequestTimeout.TotalMilliseconds);
            }

            public LogLevel MinLevel { get; private set; }

            public bool Enabled
            {
                get
                {
                    return TimeProvider.Current.TickCount < this.expiration;
                }
            }
        }

        private class LogTarget : TargetWithLayout
        {
            private readonly LocalLogObserver parent;

            public LogTarget(LocalLogObserver parent)
            {
                this.parent = parent;
            }

            protected override void Write(AsyncLogEventInfo logEvent)
            {
                this.parent.WriteLogEvent(logEvent.LogEvent, this.Layout);

                base.Write(logEvent);
            }
        }
    }
}