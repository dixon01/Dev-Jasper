// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediPeerBase.SessionRoutingUpdateHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediPeerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// Base class for "normal" Medi peers
    /// </summary>
    internal abstract partial class MediPeerBase
    {
        /// <summary>
        /// Class that handles routing updates for a session.
        /// It sends out an update when the session is created and
        /// another one when the session is disconnected.
        /// It also forwards routing table updates to the connected session.
        /// </summary>
        private class SessionRoutingUpdateHandler
        {
            private readonly MediPeerBase owner;
            private readonly ITransportSession session;

            private readonly RoutingTable routingTable;

            /// <summary>
            /// Initializes a new instance of the <see cref="SessionRoutingUpdateHandler"/> class.
            /// </summary>
            /// <param name="owner">
            /// The peer for which this handler is used.
            /// </param>
            /// <param name="session">
            /// The session that this handler handles.
            /// </param>
            public SessionRoutingUpdateHandler(MediPeerBase owner, ITransportSession session)
            {
                this.owner = owner;
                this.session = session;
                this.routingTable = owner.Dispatcher.RoutingTable;

                if (this.session.LocalGatewayMode != GatewayMode.Server)
                {
                    this.routingTable.Updated += this.RoutingTableUpdated;
                }
            }

            public void Start()
            {
                switch (this.session.LocalGatewayMode)
                {
                    case GatewayMode.Server:
                        return;
                    case GatewayMode.Client:
                        this.owner.Dispatcher.RoutingTable.Add(
                            this.session.SessionId,
                            new[] { new RoutingEntry(MediAddress.Broadcast, 1) });
                        break;
                }

                // send a routing message with our address to the newly connected session
                var updates = new RoutingUpdates { Updates = new List<RouteUpdate>() };
                foreach (var entry in this.routingTable.GetEntries(id => !id.Equals(this.session.SessionId)))
                {
                    updates.Updates.Add(
                        new RouteUpdate { Added = true, Address = entry.Address, Hops = entry.Hops });
                }

                var msg = new MediMessage
                              {
                                  Source = MediAddress.Empty,
                                  Destination = MediAddress.Empty,
                                  Payload = updates
                              };
                this.owner.EnqueueMessage(msg, this.session.SessionId);
            }

            public void Dispose()
            {
                this.routingTable.Updated -= this.RoutingTableUpdated;

                // remove all addresses registered with this session when it is disconnected
                this.routingTable.RemoveAll(this.session.SessionId);
            }

            private void RoutingTableUpdated(object sender, RouteUpdatesEventArgs e)
            {
                if (e.SessionId.Equals(this.session.SessionId))
                {
                    return;
                }

                var updates = new RoutingUpdates { Updates = new List<RouteUpdate>(e.Updates) };
                var msg = new MediMessage
                              {
                                  Source = this.owner.Dispatcher.LocalAddress,
                                  Destination = MediAddress.Broadcast,
                                  Payload = updates
                              };
                this.owner.EnqueueMessage(msg, this.session.SessionId);
            }
        }
    }
}