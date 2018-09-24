// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IQuery type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using System;

    /// <summary>
    /// The representation of a DNS-SD query.
    /// </summary>
    public interface IQuery : IDisposable
    {
        /// <summary>
        /// Event that is fired when the <see cref="Services"/> list changes.
        /// </summary>
        event EventHandler ServicesChanged;

        /// <summary>
        /// Gets the list of found services.
        /// </summary>
        IServiceInfo[] Services { get; }

        /// <summary>
        /// Starts updating this query.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops updating this query.
        /// </summary>
        void Stop();
    }
}