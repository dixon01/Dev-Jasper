// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsSdException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DnsSdException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Server
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The DNS-SD exception.
    /// </summary>
    public partial class DnsSdException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected DnsSdException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}