// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteUpdatesEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RouteUpdatesEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// Event arguments containing a list of route updates.
    /// </summary>
    public class RouteUpdatesEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteUpdatesEventArgs"/> class.
        /// </summary>
        /// <param name="sessionId">
        /// The session id
        /// </param>
        /// <param name="updates">
        /// The updates.
        /// </param>
        public RouteUpdatesEventArgs(ISessionId sessionId, IEnumerable<RouteUpdate> updates)
        {
            this.SessionId = sessionId;
            this.Updates = updates;
        }

        /// <summary>
        /// Gets the session id that triggered this update.
        /// </summary>
        public ISessionId SessionId { get; private set; }

        /// <summary>
        /// Gets all updates related to this event.
        /// </summary>
        public IEnumerable<RouteUpdate> Updates { get; private set; }
    }
}