// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpSenderControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpSenderControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Timers;
    using System.Windows.Forms;

    using NLog;

    using Timer = System.Timers.Timer;

    public partial class UdpSenderControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Timer sendTimer = new Timer();

        private UdpClient udpClient;

        private int messageCounter;

        public UdpSenderControl()
        {
            this.InitializeComponent();

            this.sendTimer.Elapsed += this.SendTimerOnElapsed;
        }

        private void ButtonRunCheckedChanged(object sender, EventArgs e)
        {
            if (this.udpClient != null)
            {
                this.sendTimer.Enabled = false;
                this.udpClient.Close();
                this.udpClient = null;

                this.messageCounter = 0;
            }

            this.SetEnabled(!this.buttonRun.Checked);

            if (!this.buttonRun.Checked)
            {
                return;
            }

            try
            {
                var addr = IPAddress.Parse(this.textBoxRemoteAddress.Text);
                var remoteEndPoint = new IPEndPoint(addr, (int)this.nudRemotePort.Value);

                this.textBoxSentMessages.Text = string.Empty;

                var localPort = (int)this.nudLocalPort.Value;
                this.udpClient = new UdpClient(localPort);
                Logger.Debug("Connecting from port {0} to {1}", localPort, remoteEndPoint);
                this.udpClient.Connect(remoteEndPoint);

                this.sendTimer.Interval = (double)this.nudSendInterval.Value;
                this.sendTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't start sending", ex);
            }
        }

        private void HandleException(string message, Exception ex)
        {
            Logger.WarnException(message, ex);
            MessageBox.Show(this, message + ":\n" + ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetEnabled(bool enabled)
        {
            this.nudLocalPort.Enabled = enabled;
            this.textBoxRemoteAddress.Enabled = enabled;
            this.nudRemotePort.Enabled = enabled;
            this.nudPayloadSize.Enabled = enabled;
            this.nudSendInterval.Enabled = enabled;
        }

        private void SendTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var client = this.udpClient;
            if (client == null)
            {
                return;
            }

            try
            {
                var length = Math.Max((int)this.nudPayloadSize.Value, 10);
                var data = new byte[length];
                data[0] = (byte)(this.messageCounter >> 8);
                data[1] = (byte)(this.messageCounter & 0xFF);
                this.messageCounter++;

                var timestamp = Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency;
                var timeBytes = BitConverter.GetBytes(timestamp);
                Array.Copy(timeBytes, 0, data, 2, timeBytes.Length);

                for (var i = 10; i < length; i++)
                {
                    data[i] = (byte)i;
                }

                client.BeginSend(data, length, this.SendCompleted, this.messageCounter);
                Logger.Debug("Data sent is: {0}", data);
                Logger.Debug("Sender Message Counter is: {0} at time {1}", this.messageCounter, timestamp);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't send data", ex);
            }
        }

        private void SendCompleted(IAsyncResult ar)
        {
            var client = this.udpClient;
            if (client == null)
            {
                return;
            }

            int counter;
            try
            {
                counter = (int)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't finish sending data", ex);
                return;
            }

            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxSentMessages.Text = counter.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
