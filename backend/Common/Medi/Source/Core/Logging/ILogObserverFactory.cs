// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogObserverFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILogObserverFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    /// <summary>
    /// Factory interface for <see cref="ILogObserver"/>s.
    /// </summary>
    public interface ILogObserverFactory
    {
        /// <summary>
        /// Gets the local log observer.
        /// </summary>
        ILogObserver LocalObserver { get; }

        /// <summary>
        /// Creates a remote observer for the given address.
        /// </summary>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        /// <returns>
        /// The <see cref="ILogObserver"/>.
        /// </returns>
        ILogObserver CreateRemoteObserver(MediAddress remoteAddress);
    }
}