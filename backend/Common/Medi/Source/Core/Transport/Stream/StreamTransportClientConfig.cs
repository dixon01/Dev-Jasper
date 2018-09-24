// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTransportClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamTransportClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Abstract configuration base for stream transport clients.
    /// </summary>
    public abstract class StreamTransportClientConfig : TransportClientConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamTransportClientConfig"/> class.
        /// </summary>
        protected StreamTransportClientConfig()
        {
            this.ReconnectWait = 10000;
            this.IdleKeepAliveWait = 30000;
        }

        /// <summary>
        /// Gets or sets the time in milliseconds to wait
        /// before reconnecting if the connection was dropped.
        /// </summary>
        [DefaultValue(10000)]
        public int ReconnectWait { get; set; }

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