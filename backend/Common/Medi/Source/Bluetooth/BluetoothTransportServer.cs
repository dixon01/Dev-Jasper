// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothTransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothTransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Bluetooth
{
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="StreamTransportServer"/> implementation for bluetooth.
    /// </summary>
    internal class BluetoothTransportServer : StreamTransportServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothTransportServer"/> class.
        /// </summary>
        public BluetoothTransportServer()
            : base(new BluetoothStreamFactory())
        {
        }
    }
}