// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoutingTable.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRoutingTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to access the contents of the Medi routing table and register to notifications about changes.
    /// </summary>
    public interface IRoutingTable
    {
        /// <summary>
        /// Event that is fired every time the routing table changes.
        /// </summary>
        event EventHandler<RouteUpdatesEventArgs> Updated;

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
        ISessionId GetSessionId(MediAddress address);

        /// <summary>
        /// Get all entries associated to a given session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// A list of entries. The list can be empty, but never null.
        /// </returns>
        IEnumerable<RoutingEntry> GetEntriesFor(ISessionId sessionId);

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
        IEnumerable<RoutingEntry> GetEntries(Predicate<ISessionId> filter);
    }
}