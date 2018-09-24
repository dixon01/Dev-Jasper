// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpServerForwarder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpServerForwarder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Forwarder
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// The TCP server forwarder.
    /// </summary>
    internal class TcpServerForwarder : TcpForwarderBase<TcpServerEndPointConfig>
    {
        private readonly Socket serverSocket;

        private readonly Dictionary<int, Socket> clientSockets = new Dictionary<int, Socket>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerForwarder"/> class.
        /// </summary>
        public TcpServerForwarder()
        {
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        /// <returns>
        /// The actual <see cref="TcpServerEndPointConfig"/> used.
        /// </returns>
        protected override TcpServerEndPointConfig DoStart()
        {
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, this.Config.LocalPort));
            this.serverSocket.Listen(10);
            this.serverSocket.BeginAccept(this.Accepted, null);
            var port = ((IPEndPoint)this.serverSocket.LocalEndPoint).Port;
            return new TcpServerEndPointConfig { LocalPort = port };
        }

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected override void DoStop()
        {
            lock (this.clientSockets)
            {
                this.clientSockets.Clear();
            }

            this.serverSocket.Close();
        }

        /// <summary>
        /// Disconnects the given stream.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void Disconnect(int streamId)
        {
            Socket clientSocket;
            lock (this.clientSockets)
            {
                if (!this.clientSockets.TryGetValue(streamId, out clientSocket))
                {
                    this.Logger.Warn("Trying to disconnect unknown client socket {0}", streamId);
                    return;
                }

                this.clientSockets.Remove(streamId);
            }

            this.SendDisconnect(streamId);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        /// <summary>
        /// Handles the connect message for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void HandleConnect(MediAddress source, int streamId)
        {
            throw new NotSupportedException("Can't connect to a " + this.GetType().Name);
        }

        /// <summary>
        /// Handles the reception of data for a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        protected override void HandleData(MediAddress source, int streamId, byte[] data)
        {
            Socket clientSocket;
            lock (this.clientSockets)
            {
                if (!this.clientSockets.TryGetValue(streamId, out clientSocket))
                {
                    this.Logger.Warn("Got data for an unknown stream {0}", streamId);
                    return;
                }
            }

            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, this.DataSent, clientSocket);
        }

        private void DataSent(IAsyncResult ar)
        {
            var clientSocket = (Socket)ar.AsyncState;
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't send data to " + clientSocket.RemoteEndPoint);
                this.Disconnect(this.GetStreamId(clientSocket));
            }
        }

        private void Accepted(IAsyncResult ar)
        {
            Socket clientSocket;
            try
            {
                clientSocket = this.serverSocket.EndAccept(ar);
                this.serverSocket.BeginAccept(this.Accepted, null);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't accept new client");
                return;
            }

            var streamId = this.GetStreamId(clientSocket);
            lock (this.clientSockets)
            {
                this.clientSockets[streamId] = clientSocket;
            }

            this.SendConnect(streamId);

            this.BeginReceive(clientSocket, streamId);
        }

        private int GetStreamId(Socket clientSocket)
        {
            var endPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            return BitConverter.ToInt32(endPoint.Address.GetAddressBytes(), 0) ^ endPoint.Port;
        }
    }
}