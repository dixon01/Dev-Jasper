// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutingTable.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RoutingTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Routing table used by message dispatcher to know through which
    /// peer messages for a certain address have to be routed.
    /// </summary>
    internal sealed class RoutingTable : IManageableTable, IRoutingTable
    {
        private static readonly RoutingEntry[] EmptyEntries = new RoutingEntry[0];

        private readonly ReadWriteLock locker = new ReadWriteLock();
        private readonly Dictionary<MediAddress, ISessionId> sessionIds = new Dictionary<MediAddress, ISessionId>();
        private readonly Dictionary<ISessionId, List<RoutingEntry>> addresses =
            new Dictionary<ISessionId, List<RoutingEntry>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutingTable"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher instance that owns this routing table.
        /// </param>
        public RoutingTable(IMessageDispatcherImpl messageDispatcher)
        {
            var factory = messageDispatcher.ManagementProviderFactory;
            var parent =
                (IModifiableManagementProvider)factory.LocalRoot.GetDescendant(true, MessageDispatcher.ManagementName);
            var provider = factory.CreateManagementProvider("RoutingTable", parent, this);
            parent.AddChild(provider);
        }

        /// <summary>
        /// Event that is fired every time the routing table changes.
        /// </summary>
        public event EventHandler<RouteUpdatesEventArgs> Updated;

        /// <summary>
        /// Get the session id that has to be used to route
        /// a message to a given address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The session id or null if the address is unknown.
        /// </returns>
        public ISessionId GetSessionId(MediAddress address)
        {
            ISessionId sessionId;
            using (this.locker.AcquireReadLock())
            {
                this.sessionIds.TryGetValue(address, out sessionId);
            }

            return sessionId;
        }

        /// <summary>
        /// Get all entries associated to a given session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// A list of entries. The list can be empty, but never null.
        /// </returns>
        public IEnumerable<RoutingEntry> GetEntriesFor(ISessionId sessionId)
        {
            using (this.locker.AcquireReadLock())
            {
                List<RoutingEntry> addrs;
                if (this.addresses.TryGetValue(sessionId, out addrs))
                {
                    return addrs.ToArray();
                }
            }

            return EmptyEntries;
        }

        /// <summary>
        /// Gets a list of all entries that fulfill the given filter criteria.
        /// </summary>
        /// <param name="filter">
        /// The filter. If the method returns true for a given session id,
        /// all entries belonging to that session are returned.
        /// </param>
        /// <returns>
        /// A list of entries matching the filter.
        /// The returned enumeration is evaluated before this method returns, it is therefore thread-safe.
        /// </returns>
        public IEnumerable<RoutingEntry> GetEntries(Predicate<ISessionId> filter)
        {
            var all = new List<RoutingEntry>();
            using (this.locker.AcquireReadLock())
            {
                foreach (var kvp in this.addresses)
                {
                    if (!filter(kvp.Key))
                    {
                        continue;
                    }

                    all.AddRange(kvp.Value);
                }
            }

            return all;
        }

        /// <summary>
        /// Adds one or more entries to this routing table.
        /// </summary>
        /// <param name="sessionId">
        /// The session id through which the given addresses will be routed.
        /// </param>
        /// <param name="entries">
        /// The addresses.
        /// </param>
        public void Add(ISessionId sessionId, IEnumerable<RoutingEntry> entries)
        {
            var updates = new List<RouteUpdate>();
            foreach (var entry in entries)
            {
                updates.Add(new RouteUpdate { Added = true, Address = entry.Address, Hops = entry.Hops });
            }

            this.Update(sessionId, updates);
        }

        /// <summary>
        /// Removes the entry for the given address from this routing table.
        /// </summary>
        /// <param name="addr">
        /// The address.
        /// </param>
        public void Remove(MediAddress addr)
        {
            var sessionId = this.GetSessionId(addr);
            if (sessionId == null)
            {
                return;
            }

            var updates = new List<RouteUpdate> { new RouteUpdate { Added = false, Address = addr } };
            this.Update(sessionId, updates);
        }

        /// <summary>
        /// Removes all entries related to a given session id from this routing table.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        public void RemoveAll(ISessionId sessionId)
        {
            List<RouteUpdate> updates;

            using (this.locker.AcquireWriteLock())
            {
                List<RoutingEntry> addrs;
                if (!this.addresses.TryGetValue(sessionId, out addrs))
                {
                    return;
                }

                updates =
                    addrs.ConvertAll(
                        entry => new RouteUpdate { Added = false, Address = entry.Address, Hops = entry.Hops });
            }

            this.Update(sessionId, updates);
        }

        /// <summary>
        /// Updates the routing table entries for a given session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <param name="updates">
        /// The updates to perform on this routing table.
        /// </param>
        public void Update(ISessionId sessionId, ICollection<RouteUpdate> updates)
        {
            var allUpdates = new List<RouteUpdate>();
            using (this.locker.AcquireWriteLock())
            {
                foreach (var update in updates)
                {
                    if (update.Added)
                    {
                        this.DoAdd(sessionId, new RoutingEntry(update.Address, update.Hops), allUpdates);
                    }
                    else
                    {
                        this.DoRemove(sessionId, update.Address, allUpdates);
                    }
                }
            }

            this.RaiseUpdated(new RouteUpdatesEventArgs(sessionId, allUpdates));
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var kvp in this.addresses)
            {
                foreach (var entry in kvp.Value)
                {
                    yield return new List<ManagementProperty>
                        {
                            new ManagementProperty<string>("Unit", entry.Address.Unit, true),
                            new ManagementProperty<string>("Application", entry.Address.Application, true),
                            new ManagementProperty<string>("Session ID", kvp.Key.ToString(), true),
                            new ManagementProperty<int>("Hops", entry.Hops, true)
                        };
                }
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        private void DoAdd(ISessionId sessionId, RoutingEntry entry, List<RouteUpdate> allUpdates)
        {
            List<RoutingEntry> addrs;
            ISessionId oldSessionId;
            if (this.sessionIds.TryGetValue(entry.Address, out oldSessionId))
            {
                if (sessionId.Equals(oldSessionId))
                {
                    // re-adding for the same session ID, we don't care
                    return;
                }

                // check the number of hops, only add the entry if it has
                // the same number of hops or less
                if (this.addresses.TryGetValue(oldSessionId, out addrs))
                {
                    var oldEntry = addrs.Find(e => e.Address.Equals(entry.Address));
                    if (oldEntry != null && oldEntry.Hops < entry.Hops)
                    {
                        return;
                    }
                }

                this.DoRemove(oldSessionId, entry.Address, allUpdates);
            }

            this.sessionIds.Add(entry.Address, sessionId);

            if (!this.addresses.TryGetValue(sessionId, out addrs))
            {
                addrs = new List<RoutingEntry>();
                this.addresses.Add(sessionId, addrs);
            }

            addrs.Add(entry);
            allUpdates.Add(new RouteUpdate { Added = true, Address = entry.Address, Hops = entry.Hops });
        }

        private void DoRemove(ISessionId sessionId, MediAddress address, List<RouteUpdate> allUpdates)
        {
            ISessionId oldSessionId;
            if (!this.sessionIds.TryGetValue(address, out oldSessionId) || !sessionId.Equals(oldSessionId))
            {
                return;
            }

            this.sessionIds.Remove(address);

            var addrs = this.addresses[sessionId];
            addrs.Remove(new RoutingEntry(address, 0));
            if (addrs.Count == 0)
            {
                this.addresses.Remove(sessionId);
            }

            allUpdates.Add(new RouteUpdate { Added = false, Address = address });
        }

        private void RaiseUpdated(RouteUpdatesEventArgs e)
        {
            var handler = this.Updated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
