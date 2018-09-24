// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SubscriptionEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;

    /// <summary>
    /// Event arguments for subscription add and remove events.
    /// </summary>
    internal class SubscriptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionEventArgs"/> class.
        /// </summary>
        /// <param name="subscription">
        /// The subscription being added or removed.
        /// </param>
        public SubscriptionEventArgs(ISubscription subscription)
        {
            this.Subscription = subscription;
        }

        /// <summary>
        /// Gets the subscription being added or removed.
        /// </summary>
        public ISubscription Subscription { get; private set; }
    }
}