// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRemoteManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a management provider from a different host.
    /// Supports reloading the cached management information and asynchronous methods.
    /// </summary>
    public interface IRemoteManagementProvider : IManagementProvider
    {
        /// <summary>
        /// Synchronously reloads the cached management information from the remote node.
        /// </summary>
        void Reload();

        /// <summary>
        /// Asynchronously begins to reload the cached management information from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when information was refreshed.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndReload"/>.
        /// </returns>
        IAsyncResult BeginReload(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to reload the cached management information from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginReload"/>.
        /// </param>
        void EndReload(IAsyncResult result);

        /// <summary>
        /// Asynchronously begins to fetch the children from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the children where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetChildren"/>.
        /// </returns>
        IAsyncResult BeginGetChildren(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch the children from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetChildren"/>.
        /// </param>
        /// <returns>
        /// The list of all children.
        /// </returns>
        IEnumerable<IManagementProvider> EndGetChildren(IAsyncResult result);

        /// <summary>
        /// Asynchronously begins to fetch the child with the given name from the remote node.
        /// </summary>
        /// <param name="name">
        /// The name of the child.
        /// </param>
        /// <param name="callback">
        /// The callback called when the child was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetChild"/>.
        /// </returns>
        IAsyncResult BeginGetChild(string name, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch a child from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetChild"/>.
        /// </param>
        /// <returns>
        /// The child with the name given to <see cref="BeginGetChild"/>
        /// or null if no child with the given name exists.
        /// </returns>
        IManagementProvider EndGetChild(IAsyncResult result);
    }
}