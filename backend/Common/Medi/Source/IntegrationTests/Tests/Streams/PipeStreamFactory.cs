// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeStreamFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeStreamFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Streams
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Tests.Transport.Stream;
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="IStreamFactory"/> implementation for
    /// <see cref="PipeServer"/> and <see cref="PipeClient"/>.
    /// </summary>
    internal class PipeStreamFactory : IStreamFactory
    {
        /// <summary>
        /// The single instance.
        /// </summary>
        public static readonly PipeStreamFactory Instance = new PipeStreamFactory();

        private readonly Dictionary<int, PipeServer> servers = new Dictionary<int, PipeServer>();

        private PipeStreamFactory()
        {
        }

        /// <summary>
        /// Gets the pipe server for a given pipe ID.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="PipeServer"/>.
        /// </returns>
        public PipeServer this[int id]
        {
            get
            {
                return this.servers[id];
            }
        }

        /// <summary>
        /// Creates a new stream server for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new stream server.
        /// </returns>
        public IStreamServer CreateServer(TransportServerConfig config)
        {
            var pipeConfig = (PipeTransportServerConfig)config;
            var id = pipeConfig.ServerId;
            var server = new PipeServer(id);
            server.DisconnectAfterBytes = pipeConfig.DisconnectAfterBytes;
            server.Disposing += (sender, args) =>
                {
                    lock (this.servers)
                    {
                        this.servers.Remove(id);
                    }
                };

            lock (this.servers)
            {
                this.servers[id] = server;
            }

            return server;
        }

        /// <summary>
        /// Creates a new stream client for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new stream client.
        /// </returns>
        public IStreamClient CreateClient(TransportClientConfig config)
        {
            var pipeConfig = (PipeTransportClientConfig)config;
            var id = pipeConfig.ServerId;
            PipeServer server;
            lock (this.servers)
            {
                this.servers.TryGetValue(id, out server);
            }

            if (server == null)
            {
                throw new IOException("Couldn't find server with ID " + id);
            }

            var client = new PipeClient(server);
            client.DisconnectAfterBytes = pipeConfig.DisconnectAfterBytes;
            return client;
        }

        /// <summary>
        /// Resets the list of servers to ensure a client doesn't connect to the wrong server.
        /// </summary>
        public void Reset()
        {
            var pipeServers = new List<PipeServer>();
            lock (this.servers)
            {
                pipeServers.AddRange(this.servers.Values);
            }

            foreach (IDisposable server in pipeServers)
            {
                server.Dispose();
            }

            lock (this.servers)
            {
                this.servers.Clear();
            }
        }
    }
}