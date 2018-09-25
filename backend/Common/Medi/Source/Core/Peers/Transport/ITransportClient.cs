// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransportClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransportClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Interface to be implemented by a transport client.
    /// It unites all necessary interfaces.
    /// </summary>
    internal interface ITransportClient
        : IMessageTransport, ITransportImplementation, IConfigurable<TransportClientConfig>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this client is a gateway client.
        /// </summary>
        bool IsGateway { get; set; }
    }
}
