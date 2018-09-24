// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageDispatcherImpl.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageDispatcherImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Subscription;

    /// <summary>
    /// Interface implemented by real message dispatchers.
    /// This is used to be able to mock the message dispatcher during testing.
    /// </summary>
    internal interface IMessageDispatcherImpl : IRootMessageDispatcher
    {
        /// <summary>
        /// Event that is fired whenever somebody subscribes to a certain message type.
        /// </summary>
        event EventHandler<SubscriptionEventArgs> SubscriptionAdded;

        /// <summary>
        /// Event that is fired whenever somebody unsubscribes from a certain message type.
        /// </summary>
        event EventHandler<SubscriptionEventArgs> SubscriptionRemoved;

        /// <summary>
        /// Gets a list of subscriptions currently in this message dispatcher.
        /// </summary>
        IEnumerable<ISubscription> Subscriptions { get; }

        /// <summary>
        /// Gets the routing table.
        /// </summary>
        new RoutingTable RoutingTable { get; }

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <param name="subscription">
        /// The subscription to add.
        /// </param>
        void AddSubscription(ISubscription subscription);

        /// <summary>
        /// Unsubscribe from a certain event to the given address
        /// </summary>
        /// <param name="subscription">
        /// The subscription to remove.
        /// </param>
        /// <returns>
        /// True if a subscription was found and removed.
        /// </returns>
        bool RemoveSubscription(ISubscription subscription);

        /// <summary>
        /// Send a message to a given subscriber.
        /// </summary>
        /// <param name="source">
        /// The source address.
        /// </param>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        /// <param name="sourceSessionId">
        /// The session id where the message comes from.
        /// This can be null which means the message was received locally
        /// (this is the same as using <see cref="SessionIds.Local"/>).
        /// </param>
        /// <returns>
        /// True if at least one subscription was successfully notified.
        /// </returns>
        bool Send(MediAddress source, MediAddress destination, object message, ISessionId sourceSessionId);
    }
}