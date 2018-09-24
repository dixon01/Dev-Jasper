// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimerFactory.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Threading;

    using NLog;

    using OpenNETCF.WindowsCE;

    /// <summary>
    /// Factory for <see cref="ITimer"/> and <see cref="IDeadlineTimer"/> objects.
    /// </summary>
    public partial class TimerFactory
    {
        private class DefaultTimer : ITimer
        {
            private static readonly Logger Logger = LogManager.GetLogger(typeof(DefaultTimer).FullName);

            private Timer timer;

            private bool autoReset;

            private TimeSpan interval;

            private bool enabled;

            public DefaultTimer(string name)
            {
                this.Name = name;
                this.CreateTimer();

                // store the current default values
                this.interval = this.Interval;
                this.autoReset = this.AutoReset;
                this.enabled = this.Enabled;
            }

            public event EventHandler Elapsed;

            public string Name { get; private set; }

            public bool Enabled
            {
                get
                {
                    if (this.timer == null)
                    {
                        return false;
                    }

                    return this.enabled;
                }

                set
                {
                    this.enabled = value;
                    if (this.timer == null)
                    {
                        return;
                    }

                    try
                    {
                        this.EnableTimer(value);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        // this is a very rare condition, but we have seen it at least once
                        // more info: http://stackoverflow.com/questions/4793726
                        Logger.WarnException(
                            "Couldn't set ITimer.Enabled to " + value + "; recreating internal timer", ex);
                        lock (this)
                        {
                            this.CreateTimer();
                            this.Interval = this.interval;
                            this.AutoReset = this.autoReset;
                            this.EnableTimer(value);
                        }
                    }
                }
            }

            public bool AutoReset
            {
                get
                {
                    return this.autoReset;
                }

                set
                {
                    this.autoReset = value;
                    this.EnableTimer(this.Enabled);
                }
            }

            public TimeSpan Interval
            {
                get
                {
                    return this.interval;
                }

                set
                {
                    this.interval = value;
                    this.EnableTimer(this.Enabled);
                }
            }

            public void Dispose()
            {
                this.DisposeTimer();
            }

            private void EnableTimer(bool enable)
            {
                if (this.timer == null)
                {
                    return;
                }

                this.timer.Change(
                    enable ? (int)this.interval.TotalMilliseconds : Timeout.Infinite,
                    enable && this.autoReset ? (int)this.interval.TotalMilliseconds : Timeout.Infinite);
            }

            private void CreateTimer()
            {
                this.DisposeTimer();

                this.timer = new Timer(this.HandleTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            }

            private void DisposeTimer()
            {
                var t = this.timer;
                this.timer = null;
                if (t == null)
                {
                    return;
                }

                t.Dispose();
            }

            private void HandleTimerCallback(object state)
            {
                var handler = this.Elapsed;
                if (handler == null || !this.enabled)
                {
                    return;
                }

                try
                {
                    handler(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Unhandled exception in ITimer.Elapsed", ex);
                }
            }
        }

        private partial class DefaultDeadlineTimer
        {
            partial void Initialize()
            {
                DeviceManagement.TimeChanged += this.DeviceManagementOnTimeChanged;
            }

            partial void Deinitialize()
            {
                DeviceManagement.TimeChanged -= this.DeviceManagementOnTimeChanged;
            }

            private void DeviceManagementOnTimeChanged()
            {
                this.RestartTimer(this.TriggerIfPassed);
            }
        }
    }
}
