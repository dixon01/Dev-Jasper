// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTransportServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpTransportServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Tcp
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the TCP Transport Server.
    /// </summary>
    [Implementation(typeof(TcpTransportServer))]
    public class TcpTransportServerConfig : StreamTransportServerConfig
    {
        /// <summary>
        /// Default local port: 1596.
        /// </summary>
        public static readonly int DefaultPort = 1596;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTransportServerConfig"/> class.
        /// </summary>
        public TcpTransportServerConfig()
        {
            this.LocalPort = DefaultPort;
        }

        /// <summary>
        /// Gets or sets the local server TCP port.
        /// </summary>
        [DefaultValue(1596)]
        public int LocalPort { get; set; }
    }
}
