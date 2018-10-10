// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Monitor.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Monitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using NLog;

    /// <summary>
    /// Monitors memory and CPU usage of the containing process.
    /// </summary>
    public class Monitor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Options options;

        private readonly string start;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="Monitor"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="start">The start.</param>
        public Monitor(Options options, string start)
        {
            this.options = options;
            this.start = start;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            var task = new Task(
                this.InternalStart, this.cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.ContinueWith(this.OnTaskCompleted);
            task.Start();
            Logger.Info(this.options.Type, this.start, "Monitor started");
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
        }

        private void InternalStart()
        {
            var p = Process.GetCurrentProcess();
            var ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
            while (!this.cancellationTokenSource.Token.IsCancellationRequested)
            {
                var cpu = cpuCounter.NextValue();
                var ram = ramCounter.NextValue();
                Logger.Info(this.options.Type, this.start, "Memory: {0} MB, CPU: {1}%", ram / 1024 / 1024, cpu);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void OnTaskCompleted(Task previousTask)
        {
            if (previousTask.Exception == null)
            {
                Logger.Info(this.options.Type, this.start, "Monitor completed");
                return;
            }

            Logger.WarnException("Monitor completed with errors", previousTask.Exception.Flatten());
        }
    }
}