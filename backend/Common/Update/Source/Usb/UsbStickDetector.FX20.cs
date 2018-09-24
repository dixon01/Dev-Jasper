// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbStickDetector.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UsbStickDetector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Usb
{
    using System;
    using System.Management;

    using Gorba.Common.Configuration.Update.Clients;

    using NLog;

    /// <summary>
    /// Class that detects the insertion of a USB stick.
    /// </summary>
    public partial class UsbStickDetector
    {
        private const string InsertQueryStr = "SELECT * FROM __InstanceCreationEvent " + "WITHIN 2 "
                        + "WHERE TargetInstance ISA 'Win32_DiskDriveToDiskPartition'";

        private const string RemoveQueryStr = "SELECT * FROM __InstanceDeletionEvent " + "WITHIN 2 "
                        + "WHERE TargetInstance ISA 'Win32_DiskDriveToDiskPartition'";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ManagementEventWatcher insertWatcher;
        private ManagementEventWatcher removeWatcher;

        /// <summary>
        /// Starts this detector.
        /// </summary>
        public void Start()
        {
            this.Stop();

            Logger.Debug("Starting");

            this.insertWatcher = new ManagementEventWatcher(InsertQueryStr);
            this.insertWatcher.EventArrived += this.UsbInsertionEventArrived;
            this.insertWatcher.Start();

            this.removeWatcher = new ManagementEventWatcher(RemoveQueryStr);
            this.removeWatcher.EventArrived += this.UsbRemovalEventArrived;
            this.removeWatcher.Start();
        }

        /// <summary>
        /// Stops this detector.
        /// </summary>
        public void Stop()
        {
            if (this.insertWatcher == null && this.removeWatcher == null)
            {
                return;
            }

            Logger.Debug("Stopping");

            this.StopWatcher(this.insertWatcher);
            this.StopWatcher(this.removeWatcher);

            this.insertWatcher = null;
            this.removeWatcher = null;
        }

        private void StopWatcher(ManagementEventWatcher watcher)
        {
            if (watcher == null)
            {
                return;
            }

            try
            {
                watcher.EventArrived -= this.UsbInsertionEventArrived;
                watcher.EventArrived -= this.UsbRemovalEventArrived;
                watcher.Stop();
                watcher.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't stop watcher for " + watcher.Query.QueryString);
            }
        }

        private void UsbInsertionEventArrived(object sender, EventArrivedEventArgs e)
        {
            Logger.Debug("Got USB insertion event");
            this.RaiseInserted(e);
        }

        private void UsbRemovalEventArrived(object sender, EventArrivedEventArgs e)
        {
            Logger.Debug("Got USB removal event");
            this.RaiseRemoved(e);
        }
    }
}
