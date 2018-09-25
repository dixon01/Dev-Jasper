// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerClientPeer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerClientPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Edi
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The <see cref="EventHandlerClientPeer"/> uses a TCP
    /// socket to the exchange of XML serialized objects.
    /// This is a legacy interface to support scenarios
    /// that still use the old EventHandler server (EdiServer).
    /// </summary>
    internal class EventHandlerClientPeer :
        EventHandlerPeerBase, IConfigurable<EventHandlerClientPeerConfig>, IManageableObject
    {
        private readonly ITimer reconnectTimer;

        private EventHandlerClientPeerConfig config;

        private TcpStreamClient tcpClient;

        private ConnectionHandler connectionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerClientPeer"/> class.
        /// </summary>
        public EventHandlerClientPeer()
        {
            this.reconnectTimer = TimerFactory.Current.CreateTimer("EventHandlerClient-Reconnect");
            this.reconnectTimer.AutoReset = false;
            this.reconnectTimer.Interval = TimeSpan.FromSeconds(10);
            this.reconnectTimer.Elapsed += this.ReconnectTimerOnElapsed;
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="peerConfig">
        /// The config object.
        /// </param>
        public void Configure(EventHandlerClientPeerConfig peerConfig)
        {
            this.config = peerConfig;
            base.Configure(peerConfig);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Connected", this.connectionHandler != null, true);
        }

        /// <summary>
        /// Implementation of the <see cref="EventHandlerPeerBase.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            this.BeginConnect();
        }

        /// <summary>
        /// Implementation of the <see cref="EventHandlerPeerBase.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            this.reconnectTimer.Enabled = false;

            if (this.connectionHandler != null)
            {
                this.connectionHandler.Stop();
                this.connectionHandler = null;
            }

            if (this.tcpClient != null)
            {
                this.tcpClient.Dispose();
                this.tcpClient = null;
            }
        }

        /// <summary>
        /// Sends a message to the connected peer.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        protected override void SendMessage(object message, byte[] data, int offset, int length)
        {
            var cli = this.connectionHandler;
            if (cli == null)
            {
                return;
            }

            cli.EnqueueWrite(message, data, offset, length);
        }

        /// <summary>
        /// Gets an identifier for this peer.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetIdentifier()
        {
            return string.Format("{0}:{1}", this.config.RemoteHost, this.config.RemotePort);
        }

        private void BeginConnect()
        {
            this.tcpClient = new TcpStreamClient(this.config.RemoteHost, this.config.RemotePort);
            this.tcpClient.BeginConnect(this.TcpClientConnected, null);
        }

        private void TcpClientConnected(IAsyncResult ar)
        {
            IStreamConnection connection;
            try
            {
                connection = this.tcpClient.EndConnect(ar);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't connect to " + this.tcpClient);
                this.tcpClient.Dispose();
                this.tcpClient = null;
                this.reconnectTimer.Enabled = true;
                return;
            }

            this.connectionHandler = new ConnectionHandler(connection, this);
            this.connectionHandler.Failed += this.ConnectionHandlerOnFailed;
            this.connectionHandler.Start();
        }

        private void ConnectionHandlerOnFailed(object sender, EventArgs e)
        {
            this.reconnectTimer.Enabled = true;
        }

        private void ReconnectTimerOnElapsed(object sender, EventArgs e)
        {
            this.BeginConnect();
        }
    }
}