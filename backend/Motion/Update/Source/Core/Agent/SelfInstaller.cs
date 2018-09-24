// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfInstaller.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelfInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Installer that begins to install Update and its configuration.
    /// This is completed by re-running Update.exe twice.
    /// </summary>
    public class SelfInstaller : InstallationEngineBase
    {
        private readonly FolderUpdate progsUpdate;
        private readonly FolderUpdate configUpdate;

        private string tempProgs;
        private string tempConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfInstaller"/> class.
        /// </summary>
        /// <param name="progsUpdate">
        /// The folder defining the update of the application directory.
        /// </param>
        /// <param name="configUpdate">
        /// The folder defining the update of the config directory.
        /// </param>
        public SelfInstaller(FolderUpdate progsUpdate, FolderUpdate configUpdate)
        {
            if (progsUpdate == null)
            {
                throw new ArgumentNullException("progsUpdate");
            }

            this.progsUpdate = progsUpdate;
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
            this.Logger.Info("Installing update of update application");

            this.State = UpdateState.Installing;

            var pathManager = PathManager.Instance;

            var guid = Guid.NewGuid().ToString("N");
            var progs = pathManager.GetPath(FileType.Application, string.Empty);
            this.tempProgs = string.Format("{0}_{1}", progs, guid);

            this.Logger.Debug("Temporary application directory: {0}", this.tempProgs);

            this.Progress(0.1, "Installing temporary Update.exe");
            this.InstallToFolder(this.progsUpdate, this.tempProgs);

            if (this.configUpdate != null)
            {
                var config = pathManager.GetPath(FileType.Config, string.Empty);
                this.tempConfig = string.Format("{0}_{1}", config, guid);

                this.Logger.Debug("Temporary config directory: {0}", this.tempConfig);
                this.Progress(0.2, "Installing temporary Update.exe config");
                this.InstallToFolder(this.configUpdate, this.tempConfig);
            }

            this.Progress(0.3, "Launching temporary Update.exe");
            var updateFile = pathManager.CreatePath(FileType.Data, guid + FileDefinitions.UpdateCommandExtension);

            using (var updateFileStream = this.FileSystem.CreateFile(updateFile).OpenWrite())
            {
                var configurator = new Configurator(updateFileStream);
                configurator.Serialize(this.Command);
            }

            this.Logger.Debug("Update command: {0}", updateFile);

            var appName = Path.GetFileName(host.ExecutablePath);
            Debug.Assert(appName != null, "appName can't be null");
            var originalApp = Path.Combine(this.tempProgs, appName);
            var temporaryApp = Path.Combine(this.tempProgs, guid + "-" + appName);
            this.FileSystem.GetFile(originalApp).CopyTo(temporaryApp);

            var args = string.Format(
                "/{0} \"{1}\" /{2} \"{3}\" /{4} {5}",
                CommandLineOptions.InstallCommand,
                updateFile,
                CommandLineOptions.TargetArgument,
                progs,
                CommandLineOptions.WaitForExitArgument,
                Process.GetCurrentProcess().Id);
            var start = new ProcessStartInfo(temporaryApp, args);
            start.WorkingDirectory = this.tempProgs;

            host.StartProcess(start);

            this.Logger.Info("Update of update is launched as a new process");

            host.Exit("Launching self-update");

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
            this.Logger.Debug("Rollback of update of update application (remove temporary directories)");
            this.FileUtility.DeleteDirectory(this.tempProgs);
            if (this.tempConfig != null)
            {
                this.FileUtility.DeleteDirectory(this.tempConfig);
            }
        }

        private void InstallToFolder(FolderUpdate folder, string directory)
        {
            this.Logger.Trace("Installing files to {0}", directory);
            this.FileSystem.CreateDirectory(directory);

            foreach (var item in folder.Items)
            {
                this.CheckCancelled();
                var path = Path.Combine(directory, item.Name);
                var file = item as FileUpdate;
                if (file != null)
                {
                    this.ExportResource(new ResourceId(file.Hash), path);
                    continue;
                }

                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    this.InstallToFolder(subFolder, path);
                }
            }
        }
    }
}