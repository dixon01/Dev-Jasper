// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBroadcastSubscriptionService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBroadcastSubscriptionService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Subscription
{
    using System;

    using Gorba.Common.Medi.Core.Services;

    /// <summary>
    /// A service that allows to request broadcasts from a remote
    /// node that is not sending broadcasts to us.
    /// This service that is always available in a Medi stack.
    /// </summary>
    public interface IBroadcastSubscriptionService : IService
    {
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
        IDisposable AddSubscription<T>(MediAddress remoteAddress);
    }
}