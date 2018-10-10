// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpEchoClientControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpEchoClientControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Timers;
    using System.Windows.Forms;

    using NLog;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The udp echo client control.
    /// </summary>
    public partial class UdpEchoClientControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Timer sendTimer = new Timer();

        private UdpClient udpClient;

        private int messageCounter;

        private double sentTimestamp;

        private double rxdTimestamp;

        private Stopwatch watch;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpEchoClientControl"/> class.
        /// </summary>
        public UdpEchoClientControl()
        {
            this.InitializeComponent();
            this.watch = Stopwatch.StartNew();
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

            this.textBoxRxdMessages.Text = "0";
            this.textBoxRxdMessages.Text = "0";
            this.textBoxRxdTimeStamp.Text = "0";
            this.textBoxTotalTime.Text = "0";

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

        private void SetEnabled(bool enabled)
        {
            this.nudLocalPort.Enabled = enabled;
            this.textBoxRemoteAddress.Enabled = enabled;
            this.nudRemotePort.Enabled = enabled;
            this.textBoxPayload.Enabled = enabled;
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
                byte[] data = System.Text.Encoding.ASCII.GetBytes(this.textBoxPayload.Text);
                this.messageCounter++;
                this.watch.Start();
                this.sentTimestamp = this.watch.Elapsed.TotalMilliseconds;
                client.BeginSend(data, data.Length, this.SendCompleted, this.messageCounter);
                this.BeginInvoke(
                    new MethodInvoker(() => this.textBoxSentTimeStamp.Text = this.sentTimestamp.ToString(CultureInfo.InvariantCulture)));
                Logger.Debug("Data sent from Echo client is: {0} at time {1}", data, this.sentTimestamp);
                this.udpClient.BeginReceive(this.Received, null);
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

            try
            {
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't finish sending data", ex);
                return;
            }

            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxSentMessages.Text = this.textBoxPayload.Text));
        }

        private void HandleException(string message, Exception ex)
        {
            Logger.WarnException(message, ex);
            MessageBox.Show(this, message + ":\n" + ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Received(IAsyncResult ar)
        {
            var client = this.udpClient;
            if (client == null)
            {
                return;
            }

            try
            {
                var endPoint = new IPEndPoint(0, 1);
                var message = client.EndReceive(ar, ref endPoint);
                this.HandleReceivedMessage(message, endPoint);

                client.BeginReceive(this.Received, null);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't receive message", ex);
            }
        }

        private void HandleReceivedMessage(byte[] message, IPEndPoint endPoint)
        {
            this.rxdTimestamp = this.watch.Elapsed.TotalMilliseconds;

            var msg = this.GetString(message);
            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxRxdMessages.Text = msg));
            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxRxdTimeStamp.Text = this.rxdTimestamp.ToString(CultureInfo.InvariantCulture)));

            var timeDiff = this.rxdTimestamp - this.sentTimestamp;
            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxTotalTime.Text = timeDiff.ToString(CultureInfo.InvariantCulture)));

            Logger.Debug("Answer received is: {0} at time {1}", msg, this.sentTimestamp);
            Logger.Debug("Time between transmission and reception is: {0}", timeDiff);
            this.watch.Stop();
        }

        private string GetString(byte[] input)
        {
            string str = System.Text.Encoding.ASCII.GetString(input);
            return str;
        }
    }
}
