// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothStreamClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothStreamClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using System;

    using Gorba.Common.Medi.Core.Transport.Bluetooth;
    using Gorba.Common.Medi.Core.Transport.Stream;

    using InTheHand.Net;
    using InTheHand.Net.Sockets;

    /// <summary>
    /// <see cref="IStreamClient"/> for bluetooth.
    /// </summary>
    internal class BluetoothStreamClient : IStreamClient
    {
        private readonly BluetoothClient client;

        private readonly BluetoothEndPoint endPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothStreamClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public BluetoothStreamClient(BluetoothTransportClientConfig config)
        {
            this.client = new BluetoothClient();

            var address = BluetoothAddress.Parse(config.RemoteAddress);
            this.endPoint = new BluetoothEndPoint(address, BluetoothStreamFactory.ServiceUuid);
        }

        /// <summary>
        /// Connects asynchronously to a remote <see cref="IStreamServer"/>. 
        /// </summary>
        /// <param name="callback">
        /// The callback that is called when the client is connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="IStreamClient.EndConnect"/>.
        /// </returns>
        public IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {
            return this.client.BeginConnect(this.endPoint, callback, state);
        }

        /// <summary>
        /// Completes the connection process that was initiated with
        /// <see cref="IStreamClient.BeginConnect"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IStreamClient.BeginConnect"/>.
        /// </param>
        /// <returns>
        /// A new stream connection to the remote server.
        /// </returns>
        public IStreamConnection EndConnect(IAsyncResult result)
        {
            this.client.EndConnect(result);
            return new BluetoothStreamConnection(this.client);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}