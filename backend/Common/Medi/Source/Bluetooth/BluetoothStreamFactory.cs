// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothStreamFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothStreamFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using System;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Bluetooth;
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="IStreamFactory"/> implementation for bluetooth.
    /// </summary>
    internal class BluetoothStreamFactory : IStreamFactory
    {
        /// <summary>
        /// The service UUID used by the bluetooth stream to send data between peers.
        /// </summary>
        public static readonly Guid ServiceUuid = new Guid("A655A737-612D-4DB4-AA30-53AC0A565779");

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
            return new BluetoothStreamServer((BluetoothTransportServerConfig)config);
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
            return new BluetoothStreamClient((BluetoothTransportClientConfig)config);
        }
    }
}