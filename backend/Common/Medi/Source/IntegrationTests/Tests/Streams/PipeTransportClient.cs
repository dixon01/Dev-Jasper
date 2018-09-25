// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeTransportClient.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeTransportClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Streams
{
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="StreamTransportClient"/> that uses pipes to connect.
    /// </summary>
    internal class PipeTransportClient : StreamTransportClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTransportClient"/> class.
        /// </summary>
        public PipeTransportClient()
            : base(PipeStreamFactory.Instance)
        {
        }
    }
}