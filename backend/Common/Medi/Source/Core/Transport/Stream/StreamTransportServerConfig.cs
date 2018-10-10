// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTransportServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamTransportServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Abstract configuration base for stream transport servers.
    /// </summary>
    public abstract class StreamTransportServerConfig : TransportServerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTransportServerConfig"/> class.
        /// </summary>
        protected StreamTransportServerConfig()
        {
            this.SessionDisconnectTimeout = 120000;
            this.IdleKeepAliveWait = 30000;
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds until a session is discarded when
        /// it wasn't reopened after a failure.
        /// </summary>
        [DefaultValue(120000)]
        public int SessionDisconnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the time in milliseconds to wait
        /// before sending a keep-alive message when nothing is
        /// happening on this connection (no receive or send).
        /// Set to -1 to disable.
        /// </summary>
        [DefaultValue(30000)]
        public int IdleKeepAliveWait { get; set; }
    }
}