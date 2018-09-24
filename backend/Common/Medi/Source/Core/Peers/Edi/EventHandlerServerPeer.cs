// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerServerPeer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerServerPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Edi
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The <see cref="EventHandlerServerPeer"/> provides a TCP
    /// socket for the exchange of XML serialized objects.
    /// This is a legacy interface to support applications
    /// that still require the old EventHandler server interface.
    /// </summary>
    internal class EventHandlerServerPeer : EventHandlerPeerBase,
                                            IConfigurable<EventHandlerServerPeerConfig>,
                                            IConfigurable<EventHandlerPeerConfig>,
                                            IManageable
    {
        private readonly List<ConnectionHandler> connectionHandlers = new List<ConnectionHandler>();

        private readonly ReadWriteLock locker = new ReadWriteLock();

        private EventHandlerServerPeerConfig config;

        private TcpStreamServer server;

        private IManagementProvider managementProvider;

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="peerConfig">
        /// The config object.
        /// </param>
        public void Configure(EventHandlerServerPeerConfig peerConfig)
        {
            this.config = peerConfig;
            base.Configure(peerConfig);
        }

        void IConfigurable<EventHandlerPeerConfig>.Configure(EventHandlerPeerConfig peerConfig)
        {
            this.Configure(peerConfig);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            for (int i = 0; i < this.connectionHandlers.Count; i++)
            {
                yield return parent.Factory.CreateManagementProvider("Client" + i, parent, this.connectionHandlers[i]);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="EventHandlerPeerBase.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            var factory = this.MessageDispatcher.ManagementProviderFactory;
            var mgmtParent =
                (IModifiableManagementProvider)
                factory.LocalRoot.GetDescendant(
                    true, Core.MessageDispatcher.ManagementName, MediPeerBase.PeersManagementName);

            this.managementProvider =
                factory.CreateManagementProvider(
                    factory.CreateUniqueName(mgmtParent, this.GetType().Name), mgmtParent, this);
            mgmtParent.AddChild(this.managementProvider);

            this.server = new TcpStreamServer(this.config.LocalPort);
            this.server.BeginAccept(this.Accepted, null);

            Logger.Info("Server started on port {0}", this.config.LocalPort);
        }

        /// <summary>
        /// Implementation of the <see cref="EventHandlerPeerBase.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            if (this.server == null)
            {
                return;
            }

            ConnectionHandler[] connectionHandlerList;
            using (this.locker.AcquireReadLock())
            {
                connectionHandlerList = this.connectionHandlers.ToArray();
            }

            foreach (var client in connectionHandlerList)
            {
                client.Failed -= this.ClientOnFailed;
                client.Stop();
            }

            using (this.locker.AcquireWriteLock())
            {
                this.connectionHandlers.Clear();
            }

            this.server.Dispose();
            this.server = null;

            if (this.managementProvider != null)
            {
                this.managementProvider.Dispose();
                this.managementProvider = null;
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
            ConnectionHandler[] connectionHandlerList;
            using (this.locker.AcquireReadLock())
            {
                connectionHandlerList = this.connectionHandlers.ToArray();
            }

            foreach (var client in connectionHandlerList)
            {
                client.EnqueueWrite(message, data, offset, length);
            }
        }

        /// <summary>
        /// Gets an identifier for this peer.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetIdentifier()
        {
            return this.config.LocalPort.ToString(CultureInfo.InvariantCulture);
        }

        private void Accepted(IAsyncResult result)
        {
            if (this.server == null)
            {
                // we were stopped
                return;
            }

            var connection = this.server.EndAccept(result);
            this.server.BeginAccept(this.Accepted, null);
            var connectionHandler = new ConnectionHandler(connection, this);
            connectionHandler.Failed += this.ClientOnFailed;
            using (this.locker.AcquireWriteLock())
            {
                this.connectionHandlers.Add(connectionHandler);
            }

            connectionHandler.Start();
        }

        private void ClientOnFailed(object sender, EventArgs eventArgs)
        {
            using (this.locker.AcquireWriteLock())
            {
                this.connectionHandlers.Remove((ConnectionHandler)sender);
            }
        }
    }
}
