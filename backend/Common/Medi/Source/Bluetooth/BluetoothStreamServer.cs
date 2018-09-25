// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothStreamServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothStreamServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using System;

    using Gorba.Common.Medi.Core.Transport.Bluetooth;
    using Gorba.Common.Medi.Core.Transport.Stream;

    using InTheHand.Net.Sockets;

    /// <summary>
    /// <see cref="IStreamServer"/> implementation for bluetooth.
    /// </summary>
    internal class BluetoothStreamServer : IStreamServer
    {
        private readonly BluetoothListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothStreamServer"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public BluetoothStreamServer(BluetoothTransportServerConfig config)
        {
            this.listener = new BluetoothListener(BluetoothStreamFactory.ServiceUuid);
            this.listener.Start();
        }

        /// <summary>
        /// Accepts asynchronously a remote request from a <see cref="IStreamClient"/>. 
        /// </summary>
        /// <param name="callback">
        /// The callback that is called when a client connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="IStreamServer.EndAccept"/>.
        /// </returns>
        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            return this.listener.BeginAcceptBluetoothClient(callback, state);
        }

        /// <summary>
        /// Completes the accept process that was initiated with
        /// <see cref="IStreamServer.BeginAccept"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="IStreamServer.BeginAccept"/>.
        /// </param>
        /// <returns>
        /// A new stream connection to the remote client.
        /// </returns>
        public IStreamConnection EndAccept(IAsyncResult result)
        {
            return new BluetoothStreamConnection(this.listener.EndAcceptBluetoothClient(result));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.listener.Stop();
        }
    }
}