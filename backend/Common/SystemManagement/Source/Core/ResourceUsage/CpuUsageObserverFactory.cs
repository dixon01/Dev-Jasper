// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CpuUsageObserverFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CpuUsageObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="ICpuUsageObserverFactory"/> using a single timer
    /// to observe all processes.
    /// </summary>
    public partial class CpuUsageObserverFactory : ICpuUsageObserverFactory
    {
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(10);

        private static readonly Logger Logger = LogHelper.GetLogger<CpuUsageObserverFactory>();

        private readonly List<CpuUsageObserverBase> usages = new List<CpuUsageObserverBase>();

        private readonly ITimer updateTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuUsageObserverFactory"/> class.
        /// </summary>
        public CpuUsageObserverFactory()
        {
            this.updateTimer = TimerFactory.Current.CreateTimer("CpuUsageObserver");
            this.updateTimer.Elapsed += this.UpdateTimerOnElapsed;
            this.updateTimer.Interval = UpdateInterval;
            this.updateTimer.AutoReset = true;
        }

        /// <summary>
        /// Creates a new observer for the given process or the overall usage.
        /// </summary>
        /// <param name="process">
        /// The process or null to get the overall CPU usage.
        /// </param>
        /// <returns>
        /// A new <see cref="ICpuUsageObserver"/> implementation.
        /// </returns>
        public ICpuUsageObserver CreateObserver(Process process)
        {
            CpuUsageObserverBase usage;
            if (process != null)
            {
                usage = new ProcessCpuUsageObserver(process, this);
            }
            else
            {
                try
                {
                    usage = new SystemCpuUsageObserver(this);
                }
                catch (NotSupportedException ex)
                {
                    Logger.Warn(ex, "Couldn't create real system CPU usage observer, using fake");
                    usage = new FakeCpuUsageObserver(this);
                }
            }

            lock (this.usages)
            {
                this.usages.Add(usage);
                if (this.usages.Count == 1)
                {
                    this.updateTimer.Enabled = true;
                }
            }

            return usage;
        }

        private void Remove(CpuUsageObserverBase usage)
        {
            lock (this.usages)
            {
                this.usages.Remove(usage);

                if (this.usages.Count == 0)
                {
                    this.updateTimer.Enabled = false;
                }
            }
        }

        private void UpdateCpuUsages()
        {
            List<CpuUsageObserverBase> notify;
            lock (this.usages)
            {
                notify = new List<CpuUsageObserverBase>(this.usages.Count);
                foreach (var usage in this.usages)
                {
                    try
                    {
                        if (usage.Update())
                        {
                            notify.Add(usage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Couldn't update CPU usage");
                    }
                }
            }

            foreach (var observer in notify)
            {
                try
                {
                    observer.Notify();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't notify CPU usage");
                }
            }
        }

        private abstract class CpuUsageObserverBase : ICpuUsageObserver
        {
            protected readonly CpuUsageObserverFactory Factory;

            protected CpuUsageObserverBase(CpuUsageObserverFactory factory)
            {
                this.Factory = factory;
            }

            public event EventHandler Updated;

            public double Usage { get; protected set; }

            public abstract bool Update();

            public void Notify()
            {
                this.RaiseUpdated(EventArgs.Empty);
            }

            public void Dispose()
            {
                this.Factory.Remove(this);
            }

            private void RaiseUpdated(EventArgs e)
            {
                var handler = this.Updated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        private class FakeCpuUsageObserver : CpuUsageObserverBase
        {
            public FakeCpuUsageObserver(CpuUsageObserverFactory factory)
                : base(factory)
            {
            }

            public override bool Update()
            {
                this.Usage = 0.5;
                return true;
            }
        }
    }
}
