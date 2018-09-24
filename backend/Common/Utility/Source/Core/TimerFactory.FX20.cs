// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Timers;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Factory for <see cref="ITimer"/> objects.
    /// Default timer implementation for .NET 2.0.
    /// </summary>
    public abstract partial class TimerFactory
    {
        private class DefaultTimer : ITimer
        {
            private readonly Logger logger;

            private readonly object locker = new object();

            private readonly Timer timer;

            private bool enabled;

            public DefaultTimer(string name)
            {
                this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + name);
                this.Name = name;

                this.timer = new Timer();
                this.timer.Elapsed += this.TimerOnElapsed;

                // store the current default values
                this.enabled = this.Enabled;
            }

            public event EventHandler Elapsed;

            public string Name { get; private set; }

            public bool Enabled
            {
                get
                {
                    return this.timer.Enabled;
                }

                set
                {
                    this.enabled = value;

                    lock (this.locker)
                    {
                        this.timer.Enabled = value;
                    }
                }
            }

            public bool AutoReset
            {
                get
                {
                    return this.timer.AutoReset;
                }

                set
                {
                    lock (this.locker)
                    {
                        this.timer.AutoReset = value;
                    }
                }
            }

            public TimeSpan Interval
            {
                get
                {
                    return TimeSpan.FromMilliseconds(this.timer.Interval);
                }

                set
                {
                    lock (this.locker)
                    {
                        this.timer.Interval = value.TotalMilliseconds;
                    }
                }
            }

            public void Dispose()
            {
                this.timer.Elapsed -= this.TimerOnElapsed;
                this.timer.Dispose();
            }

            private void TimerOnElapsed(object sender, EventArgs args)
            {
                var handler = this.Elapsed;
                if (handler == null || !this.enabled)
                {
                    return;
                }

                try
                {
                    handler(this, args);
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex.Message, "Unhandled exception in ITimer.Elapsed {0}");
                }
            }
        }

        private partial class DefaultDeadlineTimer
        {
            partial void Initialize()
            {
                SystemEvents.TimeChanged += this.SystemEventsOnTimeChanged;
            }

            partial void Deinitialize()
            {
                SystemEvents.TimeChanged -= this.SystemEventsOnTimeChanged;
            }

            private void SystemEventsOnTimeChanged(object sender, EventArgs e)
            {
                this.RestartTimer(this.TriggerIfPassed);
            }
        }
    }
}
