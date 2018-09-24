// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpServerChannel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpServerChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Motion.Protran.Core.Buffers;

    /// <summary>
    /// IBIS channel that listens on a UDP socket for telegrams.
    /// </summary>
    public class UdpServerChannel : IbisChannel, IManageableObject
    {
        private readonly UdpServerConfig udpServerConfig;

        private readonly byte[] readBuffer = new byte[1024];

        private Socket udpSocket;

        private EndPoint lastRemoteEndPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpServerChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public UdpServerChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.udpServerConfig = configContext.Config.Sources.UdpServer;
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Udp Channel open", this.IsOpen, true);
        }

        /// <summary>
        /// Opens this channel and opens the UDP port.
        /// </summary>
        protected override void DoOpen()
        {
            base.DoOpen();

            this.udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            this.Logger.Info("Binding UDP server to local port {0}", this.udpServerConfig.LocalPort);
            this.udpSocket.Bind(new IPEndPoint(IPAddress.Any, this.udpServerConfig.LocalPort));
            this.BeginReceive();
        }

        /// <summary>
        /// Closes this channel and closes the UDP port.
        /// </summary>
        protected override void DoClose()
        {
            if (this.udpSocket != null)
            {
                this.udpSocket.Close();
                this.udpSocket = null;
            }

            base.DoClose();
        }

        /// <summary>
        /// Sends an answer to the UDP port that sent us the last datagram.
        /// </summary>
        /// <param name="bytes">
        /// The buffer to send.
        /// </param>
        /// <param name="offset">
        /// The offset inside the buffer.
        /// </param>
        /// <param name="length">
        /// The number of bytes to send starting from <see cref="offset"/>.
        /// </param>
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
            var socket = this.udpSocket;
            if (socket == null)
            {
                return;
            }

            switch (this.udpServerConfig.ReceiveFormat)
            {
                case TelegramFormat.NoChecksum:
                    length -= this.Parser.ByteInfo.ByteSize;
                    break;
                case TelegramFormat.NoFooter:
                    length -= 2 * this.Parser.ByteInfo.ByteSize;
                    break;
            }

            socket.SendTo(bytes, offset, length, SocketFlags.None, this.lastRemoteEndPoint);
        }

        private void BeginReceive()
        {
            var socket = this.udpSocket;
            if (socket == null)
            {
                return;
            }

            EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            socket.BeginReceiveFrom(
                this.readBuffer, 0, this.readBuffer.Length, SocketFlags.None, ref ep, this.Received, null);
        }

        private void Received(IAsyncResult ar)
        {
            var socket = this.udpSocket;
            if (socket == null)
            {
                return;
            }

            try
            {
                this.lastRemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                int received = socket.EndReceiveFrom(ar, ref this.lastRemoteEndPoint);
                this.Logger.Debug("Received {0} bytes from {1}", received, this.lastRemoteEndPoint);

                this.Logger.Trace(() => BufferUtils.FromByteArrayToHexString(this.readBuffer, 0, received, false));

                if (received > 0)
                {
                    this.ManageTelegram(this.CreateTelegram(received));
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Excption while receiving data");
            }

            this.BeginReceive();
        }

        /// <summary>
        /// Create a telegram according to the configured ReceiveFormat.
        /// </summary>
        /// <param name="received">the number of bytes received</param>
        /// <returns>a new byte array containing the telegram.</returns>
        private byte[] CreateTelegram(int received)
        {
            var size = received;
            switch (this.udpServerConfig.ReceiveFormat)
            {
                case TelegramFormat.NoChecksum:
                    size += this.Parser.ByteInfo.ByteSize;
                    break;
                case TelegramFormat.NoFooter:
                    size += 2 * this.Parser.ByteInfo.ByteSize;
                    break;
            }

            var telegram = new byte[size];
            Array.Copy(this.readBuffer, telegram, received);

            switch (this.udpServerConfig.ReceiveFormat)
            {
                case TelegramFormat.NoChecksum:
                    this.Parser.UpdateChecksum(telegram);
                    break;
                case TelegramFormat.NoFooter:
                    telegram[size - this.Parser.ByteInfo.ByteSize - 1] = 0x0D;
                    this.Parser.UpdateChecksum(telegram);
                    break;
            }

            return telegram;
        }
    }
}
