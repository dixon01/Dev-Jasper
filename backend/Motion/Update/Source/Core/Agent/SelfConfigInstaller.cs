// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfConfigInstaller.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelfConfigInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Installer for the Update configuration.
    /// This installer only installs the config, but doesn't update the application.
    /// </summary>
    public class SelfConfigInstaller : InstallationEngineBase
    {
        private readonly UpdateFolder configUpdate;

        private string configPath;
        private string configUpdateDirectory;
        private string configBackupDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfConfigInstaller"/> class.
        /// </summary>
        /// <param name="configUpdate">
        /// The folder defining the update of the config directory.
        /// </param>
        public SelfConfigInstaller(UpdateFolder configUpdate)
        {
            this.configUpdate = configUpdate;
        }

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
            this.Logger.Info("Installing update of update configuration");

            this.State = UpdateState.Installing;

            var pathManager = PathManager.Instance;

            this.configPath = pathManager.GetPath(FileType.Config, string.Empty);

            this.configUpdateDirectory = pathManager.CreatePath(
                FileType.Data,
                "ConfigInstall" + System.IO.Path.DirectorySeparatorChar);
            this.configBackupDirectory = pathManager.CreatePath(
                FileType.Data,
                "ConfigBackup" + System.IO.Path.DirectorySeparatorChar);

            this.FileUtility.ClearDirectory(this.configUpdateDirectory);
            this.FileUtility.ClearDirectory(this.configBackupDirectory);

            this.Logger.Debug("Config directory: {0}", this.configPath);
            this.Logger.Trace("Temporary installation directory: {0}", this.configUpdateDirectory);
            this.Logger.Trace("Temporary backup directory: {0}", this.configBackupDirectory);

            this.Progress(0.25, "Getting new Update config files");
            this.GetNewFiles(this.configUpdate, this.configUpdateDirectory, null);
            this.Progress(0.5, "Backing up old Update config files");
            this.BackupOldFiles(this.configUpdate, this.configPath, this.configBackupDirectory, null);
            this.Progress(0.75, "Installing new Update config files");
            this.InstallNewFiles(this.configUpdate, this.configUpdateDirectory, this.configPath, null);

            this.CleanUp();

            host.Relaunch("Self-updated configuration");

            this.Logger.Info("Installation completed");
            return false;
        }

        /// <summary>
        /// Rolls back anything that was previously done by <see cref="Install"/>.
        /// This method is only called in case the installation failed.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        public override void Rollback(IInstallationHost host)
        {
            this.Logger.Info("Rollback update of update configuration");

            this.RollbackBackupFiles(this.configUpdate, this.configBackupDirectory, this.configPath, null);

            this.CleanUp();
        }

        private void CleanUp()
        {
            this.FileUtility.DeleteDirectory(this.configUpdateDirectory);
            this.FileUtility.DeleteDirectory(this.configBackupDirectory);
        }
    }
}