// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Subscription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Subscription for an event.
    /// </summary>
    internal abstract class Subscription : ISubscription
    {
        private readonly TypeName[] typeNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="typeNames">
        /// The type names this subscription should handle.
        /// </param>
        protected Subscription(MediAddress destination, params TypeName[] typeNames)
        {
            this.typeNames = typeNames;
            this.Destination = destination;
        }

        /// <summary>
        /// Event that is fired whenever <see cref="ISubscription.Types"/> changes.
        /// </summary>
        public event EventHandler<SubscribedTypesEventArgs> TypesChanged;

        /// <summary>
        /// Gets the destination.
        /// </summary>
        public MediAddress Destination { get; private set; }

        /// <summary>
        /// Gets the types which this subscription can handle.
        /// The returned value can be empty but never null.
        /// </summary>
        public virtual TypeName[] Types
        {
            get
            {
                return this.typeNames;
            }
        }

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
        public virtual bool CanHandle(MediAddress address, TypeName type)
        {
            return address.Matches(this.Destination) && ArrayUtil.Find(this.typeNames, t => t.Equals(type)) != null;
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
        public abstract void Handle(
            IMessageDispatcherImpl medi,
            ISessionId sourceSessionId,
            MediAddress source,
            MediAddress destination,
            object message);

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Subscription<{0}>", this.Destination);
        }

        /// <summary>
        /// Raises the <see cref="TypesChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTypesChanged(SubscribedTypesEventArgs e)
        {
            var handler = this.TypesChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}