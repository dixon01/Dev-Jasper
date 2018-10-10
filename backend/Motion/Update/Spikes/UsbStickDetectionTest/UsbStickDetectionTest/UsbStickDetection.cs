// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbStickDetection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UsbStickDetection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UsbStickDetectionTest
{
    using System;
    using System.Management;
    using System.Windows.Forms;

    /// <summary>
    /// The USB stick detection.
    /// </summary>
    public partial class UsbStickDetection : Form
    {
        private const string QueryStr = "SELECT * FROM __InstanceCreationEvent " + "WITHIN 2 "
                        + "WHERE TargetInstance ISA 'Win32_DiskDriveToDiskPartition'";

        private const int WmDevicechange = 0x0219;

        private const int DbtDevicearrival = 0x8000;

        private ManagementEventWatcher watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbStickDetection"/> class.
        /// </summary>
        public UsbStickDetection()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The value delegate.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public delegate void TextDelegate(string value);

        /// <summary>
        /// The windows procedure.
        /// </summary>
        /// <param name="m">
        /// The message.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WmDevicechange)
            {
                if (m.WParam.ToInt64() == DbtDevicearrival)
                {
                    this.WriteText("Usb stick detected by Windows event method");
                }
            }
        }

        private void ButtonWmiClick(object sender, EventArgs e)
        {
            this.watcher = new ManagementEventWatcher(QueryStr);
            this.watcher.EventArrived += this.UsbEventArrivedCreation;
            this.watcher.Start();
        }

        private void UsbEventArrivedCreation(object sender, EventArrivedEventArgs e)
        {
            this.WriteText("Usb stick detected by wmi method");
        }

        private void WriteText(string usbStickDetected)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new TextDelegate(this.WriteText), usbStickDetected);
            }
            else
            {
                this.richTextBoxWmi.AppendText(usbStickDetected);
                this.richTextBoxWmi.AppendText(Environment.NewLine);
            }
        }
    }
}
