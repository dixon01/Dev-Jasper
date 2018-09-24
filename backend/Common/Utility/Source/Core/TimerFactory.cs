// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;

    /// <summary>
    /// Factory for <see cref="ITimer"/> and <see cref="IDeadlineTimer"/> objects.
    /// </summary>
    public abstract partial class TimerFactory
    {
        static TimerFactory()
        {
            Reset();
        }

        /// <summary>
        /// Gets or sets the current timer factory.
        /// The default value creates timers using <see cref="System.Timers.Timer"/>.
        /// </summary>
        public static TimerFactory Current { get; set; }

        /// <summary>
        /// Resets the <see cref="Current"/> factory to the default value.
        /// </summary>
        public static void Reset()
        {
            Current = new DefaultTimerFactory();
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
        public abstract ITimer CreateTimer(string name);

        /// <summary>
        /// Creates a new deadline timer that triggers at a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IDeadlineTimer"/> implementation.
        /// </returns>
        public abstract IDeadlineTimer CreateDeadlineTimer(string name);

        private class DefaultTimerFactory : TimerFactory
        {
            public override ITimer CreateTimer(string name)
            {
                return new DefaultTimer(name);
            }

            public override IDeadlineTimer CreateDeadlineTimer(string name)
            {
                return new DefaultDeadlineTimer(name);
            }
        }

        private partial class DefaultDeadlineTimer : IDeadlineTimer
        {
            private static readonly TimeSpan MaxInterval = TimeSpan.FromMilliseconds(0x7FFFFFFF);
            private readonly DefaultTimer timer;

            private bool enabled;

            private DateTime utcDeadline;

            public DefaultDeadlineTimer(string name)
            {
                this.Name = name;

                this.timer = new DefaultTimer(name);
                this.timer.AutoReset = false;
                this.timer.Elapsed += this.TimerOnElapsed;

                this.Initialize();
            }

            public event EventHandler Elapsed;

            public string Name { get; private set; }

            public bool Enabled
            {
                get
                {
                    return this.enabled;
                }

                set
                {
                    if (this.enabled == value)
                    {
                        return;
                    }

                    this.enabled = value;
                    this.timer.Enabled = false;
                    this.RestartTimer(false);
                }
            }

            public bool TriggerIfPassed { get; set; }

            public DateTime UtcDeadline
            {
                get
                {
                    return this.utcDeadline;
                }

                set
                {
                    if (this.utcDeadline == value)
                    {
                        return;
                    }

                    this.utcDeadline = value;
                    this.RestartTimer(false);
                }
            }

            public void Dispose()
            {
                this.Deinitialize();
                this.timer.Dispose();
            }

            partial void Initialize();

            partial void Deinitialize();

            private void RestartTimer(bool triggerAlways)
            {
                if (!this.enabled)
                {
                    // don't restart the timer if it wasn't even enabled before!
                    return;
                }

                this.timer.Enabled = false;

                var delta = this.utcDeadline - TimeProvider.Current.UtcNow;
                if (delta <= TimeSpan.Zero)
                {
                    this.enabled = false;
                    if (triggerAlways)
                    {
                        this.RaiseElapsed(EventArgs.Empty);
                    }

                    return;
                }

                if (delta > MaxInterval)
                {
                    delta = MaxInterval;
                }

                this.enabled = true;
                this.timer.Interval = delta;
                this.timer.Enabled = true;
            }

            private void TimerOnElapsed(object sender, EventArgs eventArgs)
            {
                if (!this.enabled)
                {
                    return;
                }

                if (TimeProvider.Current.UtcNow < this.UtcDeadline)
                {
                    this.RestartTimer(false);
                    if (this.enabled)
                    {
                        return;
                    }
                }

                this.enabled = false;
                this.RaiseElapsed(eventArgs);
            }

            private void RaiseElapsed(EventArgs eventArgs)
            {
                var handler = this.Elapsed;
                if (handler != null)
                {
                    handler(this, eventArgs);
                }
            }
        }
    }
}
