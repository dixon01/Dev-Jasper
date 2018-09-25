// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpTimeSyncControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpTimeSyncControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.TimeSync
{
    using System;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Protocols.Sntp;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.HardwareManager.Core.Common;

    using NLog;

    using RemoteSntpServer = Gorba.Common.Protocols.Sntp.RemoteSntpServer;

    /// <summary>
    /// Base class for all time synchronization controllers.
    /// </summary>
    public abstract class SntpTimeSyncControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly SntpConfigBase config;

        private readonly IPort systemTimeOutput;

        private readonly SntpClient sntpClient;

        private readonly ITimer retryTimer;

        private int retryCount;

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpTimeSyncControllerBase"/> class.
        /// </summary>
        /// <param name="host">
        /// The host name or IP address of the SNTP server.
        /// </param>
        /// <param name="port">
        /// The UDP port of the SNTP server.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="systemTimeOutput">
        /// The system time port to be used to update the time.
        /// </param>
        protected SntpTimeSyncControllerBase(
            string host, int port, SntpConfigBase config, SystemTimeOutput systemTimeOutput)
        {
            this.config = config;
            this.systemTimeOutput = systemTimeOutput;

            this.Logger = LogHelper.GetLogger(this.GetType());

            this.sntpClient = new SntpClient();
            this.sntpClient.RemoteSntpServer = new RemoteSntpServer(host, port);
            switch (this.config.VersionNumber)
            {
                case SntpVersionNumber.Version3:
                    this.sntpClient.VersionNumber = VersionNumber.Version3;
                    break;
                case SntpVersionNumber.Version4:
                    this.sntpClient.VersionNumber = VersionNumber.Version4;
                    break;
                default:
                    this.sntpClient.VersionNumber = VersionNumber.Version3;
                    break;
            }

            this.sntpClient.QueryServerCompleted += this.SntpClientOnQueryServerCompleted;

            this.retryTimer = TimerFactory.Current.CreateTimer("TimeSync-Retry");
            this.retryTimer.AutoReset = false;
            this.retryTimer.Interval = this.config.RetryInterval;
            this.retryTimer.Elapsed += this.RetryTimerOnElapsed;
        }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public virtual void Start()
        {
            this.Logger.Info("Starting");
            this.running = true;
            this.StartQuery();
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public virtual void Stop()
        {
            this.running = false;
            this.retryTimer.Enabled = false;
            this.Logger.Info("Stopped");
        }

        /// <summary>
        /// Starts querying the SNTP server asynchronously.
        /// </summary>
        protected void StartQuery()
        {
            this.retryCount = 0;
            this.RestartQuery();
        }

        private void RestartQuery()
        {
            this.retryTimer.Enabled = false;
            if (this.sntpClient.QueryServerAsync())
            {
                return;
            }

            this.retryCount++;
            this.Logger.Warn(
                "Couldn't query time because the client wasn't ready ({0}/{1})",
                this.retryCount,
                this.config.RetryCount);
            if (this.retryCount < this.config.RetryCount)
            {
                this.retryTimer.Enabled = true;
            }
        }

        private void SntpClientOnQueryServerCompleted(object sender, QueryServerCompletedEventArgs e)
        {
            if (!this.running)
            {
                return;
            }

            if (!e.Succeeded)
            {
                this.retryCount++;
                var level = this.retryCount >= this.config.RetryCount ? LogLevel.Error : LogLevel.Warn;
                var message = string.Format(
                    "Couldn't get time from {0}:{1}: {2} ({3}/{4})",
                    this.sntpClient.RemoteSntpServer.HostNameOrAddress,
                    this.sntpClient.RemoteSntpServer.Port,
                    e.ErrorData.ErrorText,
                    this.retryCount,
                    this.config.RetryCount);
                this.Logger.Log(level, e.ErrorData.Exception, message);
                if (level != LogLevel.Error)
                {
                    this.retryTimer.Enabled = true;
                }

                return;
            }

            var utc = TimeProvider.Current.UtcNow.AddSeconds(e.Data.LocalClockOffset);

            this.systemTimeOutput.IntegerValue = (int)(utc - SystemTimeOutput.Zero).TotalSeconds;

            // log AFTER setting the time, so we don't loose any more time before setting the time
            this.Logger.Info("Updated system time to {0:R}, offset was {1:0.00}s", utc, e.Data.LocalClockOffset);
        }

        private void RetryTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.RestartQuery();
        }
    }
}