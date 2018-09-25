// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Port80WatchdogController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Port80WatchdogController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Watchdog
{
    using System;
    using System.IO;
    using System.ServiceProcess;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The implementation for the Gorba Topbox port 80 watchdog controller.
    /// </summary>
    public partial class Port80WatchdogController : HardwareWatchdogControllerBase
    {
        private const string FileName = "C:\\Temp\\Port80.txt";
        private const string TestServiceName = "Port 80 TestService";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITimer writeTimer;

        private StreamWriter writer;

        private byte counter = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Port80WatchdogController"/> class.
        /// </summary>
        internal Port80WatchdogController()
        {
            this.writeTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.writeTimer.AutoReset = true;
            this.writeTimer.Interval = TimeSpan.FromSeconds(10);
            this.writeTimer.Elapsed += this.WriteTimerOnElapsed;
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            Logger.Debug("Starting looking for Service {0} to stop.", TestServiceName);

            try
            {
                var serviceController = new ServiceController(TestServiceName);
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    Logger.Trace("'{0}' is not running", TestServiceName);
                }
                else
                {
                    Logger.Trace("Stopping '{0}'", TestServiceName);
                    serviceController.Stop();
                    Logger.Trace("Waiting for '{0}' to stop", TestServiceName);
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));

                    if (serviceController.Status == ServiceControllerStatus.Stopped)
                    {
                        Logger.Trace("'{0}' stopped", TestServiceName);
                    }
                    else
                    {
                        Logger.Warn("Couldn't stop '{0}', it's in state {1}", TestServiceName, serviceController.Status);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Port80WatchdogController Couldn't stop service {0} {1}", TestServiceName, ex.Message);
            }

            try
            {
                if (Directory.Exists(Path.GetDirectoryName(FileName)))
                {
                    this.writer = new StreamWriter(new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
                    this.writeTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't create File " + FileName);
            }
        }

        /// <summary>
        /// Actual implementation of <see cref="HardwareWatchdogControllerBase.Stop"/>.
        /// </summary>
        protected override void DoStop()
        {
            Logger.Debug("Stopping");

            this.writeTimer.Enabled = false;

            if (this.writer != null)
            {
                this.writer.Close();
                this.writer = null;
            }
        }

        private void WriteTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                this.writer.Write("{0:X2}\n", this.counter++);
                this.writer.Flush();

                Logger.Trace("Wrote to {0}", FileName);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't write to File " + FileName);
            }
        }
    }
}