// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IServiceHost type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    /// <summary>
    /// Defines a service host with a name that can be opened and closed.
    /// </summary>
    public interface IServiceHost
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Opens the host.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the host.
        /// </summary>
        void Close();
    }
}