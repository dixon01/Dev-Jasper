// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeTransportClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeTransportClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Streams
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transport.Stream;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The pipe transport client config.
    /// </summary>
    [Implementation(typeof(PipeTransportClient))]
    public class PipeTransportClientConfig : StreamTransportClientConfig
    {
        /// <summary>
        /// Gets or sets the server pipe id.
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// Gets or sets after how many bytes the connection should be closed.
        /// This is used for testing to make sure connections can be reopened and transfers recovered.
        /// </summary>
        public int DisconnectAfterBytes { get; set; }
    }
}