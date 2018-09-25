// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStreamServer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStreamServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    /// <summary>
    /// Interface to be implemented by a stream server that allows
    /// <see cref="IStreamClient"/>s to connect to it. The concrete implementation is
    /// configured at construction time.
    /// </summary>
    internal interface IStreamServer : IDisposable
    {
        /// <summary>
        /// Accepts asynchronously a remote request from a <see cref="IStreamClient"/>. 
        /// </summary>
        /// <param name="callback">
        /// The callback that is called when a client connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="EndAccept"/>.
        /// </returns>
        IAsyncResult BeginAccept(AsyncCallback callback, object state);

        /// <summary>
        /// Completes the accept process that was initiated with
        /// <see cref="BeginAccept"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginAccept"/>.
        /// </param>
        /// <returns>
        /// A new stream connection to the remote client.
        /// </returns>
        IStreamConnection EndAccept(IAsyncResult result);
    }
}
