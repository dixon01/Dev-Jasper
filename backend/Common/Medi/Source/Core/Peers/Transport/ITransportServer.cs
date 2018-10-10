// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Interface to be implemented by a transport server.
    /// It unites all necessary interfaces.
    /// </summary>
    internal interface ITransportServer
        : IMessageTransport, ITransportImplementation, IConfigurable<TransportServerConfig>
    {
    }
}
