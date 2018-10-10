// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIbisServiceLocator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIbisServiceLocator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// The interface to query <see cref="IVdv301Service"/>s.
    /// </summary>
    public interface IIbisServiceLocator : IDisposable
    {
        /// <summary>
        /// Registers the given service with the service locator.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <typeparam name="TService">
        /// The type of the service to be registered,
        /// should be one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </typeparam>
        /// <exception cref="ArgumentException">
        /// If the given <paramref name="service"/> doesn't implement <see cref="IVdv301ServiceImpl"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <typeparamref name="TService"/> isn't one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </exception>
        void RegisterService<TService>(TService service)
            where TService : class, IVdv301Service;

        /// <summary>
        /// Gets a query for a given service type.
        /// You need to call <see cref="IServiceQuery{T}.Start"/> on the returned object to start getting services.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service to be returned,
        /// should be one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </typeparam>
        /// <returns>
        /// The query for <see cref="TService"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <typeparamref name="TService"/> isn't one of the sub-interfaces of <see cref="IVdv301Service"/>.
        /// </exception>
        IServiceQuery<TService> QueryServices<TService>()
            where TService : class, IVdv301Service;
    }
}
