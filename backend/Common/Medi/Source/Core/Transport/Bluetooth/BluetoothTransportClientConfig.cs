// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothTransportClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothTransportClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Bluetooth
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The bluetooth transport client configuration.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Bluetooth.BluetoothTransportClient, Gorba.Common.Medi.Bluetooth")]
    public class BluetoothTransportClientConfig : StreamTransportClientConfig
    {
        /// <summary>
        /// Gets or sets the remote address (MAC address).
        /// </summary>
        public string RemoteAddress { get; set; }
    }
}