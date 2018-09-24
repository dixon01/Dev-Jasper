// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    /// <summary>
    /// Attribute of an <see cref="IServiceInfo"/>.
    /// </summary>
    public interface IServiceAttribute
    {
        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        string Value { get; }
    }
}