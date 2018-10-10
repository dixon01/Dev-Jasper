// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CpuUser.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CpuUser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.TestApps.WinFormsTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Class that uses up a CPU core for the given percentage.
    /// Idea borrowed from:
    /// <see cref="http://www.vbdotnetforums.com/vb-net-general-discussion/47713-increasing-cpu-usage-simulate-heavy-computer-use.html"/>
    /// </summary>
    internal class CpuUser
    {
        private Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="CpuUser"/> class.
        /// </summary>
        public CpuUser()
        {
            this.thread = new Thread(this.Run);
            this.thread.IsBackground = true;
            this.thread.Priority = ThreadPriority.BelowNormal;
            this.thread.Start();
        }

        /// <summary>
        /// Gets or sets the expected usage in percentage (0..1).
        /// </summary>
        public double ExpectedUsage { get; set; }

        /// <summary>
        /// Stops this user.
        /// </summary>
        public void Stop()
        {
            this.thread = null;
        }

        private void Run()
        {
            var stopwatch = Stopwatch.StartNew();
            while (this.thread != null)
            {
                var loadMilliseconds = 1000 * this.ExpectedUsage;
                var sleepMilliseconds = 1000 - loadMilliseconds;
                if (stopwatch.ElapsedMilliseconds >= loadMilliseconds)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(sleepMilliseconds));
                    stopwatch.Reset();
                    stopwatch.Start();
                }
            }
        }
    }
}