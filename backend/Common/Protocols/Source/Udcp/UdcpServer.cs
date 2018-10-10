// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpServer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Utility.Core;

    using NLog;

    // [WES] we use an #if here because like this we can reuse the entire code without having to write it twice
#if WindowsCE
    using OpenNETCF.Net.NetworkInformation;
#else
    using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;
    using OperationalStatus = System.Net.NetworkInformation.OperationalStatus;

#endif

    /// <summary>
    /// The central UDCP component that is used for sending and receiving UDCP datagrams.
    /// </summary>
    public class UdcpServer : IDisposable
    {
        /// <summary>
        /// The default UDP port (1600).
        /// </summary>
        public static readonly int DefaultUdpPort = 1600;

        private const int SizeOf = 6;

        private static readonly Logger Logger = LogHelper.GetLogger<UdcpServer>();

        private static readonly IPEndPoint SendEndpoint = new IPEndPoint(uint.MaxValue, DefaultUdpPort);

        private readonly UdcpSerializer serializer = new UdcpSerializer();

        private readonly byte[] receiveBuffer = new byte[1024];

        private Socket socket;

        /// <summary>
        /// Event that is fired whenever a <see cref="UdcpRequest"/> is received that is meant for the local unit.
        /// </summary>
        public event EventHandler<UdcpDatagramEventArgs<UdcpRequest>> RequestReceived;

        /// <summary>
        /// Event that is fired whenever a <see cref="UdcpResponse"/> is received.
        /// </summary>
        public event EventHandler<UdcpDatagramEventArgs<UdcpResponse>> ResponseReceived;

        /// <summary>
        /// Gets the local MAC address that is used for identification of this unit via UDCP.
        /// </summary>
        public UdcpAddress LocalAddress { get; private set; }

        /// <summary>
        /// Starts this server.
        /// </summary>
        public void Start()
        {
            if (this.socket != null)
            {
                return;
            }

            UdcpAddress downAddress = null;
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (iface.GetIPProperties() == null)
                {
                    continue;
                }

                var addressBytes = iface.GetPhysicalAddress().GetAddressBytes();
                if (addressBytes.Length != SizeOf)
                {
                    continue;
                }

                if (iface.OperationalStatus != OperationalStatus.Up)
                {
                    downAddress = new UdcpAddress(addressBytes);
                    continue;
                }

                this.LocalAddress = new UdcpAddress(addressBytes);
                break;
            }

            if (this.LocalAddress == null)
            {
                if (downAddress != null)
                {
                    this.LocalAddress = downAddress;
                }
                else
                {
                    Logger.Warn("Coudln't find valid UDCP address, generating random numbers");
                    var random = new Random();
                    var addressBytes = new byte[SizeOf];
                    random.NextBytes(addressBytes);
                    this.LocalAddress = new UdcpAddress(addressBytes);
                }
            }

            Logger.Info("Starting for address {0}", this.LocalAddress);

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.socket.Bind(new IPEndPoint(IPAddress.Any, DefaultUdpPort));
            this.BeginReceive();
        }

        /// <summary>
        /// Stops this server.
        /// </summary>
        public void Stop()
        {
            this.LocalAddress = null;
            var sock = this.socket;
            if (sock == null)
            {
                return;
            }

            this.socket = null;
            sock.Close();
        }

        /// <summary>
        /// Broadcasts a given datagram on all network interfaces.
        /// </summary>
        /// <param name="datagram">
        /// The datagram to be sent.
        /// </param>
        public void SendDatagram(UdcpDatagram datagram)
        {
            var sock = this.socket;
            if (sock == null)
            {
                throw new UdcpException("Server not started");
            }

            var data = this.serializer.Serialize(datagram);
            sock.SendTo(data, SendEndpoint);
        }

        void IDisposable.Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Raises the <see cref="RequestReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseRequestReceived(UdcpDatagramEventArgs<UdcpRequest> e)
        {
            var handler = this.RequestReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ResponseReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseResponseReceived(UdcpDatagramEventArgs<UdcpResponse> e)
        {
            var handler = this.ResponseReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BeginReceive()
        {
            var sock = this.socket;
            if (sock == null)
            {
                return;
            }

            sock.BeginReceive(
                this.receiveBuffer,
                0,
                this.receiveBuffer.Length,
                SocketFlags.None,
                this.HandleReceived,
                null);
        }

        private void HandleReceived(IAsyncResult ar)
        {
            int received;
            var sock = this.socket;
            if (sock == null)
            {
                return;
            }

            try
            {
                received = sock.EndReceive(ar);
            }
            catch (Exception ex)
            {
                Logger.Error("Couldn't receive UDCP datagram, stopping server {0}", ex.Message);
                this.Stop();
                return;
            }

            try
            {
                var datagram = this.serializer.Deserialize(new MemoryStream(this.receiveBuffer, 0, received, false));
                this.HandleDatagram(datagram);
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't handle UDCP datagram {0}", ex.Message);
            }

            this.BeginReceive();
        }

        private void HandleDatagram(UdcpDatagram datagram)
        {
            var response = datagram as UdcpResponse;
            if (response != null)
            {
                this.RaiseResponseReceived(new UdcpDatagramEventArgs<UdcpResponse>(response));
                return;
            }

            var request = datagram as UdcpRequest;
            if (request == null)
            {
                throw new NotSupportedException("Unsupported datagram type: " + datagram.GetType().FullName);
            }

            if (!request.Header.UnitAddress.Equals(this.LocalAddress)
                && !request.Header.UnitAddress.Equals(UdcpAddress.BroadcastAddress))
            {
                // ignore requests that are not for us
                Logger.Debug(
                    "Ignoring {0} request which is not for us ({1})", request.Header.Type, request.Header.UnitAddress);
                return;
            }

            this.RaiseRequestReceived(new UdcpDatagramEventArgs<UdcpRequest>(request));
        }
    }
}
