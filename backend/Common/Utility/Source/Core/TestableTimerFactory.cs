// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestableTimerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestableTimerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Timer factory that creates timers that can be fired manually to test
    /// behavior of classes depending on timers.
    /// </summary>
    public class TestableTimerFactory : TimerFactory
    {
        private readonly Dictionary<string, List<Timer>> timers = new Dictionary<string, List<Timer>>();

        /// <summary>
        /// Gets all timers with the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// An enumeration over all timers with the given name. Never null.
        /// </returns>
        public IEnumerable<Timer> this[string name]
        {
            get
            {
                List<Timer> list;
                if (this.timers.TryGetValue(name, out list))
                {
                    return list;
                }

                return new Timer[0];
            }
        }

        /// <summary>
        /// Creates a new timer.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ITimer"/> implementation.
        /// </returns>
        public override ITimer CreateTimer(string name)
        {
            return this.DoCreateTimer(name);
        }

        /// <summary>
        /// Creates a new deadline timer that triggers at a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IDeadlineTimer"/> implementation.
        /// </returns>
        public override IDeadlineTimer CreateDeadlineTimer(string name)
        {
            return this.DoCreateTimer(name);
        }

        private Timer DoCreateTimer(string name)
        {
            var timer = new Timer(name);
            lock (this.timers)
            {
                List<Timer> list;
                if (!this.timers.TryGetValue(name, out list))
                {
                    list = new List<Timer>();
                    this.timers.Add(name, list);
                }

                list.Add(timer);
            }

            return timer;
        }

        /// <summary>
        /// Testable timer implementation.
        /// </summary>
        public class Timer : ITimer, IDeadlineTimer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Timer"/> class.
            /// </summary>
            /// <param name="name">
            /// The name.
            /// </param>
            internal Timer(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Event that is fired whenever the timer elapses.
            /// </summary>
            public event EventHandler Elapsed;

            /// <summary>
            /// Gets the name of this timer used when creating it.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether the timer is enabled.
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the timer is automatically
            /// restarted when it elapsed.
            /// </summary>
            public bool AutoReset { get; set; }

            /// <summary>
            /// Gets or sets the interval at which the <see cref="Elapsed"/>
            /// event is fired.
            /// </summary>
            public TimeSpan Interval { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the timer should be triggered
            /// (<see cref="Elapsed"/> is fired) when the system UTC time changes past the
            /// <see cref="UtcDeadline"/>.
            /// If true, this timer will raise the <see cref="Elapsed"/> event if the
            /// system UTC time changes and the new system UTC time is greater than
            /// <see cref="UtcDeadline"/>.
            /// </summary>
            /// <example>
            /// - Timer is set to elapse on 14.03.2014 at 15:52 UTC
            /// - on 14.03.2014 at 15:00 UTC, the system time changes to 14.03.2014 16:00 UTC
            /// - if this property is set to true, the <see cref="Elapsed"/> event is fired, otherwise not
            /// </example>
            public bool TriggerIfPassed { get; set; }

            /// <summary>
            /// Gets or sets the date and time at which the <see cref="Elapsed"/>
            /// event is fired.
            /// </summary>
            public DateTime UtcDeadline { get; set; }

            /// <summary>
            /// Raises the <see cref="Elapsed"/> event.
            /// </summary>
            /// <returns>
            /// True if the event was really risen.
            /// </returns>
            public bool RaiseElapsed()
            {
                if (!this.Enabled)
                {
                    return false;
                }

                var handler = this.Elapsed;
                if (handler == null)
                {
                    return false;
                }

                handler(this, EventArgs.Empty);
                return true;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
            }
        }
    }
}