// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteManagementObjectProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRemoteManagementObjectProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for an object management provider from a different host.
    /// Supports reloading the cached management information and asynchronous methods.
    /// </summary>
    public interface IRemoteManagementObjectProvider : IRemoteManagementProvider, IManagementObjectProvider
    {
        /// <summary>
        /// Asynchronously begins to fetch the properties from the remote node.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the properties where fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetProperties"/>.
        /// </returns>
        IAsyncResult BeginGetProperties(AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch the properties from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetProperties"/>.
        /// </param>
        /// <returns>
        /// The list of all properties.
        /// </returns>
        IEnumerable<ManagementProperty> EndGetProperties(IAsyncResult result);

        /// <summary>
        /// Asynchronously begins to fetch the property with the given name from the remote node.
        /// </summary>
        /// <param name="name">
        /// The name of the property.
        /// </param>
        /// <param name="callback">
        /// The callback called when the property was fetched.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndGetProperty"/>.
        /// </returns>
        IAsyncResult BeginGetProperty(string name, AsyncCallback callback, object state);

        /// <summary>
        /// Ends the asynchronous request to fetch a property from the remote node.
        /// </summary>
        /// <param name="result">
        /// The async result returned from <see cref="BeginGetProperty"/>.
        /// </param>
        /// <returns>
        /// The property with the name given to <see cref="BeginGetProperty"/>
        /// or null if no property with the given name exists.
        /// </returns>
        ManagementProperty EndGetProperty(IAsyncResult result);
    }
}