// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediPeerBase.SessionSubscriptionsHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediPeerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Subscription;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for "normal" Medi peers
    /// </summary>
    internal abstract partial class MediPeerBase
    {
        /// <summary>
        /// Class that handles subscriptions for a certain session.
        /// It sends subscription and un-subscription messages to the
        /// session for types subscribed to locally.
        /// It also handles those messages coming from the session.
        /// </summary>
        private class SessionSubscriptionsHandler : ISubscription, IDisposable
        {
            private static readonly Logger Logger = LogHelper.GetLogger<SessionSubscriptionsHandler>();

            private readonly Dictionary<TypeName, List<ISubscription>> localTypes =
                new Dictionary<TypeName, List<ISubscription>>();

            private readonly List<TypeName> remoteTypes = new List<TypeName>();

            private readonly MediPeerBase owner;
            private readonly ITransportSession session;

            public SessionSubscriptionsHandler(
                MediPeerBase owner, ITransportSession session, IEnumerable<TypeName> additionalTypes)
            {
                this.owner = owner;
                this.session = session;

                if (session.LocalGatewayMode != GatewayMode.None)
                {
                    Logger.Debug(
                        "Not handling subscriptions for {0} since it is in gateway mode {1}",
                        this.session.SessionId,
                        session.LocalGatewayMode);
                    return;
                }

                lock (this.localTypes)
                {
                    foreach (var type in additionalTypes)
                    {
                        this.localTypes.Add(type, new List<ISubscription> { this });
                    }

                    foreach (var subscription in this.owner.Dispatcher.Subscriptions)
                    {
                        this.AddLocalTypes(subscription, subscription.Types, false);
                        subscription.TypesChanged += this.SubscriptionOnTypesChanged;
                    }

                    this.owner.Dispatcher.SubscriptionAdded += this.DispatcherOnSubscriptionAdded;
                    this.owner.Dispatcher.SubscriptionRemoved += this.DispatcherOnSubscriptionRemoved;
                }
            }

            public event EventHandler<SubscribedTypesEventArgs> TypesChanged;

            TypeName[] ISubscription.Types
            {
                get
                {
                    return this.remoteTypes.ToArray();
                }
            }

            public void Start()
            {
                if (this.session.LocalGatewayMode == GatewayMode.None)
                {
                    // send existing subscriptions
                    List<string> messageTypes;
                    lock (this.localTypes)
                    {
                        messageTypes = new List<string>(this.localTypes.Count);
                        foreach (var localType in this.localTypes.Keys)
                        {
                            messageTypes.Add(localType.FullName);
                        }
                    }

                    this.SendSubscriptionMessage<AddSubscriptions>(messageTypes);
                }

                this.owner.Dispatcher.AddSubscription(this);
            }

            public void HandleSubscriptionMessage(ISubscriptionMessage message)
            {
                if (message is AddSubscriptions)
                {
                    var added = new List<TypeName>(message.Types.Length);
                    foreach (var type in message.Types)
                    {
                        var typeName = new TypeName(type);
                        if (!this.remoteTypes.Contains(typeName))
                        {
                            added.Add(typeName);
                            this.remoteTypes.Add(typeName);
                            Logger.Trace("Added remote subscription to {0} for {1}", this.session.SessionId, typeName);
                        }
                    }

                    if (added.Count > 0)
                    {
                        this.RaiseTypesChanged(new SubscribedTypesEventArgs(true, added.ToArray()));
                    }

                    return;
                }

                if (!(message is RemoveSubscriptions))
                {
                    return;
                }

                var removed = new List<TypeName>(message.Types.Length);
                foreach (var type in message.Types)
                {
                    var typeName = new TypeName(type);
                    if (this.remoteTypes.Remove(typeName))
                    {
                        removed.Add(typeName);
                        Logger.Trace("Removed remote subscription from {0} for {1}", this.session.SessionId, typeName);
                    }
                }

                if (removed.Count > 0)
                {
                    this.RaiseTypesChanged(new SubscribedTypesEventArgs(false, removed.ToArray()));
                }
            }

            public void Dispose()
            {
                this.owner.Dispatcher.RemoveSubscription(this);
                this.owner.Dispatcher.SubscriptionAdded -= this.DispatcherOnSubscriptionAdded;
                this.owner.Dispatcher.SubscriptionRemoved -= this.DispatcherOnSubscriptionRemoved;

                lock (this.localTypes)
                {
                    foreach (var subscriptions in this.localTypes.Values)
                    {
                        foreach (var subscription in subscriptions)
                        {
                            subscription.TypesChanged -= this.SubscriptionOnTypesChanged;
                        }
                    }

                    this.localTypes.Clear();
                }
            }

            public override bool Equals(object obj)
            {
                var other = obj as SessionSubscriptionsHandler;
                return other != null && this.session.SessionId.Equals(other.session.SessionId);
            }

            public override int GetHashCode()
            {
                return this.session.SessionId.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("RemoteSubscription<{0}>", this.session.SessionId);
            }

            bool ISubscription.CanHandle(MediAddress address, TypeName type)
            {
                var hasWildcard = address.Unit == MediAddress.Wildcard || address.Application == MediAddress.Wildcard;
                if (this.session.LocalGatewayMode != GatewayMode.None && hasWildcard)
                {
                    // in gateway mode we don't forward broadcasts and multicasts
                    return false;
                }

                var match = this.remoteTypes.Contains(type)
                                ? address.Matches
                                : new Predicate<MediAddress>(address.Equals);
                var foundGateway = false;
                foreach (var entry in this.owner.Dispatcher.RoutingTable.GetEntriesFor(this.session.SessionId))
                {
                    if (match(entry.Address))
                    {
                        return true;
                    }

                    if (!hasWildcard && entry.Address.Equals(MediAddress.Broadcast))
                    {
                        foundGateway = true;
                    }
                }

                // we found a gateway and nobody else is responsible for the address, let's send it through this session
                return foundGateway && this.owner.Dispatcher.RoutingTable.GetSessionId(address) == null;
            }

            void ISubscription.Handle(
                IMessageDispatcherImpl medi,
                ISessionId sourceSessionId,
                MediAddress source,
                MediAddress destination,
                object message)
            {
                if (this.session.SessionId.Equals(sourceSessionId))
                {
                    // don't send messages back on the session they arrived (happens for broadcasts)
                    return;
                }

                this.owner.SendSubscribedMessage(this.session, source, destination, message);
            }

            private void AddLocalTypes(ISubscription subscription, IEnumerable<TypeName> types, bool sendMessage)
            {
                var added = new List<string>();
                lock (this.localTypes)
                {
                    foreach (var messageType in types)
                    {
                        List<ISubscription> subscriptions;
                        if (!this.localTypes.TryGetValue(messageType, out subscriptions))
                        {
                            added.Add(messageType.FullName);
                            subscriptions = new List<ISubscription>();
                            this.localTypes.Add(messageType, subscriptions);
                        }

                        subscriptions.Add(subscription);
                        Logger.Trace(
                            "Added local subscription to {0} for {1} [{2}]",
                            this.session.SessionId,
                            messageType,
                            subscriptions.Count);
                    }
                }

                if (sendMessage && added.Count > 0)
                {
                    this.SendSubscriptionMessage<AddSubscriptions>(added);
                }
            }

            private void RemoveLocalTypes(ISubscription subscription, IEnumerable<TypeName> types)
            {
                var removed = new List<string>();
                lock (this.localTypes)
                {
                    foreach (var messageType in types)
                    {
                        List<ISubscription> subscriptions;
                        if (!this.localTypes.TryGetValue(messageType, out subscriptions))
                        {
                            Logger.Trace(
                                "Couldn't remove local subscription from {0} for {1}",
                                this.session.SessionId,
                                messageType);
                            continue;
                        }

                        if (subscriptions.Remove(subscription) && subscriptions.Count == 0)
                        {
                            removed.Add(messageType.FullName);
                            this.localTypes.Remove(messageType);
                        }

                        Logger.Trace(
                            "Removed local subscription from {0} for {1} [{2}]",
                            this.session.SessionId,
                            messageType,
                            subscriptions.Count);
                    }
                }

                if (removed.Count > 0)
                {
                    this.SendSubscriptionMessage<RemoveSubscriptions>(removed);
                }
            }

            private void DispatcherOnSubscriptionAdded(object sender, SubscriptionEventArgs e)
            {
                if (object.ReferenceEquals(e.Subscription, this))
                {
                    return;
                }

                this.AddLocalTypes(e.Subscription, e.Subscription.Types, true);

                e.Subscription.TypesChanged += this.SubscriptionOnTypesChanged;
            }

            private void DispatcherOnSubscriptionRemoved(object sender, SubscriptionEventArgs e)
            {
                if (object.ReferenceEquals(e.Subscription, this))
                {
                    return;
                }

                this.RemoveLocalTypes(e.Subscription, e.Subscription.Types);

                e.Subscription.TypesChanged -= this.SubscriptionOnTypesChanged;
            }

            private void SubscriptionOnTypesChanged(object sender, SubscribedTypesEventArgs e)
            {
                var subscription = sender as ISubscription;
                if (subscription == null)
                {
                    return;
                }

                if (e.Added)
                {
                    this.AddLocalTypes(subscription, e.Types, true);
                    return;
                }

                this.RemoveLocalTypes(subscription, e.Types);
            }

            /// <summary>
            /// Sends a subscription message for a given list of message types.
            /// </summary>
            /// <typeparam name="T">The type of the subscription message.</typeparam>
            /// <param name="messageTypes">a list of types to (un)subscribe to.</param>
            private void SendSubscriptionMessage<T>(List<string> messageTypes)
                where T : ISubscriptionMessage, new()
            {
                if (messageTypes.Count == 0)
                {
                    return;
                }

                this.owner.EnqueueMessage(
                    new MediMessage
                    {
                        Source = this.owner.Dispatcher.LocalAddress,
                        Destination = MediAddress.Broadcast,
                        Payload = new T { Types = messageTypes.ToArray() }
                    },
                    this.session.SessionId);
            }

            private void RaiseTypesChanged(SubscribedTypesEventArgs e)
            {
                var handler = this.TypesChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
    }
}