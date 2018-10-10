// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using System.Net;

    /// <summary>
    /// Information about a local or discovered service.
    /// </summary>
    public interface IServiceInfo
    {
        /// <summary>
        /// Gets the service name (without protocol or ".local").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the protocol name (including ".local").
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Gets the full service name (including protocol and ".local").
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets the service attributes.
        /// </summary>
        IServiceAttribute[] Attributes { get; }

        /// <summary>
        /// Gets the addresses where this service can be found.
        /// </summary>
        IPAddress[] Addresses { get; }

        /// <summary>
        /// Gets the given attribute.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The attribute value or null if not found.
        /// </returns>
        string GetAttribute(string name);
    }
}