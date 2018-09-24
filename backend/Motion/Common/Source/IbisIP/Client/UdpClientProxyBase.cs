// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdpClientProxyBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdpClientProxyBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Client
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using NLog;

    /// <summary>
    /// Base class for all IBIS-IP UDP client proxies.
    /// </summary>
    internal abstract class UdpClientProxyBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly byte[] buffer = new byte[65536];
        private readonly IPEndPoint endPoint;
        private Socket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpClientProxyBase"/> class.
        /// </summary>
        /// <param name="endPoint">
        /// The multicast IP address to subscribe to and the port on which the data arrives.
        /// </param>
        protected UdpClientProxyBase(IPEndPoint endPoint)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.endPoint = endPoint;
        }

        /// <summary>
        /// Handles one incoming datagram.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset into the <paramref name="data"/>.
        /// </param>
        /// <param name="size">
        /// The number of bytes in the <paramref name="data"/> from the given <paramref name="offset"/>.
        /// </param>
        protected abstract void HandleDatagram(byte[] data, int offset, int size);

        /// <summary>
        /// Subscribes to the multicast group and begins receiving datagrams.
        /// After calling this method, the <see cref="HandleDatagram"/> method
        /// is called every time we receive a datagram.
        /// </summary>
        protected void Subscribe()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // TODO: do we need this? (not available in CF 3.5)
            ////this.socket.ExclusiveAddressUse = false;
            this.socket.Bind(new IPEndPoint(IPAddress.Any, this.endPoint.Port));
            this.socket.SetSocketOption(
                SocketOptionLevel.IP,
                SocketOptionName.AddMembership,
                new MulticastOption(this.endPoint.Address));
            this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            this.socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, this.Received, null);
        }

        /// <summary>
        /// Unsubscribes from the multicast group.
        /// </summary>
        protected void Unsubscribe()
        {
            this.socket.Close();
            this.socket = null;
        }

        private void Received(IAsyncResult ar)
        {
            int received;
            try
            {
                received = this.socket.EndReceive(ar);
            }
            catch (ObjectDisposedException ex)
            {
                this.Logger.Debug(ex, "UDP client was closed");
                return;
            }

            try
            {
                this.HandleDatagram(this.buffer, 0, received);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't handle data");
            }

            this.socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, this.Received, null);
        }
    }
}
