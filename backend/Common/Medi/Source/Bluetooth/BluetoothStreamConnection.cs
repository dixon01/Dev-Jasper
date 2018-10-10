// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothStreamConnection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothStreamConnection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using System.IO;

    using Gorba.Common.Medi.Core.Transport.Stream;

    using InTheHand.Net.Sockets;

    /// <summary>
    /// <see cref="IStreamConnection"/> for bluetooth.
    /// </summary>
    internal class BluetoothStreamConnection : IStreamConnection
    {
        private readonly BluetoothClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothStreamConnection"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public BluetoothStreamConnection(BluetoothClient client)
        {
            this.client = client;
            this.Stream = this.client.GetStream();
        }

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Creates a more or less unique ID for this connection.
        /// </summary>
        /// <returns>
        /// The pseudo-unique ID.
        /// </returns>
        public int CreateId()
        {
            var addr = this.client.RemoteEndPoint.Address;
            return (int)(addr.Nap ^ addr.Sap ^ this.client.RemoteEndPoint.Port);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}