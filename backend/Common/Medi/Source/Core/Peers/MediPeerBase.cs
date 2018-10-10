// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediPeerBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
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
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for "normal" Medi peers
    /// </summary>
    internal abstract partial class MediPeerBase : IPeerImpl
    {
        /// <summary>
        /// Management name for "Peers" node.
        /// </summary>
        public static readonly string PeersManagementName = "Peers";

        /// <summary>
        /// Logger for this class and subclasses.
        /// </summary>
        protected readonly Logger Logger;

        private const int SendQueueCapacity = 1000;

        private readonly ProducerConsumerQueue<SendItem> sendQueue;

        private readonly Dictionary<ISessionId, SessionSubscriptionsHandler> subscriptionsHandlers =
            new Dictionary<ISessionId, SessionSubscriptionsHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediPeerBase"/> class.
        /// </summary>
        protected MediPeerBase()
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.sendQueue = new ProducerConsumerQueue<SendItem>(
                item => this.SendMessage(item.Message, item.DestinationSessionId), SendQueueCapacity);
        }

        /// <summary>
        /// Gets the message dispatcher.
        /// </summary>
        protected IMessageDispatcherImpl Dispatcher { get; private set; }

        /// <summary>
        /// Starts this peer by registering to all necessary events
        /// and starting the transport (which in the case of a client
        /// transport will connect to the server).
        /// </summary>
        /// <param name="medi">
        /// The message dispatcher to be used by this peer.
        /// </param>
        public virtual void Start(IMessageDispatcherImpl medi)
        {
            this.Dispatcher = medi;
            this.sendQueue.StartConsumer();
        }

        /// <summary>
        /// Stops this peer by deregistering from all necessary events and
        /// stopping the transport.
        /// </summary>
        public virtual void Stop()
        {
            this.sendQueue.StopConsumer();
        }

        /// <summary>
        /// Registers a new session; this method has to be called by subclasses whenever
        /// a new transport session has been set up.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        protected void RegisterSession(ITransportSession session)
        {
            var additionalMessages = new List<TypeName>(3) { TypeName.Of<Ping>() };
            if (this.Dispatcher.GetService<IResourceServiceImpl>() != null)
            {
                additionalMessages.Add(TypeName.Of<StreamMessage>());
                additionalMessages.Add(TypeName.Of<ResourceAnnouncement>());
            }

            // create a routing and a subscriptions handler for this session
            var routingHandler = new SessionRoutingUpdateHandler(this, session);
            var subscriptionHandler = new SessionSubscriptionsHandler(this, session, additionalMessages);
            this.subscriptionsHandlers[session.SessionId] = subscriptionHandler;

            session.Disconnected += (s, e) =>
            {
                routingHandler.Dispose();
                subscriptionHandler.Dispose();
                this.subscriptionsHandlers.Remove(session.SessionId);
            };
            routingHandler.Start();
            subscriptionHandler.Start();
        }

        /// <summary>
        /// Adds a message to the internal queue which will then be handled on a separate thread.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given message must be sent.
        /// </param>
        protected virtual void EnqueueMessage(IMessage message, ISessionId destinationSessionId)
        {
            if (!this.sendQueue.Enqueue(new SendItem(message, destinationSessionId)))
            {
                this.Logger.Error("Send queue is full, we are losing messages! Lost message: {0}", message);
            }
        }

        /// <summary>
        /// Do not use this method to send a message, use
        /// <see cref="EnqueueMessage"/> instead!
        /// Implementations will have to do the actual sending in this method.
        /// This is called when a message was de-queued from the internal queue.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="destinationSessionId">
        /// The id of the session to which the given message must be written.
        /// </param>
        protected abstract void SendMessage(IMessage message, ISessionId destinationSessionId);

        /// <summary>
        /// Posts a message to the message dispatcher.
        /// This method also handles internal messages.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        protected void PostMessageToMedi(MediMessage message, ITransportSession from)
        {
            var payload = message.Payload;

            var subscr = payload as ISubscriptionMessage;
            if (subscr != null)
            {
                if (!this.HandleSubscriptionMessage(subscr, from))
                {
                    return;
                }
            }

            var netMessage = payload as INetworkMessage;
            if (netMessage != null)
            {
                if (!this.HandleNetworkMessage(netMessage, from, message.Source, message.Destination))
                {
                    return;
                }
            }

            var resourceMessage = payload as IResourceMessage;
            if (resourceMessage != null)
            {
                if (!this.HandleResourceMessage(message.Source, message.Destination, resourceMessage))
                {
                    return;
                }
            }

            if (message.Destination.Equals(MediAddress.Empty))
            {
                // ignore non-addressed messages
                // (they are only used to exchange information between directly connected peers)
                return;
            }

            this.Dispatcher.Send(message.Source, message.Destination, message.Payload, from.SessionId);
        }

        private bool HandleResourceMessage(
            MediAddress source, MediAddress destination, IResourceMessage resourceMessage)
        {
            var resourceService = this.Dispatcher.GetService<IResourceServiceImpl>();
            if (resourceService == null)
            {
                this.Logger.Error("Got resource message, but no resource service configured; verify your medi.config");
                return true;
            }

            var resourceAnnouncement = resourceMessage as ResourceAnnouncement;
            if (resourceAnnouncement != null)
            {
                resourceService.AnnounceResource(source, destination, resourceAnnouncement);
            }

            return true;
        }

        /// <summary>
        /// Handles AddSubscriptions and RemoveSubscriptions messages.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="session">the session from which the message arrived.</param>
        /// <returns>true if the message has to be treated (e.g. forwarded) like a normal message.</returns>
        private bool HandleSubscriptionMessage(ISubscriptionMessage message, ITransportSession session)
        {
            SessionSubscriptionsHandler handler;
            if (this.subscriptionsHandlers.TryGetValue(session.SessionId, out handler))
            {
                handler.HandleSubscriptionMessage(message);
            }

            return false;
        }

        /// <summary>
        /// Handles network messages (routing updates, pings).
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="session">the session from which the message arrived.</param>
        /// <param name="source">the sender of the message.</param>
        /// <param name="destination">the destination of the message.</param>
        /// <returns>true if the message has to be treated (e.g. forwarded) like a normal message.</returns>
        private bool HandleNetworkMessage(
            INetworkMessage message, ITransportSession session, MediAddress source, MediAddress destination)
        {
            var routing = message as RoutingUpdates;
            if (routing != null)
            {
                if (routing.Updates.Count == 0)
                {
                    return false;
                }

                // increase the hops by one since we received the message from another node
                var updates = new List<RouteUpdate>();
                foreach (var update in routing.Updates)
                {
                    updates.Add(
                        new RouteUpdate { Added = update.Added, Address = update.Address, Hops = update.Hops + 1 });
                }

                this.Dispatcher.RoutingTable.Update(session.SessionId, updates);
                return false;
            }

            var ping = message as Ping;
            if (ping != null && destination.Matches(this.Dispatcher.LocalAddress))
            {
                var pong = new Pong { RequestTimestamp = ping.Timestamp };
                var msg = new MediMessage
                {
                    Source = this.Dispatcher.LocalAddress,
                    Destination = source,
                    Payload = pong
                };
                this.EnqueueMessage(msg, session.SessionId);
            }

            return true;
        }

        private void SendSubscribedMessage(
            ITransportSession session, MediAddress source, MediAddress destination, object message)
        {
            var msg = message as IMessage
                      ?? new MediMessage { Source = source, Destination = destination, Payload = message };

            this.EnqueueMessage(msg, session.SessionId);
        }

        private class SendItem
        {
            public SendItem(IMessage message, ISessionId destinationSessionId)
            {
                this.Message = message;
                this.DestinationSessionId = destinationSessionId;
            }

            public IMessage Message { get; private set; }

            public ISessionId DestinationSessionId { get; private set; }

            public override string ToString()
            {
                return "[SendItem: " + this.Message + "]";
            }
        }
    }
}