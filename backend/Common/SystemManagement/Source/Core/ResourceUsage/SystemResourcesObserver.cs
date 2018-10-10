// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemResourcesObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of the system and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class SystemResourcesObserver : ISystemResourceInfo
    {
        private const int CpuExceedMaxCount = 2;
        private const int RamExceedMaxCount = 2;

        private const double MegaBytes = 1024 * 1024;

        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(10);

        private static readonly Logger Logger = LogHelper.GetLogger<SystemResourcesObserver>();

        private readonly SystemConfig config;

        private readonly LimitController cpuLimitController;
        private readonly LimitController ramLimitController;

        private readonly List<DiskSpaceObserver> diskLimitObservers = new List<DiskSpaceObserver>();

        private readonly ITimer checkTimer;

        private ICpuUsageObserver cpuUsageObserver;

        private int cpuLimitExceededCounter;
        private int ramLimitMbExceededCounter;
        private int ramLimitPercentageExceededCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemResourcesObserver"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SystemResourcesObserver(SystemConfig config)
        {
            this.config = config;

            if (config.CpuLimit != null && config.CpuLimit.Enabled)
            {
                this.cpuLimitController = new LimitController(
                    config.CpuLimit, ApplicationRelaunchAttribute.CpuExcess, null);
            }

            if (config.RamLimit != null && config.RamLimit.Enabled)
            {
                this.ramLimitController = new LimitController(
                    config.RamLimit, ApplicationRelaunchAttribute.MemoryLow, null);
            }

            if (this.config.DiskLimits != null && this.config.DiskLimits.Enabled)
            {
                foreach (var diskLimit in config.DiskLimits.Disks)
                {
                    if (diskLimit.Enabled)
                    {
                        this.diskLimitObservers.Add(new DiskSpaceObserver(diskLimit));
                    }
                }
            }

            this.checkTimer = TimerFactory.Current.CreateTimer("SystemResourcesCheck");
            this.checkTimer.AutoReset = true;
            this.checkTimer.Interval = CheckInterval;
            this.checkTimer.Elapsed += this.CpuUsageObserverOnUpdated;
        }

        /// <summary>
        /// Gets the system CPU usage in percent (0.0 .. 1.0).
        /// </summary>
        public double CpuUsage
        {
            get
            {
                return this.cpuUsageObserver == null ? 0 : this.cpuUsageObserver.Usage;
            }
        }

        /// <summary>
        /// Starts this observer.
        /// </summary>
        public void Start()
        {
            this.Stop();

            try
            {
                this.cpuUsageObserver =
                    ServiceLocator.Current.GetInstance<ICpuUsageObserverFactory>().CreateObserver(null);
                this.cpuUsageObserver.Updated += this.CpuUsageObserverOnUpdated;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't create CPU observation, only observing RAM and disks");
                this.checkTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Stops this observer.
        /// </summary>
        public void Stop()
        {
            this.checkTimer.Enabled = false;

            if (this.cpuUsageObserver == null)
            {
                return;
            }

            this.cpuUsageObserver.Dispose();
            this.cpuUsageObserver = null;
        }

        private void CpuUsageObserverOnUpdated(object sender, EventArgs eventArgs)
        {
            this.VerifyRamLimit(this.AvailableRam, this.TotalRam);

            var usage = this.cpuUsageObserver;
            if (usage != null)
            {
                this.VerifyCpuLimit(usage.Usage);
            }

            foreach (var observer in this.diskLimitObservers)
            {
                observer.Verify();
            }
        }

        private void VerifyCpuLimit(double usage)
        {
            Logger.Trace("Overall CPU usage: {0:0.00}%", usage * 100);
            if (this.cpuLimitController == null)
            {
                return;
            }

            var limit = this.config.CpuLimit.MaxCpuPercentage * 0.01;
            if (usage > limit)
            {
                this.cpuLimitExceededCounter++;
                Logger.Warn(
                    "Overall CPU usage above limit: {0:0.00}% > {1:0.00}% ({2}/{3})",
                    usage * 100,
                    limit * 100,
                    this.cpuLimitExceededCounter,
                    CpuExceedMaxCount);
                if (this.cpuLimitExceededCounter >= CpuExceedMaxCount)
                {
                    this.cpuLimitExceededCounter = 0;
                    this.cpuLimitController.ExecuteNextAction(string.Format("CPU @ {0:0.00}%", usage * 100), false);
                }
            }
            else
            {
                this.cpuLimitExceededCounter = 0;
            }
        }

        private void VerifyRamLimit(long availRam, long totalRam)
        {
            Logger.Trace(
                "Overall RAM usage: {0:0.00}MB / {1:0.00}MB ({2:0.00}%)",
                (totalRam - availRam) / MegaBytes,
                totalRam / MegaBytes,
                100.0 * (totalRam - availRam) / totalRam);
            if (this.ramLimitController == null)
            {
                return;
            }

            // RAM limit in bytes
            if (this.config.RamLimit.FreeRamMb != null)
            {
                var minimumRam = this.config.RamLimit.FreeRamMb * MegaBytes;
                if (availRam < minimumRam)
                {
                    this.ramLimitMbExceededCounter++;
                    Logger.Warn(
                        "Overall free RAM below minimum: {0:0.00}MB < {1:0.00}MB ({2}/{3})",
                        availRam / MegaBytes,
                        minimumRam / MegaBytes,
                        this.ramLimitMbExceededCounter,
                        RamExceedMaxCount);
                    if (this.ramLimitMbExceededCounter >= RamExceedMaxCount)
                    {
                        this.ramLimitMbExceededCounter = 0;
                        this.ramLimitPercentageExceededCounter = 0;
                        this.ramLimitController.ExecuteNextAction(
                            string.Format("Free RAM @ {0:0.00}MB", availRam / MegaBytes), false);
                        return;
                    }
                }
                else
                {
                    this.ramLimitMbExceededCounter = 0;
                }
            }

            // RAM limit in %
            if (this.config.RamLimit.FreeRamPercentage != null)
            {
                var limit = this.config.RamLimit.FreeRamPercentage.Value * 0.01;
                var available = (double)availRam / totalRam;
                if (available < limit)
                {
                    this.ramLimitPercentageExceededCounter++;
                    Logger.Warn(
                        "Overall free RAM below minimum: {0:0.00}% ({1:0.00}MB / {2:0.00}MB) < {3:0.00}% ({4}/{5})",
                        available * 100,
                        availRam / MegaBytes,
                        totalRam / MegaBytes,
                        limit * 100,
                        this.ramLimitPercentageExceededCounter,
                        RamExceedMaxCount);
                    if (this.ramLimitPercentageExceededCounter >= RamExceedMaxCount)
                    {
                        this.ramLimitMbExceededCounter = 0;
                        this.ramLimitPercentageExceededCounter = 0;
                        this.ramLimitController.ExecuteNextAction(
                            string.Format("Free RAM @ {0:0.00}%", available * 100), false);
                    }
                }
                else
                {
                    this.ramLimitPercentageExceededCounter = 0;
                }
            }
        }
    }
}