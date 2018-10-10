// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpStreamFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpStreamFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// Stream factory for TCP protocol
    /// </summary>
    internal class TcpStreamFactory : IStreamFactory
    {
        /// <summary>
        /// Creates a new stream server for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new <see cref="TcpStreamServer"/>.
        /// </returns>
        public IStreamServer CreateServer(TransportServerConfig config)
        {
            return new TcpStreamServer(((TcpTransportServerConfig)config).LocalPort);
        }

        /// <summary>
        /// Creates a new stream client for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new <see cref="TcpStreamClient"/>.
        /// </returns>
        public IStreamClient CreateClient(TransportClientConfig config)
        {
            var clientConfig = (TcpTransportClientConfig)config;
            return new TcpStreamClient(clientConfig.RemoteHost, clientConfig.RemotePort);
        }
    }
}