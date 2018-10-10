// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeTransportServer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PipeTransportServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Tests.Streams
{
    using Gorba.Common.Medi.Core.Transport.Stream;

    /// <summary>
    /// <see cref="StreamTransportServer"/> that uses pipes to connect.
    /// </summary>
    internal class PipeTransportServer : StreamTransportServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipeTransportServer"/> class.
        /// </summary>
        public PipeTransportServer()
            : base(PipeStreamFactory.Instance)
        {
        }
    }
}