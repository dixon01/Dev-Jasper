// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRootMessageDispatcher.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRootMessageDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;

    using Gorba.Common.Medi.Core.Logging;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Services;

    /// <summary>
    /// Interface implemented by all message dispatchers that have a stack of peers
    /// and all necessary factories for querying the dispatcher.
    /// </summary>
    public interface IRootMessageDispatcher : IMessageDispatcher
    {
        /// <summary>
        /// Gets the log observer factory.
        /// </summary>
        ILogObserverFactory LogObserverFactory { get; }

        /// <summary>
        /// Gets the management provider factory.
        /// </summary>
        IManagementProviderFactory ManagementProviderFactory { get; }

        /// <summary>
        /// Gets the routing table.
        /// </summary>
        IRoutingTable RoutingTable { get; }

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
        IMessageDispatcher GetNamedDispatcher(string appName);

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
        IMessageDispatcher GetNamedDispatcher(MediAddress address);

        /// <summary>
        /// Gets a service implementation for a given type of service.
        /// </summary>
        /// <typeparam name="T">
        /// The type of service requested.
        /// </typeparam>
        /// <returns>
        /// The service implementation or null if no such service is found.
        /// </returns>
        T GetService<T>() where T : class, IService;
    }
}