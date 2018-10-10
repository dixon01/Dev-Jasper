// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISubscription.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISubscription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Subscription for an event.
    /// </summary>
    internal interface ISubscription
    {
        /// <summary>
        /// Event that is fired whenever <see cref="Types"/> changes.
        /// </summary>
        event EventHandler<SubscribedTypesEventArgs> TypesChanged;

        /// <summary>
        /// Gets the types which this subscription can handle.
        /// The returned value can be empty but never null.
        /// </summary>
        TypeName[] Types { get; }

        /// <summary>
        /// Checks whether this subscription is responsible for a given address.
        /// </summary>
        /// <param name="address">
        /// The destination address.
        /// </param>
        /// <param name="type">
        /// The type of message.
        /// </param>
        /// <returns>
        /// true if and only if this subscription is responsible for a given address.
        /// </returns>
        bool CanHandle(MediAddress address, TypeName type);

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
        void Handle(
            IMessageDispatcherImpl medi,
            ISessionId sourceSessionId,
            MediAddress source,
            MediAddress destination,
            object message);
    }
}