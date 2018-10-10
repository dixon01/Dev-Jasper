// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransportServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Xml.Serialization;

    /// <summary>
    /// <see cref="TransportConfig"/> subclass used to configure transport servers.
    /// </summary>
    [XmlInclude(typeof(Transport.Tcp.TcpTransportServerConfig))]
    [XmlInclude(typeof(Transport.File.FileTransportServerConfig))]
    [XmlInclude(typeof(Transport.Bluetooth.BluetoothTransportServerConfig))]
    public class TransportServerConfig : TransportConfig
    {
    }
}
