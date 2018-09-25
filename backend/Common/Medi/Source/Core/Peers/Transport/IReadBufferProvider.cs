// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadBufferProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IReadBufferProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Transport
{
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Provider for message buffers used for reading from a given session.
    /// </summary>
    internal interface IReadBufferProvider
    {
        /// <summary>
        /// Gets the buffer to be used for the given session.
        /// </summary>
        /// <param name="session">
        /// The session for which the buffer is to be provided.
        /// </param>
        /// <returns>
        /// The <see cref="MessageBuffer"/> that can be used exclusively for the given session.
        /// </returns>
        MessageBuffer GetReadBuffer(ITransportSession session);
    }
}