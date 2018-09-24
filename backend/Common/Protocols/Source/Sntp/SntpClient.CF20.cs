// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpClient.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Sntp
{
    using System.Net.Sockets;

    /// <summary>
    /// SNTP client implementation for .NET CF 2.0.
    /// </summary>
    public partial class SntpClient
    {
        private UdpClient CreateUdpClient()
        {
            return new UdpClient();
        }
    }
}
