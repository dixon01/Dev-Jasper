// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpClientEndPointConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpClientEndPointConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Config
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Ports.Forwarder;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The TCP client forwarding endpoint config.
    /// This config is used if you want to connect to another TCP server
    /// from a node.
    /// </summary>
    [Implementation(typeof(TcpClientForwarder))]
    public class TcpClientEndPointConfig : ForwardingEndPointConfig
    {
        /// <summary>
        /// Gets or sets the IP address of the remote server this client will connect to.
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        /// Gets or sets the TCP port of the remote server this client will connect to.
        /// </summary>
        public int RemotePort { get; set; }
    }
}