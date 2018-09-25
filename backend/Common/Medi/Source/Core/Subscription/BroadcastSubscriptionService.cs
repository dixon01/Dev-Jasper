// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcastSubscriptionService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BroadcastSubscriptionService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Messages;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Implementation of <see cref="IBroadcastSubscriptionService"/> that handles
    /// local method calls as well as remote requests.
    /// </summary>
    internal class BroadcastSubscriptionService : IBroadcastSubscriptionService, IServiceImpl
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(5);

        private readonly Dictionary<RemoteBroadcastSubscription, ITimer> subscriptionTimers =
            new Dictionary<RemoteBroadcastSubscription, ITimer>();

        private IMessageDispatcherImpl messageDispatcher;

        /// <summary>
        /// Starts this service.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that owns this service.
        /// </param>
        public void Start(IMessageDispatcherImpl dispatcher)
        {
            this.messageDispatcher = dispatcher;
            this.messageDispatcher.Subscribe<BroadcastSubscriptionRequest>(this.HandleRequest);
        }

        /// <summary>
        /// Stops this service.
        /// </summary>
        public void Stop()
        {
            this.messageDispatcher.Unsubscribe<BroadcastSubscriptionRequest>(this.HandleRequest);
            foreach (var pair in this.subscriptionTimers)
            {
                pair.Value.Enabled = false;
                this.messageDispatcher.RemoveSubscription(pair.Key);
            }

            this.subscriptionTimers.Clear();
        }

        /// <summary>
        /// Adds a subscription for the given type of message from the <paramref name="remoteAddress"/>.
        /// </summary>
        /// <remarks>
        /// It is important to note that all broadcast messages of the given type received on the given node will
        /// be forwarded to the local node. This means that messages might come from a node different than
        /// the one given in <paramref name="remoteAddress"/>.
        /// </remarks>
        /// <param name="remoteAddress">
        /// The remote address from which broadcasts should be received.
        /// </param>
        /// <typeparam name="T">
        /// The type of message expected.
        /// </typeparam>
        /// <returns>
        /// A <see cref="IDisposable"/> that needs to be disposed when the broadcast messages are no longer needed.
        /// </returns>
        public IDisposable AddSubscription<T>(MediAddress remoteAddress)
        {
            return new LocalBroadcastSubscription<T>(remoteAddress, this.messageDispatcher);
        }

        private void HandleRequest(object sender, MessageEventArgs<BroadcastSubscriptionRequest> e)
        {
            var subscription = new RemoteBroadcastSubscription(e.Message.Type, e.Source);
            if (e.Message.Timeout <= TimeSpan.Zero)
            {
                this.RemoveSubscription(subscription);
                return;
            }

            ITimer timer;
            if (this.subscriptionTimers.TryGetValue(subscription, out timer))
            {
                timer.Enabled = false;
            }
            else
            {
                timer = TimerFactory.Current.CreateTimer("BroadcastHandler-" + e.Message.Type.FullName);
                timer.AutoReset = false;
                timer.Elapsed += (s, ev) => this.RemoveSubscription(subscription);
                this.subscriptionTimers[subscription] = timer;
                this.messageDispatcher.AddSubscription(subscription);
            }

            timer.Interval = e.Message.Timeout;
            timer.Enabled = true;
        }

        private void RemoveSubscription(RemoteBroadcastSubscription subscription)
        {
            this.subscriptionTimers.Remove(subscription);
            this.messageDispatcher.RemoveSubscription(subscription);
        }

        private class LocalBroadcastSubscription<T> : IDisposable
        {
            private readonly MediAddress remoteAddress;

            private readonly IMessageDispatcherImpl dispatcher;

            private readonly ITimer timer;

            public LocalBroadcastSubscription(MediAddress remoteAddress, IMessageDispatcherImpl dispatcher)
            {
                this.remoteAddress = remoteAddress;
                this.dispatcher = dispatcher;

                this.SendRequest(DefaultTimeout);

                this.timer = TimerFactory.Current.CreateTimer("BroadcastSubscription-" + typeof(T).Name);
                this.timer.AutoReset = true;
                this.timer.Interval = TimeSpan.FromSeconds(DefaultTimeout.TotalSeconds * 0.8);
                this.timer.Elapsed += (s, e) => this.SendRequest(DefaultTimeout);
                this.timer.Enabled = true;
            }

            public void Dispose()
            {
                this.timer.Dispose();
                this.SendRequest(TimeSpan.Zero);
            }

            private void SendRequest(TimeSpan timeout)
            {
                this.dispatcher.Send(
                    this.remoteAddress,
                    new BroadcastSubscriptionRequest { Timeout = timeout, Type = TypeName.Of<T>() });
            }
        }

        private class RemoteBroadcastSubscription : ISubscription
        {
            private readonly TypeName typeName;

            private readonly MediAddress remoteAddress;

            public RemoteBroadcastSubscription(TypeName typeName, MediAddress remoteAddress)
            {
                this.typeName = typeName;
                this.remoteAddress = remoteAddress;
            }
#pragma warning disable 67
            public event EventHandler<SubscribedTypesEventArgs> TypesChanged;
#pragma warning restore 67
            public TypeName[] Types
            {
                get
                {
                    return new[] { this.typeName };
                }
            }

            public bool CanHandle(MediAddress address, TypeName type)
            {
                return (address.Unit == MediAddress.Wildcard || address.Application == MediAddress.Wildcard)
                       && this.typeName.Equals(type);
            }

            /// <summary>
            /// Sends the message to the handler.
            /// </summary>
            /// <param name="medi">
            /// The message dispatcher dispatching the message.
            /// </param>
            /// <param name="sourceSessionId">
            /// The session id from where the message originates, should never be null.
            /// </param>
            /// <param name="source">
            /// The source address.
            /// </param>
            /// <param name="destination">
            /// The destination address.
            /// </param>
            /// <param name="message">
            /// The message.
            /// </param>
            public void Handle(
                IMessageDispatcherImpl medi,
                ISessionId sourceSessionId,
                MediAddress source,
                MediAddress destination,
                object message)
            {
                // IMPORTANT: don't use the sourceSessionId, otherwise the message wouldn't pass through the
                // session it came from (which might be necessary).
                medi.Send(source, this.remoteAddress, message, null);
            }

            public override bool Equals(object obj)
            {
                var other = obj as RemoteBroadcastSubscription;
                return other != null && other.remoteAddress.Equals(this.remoteAddress)
                       && other.typeName.Equals(this.typeName);
            }

            public override int GetHashCode()
            {
                return this.remoteAddress.GetHashCode() ^ this.typeName.GetHashCode();
            }
        }
    }
}
