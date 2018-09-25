// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessResourcesObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.SystemManagement.Core.Applications;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of a process and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class ProcessResourcesObserver
    {
        private const int CpuExceedMaxCount = 2;
        private const int RamExceedMaxCount = 2;

        private const double MegaBytes = 1024 * 1024;

        private readonly Logger logger;

        private readonly Process process;
        private readonly CpuLimitConfig cpuLimit;
        private readonly ApplicationRamLimitConfig ramLimit;

        private readonly LimitController cpuLimitController;
        private readonly LimitController ramLimitController;

        private ICpuUsageObserver cpuUsageObserver;

        private int cpuLimitExceededCounter;
        private int ramLimitExceededCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResourcesObserver"/> class.
        /// </summary>
        /// <param name="process">
        /// The process to monitor.
        /// </param>
        /// <param name="controller">
        /// The controller for which this observer is working.
        /// </param>
        /// <param name="cpuLimit">
        /// The CPU limit configuration.
        /// </param>
        /// <param name="ramLimit">
        /// The RAM limit configuration.
        /// </param>
        public ProcessResourcesObserver(
            Process process,
            ApplicationControllerBase controller,
            CpuLimitConfig cpuLimit,
            ApplicationRamLimitConfig ramLimit)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + controller.Name);
            this.process = process;
            this.cpuLimit = cpuLimit;
            this.ramLimit = ramLimit;

            if (cpuLimit != null && cpuLimit.Enabled)
            {
                this.cpuLimitController = new LimitController(
                    cpuLimit, ApplicationRelaunchAttribute.CpuExcess, controller);
            }

            if (ramLimit != null && ramLimit.Enabled)
            {
                this.ramLimitController = new LimitController(
                    ramLimit, ApplicationRelaunchAttribute.MemoryLow, controller);
            }
        }

        /// <summary>
        /// Gets the CPU usage in percent (0.0 .. 1.0).
        /// </summary>
        public double CpuUsage
        {
            get
            {
                var cpu = this.cpuUsageObserver;
                return cpu != null ? cpu.Usage : 0;
            }
        }

        /// <summary>
        /// Starts this observer.
        /// </summary>
        public void Start()
        {
            this.Stop();

            this.cpuUsageObserver =
                ServiceLocator.Current.GetInstance<ICpuUsageObserverFactory>().CreateObserver(this.process);
            this.cpuUsageObserver.Updated += this.CpuUsageObserverOnUpdated;
        }

        /// <summary>
        /// Stops this observer.
        /// </summary>
        public void Stop()
        {
            if (this.cpuUsageObserver == null)
            {
                return;
            }

            this.cpuUsageObserver.Dispose();
            this.cpuUsageObserver = null;
        }

        private void CpuUsageObserverOnUpdated(object sender, EventArgs eventArgs)
        {
            var usage = this.cpuUsageObserver;
            if (usage == null)
            {
                return;
            }

            this.VerifyRamLimit(this.RamBytes);
            this.VerifyCpuLimit(usage.Usage);
        }

        private void VerifyCpuLimit(double usage)
        {
            this.logger.Trace("CPU usage: {0:0.00}%", usage * 100);
            if (this.cpuLimitController == null)
            {
                return;
            }

            var limit = this.cpuLimit.MaxCpuPercentage * 0.01;
            if (usage > limit)
            {
                this.cpuLimitExceededCounter++;
                this.logger.Warn(
                    "CPU usage above limit: {0:0.00}% > {1:0.00}% ({2}/{3})",
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

        private void VerifyRamLimit(long ram)
        {
            this.logger.Trace("RAM usage: {0:0.00}MB", ram / MegaBytes);
            if (this.ramLimitController == null)
            {
                return;
            }

            // RAM limit in bytes
            var limit = this.ramLimit.MaxRamMb * MegaBytes;
            if (ram > limit)
            {
                this.ramLimitExceededCounter++;
                this.logger.Warn(
                    "RAM usage above limit: {0:0.00}MB > {1:0.00}MB ({2}/{3})",
                    ram / MegaBytes,
                    limit / MegaBytes,
                    this.ramLimitExceededCounter,
                    RamExceedMaxCount);
                if (this.ramLimitExceededCounter >= RamExceedMaxCount)
                {
                    this.ramLimitExceededCounter = 0;
                    this.ramLimitController.ExecuteNextAction(
                        string.Format("RAM @ {0:0.00}MB", ram / MegaBytes), false);
                }
            }
            else
            {
                this.ramLimitExceededCounter = 0;
            }
        }
    }
}