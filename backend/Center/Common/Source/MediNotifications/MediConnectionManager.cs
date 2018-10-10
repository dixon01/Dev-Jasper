// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediConnectionManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediConnectionManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.MediNotifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;

    /// <summary>
    /// Manager for a single connection to a remote Medi server.
    /// This intermediate class exists, so we don't create a connection for every path we are
    /// subscribing, but rather a single connection with multiple subscriptions.
    /// </summary>
    internal class MediConnectionManager
    {
        private readonly TaskCompletionSource<bool> completed = new TaskCompletionSource<bool>();

        private readonly bool isLocal;

        private readonly IRootMessageDispatcher messageDispatcher;

        private readonly List<MediNotificationManager> notificationManagers = new List<MediNotificationManager>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediConnectionManager"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string in the form <c>medi://{hostname}[:{port}]</c>.
        /// </param>
        /// <param name="isLocal">
        /// Flag to set when the connection is local.
        /// </param>
        public MediConnectionManager(string connectionString, bool isLocal)
        {
            var client = new TcpTransportClientConfig();
            var uriBuilder = new UriBuilder(connectionString);
            client.RemoteHost = uriBuilder.Host;
            if (uriBuilder.Port != -1)
            {
                client.RemotePort = uriBuilder.Port;
            }

            var uniqueInstanceName = Guid.NewGuid().ToString();
            var config = new MediConfig { InterceptLocalLogs = false };
            config.Peers.Add(new ClientPeerConfig { Codec = new BecCodecConfig(), Transport = client });
            this.isLocal = isLocal;
            this.messageDispatcher = MessageDispatcher.Create(
                new ObjectConfigurator(config, uniqueInstanceName, "Root"));
            this.messageDispatcher.RoutingTable.Updated += this.RoutingTableOnUpdated;
        }

        /// <summary>
        /// Event that is risen when this connection manager is being disposed of.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Creates a notification manager for the given path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="MediNotificationManager"/>.
        /// </returns>
        public MediNotificationManager CreateNotificationManager(string path)
        {
            var notificationManager = new MediNotificationManager(this.messageDispatcher, path, this.completed.Task);
            notificationManager.Disposing += this.NotificationManagerOnDisposing;
            lock (this.notificationManagers)
            {
                this.notificationManagers.Add(notificationManager);
            }

            return notificationManager;
        }

        private void RoutingTableOnUpdated(object sender, RouteUpdatesEventArgs routeUpdatesEventArgs)
        {
            if ((!this.isLocal && routeUpdatesEventArgs.SessionId.Equals(SessionIds.Local))
                || routeUpdatesEventArgs.Updates.All(update => !update.Added))
            {
                return;
            }

            this.completed.TrySetResult(true);
        }

        private void NotificationManagerOnDisposing(object sender, EventArgs eventArgs)
        {
            bool dispose;
            lock (this.notificationManagers)
            {
                this.notificationManagers.Remove((MediNotificationManager)sender);
                dispose = this.notificationManagers.Count == 0;
            }

            if (!dispose)
            {
                return;
            }

            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            this.notificationManagers.Clear();

            this.messageDispatcher.Dispose();
        }
    }
}