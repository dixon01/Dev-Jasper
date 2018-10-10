// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbStickDetector.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UsbStickDetector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Usb
{
    using System;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Utility.Core;

    using NLog;

    using OpenNETCF.WindowsCE;

    /// <summary>
    /// Class that detects the insertion of a USB stick.
    /// </summary>
    public partial class UsbStickDetector
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UsbStickDetector>();

        private DeviceStatusMonitor statusMonitor;

        /// <summary>
        /// Starts this detector.
        /// </summary>
        public void Start()
        {
            if (this.statusMonitor == null)
            {
                this.statusMonitor = new DeviceStatusMonitor(DeviceClass.FileSystem, false);
                this.statusMonitor.DeviceNotification += this.StatusMonitorOnDeviceNotification;
            }

            this.statusMonitor.StartStatusMonitoring();
        }

        /// <summary>
        /// Stops this detector.
        /// </summary>
        public void Stop()
        {
            this.statusMonitor.StopStatusMonitoring();
        }

        private void StatusMonitorOnDeviceNotification(object sender, DeviceNotificationArgs e)
        {
            Logger.Debug("Detected: attached={0} class={1} name={2}", e.DeviceAttached, e.DeviceClass, e.DeviceName);
            if (e.DeviceAttached)
            {
                this.RaiseInserted(e);
            }
            else
            {
                this.RaiseRemoved(e);
            }
        }
    }
}
