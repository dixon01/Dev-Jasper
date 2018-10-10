// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceQuery.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceQuery type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;

    /// <summary>
    /// Query created from an <see cref="IIbisServiceLocator"/>.
    /// When using this query object, first call <see cref="Start"/> to get it stared.
    /// When finished, call <see cref="Stop"/> or dispose this object.
    /// </summary>
    /// <typeparam name="T">
    /// The type of service expected to be returned from this query.
    /// </typeparam>
    public interface IServiceQuery<T> : IDisposable
        where T : class, IVdv301Service
    {
        /// <summary>
        /// Event that is fired when the <see cref="Services"/> list changes.
        /// </summary>
        event EventHandler ServicesChanged;

        /// <summary>
        /// Gets the list of found services.
        /// </summary>
        T[] Services { get; }

        /// <summary>
        /// Starts updating this query.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops updating this query.
        /// </summary>
        void Stop();
    }
}