// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCacher.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.LogCache
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Timers;

    using Microsoft.Win32;

    using NLog.Common;
    using NLog.Targets;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Object tasked to represent a complete valid target for the NLog library
    /// and having the capabilities to store the logs in the RAM until a certain
    /// flush event.
    /// </summary>
    [Target("LogCacher")]
    public partial class LogCacher : FileTarget
    {
        /// <summary>
        /// List tasked to store all the log messages until a flush does occur.
        /// </summary>
        private readonly List<AsyncLogEventInfo> logsList;

        /// <summary>
        /// The timer tasked to flush all the cached logs whenever the period elapses.
        /// </summary>
        private Timer logsFlusherTimer;

        /// <summary>
        /// The amount of flush operations to be done within an hour.
        /// </summary>
        private int flushesPerHour;

        /// <summary>
        /// The amount of bytes currently stored in the logs cache.
        /// </summary>
        private int currentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCacher"/> class.
        /// </summary>
        public LogCacher()
        {
            this.logsList = new List<AsyncLogEventInfo>();
            this.CacheSizeKb = 10 * 1024;
            this.FlushesPerHour = 1;
        }

        /// <summary>
        /// Gets or sets FlushesPerHour.
        /// </summary>
        /// <exception cref="Exception">If the value is equal or less then zero.</exception>
        public int FlushesPerHour
        {
            get
            {
                return this.flushesPerHour;
            }

            set
            {
                if (value <= 0)
                {
                    // bad value.
                    throw new Exception("Invalid value for a timer.");
                }

                if (this.flushesPerHour == value)
                {
                    // nothing to do. Everything is already fine.
                    return;
                }

                // I've to re-start the timer using now the new timing.
                this.flushesPerHour = value;
                this.RestartTimer();
            }
        }

        /// <summary>
        /// Gets or sets CacheSizeKb.
        /// </summary>
        public int CacheSizeKb { get; set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// and flushes the whole cache in the file.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // first I do my private stuffes...
            // (attention: function not invoked by the NLog manager. here for completeness)
            this.FlushCache();

            // and now I can invoke the original "Dispose"
            base.Dispose(disposing);
        }

        /// <summary>
        /// Closes all the resources allocated by this object
        /// and flushes the cache content into the file.
        /// </summary>
        protected override void CloseTarget()
        {
            // first I do my private stuffes...
            // (attention: function not invoked by the NLog manager. here for completeness)
            this.FlushCache();

            // and now I can invoke the original "Dispose"
            base.CloseTarget();
        }

        /// <summary>
        /// Initializes the log cache target.
        /// </summary>
        protected override void InitializeTarget()
        {
            // first I do my provate stuffes...
            SystemEvents.SessionEnded += this.OnSystemSessionEnded;
            SystemEvents.SessionEnding += this.OnSystemSessionEnding;
            SystemEvents.PowerModeChanged += this.OnSystemPowerModeChanged;

            // and then I invoke the base method
            base.InitializeTarget();
        }

        /// <summary>
        /// Writes a log message to the targeted destination
        /// (the RAM memory).
        /// </summary>
        /// <param name="logEvent">The log event to be logged.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            // first I take the cache in mutual exclusion
            lock (((ICollection)this.logsList).SyncRoot)
            {
                byte[] bytesToWrite = this.GetBytesToWrite(logEvent.LogEvent);
                bool forceFlush = this.currentSize + bytesToWrite.Length > (this.CacheSizeKb * 1024);
                if (forceFlush)
                {
                    // with the log event just arrived we go outside the cache's limit.
                    // I've to force a flush, restart the timer and then add the log to the (cleared) cache.
                    this.FlushCache();
                    this.RestartTimer();
                }

                // ok, now I can really add the log to the cache in a safe way...
                this.logsList.Add(logEvent);
                this.currentSize += bytesToWrite.Length;
            }
        }

        /// <summary>
        /// Writes a set of log messages to the targeted destination
        /// (the RAM memory).
        /// </summary>
        /// <param name="logEvents">The set of log events to be logged.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            if (logEvents == null || logEvents.Length <= 0)
            {
                // nothing to log.
                return;
            }

            foreach (var asyncLogEventInfo in logEvents)
            {
                this.Write(asyncLogEventInfo);
            }
        }

        /// <summary>
        /// Flushes all the resources allocated by this object.
        /// </summary>
        /// <param name="asyncContinuation">
        /// The information about the async flush invocation.
        /// </param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            // first I do my private stuffes...
            // (attention: function not invoked by the NLog manager. here for completeness)
            this.FlushCache();

            // and now I can invoke the original "FlushAsync"
            base.FlushAsync(asyncContinuation);
        }

        /// <summary>
        /// Starts the timer tasked to flush the logs on the file.
        /// </summary>
        private void StartTimer()
        {
            this.logsFlusherTimer = new Timer(60.0 * 60.0 * 1000.0 / this.flushesPerHour);
            this.logsFlusherTimer.Elapsed += this.OnFlushTimeoutElapsed;
            this.logsFlusherTimer.AutoReset = true;
            this.logsFlusherTimer.Start();
        }

        /// <summary>
        /// Stops the timer tasked to flush the logs on the file.
        /// </summary>
        private void StopTimer()
        {
            if (this.logsFlusherTimer != null)
            {
                this.logsFlusherTimer.Stop();
                this.logsFlusherTimer.Dispose();
            }
        }

        /// <summary>
        /// Restarts the timer tasked to flush the logs on the file.
        /// </summary>
        private void RestartTimer()
        {
            this.StopTimer();
            this.StartTimer();
        }

        /// <summary>
        /// Function called asynchronously by the O.S. whenever the user logs off or shuts down the system.
        /// </summary>
        /// <param name="sender">The sender of this event (the O.S.).</param>
        /// <param name="e">The event.</param>
        private void OnSystemSessionEnded(object sender, SessionEndedEventArgs e)
        {
            this.FlushCache();
        }

        private void OnSystemPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            this.FlushCache();
        }

        private void OnSystemSessionEnding(object sender, SessionEndingEventArgs e)
        {
            this.FlushCache();
        }

        /// <summary>
        /// Function called asynchronously by the timer "logsFlusherTimer"
        /// whenever expires the timeout about the flush activity.
        /// </summary>
        /// <param name="sender">The sender of the asynchronous event.</param>
        /// <param name="e">The event about the timeout' expiration.</param>
        private void OnFlushTimeoutElapsed(object sender, ElapsedEventArgs e)
        {
            // the timeout is elapsed.
            // I've to flush all the logs collected until now.
            this.FlushCache();
        }

        /// <summary>
        /// Flushes all the logs currently contained in the logs cache.
        /// </summary>
        private void FlushCache()
        {
            // first I take the cache in mutual exclusion
            lock (((ICollection)this.logsList).SyncRoot)
            {
                // now I can really access to the cache in a safe way
                // and write all in one shot...
                var logs = this.logsList.ToArray();
                base.Write(logs);

                // ok. now it's the time to clear all my internal variables.
                this.logsList.Clear();
                this.currentSize = 0;
            }
        }
    }
}
