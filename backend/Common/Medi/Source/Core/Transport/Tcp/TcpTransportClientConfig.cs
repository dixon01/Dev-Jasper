// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTransportClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpTransportClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System.ComponentModel;
    using System.Net;

    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the TCP Transport Client.
    /// </summary>
    [Implementation(typeof(TcpTransportClient))]
    public class TcpTransportClientConfig : StreamTransportClientConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTransportClientConfig"/> class.
        /// </summary>
        public TcpTransportClientConfig()
        {
            this.RemoteHost = IPAddress.Loopback.ToString();
            this.RemotePort = TcpTransportServerConfig.DefaultPort;
        }

        /// <summary>
        /// Gets or sets the remote server IP v4 address.
        /// This property should no longer be used,
        /// use <see cref="RemoteHost"/> instead.
        /// </summary>
        [DefaultValue("127.0.0.1")]
        public string RemoteIp
        {
            get
            {
                return this.RemoteHost;
            }

            set
            {
                this.RemoteHost = value;
            }
        }

        /// <summary>
        /// Gets or sets the remote server host name.
        /// </summary>
        [DefaultValue("127.0.0.1")]
        public string RemoteHost { get; set; }

        /// <summary>
        /// Gets or sets the remote server TCP port.
        /// </summary>
        [DefaultValue(1596)]
        public int RemotePort { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="RemoteIp"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeRemoteIp()
        {
            return false;
        }
    }
}
