// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpTransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// Transport server implementation for TCP.
    /// </summary>
    internal class TcpTransportServer : StreamTransportServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTransportServer"/> class.
        /// </summary>
        public TcpTransportServer()
            : base(new TcpStreamFactory())
        {
        }

        /// <summary>
        /// Configures this transport server with the given config.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public override void Configure(TransportServerConfig config)
        {
            var tcpConfig = config as TcpTransportServerConfig;
            if (tcpConfig == null)
            {
                throw new ArgumentException("TcpTransportServerConfig expected", "config");
            }

            base.Configure(config);
        }
    }
}
