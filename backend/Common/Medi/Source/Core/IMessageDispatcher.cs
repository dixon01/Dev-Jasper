// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessageDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;

    /// <summary>
    /// Common interface implemented by all message dispatchers.
    /// </summary>
    public interface IMessageDispatcher : IDisposable
    {
        /// <summary>
        /// Gets the local address of this message dispatcher.
        /// </summary>
        MediAddress LocalAddress { get; }

        /// <summary>
        /// Broadcasts a message to all interested subscribers.
        /// </summary>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        void Broadcast(object message);

        /// <summary>
        /// Send a message to a given subscriber.
        /// </summary>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="message">
        /// The message. Can be any XML serializable instance of a class.
        /// </param>
        void Send(MediAddress destination, object message);

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <param name="handler">
        /// The handler which will be called for the message.
        /// </param>
        /// <typeparam name="T">
        /// The type of the message to subscribe to.
        /// </typeparam>
        void Subscribe<T>(EventHandler<MessageEventArgs<T>> handler)
            where T : class;

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
        bool Unsubscribe<T>(EventHandler<MessageEventArgs<T>> handler)
            where T : class;
    }
}