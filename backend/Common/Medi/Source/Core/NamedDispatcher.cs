// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NamedDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;

    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Subscription;

    /// <summary>
    /// Named dispatcher used by <see cref="MessageDispatcher.GetNamedDispatcher(MediAddress)"/>.
    /// </summary>
    internal class NamedDispatcher : IMessageDispatcher
    {
        private readonly IMessageDispatcherImpl messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDispatcher"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The parent message dispatcher implementation.
        /// </param>
        /// <param name="address">
        ///     The address.
        /// </param>
        public NamedDispatcher(IMessageDispatcherImpl messageDispatcher, MediAddress address)
        {
            this.messageDispatcher = messageDispatcher;
            this.LocalAddress = address;

            this.messageDispatcher.RoutingTable.Add(
                SessionIds.Local,
                new[] { new RoutingEntry(address, 0) });
        }

        /// <summary>
        /// Event that is fired when this dispatcher is disposed.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets the local address of this message dispatcher.
        /// </summary>
        public MediAddress LocalAddress { get; private set; }

        /// <summary>
        /// Broadcasts a message to all interested subscribers.
        /// </summary>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        public void Broadcast(object message)
        {
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
            this.messageDispatcher.Send(this.LocalAddress, destination, message, null);
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
        public void Subscribe<T>(EventHandler<MessageEventArgs<T>> handler) where T : class
        {
            this.messageDispatcher.AddSubscription(new Subscription<T>(this.LocalAddress, handler));
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
        public bool Unsubscribe<T>(EventHandler<MessageEventArgs<T>> handler) where T : class
        {
            return this.messageDispatcher.RemoveSubscription(new Subscription<T>(this.LocalAddress, handler));
        }

        /// <summary>
        /// Performs application-defined tasks associated with
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            this.messageDispatcher.RoutingTable.Remove(this.LocalAddress);
        }
    }
}