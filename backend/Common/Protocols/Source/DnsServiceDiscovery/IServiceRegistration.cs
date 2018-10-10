// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRegistration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceRegistration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using Gorba.Common.Protocols.DnsServiceDiscovery.Server;

    /// <summary>
    /// Service registration interface used by <see cref="DnsSdServer"/>.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// Gets a value indicating whether is the service is already registered.
        /// Registration can take some time since any possible name conflicts have to be checked first.
        /// </summary>
        bool IsRegistered { get; }
    }
}