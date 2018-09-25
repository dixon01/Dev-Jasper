// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// JSON-RPC connection following specification 2.0.
    /// </summary>
    /// <seealso cref="http://www.jsonrpc.org/specification"/>
    public abstract class JsonRpcClient : JsonRpcConnectionBase
    {
        private readonly ITimer reconnectTimer;

        private bool connected;

        private bool initialized;

        private IPEndPoint endPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcClient"/> class.
        /// </summary>
        protected JsonRpcClient()
        {
            this.reconnectTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Interval = TimeSpan.FromSeconds(10);
            this.reconnectTimer.Elapsed += this.ReconnectTimerOnElapsed;
        }

        /// <summary>
        /// Event that is risen when the <see cref="Connected"/> property changes.
        /// </summary>
        public event EventHandler ConnectedChanged;

        /// <summary>
        /// Gets a value indicating whether this class is connected to the server.
        /// </summary>
        public bool Connected
        {
            get
            {
                return this.connected;
            }

            private set
            {
                if (this.connected == value)
                {
                    return;
                }

                this.connected = value;

                this.RaiseConnectedChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Connects to a remote server.
        /// The client will automatically try to reconnect if something goes wrong.
        /// You need to call <see cref="JsonRpcConnectionBase.Close"/> if you want to close the connection
        /// and/or stop the reconnecting behavior.
        /// </summary>
        /// <param name="address">
        /// The IP address. Default value is 127.0.0.1.
        /// </param>
        /// <param name="port">
        /// The TCP port. Default value is 3011.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// if <see cref="Connect"/> was already called once before.
        /// </exception>
        public void Connect(string address = "127.0.0.1", int port = 3011)
        {
            if (this.initialized)
            {
                throw new NotSupportedException("Can't connect twice");
            }

            var ip = IPAddress.Parse(address);
            this.endPoint = new IPEndPoint(ip, port);

            this.initialized = true;

            this.BeginConnect();
        }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        public override void Close()
        {
            this.initialized = false;
            base.Close();
        }

        /// <summary>
        /// Disconnects the underlying socket.
        /// </summary>
        protected override void Disconnect()
        {
            this.Connected = false;
            base.Disconnect();

            if (this.initialized)
            {
                this.StartReconnectTimer();
            }
        }

        /// <summary>
        /// Raises the <see cref="ConnectedChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseConnectedChanged(EventArgs e)
        {
            var handler = this.ConnectedChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void TryDisconnect()
        {
            this.Connected = false;
            base.Disconnect();
        }

        private void BeginConnect()
        {
            this.TryDisconnect();

            var client = new TcpClient();
            client.BeginConnect(this.endPoint.Address, this.endPoint.Port, this.ClientConnected, client);
        }

        private void ClientConnected(IAsyncResult ar)
        {
            this.reconnectTimer.Enabled = false;
            var client = (TcpClient)ar.AsyncState;
            try
            {
                client.EndConnect(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "ClientConnected Couldn't connect to EndPoint {0}", this.endPoint);
                this.StartReconnectTimer();
                return;
            }

            this.Start(client);
            this.Connected = true;
        }

        private void ReconnectTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                this.BeginConnect();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "ReconnectTimerOnElapsed Couldn't begin connect to EndPoint {0}", this.endPoint);
                this.StartReconnectTimer();
            }
        }

        private void StartReconnectTimer()
        {
            this.reconnectTimer.Enabled = false;
            this.reconnectTimer.Enabled = true;
        }
    }
}