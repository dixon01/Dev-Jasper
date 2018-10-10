// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDnsSdProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDnsSdProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to be implemented by all classes that provide DNS-SD functionality.
    /// </summary>
    public interface IDnsSdProvider
    {
        /// <summary>
        /// Starts this provider.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this provider.
        /// </summary>
        void Stop();

        /// <summary>
        /// Registers a new service with the given identity.
        /// </summary>
        /// <param name="serviceName">
        /// The service name.
        /// </param>
        /// <param name="protocol">
        /// The protocol name (without ".local").
        /// </param>
        /// <param name="port">
        /// The port number.
        /// </param>
        /// <param name="attributes">
        /// The service attributes.
        /// </param>
        /// <returns>
        /// The service registration to be used with <see cref="DeregisterService"/>.
        /// </returns>
        IServiceRegistration RegisterService(
            string serviceName, string protocol, int port, IDictionary<string, string> attributes);

        /// <summary>
        /// Deregisters a previously registered service.
        /// <seealso cref="RegisterService"/>
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the <see cref="registration"/> didn't come from <see cref="RegisterService"/>.
        /// </exception>
        void DeregisterService(IServiceRegistration registration);

        /// <summary>
        /// Creates a query for all services of a protocol.
        /// You need to call <see cref="IQuery.Start"/> on the returned object to start getting services.
        /// </summary>
        /// <param name="protocol">
        /// The protocol name without the ".local".
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/> that can be used to get the results.
        /// </returns>
        IQuery CreateQuery(string protocol);
    }
}