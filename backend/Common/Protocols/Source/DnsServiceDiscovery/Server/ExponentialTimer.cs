// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExponentialTimer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExponentialTimer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Server
{
    using System;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Timer that expires at exponential timeouts.
    /// </summary>
    internal class ExponentialTimer
    {
        private readonly int multiplier;
        private readonly int maxCount;

        private readonly ITimer timer;

        private int waitTime;
        private int elapseCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialTimer"/> class.
        /// The timer will expire (and thus raise <see cref="Elapsed"/>):
        /// - first after one (1) second
        /// - then after 1 x <see cref="multiplier"/>
        /// - then after 1 x <see cref="multiplier"/> x <see cref="multiplier"/>
        /// This will continue until a total of <see cref="maxCount"/>.
        /// After that, the <see cref="Completed"/> event is risen.
        /// </summary>
        /// <param name="multiplier">
        /// The multiplier.
        /// </param>
        /// <param name="maxCount">
        /// The maximum count.
        /// </param>
        public ExponentialTimer(int multiplier, int maxCount)
        {
            this.multiplier = multiplier;
            this.maxCount = maxCount;

            this.timer = TimerFactory.Current.CreateTimer("DNS-SD.Announcement");
            this.timer.AutoReset = false;
            this.timer.Elapsed += this.TimerOnElapsed;
        }

        /// <summary>
        /// Event that is fired when the timer elapses.
        /// </summary>
        public event EventHandler Elapsed;

        /// <summary>
        /// Event that is fired after the maximum count was reached.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Starts this timer.
        /// </summary>
        public void Start()
        {
            if (this.waitTime > 0)
            {
                return;
            }

            this.waitTime = 1;
            this.elapseCounter = 0;
            this.timer.Interval = TimeSpan.FromMilliseconds(1);
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Stops this timer.
        /// </summary>
        public void Stop()
        {
            if (this.waitTime == 0 || this.elapseCounter >= this.maxCount)
            {
                return;
            }

            this.elapseCounter = this.maxCount;
            this.timer.Enabled = false;
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.elapseCounter++;
            var handler = this.Elapsed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            if (this.elapseCounter >= this.maxCount)
            {
                handler = this.Completed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                return;
            }

            this.timer.Interval = TimeSpan.FromSeconds(this.waitTime);
            this.timer.Enabled = true;

            this.waitTime *= this.multiplier;
        }
    }
}