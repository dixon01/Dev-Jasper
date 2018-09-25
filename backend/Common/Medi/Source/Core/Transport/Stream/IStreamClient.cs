// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStreamClient.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStreamClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    /// <summary>
    /// Interface to be implemented by a stream client that allows to connect
    /// to a <see cref="IStreamServer"/>. The concrete implementation is
    /// configured at construction time.
    /// </summary>
    internal interface IStreamClient : IDisposable
    {
        /// <summary>
        /// Connects asynchronously to a remote <see cref="IStreamServer"/>. 
        /// </summary>
        /// <param name="callback">
        /// The callback that is called when the client is connected.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The result to use when completing this request with
        /// <see cref="EndConnect"/>.
        /// </returns>
        IAsyncResult BeginConnect(AsyncCallback callback, object state);

        /// <summary>
        /// Completes the connection process that was initiated with
        /// <see cref="BeginConnect"/>.
        /// </summary>
        /// <param name="result">
        /// The result returned by <see cref="BeginConnect"/>.
        /// </param>
        /// <returns>
        /// A new stream connection to the remote server.
        /// </returns>
        IStreamConnection EndConnect(IAsyncResult result);
    }
}