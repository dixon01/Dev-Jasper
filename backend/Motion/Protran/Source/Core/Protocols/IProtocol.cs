// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocol.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Protocols
{
    using System;

    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Interface to be implemented by all protocol implementations.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Event that is fired when the protocol has finished starting up.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Gets the name of this protocol.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Configures this protocol with the given configuration.
        /// </summary>
        /// <param name="dictionary">
        ///     The generic view dictionary.
        /// </param>
        void Configure(Dictionary dictionary);

        /// <summary>
        /// Stop this protocol.
        /// </summary>
        void Stop();

        /// <summary>
        /// The main function of your protocol.
        /// Will be invoked by the protocol's host.
        /// </summary>
        /// <param name="host">The owner of this protocol.</param>
        void Run(IProtocolHost host);
    }
}
