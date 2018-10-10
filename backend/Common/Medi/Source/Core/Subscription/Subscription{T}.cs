// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Subscription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;

    using Gorba.Common.Medi.Core.Network;

    /// <summary>
    /// Specific subscription for a certain type of message.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the message to subscribe to.
    /// </typeparam>
    internal class Subscription<T> : Subscription where T : class
    {
        private readonly EventHandler<MessageEventArgs<T>> handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription{T}"/> class.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public Subscription(MediAddress destination, EventHandler<MessageEventArgs<T>> handler)
            : base(destination, TypeName.Of<T>())
        {
            this.handler = handler;
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
        public override void Handle(
            IMessageDispatcherImpl medi,
            ISessionId sourceSessionId,
            MediAddress source,
            MediAddress destination,
            object message)
        {
            this.handler(medi, new MessageEventArgs<T>(source, destination, (T)message));
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as Subscription<T>;

            return other != null && this.Destination.Equals(other.Destination) && other.handler.Equals(this.handler);
        }

        /// <summary>
        /// Get the hash code for this object.
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            return this.Destination.GetHashCode() ^ this.handler.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Subscription<{0}|{1}>", typeof(T).Name, this.Destination);
        }
    }
}
