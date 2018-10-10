// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstallationEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInstallationEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Interface for classes that can install software.
    /// </summary>
    public interface IInstallationEngine
    {
        /// <summary>
        /// Event that is fired when the <see cref="State"/> changed.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the current update state.
        /// </summary>
        UpdateState State { get; }

        /// <summary>
        /// Runs the installation. This method can be long-running, so it should be
        /// executed in a separate thread.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        /// <returns>
        /// A flag indicating if the installation was completed or if only part of it was done.
        /// </returns>
        bool Install(IInstallationHost host);

        /// <summary>
        /// Rolls back anything that was previously done by <see cref="Install"/>.
        /// This method is only called in case the installation failed.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        void Rollback(IInstallationHost host);
    }
}