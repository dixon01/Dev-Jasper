// -------------------------------------public -------------------------------------------------------------------------------
// <copyright file="IntegratedLogging.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegratedLogging type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace IntegratedLoggingTest
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Timers;

    using Gorba.Common.Configuration.Core;

    using IntegratedLoggingTest.Config;

    using NLog;

    /// <summary>
    /// The integrated logging.
    /// </summary>
    public class IntegratedLogging
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConfigManager<LoggingConfig> configManager;

        private readonly Timer secTimer;

        private readonly double timerValue;

        private int counter;

        private PerformanceCounter performance;

        private ManagementObject processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegratedLogging"/> class.
        /// </summary>
        public IntegratedLogging()
        {
            this.configManager = new ConfigManager<LoggingConfig> { FileName = "Logging.xml" };

            switch (this.configManager.Config.LogLevel)
            {
                case LogLevels.Low:
                    {
                        this.timerValue = 1000;
                    }

                    break;
                case LogLevels.Mid:
                    {
                        this.timerValue = 100;
                    }

                    break;
                case LogLevels.High:
                    {
                        this.timerValue = 10;
                    }

                    break;
            }

            Logger.Info("Log Level: {0}", this.configManager.Config.LogLevel);
            this.secTimer = new Timer { AutoReset = false, Interval = this.timerValue };
            this.secTimer.Elapsed += (s, e) => this.LogStatus();
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            // Option 1
            this.performance = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // Option 2
            // this.processor = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");
            // this.processor.Get();
            this.secTimer.Start();
        }

        private void LogStatus()
        {
            // Option 1
            var cpuUsage = this.performance.NextValue();

            // Option 2
            // var cpuUsage = this.processor.Properties["PercentProcessorTime"].Value;
            var dateTime = DateTime.Now;
            Logger.Info("At DateTime: {0} ;Counter = {1} with Cpu usage: {2}", dateTime, this.counter++, cpuUsage);
            this.secTimer.Start();
        }
    }
}
