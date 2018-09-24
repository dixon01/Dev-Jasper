// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The message dispatcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Logging;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Medi.Core.Subscription;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Factory;

    using NLog;

    /// <summary>
    /// The message dispatcher.
    /// To have a fully working MessageDispatcher,
    /// it should be configured using the Configure() method.
    /// </summary>
    public class MessageDispatcher : IMessageDispatcherImpl
    {
        /// <summary>
        /// Management name of this class.
        /// </summary>
        internal static readonly string ManagementName = "MessageDispatcher";

        private static readonly Logger Logger = LogHelper.GetLogger<MessageDispatcher>();

        private static volatile MessageDispatcher instance;

        private readonly Dictionary<ISubscription, SubscriptionCounter> subscriptions;
        private readonly ReadWriteLock subscriptionsLock = new ReadWriteLock();

        private readonly List<IPeerImpl> peers;
        private readonly List<IServiceImpl> services;
        private readonly Dictionary<MediAddress, IMessageDispatcher> namedDispatchers;

        private readonly ManagementProviderFactory managementProviderFactory;

        private readonly RoutingTable routingTable;

        private EventHandler<SubscriptionEventArgs> subscriptionAdded;

        private EventHandler<SubscriptionEventArgs> subscriptionRemoved;

        private MessageDispatcher()
        {
            this.subscriptions = new Dictionary<ISubscription, SubscriptionCounter>();
            this.peers = new List<IPeerImpl>();
            this.services = new List<IServiceImpl>();
            this.namedDispatchers = new Dictionary<MediAddress, IMessageDispatcher>();

            this.managementProviderFactory = new ManagementProviderFactory(this);
            this.managementProviderFactory.CreateManagementProvider(
                ManagementName, this.managementProviderFactory.LocalRoot, null);

            this.routingTable = new RoutingTable(this);
        }

        /// <summary>
        /// Event that is fired whenever somebody subscribes to a certain message type.
        /// </summary>
        event EventHandler<SubscriptionEventArgs> IMessageDispatcherImpl.SubscriptionAdded
        {
            add
            {
                this.subscriptionAdded += value;
            }

            remove
            {
                this.subscriptionAdded -= value;
            }
        }

        /// <summary>
        /// Event that is fired whenever somebody unsubscribes from a certain message type.
        /// </summary>
        event EventHandler<SubscriptionEventArgs> IMessageDispatcherImpl.SubscriptionRemoved
        {
            add
            {
                this.subscriptionRemoved += value;
            }

            remove
            {
                this.subscriptionRemoved -= value;
            }
        }

        /// <summary>
        /// Gets the only instance of this class.
        /// </summary>
        public static MessageDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(MessageDispatcher))
                    {
                        if (instance == null)
                        {
                            instance = new MessageDispatcher();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the local address of this MEDI node.
        /// </summary>
        public MediAddress LocalAddress { get; private set; }

        /// <summary>
        /// Gets the log observer factory.
        /// </summary>
        public ILogObserverFactory LogObserverFactory { get; private set; }

        /// <summary>
        /// Gets the routing table.
        /// </summary>
        public IRoutingTable RoutingTable
        {
            get
            {
                return this.routingTable;
            }
        }

        /// <summary>
        /// Gets the management provider factory.
        /// </summary>
        public IManagementProviderFactory ManagementProviderFactory
        {
            get
            {
                return this.managementProviderFactory;
            }
        }

        /// <summary>
        /// Gets a list of types that are currently subscribed for.
        /// </summary>
        IEnumerable<ISubscription> IMessageDispatcherImpl.Subscriptions
        {
            get
            {
                using (this.subscriptionsLock.AcquireReadLock())
                {
                    return new List<ISubscription>(this.subscriptions.Keys);
                }
            }
        }

        /// <summary>
        /// Gets a list of types that are currently subscribed for.
        /// </summary>
        RoutingTable IMessageDispatcherImpl.RoutingTable
        {
            get
            {
                return this.routingTable;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MessageDispatcher"/> and configures it.
        /// This method should only be used when an application requires an independent
        /// <see cref="MessageDispatcher"/> (e.g. for separating different connections).
        /// Otherwise always use <see cref="Instance"/> and <see cref="Configure"/>
        /// instead.
        /// </summary>
        /// <param name="configurator">
        /// The configurator used to configure the returned object.
        /// </param>
        /// <returns>
        /// The newly created and configured <see cref="IMessageDispatcher"/>.
        /// </returns>
        public static IRootMessageDispatcher Create(IConfigurator configurator)
        {
            var dispatcher = new MessageDispatcher();
            dispatcher.Configure(configurator);
            return dispatcher;
        }

        /// <summary>
        /// Configure this event dispatcher with a given configuration.
        /// This will stop all running servers and clients and create new ones
        /// according to the configuration.
        /// </summary>
        /// <param name="configurator">
        /// The configurator to be used to configure this event dispatcher.
        /// </param>
        public void Configure(IConfigurator configurator)
        {
            var oldAddress = this.LocalAddress;

            this.LocalAddress = configurator.CreateLocalAddress();
            this.Configure(configurator.CreateConfig());

            if (this.LocalAddress.Equals(oldAddress))
            {
                return;
            }

            // update the named dispatchers and the routing table
            var routingUpdates = new List<RouteUpdate>();
            if (oldAddress != null)
            {
                this.namedDispatchers.Remove(oldAddress);
                routingUpdates.Add(new RouteUpdate { Added = false, Address = oldAddress, Hops = 0 });
            }

            this.namedDispatchers.Add(this.LocalAddress, this);
            routingUpdates.Add(new RouteUpdate { Added = true, Address = this.LocalAddress, Hops = 0 });
            this.routingTable.Update(SessionIds.Local, routingUpdates);
        }

        /// <summary>
        /// Broadcasts a message to all interested subscribers.
        /// </summary>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        public void Broadcast(object message)
        {
            if (message == null)
            {
                var stackTrace = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod().Name;
                throw new ArgumentNullException(nameof(message), string.Format("Medi Broadcast Null argument in {0}", callingMethod));
            }

            this.Send(MediAddress.Broadcast, message);
        }

        /// <summary>
        /// Send a message to a given subscriber.
        /// </summary>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        public void Send(MediAddress destination, object message)
        {
            if (message == null)
            {
                var stackTrace = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod().Name;
                Logger.Error("Medi Send Null argument in {0}", callingMethod);
                throw new ArgumentNullException(nameof(message), string.Format("Medi Send Null argument in {0}", callingMethod));
            }
           
            ((IMessageDispatcherImpl)this).Send(this.LocalAddress, destination, message, null);
        }

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <param name="handler">
        /// The handler which will be called for the message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the message to subscribe to.
        /// </typeparam>
        public void Subscribe<T>(EventHandler<MessageEventArgs<T>> handler)
            where T : class
        {
            this.CheckLocalAddress();

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            ((IMessageDispatcherImpl)this).AddSubscription(new Subscription<T>(this.LocalAddress, handler));
        }

        /// <summary>
        /// Unsubscribe from a certain event to the given address
        /// </summary>
        /// <param name="handler">
        /// The handler to unsubscribe.
        /// </param>
        /// <typeparam name="T">
        /// The type of the message to unsubscribe from.
        /// </typeparam>
        /// <returns>
        /// True if a subscription was found and removed.
        /// </returns>
        public bool Unsubscribe<T>(EventHandler<MessageEventArgs<T>> handler)
            where T : class
        {
            this.CheckLocalAddress();

            return ((IMessageDispatcherImpl)this).RemoveSubscription(new Subscription<T>(this.LocalAddress, handler));
        }

        /// <summary>
        /// Gets a named dispatcher that allows to use a different
        /// application name to get messages.
        /// </summary>
        /// <param name="appName">
        /// The application name for which you want to receive messages.
        /// The unit name of the returned <see cref="IMessageDispatcher"/>
        /// will be the same as the current unit name of this
        /// <see cref="MessageDispatcher"/>.
        /// </param>
        /// <returns>
        /// a new or previously created <see cref="IMessageDispatcher"/>.
        /// If you don't use the dispatcher any more, you can dispose it
        /// by calling <see cref="IDisposable.Dispose"/> on it.
        /// </returns>
        public IMessageDispatcher GetNamedDispatcher(string appName)
        {
            this.CheckLocalAddress();

            return this.GetNamedDispatcher(new MediAddress(this.LocalAddress.Unit, appName));
        }

        /// <summary>
        /// Gets a named dispatcher that allows to use a different
        /// address to get messages.
        /// </summary>
        /// <param name="address">
        /// The address for which you want to receive messages.
        /// </param>
        /// <returns>
        /// a new or previously created <see cref="IMessageDispatcher"/>.
        /// If you don't use the dispatcher any more, you can dispose it
        /// by calling <see cref="IDisposable.Dispose"/> on it.
        /// </returns>
        public IMessageDispatcher GetNamedDispatcher(MediAddress address)
        {
            lock (this.namedDispatchers)
            {
                IMessageDispatcher dispatcher;
                if (this.namedDispatchers.TryGetValue(address, out dispatcher))
                {
                    return dispatcher;
                }

                var namedDispatcher = new NamedDispatcher(this, address);
                namedDispatcher.Disposing +=
                    (sender, args) => this.namedDispatchers.Remove(namedDispatcher.LocalAddress);
                this.namedDispatchers.Add(address, namedDispatcher);
                return namedDispatcher;
            }
        }

        /// <summary>
        /// Gets a service implementation for a given type of service.
        /// </summary>
        /// <typeparam name="T">
        /// The type of service requested.
        /// </typeparam>
        /// <returns>
        /// The service implementation or null if no such service is found.
        /// </returns>
        public T GetService<T>() where T : class, IService
        {
            foreach (var service in this.services)
            {
                var result = service as T;
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Dynamically creates, adds and starts a peer in this message dispatcher.
        /// This should be done only in rare cases when a peer is not yet
        /// known when the application starts (i.e. it can't be configured from the config file).
        /// </summary>
        /// <remarks>
        /// It is important to note that <see cref="Configure"/> needs to be called before
        /// calling this method, even if no peers are defined in the <see cref="IConfigurator.CreateConfig"/>.
        /// </remarks>
        /// <param name="config">
        /// The peer configuration object for which a new peer will be created.
        /// </param>
        /// <returns>
        /// The <see cref="IPeer"/> which is already started when this method returns.
        /// </returns>
        public IPeer AddPeer(PeerConfig config)
        {
            this.CheckLocalAddress();
            var peer = ConfigImplementationFactory.CreateFromConfig<IPeerImpl>(config);
            this.peers.Add(peer);
            peer.Start(this);
            return peer;
        }

        /// <summary>
        /// Removes the given peer from this message dispatcher.
        /// </summary>
        /// <param name="peer">
        /// The peer returned by <see cref="AddPeer"/>.
        /// Do not try to remove a peer more than once or remove a peer from a different
        /// message dispatcher.
        /// </param>
        /// <returns>
        /// True if the peer was successfully removed from this dispatcher, false otherwise.
        /// </returns>
        public bool RemovePeer(IPeer peer)
        {
            this.CheckLocalAddress();
            var impl = peer as IPeerImpl;
            if (impl == null || !this.peers.Remove(impl))
            {
                return false;
            }

            impl.Stop();
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this == instance)
            {
                throw new NotSupportedException("Can't dispose the root Message Dispatcher.");
            }

            this.Stop();
            this.subscriptions.Clear();
        }

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <param name="subscription">
        /// The subscription to add.
        /// </param>
        void IMessageDispatcherImpl.AddSubscription(ISubscription subscription)
        {
            var added = false;
            using (this.subscriptionsLock.AcquireWriteLock())
            {
                SubscriptionCounter counter;
                if (!this.subscriptions.TryGetValue(subscription, out counter))
                {
                    counter = new SubscriptionCounter();
                    this.subscriptions.Add(subscription, counter);
                    added = true;
                }

                counter.Count++;

                Logger.Trace("Added subscription: {0} [{1}, {2}]", subscription, counter, this.subscriptions.Count);
            }

            if (added)
            {
                this.RaiseSubscriptionAdded(new SubscriptionEventArgs(subscription));
            }
        }

        /// <summary>
        /// Unsubscribe from a certain event to the given address
        /// </summary>
        /// <param name="subscription">
        /// The subscription to remove.
        /// </param>
        /// <returns>
        /// True if a subscription was found and removed.
        /// </returns>
        bool IMessageDispatcherImpl.RemoveSubscription(ISubscription subscription)
        {
            var removed = false;
            using (this.subscriptionsLock.AcquireWriteLock())
            {
                SubscriptionCounter counter;
                if (!this.subscriptions.TryGetValue(subscription, out counter))
                {
                    Logger.Debug("Could not remove subscription: {0}", subscription);
                    return false;
                }

                counter.Count--;
                if (counter.Count == 0)
                {
                    this.subscriptions.Remove(subscription);
                    removed = true;
                }

                Logger.Trace("Removed subscription: {0} [{1}, {2}]", subscription, counter, this.subscriptions.Count);
            }

            if (removed)
            {
                this.RaiseSubscriptionRemoved(new SubscriptionEventArgs(subscription));
            }

            return true;
        }

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
        bool IMessageDispatcherImpl.Send(
            MediAddress source, MediAddress destination, object message, ISessionId sourceSessionId)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            var type = TypeName.GetNameFor(message);
            if (sourceSessionId == null)
            {
                sourceSessionId = SessionIds.Local;
            }

            var notify = new List<ISubscription>();

            using (this.subscriptionsLock.AcquireReadLock())
            {
                foreach (var subscription in this.subscriptions.Keys)
                {
                    if (subscription.CanHandle(destination, type))
                    {
                        notify.Add(subscription);
                    }
                }
            }
           
            if (type.FullName.Contains("Ximple"))
            {
                Logger.Trace(
                    "*** Ximple *** Send {0} -> {1} to {2} subscriptions: [{3}] {4}", source, destination, notify.Count, type, message);
            }
            else
            {
                Logger.Trace(
                  "Send {0} -> {1} to {2} subscriptions: [{3}] {4}", source, destination, notify.Count, type, message);
            }

            bool sent = false;
            foreach (var subscription in notify)
            {
                try
                {
                    subscription.Handle(this, sourceSessionId, source, destination, message);
                    sent = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Exception while sending a notification");
                }
            }

            return sent;
        }

        /// <summary>
        /// Reconfigures the clients and servers from the given configuration.
        /// </summary>
        /// <param name="config">the MEDI configuration</param>
        private void Configure(MediConfig config)
        {
            // initialize message dispatching for management only after the dispatcher is created,
            // otherwise we have an infinite loop
            this.managementProviderFactory.InitMessageDispatching();

            this.Stop();
            if (config == null)
            {
                Logger.Warn("Config in MessageDispatcher is undefined and will be ignored. Missing medi config file.");
                return;
            }

            this.LogObserverFactory = new LogObserverFactory(config.InterceptLocalLogs, this);

            this.services.Add(new BroadcastSubscriptionService());
            foreach (var serviceConfig in config.Services)
            {
                this.services.Add(ConfigImplementationFactory.CreateFromConfig<IServiceImpl>(serviceConfig));
            }

            foreach (var peerConfig in config.Peers)
            {
                this.peers.Add(ConfigImplementationFactory.CreateFromConfig<IPeerImpl>(peerConfig));
            }

            foreach (var service in this.services)
            {
                service.Start(this);
            }

            foreach (var peer in this.peers)
            {
                peer.Start(this);
            }
        }

        private void Stop()
        {
            foreach (var peer in this.peers)
            {
                peer.Stop();
            }

            this.peers.Clear();

            foreach (var service in this.services)
            {
                service.Stop();
            }

            this.services.Clear();
        }

        private void RaiseSubscriptionAdded(SubscriptionEventArgs e)
        {
            var handler = this.subscriptionAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseSubscriptionRemoved(SubscriptionEventArgs e)
        {
            var handler = this.subscriptionRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public bool IsValidLocalAddress
        {
            get
            {
                return this.LocalAddress != null;
            }
        }

        private void CheckLocalAddress()
        {
            if (this.IsValidLocalAddress == false)
            {
                throw new NotSupportedException("MessageDispatcher was not configured with a local address.");
            }
        }

        private class SubscriptionCounter
        {
            public int Count { get; set; }

            public override string ToString()
            {
                return this.Count.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}