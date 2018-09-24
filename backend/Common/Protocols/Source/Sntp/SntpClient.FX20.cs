// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpClient.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System.Net.Sockets;

    /// <summary>
    /// SNTP client implementation for .NET 2.0.
    /// </summary>
    public partial class SntpClient
    {
        private UdpClient CreateUdpClient()
        {
            var client = new UdpClient();
            client.Client.SendTimeout = this.Timeout;
            client.Client.ReceiveTimeout = this.Timeout;
            return client;
        }
    }
}
