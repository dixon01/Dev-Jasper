// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullInstaller.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NullInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Installer that does nothing except for setting the state to <see cref="UpdateState.Installed"/>.
    /// </summary>
    public class NullInstaller : InstallationEngineBase
    {
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
        public override bool Install(IInstallationHost host)
        {
            this.State = UpdateState.Installed;
            return true;
        }

        /// <summary>
        /// Rolls back anything that was previously done by <see cref="InstallationEngineBase.Install"/>.
        /// This method is only called in case the installation failed.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        public override void Rollback(IInstallationHost host)
        {
            // nothing to do if the installation had nothing to do either
        }
    }
}