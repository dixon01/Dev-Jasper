using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CompactFramework35Test
{
    using System.IO.Ports;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple;

    using OpenNETCF.IO;
    using OpenNETCF.WindowsCE;

    public partial class Form1 : Form
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly DeviceStatusMonitor statusMonitor;

        public Form1()
        {
            this.InitializeComponent();

            MessageDispatcher.Instance.Configure(new AutoConfigurator());

            MessageDispatcher.Instance.Subscribe<Ximple>(this.HandleXimple);

            /*foreach (var portName in SerialPort.GetPortNames())
            {
                var com = portName;
                this.textBoxReceived.Text += portName + Environment.NewLine;
                var thread = new Thread(() => this.WritePorts(com));
                thread.IsBackground = true;
                thread.Start();
            }*/

            this.statusMonitor = new DeviceStatusMonitor(DeviceClass.FileSystem, false);
            this.statusMonitor.DeviceNotification += this.StatusMonitorOnDeviceNotification;
        }

        private void WritePorts(string com)
        {
            var port = new SerialPort(com);
            port.Parity = Parity.Even;
            port.BaudRate = 19200;
            port.DataBits = 8;
            port.Encoding = Encoding.ASCII;
            port.Handshake = Handshake.None;
            port.Parity = Parity.Even;
            port.StopBits = StopBits.One;

            port.Open();

            for (int i = 0; true; i++)
            {
                port.WriteLine("Message from " + com + ": " + i);
                Thread.Sleep(500);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.statusMonitor.StartStatusMonitoring();
        }

        private void StatusMonitorOnDeviceNotification(object sender, DeviceNotificationArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DeviceNotificationEventHandler(this.StatusMonitorOnDeviceNotification), sender, e);
                return;
            }

            MessageBox.Show(
                "Attached: " + e.DeviceAttached + "\r\nClass: " + e.DeviceClass + "\r\nName: " + e.DeviceName
                + "\r\nGUID: " + e.DeviceInterfaceGUID);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
                this.textBox1.Text = stopwatch.Elapsed.ToString();
            }
            else
            {
                stopwatch.Start();
                this.textBox1.Text = "Running...";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ximple = new Ximple();
            ximple.Cells.Add(
                new XimpleCell
                    {
                        LanguageNumber = 0,
                        TableNumber = 1,
                        ColumnNumber = 1,
                        RowNumber = 0,
                        Value = this.textBoxMessage.Text
                    });

            MessageDispatcher.Instance.Broadcast(ximple);
        }

        private void HandleXimple(object sender, MessageEventArgs<Ximple> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<MessageEventArgs<Ximple>>(this.HandleXimple), sender, e);
                return;
            }

            this.textBoxReceived.Text = e.Message.ToXmlString();
        }

        private void Button4Click(object sender, EventArgs e)
        {
            this.textBoxReceived.Text = string.Empty;
            foreach (var drive in DriveInfo.GetDrives())
            {
                this.textBoxReceived.Text += string.Format(
                    "{0} [{1} / {2}]\r\n", drive.RootDirectory, drive.AvailableFreeSpace, drive.TotalSize);
            }
        }
    }
}