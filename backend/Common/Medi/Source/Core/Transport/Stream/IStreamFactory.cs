// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStreamFactory.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStreamFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Factory interface to create <see cref="IStreamServer"/>
    /// and <see cref="IStreamClient"/> objects from a given 
    /// configuration.
    /// </summary>
    internal interface IStreamFactory
    {
        /// <summary>
        /// Creates a new stream server for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new stream server.
        /// </returns>
        IStreamServer CreateServer(TransportServerConfig config);

        /// <summary>
        /// Creates a new stream client for the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// a new stream client.
        /// </returns>
        IStreamClient CreateClient(TransportClientConfig config);
    }
}