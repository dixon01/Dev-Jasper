// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoDnsSdProvider.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AutoDnsSdProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery
{
    using Gorba.Common.Protocols.DnsServiceDiscovery.Server;

    /// <summary>
    /// Dummy implementation extending <see cref="DnsSdServer"/> (this is here for CF 2.0 compatibility).
    /// </summary>
    public partial class AutoDnsSdProvider : DnsSdServer
    {
    }
}
