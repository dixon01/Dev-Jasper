// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTransportClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpTransportClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// Transport client using TCP. This has to connect to a remote
    /// <see cref="TcpTransportServer"/>.
    /// </summary>
    internal class TcpTransportClient : StreamTransportClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTransportClient"/> class.
        /// </summary>
        public TcpTransportClient()
            : base(new TcpStreamFactory())
        {
        }

        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="clientConfig">
        /// The config object.
        /// </param>
        public override void Configure(TransportClientConfig clientConfig)
        {
            var tcpConfig = clientConfig as TcpTransportClientConfig;
            if (tcpConfig == null)
            {
                throw new ArgumentException("TcpClientTransportConfig expected", "clientConfig");
            }

            base.Configure(clientConfig);
        }
    }
}
