// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpClientForwarder.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpClientForwarder type.
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
    /// The TCP client forwarder.
    /// </summary>
    internal class TcpClientForwarder : TcpForwarderBase<TcpClientEndPointConfig>
    {
        private readonly Dictionary<int, Socket> sockets = new Dictionary<int, Socket>();

        /// <summary>
        /// Implementation of the start method.
        /// </summary>
        /// <returns>
        /// The actual <see cref="TcpClientEndPointConfig"/> used.
        /// </returns>
        protected override TcpClientEndPointConfig DoStart()
        {
            return this.Config;
        }

        /// <summary>
        /// Implementation of the stop method.
        /// </summary>
        protected override void DoStop()
        {
            Socket[] closing;
            lock (this.sockets)
            {
                closing = new Socket[this.sockets.Count];
                this.sockets.Values.CopyTo(closing, 0);
                this.sockets.Clear();
            }

            foreach (var socket in closing)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// Disconnects the given stream.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void Disconnect(int streamId)
        {
            Socket socket;
            lock (this.sockets)
            {
                if (!this.sockets.TryGetValue(streamId, out socket))
                {
                    this.Logger.Warn("Trying to disconnect unknown socket {0}", streamId);
                    return;
                }

                this.sockets.Remove(streamId);
            }

            this.SendDisconnect(streamId);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
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
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                lock (this.sockets)
                {
                    this.sockets.Add(streamId, socket);
                }

                socket.Connect(new IPEndPoint(IPAddress.Parse(this.Config.RemoteAddress), this.Config.RemotePort));
                this.BeginReceive(socket, streamId);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't connect to " + this.Config.RemoteAddress + ":" + this.Config.RemotePort);
                this.Disconnect(streamId);
            }
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
            Socket socket;
            lock (this.sockets)
            {
                if (!this.sockets.TryGetValue(streamId, out socket))
                {
                    this.Logger.Warn("Got data for an unknown stream {0}", streamId);
                    return;
                }
            }

            var state = new SendState(socket, streamId);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, this.DataSent, state);
        }

        private void DataSent(IAsyncResult ar)
        {
            var state = (SendState)ar.AsyncState;
            try
            {
                state.Socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't send data to " + state.Socket.RemoteEndPoint);
                this.Disconnect(state.StreamId);
            }
        }

        private class SendState
        {
            public SendState(Socket socket, int streamId)
            {
                this.Socket = socket;
                this.StreamId = streamId;
            }

            public Socket Socket { get; private set; }

            public int StreamId { get; private set; }
        }
    }
}