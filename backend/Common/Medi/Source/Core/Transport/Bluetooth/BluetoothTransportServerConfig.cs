// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BluetoothTransportServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BluetoothTransportServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Bluetooth
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The bluetooth transport server configuration.
    /// </summary>
    [Implementation("Gorba.Common.Medi.Bluetooth.BluetoothTransportServer, Gorba.Common.Medi.Bluetooth")]
    public class BluetoothTransportServerConfig : StreamTransportServerConfig
    {
    }
}