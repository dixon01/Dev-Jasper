// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.CenterMotion
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using Gorba.Common.Configuration.Obc.Bus;
    using Gorba.Common.Protocols.Eci.Messages;
    using Gorba.Common.Protocols.Eci.Serialization;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The iCenter client.
    /// </summary>
    public class CenterClient
    {
        private static readonly Logger Logger = LogHelper.GetLogger<CenterClient>();

        private readonly CenterClientConfig config;

        private readonly ITimer reconnectTimer;
        private readonly byte[] readBuffer = new byte[1000];

        private readonly EciMessageQueue msgQueue = new EciMessageQueue(10);

        private Socket socket;

        private NetworkStream stream;

        private bool isRunning;

        // private Thread messageHandlerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="CenterClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public CenterClient(CenterClientConfig config)
        {
            this.config = config;
            this.reconnectTimer = TimerFactory.Current.CreateTimer("CenterClientReconnect");
            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Interval = this.config.ReconnectTime ?? TimeSpan.FromSeconds(6);
            this.reconnectTimer.Elapsed += (s, e) => this.BeginConnect();
        }

        /// <summary>
        /// The message received.
        /// </summary>
        public event EventHandler<EciEventArgs> MessageReceived;

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            if (this.isRunning)
            {
                return;
            }

            this.isRunning = true;
            this.BeginConnect();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.isRunning = false;
            this.CloseConnection();
        }

        /// <summary>
        /// The send ECI message.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void SendEciMessage(EciMessageBase msg)
        {
            if (this.stream != null)
            {
                EciSerializer.Serialize(this.stream, msg);
            }
            else
            {
                Logger.Warn("The connection is not open");
            }
        }

        private void BeginConnect()
        {
            Logger.Debug("Connecting to {0}:{1}", this.config.IpAddress, this.config.Port);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(IPAddress.Parse(this.config.IpAddress), this.config.Port);
            this.socket.BeginConnect(endPoint, this.Connected, null);
        }

        private void Connected(IAsyncResult ar)
        {
            try
            {
                this.socket.EndConnect(ar);
                this.stream = new NetworkStream(this.socket);
                Logger.Info("Connected to {0}:{1}", this.config.IpAddress, this.config.Port);
                this.stream.BeginRead(this.readBuffer, 0, this.readBuffer.Length, this.Read, null);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't connect to " + this.config.IpAddress + ":" + this.config.Port, ex);
                this.StartReconnectTimer();
            }
        }

        private void Read(IAsyncResult ar)
        {
            try
            {
                var count = this.stream.EndRead(ar);
                this.stream.BeginRead(this.readBuffer, 0, count, this.Read, null);
                if (count > 0)
                {
                    this.HandleData();
                }

                this.stream.BeginRead(this.readBuffer, count, this.readBuffer.Length - count, this.Read, null);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't read", ex);
                this.StartReconnectTimer();
            }
        }

        private void RaiseMessageReceived(EciEventArgs e)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void HandleData()
        {
            this.msgQueue.ProcessData(this.readBuffer);

            try
            {
                EciMessageBase msg = this.msgQueue.Messages.Dequeue();
                this.RaiseMessageReceived(new EciEventArgs(msg));
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Could not process eci message", ex);
            }
        }

        private void StartReconnectTimer()
        {
            this.CloseConnection();

            this.reconnectTimer.Enabled = true;
        }

        private void CloseConnection()
        {
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream = null;
            }

            if (this.socket != null)
            {
                if (this.socket.Connected)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                }

                this.socket.Close();
                this.socket = null;
            }
        }
    }
}