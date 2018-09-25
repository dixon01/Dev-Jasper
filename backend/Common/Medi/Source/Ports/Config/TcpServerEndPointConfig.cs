// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpServerEndPointConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TcpServerEndPointConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Config
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Ports.Forwarder;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The TCP server forwarding endpoint config.
    /// This config is used if you want to have a local TCP server you can connect to.
    /// </summary>
    [Implementation(typeof(TcpServerForwarder))]
    public class TcpServerEndPointConfig : ForwardingEndPointConfig
    {
        /// <summary>
        /// Gets or sets the local TCP port.
        /// </summary>
        public int LocalPort { get; set; }
    }
}