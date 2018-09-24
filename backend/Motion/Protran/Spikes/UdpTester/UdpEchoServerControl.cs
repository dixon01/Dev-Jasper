// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpEchoServerControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The udp echo server control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Spikes.UdpTester
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Windows.Forms;

    using NLog;

    /// <summary>
    /// The udp echo server control.
    /// </summary>
    public partial class UdpEchoServerControl : UserControl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private UdpClient udpClient;

        private int messageCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpEchoServerControl"/> class.
        /// </summary>
        public UdpEchoServerControl()
        {
            this.InitializeComponent();
        }

        private void ButtonRunCheckedChanged(object sender, System.EventArgs e)
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
            this.textBoxRxMessages.Text = "0";

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
                this.messageCounter++;
                byte[] data = System.Text.Encoding.ASCII.GetBytes(this.textBoxSentMessages.Text);
                client.BeginSend(data, data.Length, endPoint, this.SendCompleted, this.messageCounter);
                client.BeginReceive(this.Received, null);
            }
            catch (Exception ex)
            {
                this.HandleException("Couldn't receive message", ex);
            }
        }

        private void HandleReceivedMessage(byte[] message, IPEndPoint endPoint)
        {
            var msg = this.GetString(message);
            this.BeginInvoke(
                new MethodInvoker(() => this.textBoxRxMessages.Text = msg));
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
            }
        }

        private void HandleException(string message, Exception ex)
        {
            Logger.WarnException(message, ex);
            MessageBox.Show(this, message + ":\n" + ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string GetString(byte[] input)
        {
            string str = System.Text.Encoding.ASCII.GetString(input);
            return str;
        }
    }
}
