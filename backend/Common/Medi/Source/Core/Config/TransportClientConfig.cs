// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransportClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Xml.Serialization;

    /// <summary>
    /// <see cref="TransportConfig"/> subclass used to configure transport clients.
    /// </summary>
    [XmlInclude(typeof(Transport.Tcp.TcpTransportClientConfig))]
    [XmlInclude(typeof(Transport.Bluetooth.BluetoothTransportClientConfig))]
    public class TransportClientConfig : TransportConfig
    {
    }
}
