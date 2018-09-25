// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpTimeSyncController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpTimeSyncController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.TimeSync
{
    using System;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.HardwareManager.Core.Common;

    /// <summary>
    /// The time synchronization controller.
    /// </summary>
    public class SntpTimeSyncController : SntpTimeSyncControllerBase
    {
        private readonly SntpConfig config;

        private readonly ITimer updateTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpTimeSyncController"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="systemTimeOutput">
        /// The system time port to be used to update the time.
        /// </param>
        public SntpTimeSyncController(SntpConfig config, SystemTimeOutput systemTimeOutput)
            : base(config.Host, config.Port, config, systemTimeOutput)
        {
            this.config = config;

            this.updateTimer = TimerFactory.Current.CreateTimer("TimeSync-Update");
            this.updateTimer.AutoReset = true;
            this.updateTimer.Interval = this.config.UpdateInterval;
            this.updateTimer.Elapsed += this.UpdateTimerOnElapsed;
        }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public override void Start()
        {
            base.Start();
            this.updateTimer.Enabled = true;
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public override void Stop()
        {
            this.updateTimer.Enabled = false;
            base.Stop();
        }

        private void UpdateTimerOnElapsed(object sender, EventArgs e)
        {
            this.StartQuery();
        }
    }
}
