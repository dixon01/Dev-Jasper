// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpReceiverControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpReceiverControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows.Forms;

    using NLog;

    public partial class UdpReceiverControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private UdpClient udpClient;

        private int messageCounter;

        private int lastMessageNumber;

        private int lostMessages;

        private long? timestampDiff;

        private long totalDelta;
        private long minDelta;
        private long maxDelta;

        public UdpReceiverControl()
        {
            this.InitializeComponent();
        }

        private void ButtonRunCheckedChanged(object sender, EventArgs e)
        {
            if (this.udpClient != null)
            {
                this.udpClient.Close();
                this.udpClient = null;
            }

            if (!this.buttonRun.Checked)
            {
                return;
            }

            this.messageCounter = 0;
            this.lostMessages = 0;
            this.lastMessageNumber = -1;
            this.timestampDiff = null;

            this.totalDelta = 0;
            this.minDelta = long.MaxValue;
            this.maxDelta = long.MinValue;

            this.textBoxRxMessages.Text = "0";
            this.textBoxLostMessages.Text = "0";
            this.textBoxRxMin.Text = string.Empty;
            this.textBoxRxAvg.Text = string.Empty;
            this.textBoxRxMax.Text = string.Empty;

            try
            {
                var localPort = (int)this.nudLocalPort.Value;
                this.udpClient = new UdpClient(localPort);
                Logger.Debug("Created client listening on port {0}", localPort);

                this.udpClient.BeginReceive(this.Received, null);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't start receiving messages", ex);
            }
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
            var timestamp = Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency;
            var counter = (message[0] << 8) | message[1];
            var receivedTimestamp = BitConverter.ToInt64(message, 2);

            lock (this)
            {
                var diff = receivedTimestamp - timestamp;
                if (!this.timestampDiff.HasValue)
                {
                    this.timestampDiff = diff;
                }

                if (this.lastMessageNumber >= 0 && this.lastMessageNumber + 1 != counter)
                {
                    this.lostMessages += counter - (this.lastMessageNumber + 1);
                }

                this.messageCounter++;
                this.lastMessageNumber = counter;

                var delta = this.timestampDiff.Value - diff;
                this.totalDelta += delta;
                this.minDelta = Math.Min(delta, this.minDelta);
                this.maxDelta = Math.Max(delta, this.maxDelta);
            }

            Logger.Debug("Receiver Message Counter is: {0} at time {1}", this.messageCounter, timestamp);
            this.BeginInvoke(new MethodInvoker(this.UpdateTextBoxes));
        }

        private void UpdateTextBoxes()
        {
            this.textBoxLostMessages.Text = this.lostMessages.ToString(CultureInfo.InvariantCulture);
            this.textBoxRxMessages.Text = this.messageCounter.ToString(CultureInfo.InvariantCulture);
            this.textBoxRxMin.Text = "0";
            this.textBoxRxAvg.Text =
                ((this.totalDelta / this.messageCounter) - this.minDelta).ToString(CultureInfo.InvariantCulture);
            this.textBoxRxMax.Text = (this.maxDelta - this.minDelta).ToString(CultureInfo.InvariantCulture);
        }

        private void HandleException(string message, Exception ex)
        {
            Logger.WarnException(message, ex);
            MessageBox.Show(this, message + ":\n" + ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
