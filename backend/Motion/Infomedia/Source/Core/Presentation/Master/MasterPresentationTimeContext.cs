// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterPresentationTimeContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterPresentationTimeContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Master
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The master presentation time context.
    /// </summary>
    public class MasterPresentationTimeContext : IPresentationTimeContext, IDisposable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MasterPresentationTimeContext>();

        private static readonly TimeSpan RegularSystemTimeCheck = TimeSpan.FromSeconds(10);

        private readonly TimeProvider timeProvider;

        private readonly ITimer timer;

        private readonly List<TimedExecution> executions = new List<TimedExecution>();

        private long tickCount;

        private DateTime expectedTimerExpiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPresentationTimeContext"/> class.
        /// </summary>
        public MasterPresentationTimeContext()
        {
            this.timeProvider = TimeProvider.Current;
            this.timer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.TimerOnElapsed;

            this.UpdateUtcNow();
        }

        /// <summary>
        /// Event that is fired whenever the next timer has elapsed.
        /// </summary>
        public event EventHandler<TimeEventArgs> NextTimeReached;

        /// <summary>
        /// Gets the current presentation time in UTC.
        /// Use <see cref="DateTime.ToLocalTime()"/> to get the local time.
        /// This time is updated every time a timer expires, so it should only
        /// be used in the context of a handler registered to this class.
        /// </summary>
        public DateTime UtcNow { get; private set; }

        /// <summary>
        /// Updates the <see cref="UtcNow"/> to the current time (taken from the time provider).
        /// </summary>
        public void UpdateUtcNow()
        {
            this.UtcNow = this.timeProvider.UtcNow;
            this.tickCount = this.timeProvider.TickCount;
        }

        /// <summary>
        /// Starts this context by starting the timer.
        /// </summary>
        public void Start()
        {
            lock (this.executions)
            {
                this.UpdateUtcNow();
                this.RestartTimer();
            }
        }

        /// <summary>
        /// Stops this context by stopping the timer.
        /// </summary>
        public void Stop()
        {
            this.timer.Enabled = false;
        }

        /// <summary>
        /// Notifies all relevant handlers that the next time has been reached.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        public void NotifyTimeReached(DateTime time)
        {
            var notifications = new List<Action<DateTime>>();
            var now = this.GetNowUtc(); // update tick counter if necessary
            lock (this.executions)
            {
                while (this.executions.Count > 0 && this.executions[0].TickCount <= this.tickCount)
                {
                    notifications.Add(this.executions[0].Handler);
                    this.executions.RemoveAt(0);
                }

                this.RestartTimer();
            }

            foreach (var notification in notifications)
            {
                notification(now);
            }
        }

        /// <summary>
        /// Adds a handler for a given date and time.
        /// The handler will be called on or after the given time and the handler
        /// will automatically be removed from this context.
        /// The handler is also called if the system time changes backwards since
        /// you might have to subscribe to a different time.
        /// </summary>
        /// <param name="time">
        /// The time at which the <see cref="action"/> should be called.
        /// </param>
        /// <param name="action">
        /// The action to perform when the time is reached.
        /// The <see cref="DateTime"/> argument will be the "current" time
        /// (i.e. the one from <see cref="IPresentationTimeContext.UtcNow"/>) at the moment the action
        /// is called; it is either equal to or greater than <see cref="time"/>
        /// (except for backwards system time changes).
        /// </param>
        public void AddTimeReachedHandler(DateTime time, Action<DateTime> action)
        {
            var utc = time.ToUniversalTime();
            var utcNow = this.GetNowUtc();
            var ticks = this.tickCount + (long)(utc - utcNow).TotalMilliseconds;
            this.AddHandler(new TimedExecution(utc, ticks, action));
        }

        /// <summary>
        /// Removes a handler previously added with <see cref="IPresentationTimeContext.AddTimeReachedHandler"/>.
        /// </summary>
        /// <param name="time">
        /// The same time provided to <see cref="IPresentationTimeContext.AddTimeReachedHandler"/>.
        /// </param>
        /// <param name="action">
        /// The same action provided to <see cref="IPresentationTimeContext.AddTimeReachedHandler"/>.
        /// </param>
        public void RemoveTimeReachedHandler(DateTime time, Action<DateTime> action)
        {
            var utc = time.ToUniversalTime();
            this.RemoveHandler(e => e.Time == utc && e.Handler == action);
        }

        /// <summary>
        /// Adds a handler for a given timespan.
        /// The handler will be called on or after the given timespan has elapsed
        /// and the handler will automatically be removed from this context.
        /// </summary>
        /// <param name="timeSpan">
        /// The timespan after which the <see cref="action"/> should be called.
        /// </param>
        /// <param name="action">
        /// The action to perform when the timespan has elapsed.
        /// The <see cref="DateTime"/> argument will be the "current" time
        /// (i.e. the one from <see cref="IPresentationTimeContext.UtcNow"/>) at the moment the action
        /// is called; it is either equal to or greater than the "current" time
        /// at the moment this method was called plus the given <see cref="timeSpan"/>.
        /// </param>
        public void AddTimeElapsedHandler(TimeSpan timeSpan, Action<DateTime> action)
        {
            if (timeSpan <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeSpan", "Expected time span greater than zero");
            }

            var ticks = this.tickCount + (long)timeSpan.TotalMilliseconds;
            this.AddHandler(new TimedExecution(this.GetNowUtc(), timeSpan, ticks, action));
        }

        /// <summary>
        /// Removes a handler previously added with <see cref="IPresentationTimeContext.AddTimeElapsedHandler"/>.
        /// </summary>
        /// <param name="timeSpan">
        /// The same timespan provided to <see cref="IPresentationTimeContext.AddTimeElapsedHandler"/>.
        /// </param>
        /// <param name="action">
        /// The same action provided to <see cref="IPresentationTimeContext.AddTimeElapsedHandler"/>.
        /// </param>
        public void RemoveTimeElapsedHandler(TimeSpan timeSpan, Action<DateTime> action)
        {
            this.RemoveHandler(e => e.TimeSpan == timeSpan && e.Handler == action);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();
            this.timer.Elapsed -= this.TimerOnElapsed;
        }

        /// <summary>
        /// Raises the <see cref="NextTimeReached"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseNextTimeReached(TimeEventArgs e)
        {
            var handler = this.NextTimeReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private DateTime GetNowUtc()
        {
            if (this.UtcNow.AddMilliseconds(500) < this.timeProvider.UtcNow)
            {
                this.UpdateUtcNow();
            }

            return this.UtcNow;
        }

        private void RestartTimer()
        {
            // IMPORTANT: this method has to be called from within a
            // lock (this.executions) {}
            this.timer.Enabled = false;
            if (this.executions.Count == 0)
            {
                return;
            }

            var interval = this.executions[0].Time - this.UtcNow;
            if (interval <= TimeSpan.Zero)
            {
                interval = TimeSpan.FromMilliseconds(1);
            }
            else if (interval > RegularSystemTimeCheck)
            {
                interval = RegularSystemTimeCheck;
            }

            this.expectedTimerExpiry = this.UtcNow + interval;
            this.timer.Interval = interval;
            this.timer.Enabled = true;
        }

        private void AddHandler(TimedExecution execution)
        {
            lock (this.executions)
            {
                this.executions.Add(execution);
                this.executions.Sort();
                if (this.executions[0] == execution)
                {
                    this.RestartTimer();
                }
            }
        }

        private void RemoveHandler(Predicate<TimedExecution> match)
        {
            lock (this.executions)
            {
                int index = this.executions.FindIndex(match);
                if (index < 0)
                {
                    return;
                }

                this.executions.RemoveAt(index);

                if (index == 0)
                {
                    this.RestartTimer();
                }
            }
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            DateTime time;
            lock (this.executions)
            {
                this.UpdateUtcNow();
                time = this.UtcNow;
            }

            if ((time - this.expectedTimerExpiry).Ticks > RegularSystemTimeCheck.Ticks)
            {
                Logger.Warn(
                    "Forward jump of system time detected, expected {0} but it's {1}",
                    this.expectedTimerExpiry,
                    time);

                lock (this.executions)
                {
                    // update all "Reached" handlers to be called at the right tick count
                    foreach (var execution in this.executions)
                    {
                        if (execution.TimeSpan == TimeSpan.Zero)
                        {
                            execution.UpdateTickCount(
                                this.tickCount + (long)(execution.Time - this.UtcNow).TotalMilliseconds);
                        }
                    }

                    this.executions.Sort();
                }
            }
            else if ((this.expectedTimerExpiry - time).Ticks > RegularSystemTimeCheck.Ticks)
            {
                Logger.Warn(
                    "Backwards jump of system time detected, expected {0} but it's {1}",
                    this.expectedTimerExpiry,
                    time);

                lock (this.executions)
                {
                    // set all "Reached" handlers to be called immediately
                    foreach (var execution in this.executions)
                    {
                        if (execution.TimeSpan == TimeSpan.Zero)
                        {
                            execution.UpdateTickCount(0);
                        }
                    }

                    this.executions.Sort();
                }
            }

            try
            {
                this.RaiseNextTimeReached(new TimeEventArgs(time));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Error while raising next time reached");
            }
        }

        private class TimedExecution : IComparable<TimedExecution>
        {
            public TimedExecution(DateTime time, long tickCount, Action<DateTime> handler)
            {
                this.Time = time;
                this.TickCount = tickCount;
                this.Handler = handler;
            }

            public TimedExecution(DateTime now, TimeSpan timeSpan, long tickCount, Action<DateTime> action)
                : this(now + timeSpan, tickCount, action)
            {
                this.TimeSpan = timeSpan;
            }

            public DateTime Time { get; private set; }

            public TimeSpan TimeSpan { get; private set; }

            public long TickCount { get; private set; }

            public Action<DateTime> Handler { get; private set; }

            public void UpdateTickCount(long tickCount)
            {
                this.TickCount = tickCount;
            }

            public int CompareTo(TimedExecution other)
            {
                return this.TickCount.CompareTo(other.TickCount);
            }
        }
    }
}