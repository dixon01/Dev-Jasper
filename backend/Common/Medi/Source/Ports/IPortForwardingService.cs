// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPortForwardingService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPortForwardingService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports
{
    using System;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// Interface to the port forwarding service.
    /// This service allows to create forwarding between different endpoints.
    /// </summary>
    public interface IPortForwardingService : IService
    {
        /// <summary>
        /// Begins to create a port forwarding from a given address to another address.
        /// </summary>
        /// <param name="firstAddress">
        /// The first address.
        /// </param>
        /// <param name="firstConfig">
        /// The first forwarding config.
        /// </param>
        /// <param name="secondAddress">
        /// The second address.
        /// </param>
        /// <param name="secondConfig">
        /// The second forwarding config.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndCreateForwarding"/>.
        /// </returns>
        IAsyncResult BeginCreateForwarding(
            MediAddress firstAddress,
            ForwardingEndPointConfig firstConfig,
            MediAddress secondAddress,
            ForwardingEndPointConfig secondConfig,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Begins to create a port forwarding from the local node to another address.
        /// </summary>
        /// <param name="localConfig">
        /// The local forwarding config.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        /// <param name="remoteConfig">
        /// The remote forwarding config.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndCreateForwarding"/>.
        /// </returns>
        IAsyncResult BeginCreateForwarding(
            ForwardingEndPointConfig localConfig,
            MediAddress remoteAddress,
            ForwardingEndPointConfig remoteConfig,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Finishes the asynchronous call to BeginCreateForwarding.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned from one of the <code>BeginCreateForwarding</code> methods.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IPortForwarding"/> which can be used to close the forwarding again.
        /// </returns>
        IPortForwarding EndCreateForwarding(IAsyncResult ar);

        /// <summary>
        /// Begins to connect to a server reachable from a remote Medi node.
        /// </summary>
        /// <param name="remoteAddress">
        /// The remote address of the Medi node that is used for port forwarding.
        /// </param>
        /// <param name="remoteConfig">
        /// The configuration how to connect to the TCP server reachable from the remote note.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndConnect"/>.
        /// </returns>
        IAsyncResult BeginConnect(
            MediAddress remoteAddress,
            TcpClientEndPointConfig remoteConfig,
            AsyncCallback callback,
            object state);

        /// <summary>
        /// Finishes the asynchronous call to <see cref="BeginConnect"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned from <see cref="BeginConnect"/>.
        /// </param>
        /// <returns>
        /// The newly created <see cref="Stream"/> which allows to read and write to the remote server.
        /// </returns>
        System.IO.Stream EndConnect(IAsyncResult ar);
    }
}