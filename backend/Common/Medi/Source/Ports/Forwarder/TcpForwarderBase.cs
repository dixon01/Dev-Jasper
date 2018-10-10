// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpForwarderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpForwarderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Forwarder
{
    using System;
    using System.Net.Sockets;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// Base class for all TCP forwarders.
    /// </summary>
    /// <typeparam name="TConfig">
    /// The type of config this port forwarder uses to configure itself.
    /// </typeparam>
    internal abstract class TcpForwarderBase<TConfig> : PortForwarderBase<TConfig>
        where TConfig : ForwardingEndPointConfig
    {
        /// <summary>
        /// Disconnects the given stream.
        /// </summary>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected abstract void Disconnect(int streamId);

        /// <summary>
        /// Begins to receive data from the given socket.
        /// </summary>
        /// <param name="socket">
        /// The socket.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected void BeginReceive(Socket socket, int streamId)
        {
            var state = new ReceiveState(socket, streamId);
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.Received, state);
        }

        /// <summary>
        /// Handles the disconnection of a given stream.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        protected override void HandleDisconnect(MediAddress source, int streamId)
        {
            this.Disconnect(streamId);
        }

        private void Received(IAsyncResult ar)
        {
            var state = (ReceiveState)ar.AsyncState;
            int received;
            try
            {
                received = state.Socket.EndReceive(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Error while receiving from " + state);
                this.Disconnect(state.StreamId);
                return;
            }

            if (received == 0)
            {
                state.Socket.Shutdown(SocketShutdown.Both);
                this.Disconnect(state.StreamId);
                state.Socket.Close();
                return;
            }

            this.SendData(state.StreamId, state.Buffer, 0, received);

            state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, this.Received, state);
        }

        private class ReceiveState
        {
            private readonly string text;

            public ReceiveState(Socket socket, int streamId)
            {
                this.Socket = socket;
                this.StreamId = streamId;
                this.Buffer = new byte[1024];

                this.text = socket.RemoteEndPoint.ToString();
            }

            public Socket Socket { get; private set; }

            public int StreamId { get; private set; }

            public byte[] Buffer { get; private set; }

            public override string ToString()
            {
                return this.text;
            }
        }
    }
}