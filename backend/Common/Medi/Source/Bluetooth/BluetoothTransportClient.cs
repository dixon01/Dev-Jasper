// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothTransportClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothTransportClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="StreamTransportClient"/> implementation for bluetooth.
    /// </summary>
    internal class BluetoothTransportClient : StreamTransportClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothTransportClient"/> class.
        /// </summary>
        public BluetoothTransportClient()
            : base(new BluetoothStreamFactory())
        {
        }
    }
}
