// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultInstaller.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultInstaller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.Core.Utility;

    /// <summary>
    /// Installer for "normal" applications, i.e. not Update itself.
    /// This installer also handles installing the System Manager after all other applications.
    /// </summary>
    public class DefaultInstaller : InstallationEngineBase
    {
        /// <summary>
        /// The install directory name (not the full path!).
        /// </summary>
        public static readonly string InstallDirectory = "Install" + Path.DirectorySeparatorChar;

        /// <summary>
        /// The backup directory name (not the full path!).
        /// </summary>
        public static readonly string BackupDirectory = "Backup" + Path.DirectorySeparatorChar;

        private const double ProgressSection = 1.0 / 6;

        private readonly UpdateSet updateSet;

        private readonly string installDir;

        private readonly string backupDir;

        private readonly List<ApplicationInfo> exitedApplications = new List<ApplicationInfo>();

        private double progressOffset;

        private ApplicationInfo exitedSystemManager;

        private Predicate<UpdateSubNode> currentFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultInstaller"/> class.
        /// </summary>
        /// <param name="updateSet">
        /// The update set to install.
        /// </param>
        public DefaultInstaller(UpdateSet updateSet)
        {
            this.updateSet = updateSet;

            this.installDir = PathManager.Instance.CreatePath(FileType.Data, InstallDirectory);
            this.backupDir = PathManager.Instance.CreatePath(FileType.Data, BackupDirectory);
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
            this.Logger.Info("Installing normal update");
            IList<ApplicationInfo> apps;
            try
            {
                apps = host.GetRunningApplications();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Install Didn't get the running applications");
                apps = host.GetRunningApplications();
            }

            this.CheckCancelled();

            this.Logger.Debug("Install Deleting temporary directories");
            this.FileUtility.ClearDirectory(this.installDir);
            this.FileUtility.ClearDirectory(this.backupDir);

            this.State = UpdateState.Installing;


            // TODO: how do we figure out which application is the System Manager?
            var systemManager = this.FindApplication("System Manager", apps);
            string systemManagerPath;
            string systemManagerConfigPath;
            this.GetSystemManagerPaths(systemManager, out systemManagerPath, out systemManagerConfigPath);

            var systemManagerUpdate = TreeHelper.FindFolder(this.updateSet, systemManagerPath);
            var systemManagerConfigUpdate = TreeHelper.FindFolder(this.updateSet, systemManagerConfigPath);
            this.currentFilter = item => item != systemManagerUpdate && item != systemManagerConfigUpdate;

            this.Logger.Debug("Found {0} System Manager app update", systemManagerUpdate == null ? "no" : "a");
            this.Logger.Debug("Found {0} System Manager config update", systemManagerConfigUpdate == null ? "no" : "a");
            this.GetNewFiles(this.currentFilter);
            this.progressOffset += ProgressSection;
            var touchedApplications = this.FindTouchedApplications(apps);
            var updateSystemManager = touchedApplications.Remove(systemManager);

            this.Logger.Debug("Found {0} applications touched by update", touchedApplications.Count);
            if (this.Logger.IsTraceEnabled)
            {
                foreach (var app in touchedApplications)
                {
                    this.Logger.Trace(" - {0}", app.Path);
                }
            }

            this.ExitApplications(host, touchedApplications, "Updating");
            this.progressOffset += ProgressSection;
            this.BackupAndInstall(this.currentFilter);
            this.progressOffset += 2 * ProgressSection;
            this.SectionProgress(0, "Clearing temporary files");
            this.FileUtility.ClearDirectory(this.installDir);
            this.FileUtility.ClearDirectory(this.backupDir);

            if (updateSystemManager)
            {
                this.currentFilter =
                    item =>
                    item == systemManagerUpdate || item == systemManagerConfigUpdate
                    || item.FindParent(f => f == systemManagerUpdate) != null
                    || item.FindParent(f => f == systemManagerConfigUpdate) != null;
                this.InstallSystemManager(host, apps, systemManager, this.currentFilter);
                this.progressOffset += ProgressSection;
                return false;
            }

            // prevent a rollback since we are already restarting applications
            this.currentFilter = null;
            this.Logger.Debug("No update for System Manager, relaunching all applications");
            foreach (var app in touchedApplications)
            {
                this.exitedApplications.Remove(app);
                host.RelaunchApplication(app, "Update done");
            }

            // Force restart of Composer and DirectXRenderer
            IList<ApplicationInfo> forceRestartApps = new List<ApplicationInfo>();
            var composer = this.FindApplication("Composer", touchedApplications);
            var audioRenderer = this.FindApplication("Audio Renderer", touchedApplications);
            var directXRenderer = this.FindApplication("DirectX Renderer", touchedApplications);

            if (composer == null)
            {
                composer = this.FindApplication("Composer", apps);
                if (composer != null)
                {
                    forceRestartApps.Add(composer);
                }
            }

            if (audioRenderer == null)
            {
                audioRenderer = this.FindApplication("Audio Renderer", apps);
                if (audioRenderer != null)
                {
                    forceRestartApps.Add(audioRenderer);
                }
            }

            if (directXRenderer == null)
            {
                directXRenderer = this.FindApplication("DirectX Renderer", apps);
                if (directXRenderer != null)
                {
                    forceRestartApps.Add(directXRenderer);
                }
            }

            if (forceRestartApps.Count > 0)
            {
                this.ExitApplications(host, forceRestartApps, "Updating");
                foreach (var app in forceRestartApps)
                {
                    host.RelaunchApplication(app, "Update done");
                }
            }

            this.Cleanup();
            this.State = UpdateState.Installed;
            this.Logger.Info("Installation completed");
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
            if (this.currentFilter != null)
            {
                foreach (var folder in this.updateSet.Folders)
                {
                    var original = Path.Combine(this.InstallationRoot.FullName, folder.Name);
                    var backup = Path.Combine(this.backupDir, folder.Name);
                    this.Logger.Debug("Rollback of {0} to {1}", backup, original);

                    this.RollbackBackupFiles(folder, backup, original, this.currentFilter);
                }
            }

            if (this.exitedSystemManager != null)
            {
                this.RelaunchSystemManagerAndExit(this.exitedSystemManager, host);
                return;
            }

            this.Cleanup();

            foreach (var app in this.exitedApplications)
            {
                host.RelaunchApplication(app, "Rollback of update completed");
            }
        }

        private static bool IsExited(IApplicationStateObserver observer)
        {
            return observer.State == ApplicationState.Exited || observer.State == ApplicationState.Unknown;
        }

        private void GetSystemManagerPaths(
            ApplicationInfo systemManager, out string systemManagerPath, out string systemManagerConfigPath)
        {
            if (systemManager == null)
            {
                this.Logger.Warn("System Manager not found");

                // we use a "path" that will never exist on a real system:
                systemManagerPath = "?";
                systemManagerConfigPath = "?";
                return;
            }

            this.Logger.Debug("System Manager: {0}", systemManager.Path);
            systemManagerPath = Path.GetDirectoryName(systemManager.Path);
            Debug.Assert(systemManagerPath != null, "systemManagerPath can't be null");

            var systemManagerName = Path.GetFileName(systemManager.Path);
            systemManagerConfigPath = PathManager.Instance.GetPathManager(systemManagerName)
                                                 .GetPath(FileType.Config, string.Empty);
            systemManagerPath = systemManagerPath.Substring(this.InstallationRoot.FullName.Length);
            systemManagerConfigPath = systemManagerConfigPath.Substring(this.InstallationRoot.FullName.Length);

            this.Logger.Trace("System Manager app: {0}", systemManagerPath);
            this.Logger.Trace("System Manager config: {0}", systemManagerConfigPath);
        }

        private ApplicationInfo FindApplication(string appName, IEnumerable<ApplicationInfo> apps)
        {
            return apps.FirstOrDefault(app => app.Name.Equals(appName, StringComparison.InvariantCultureIgnoreCase));
        }

        private void GetNewFiles(Predicate<UpdateSubNode> filter)
        {
            this.Logger.Debug("Getting new files from resources");
            this.Logger.Trace("Temporary installation directory: {0}", this.installDir);

            var progress = 0;
            var maxProgress = this.updateSet.Folders.Count + 1.0;

            // export all new resources to the temporary "Install" directory
            foreach (var folder in this.updateSet.Folders)
            {
                var install = Path.Combine(this.installDir, folder.Name);
                this.SectionProgress((++progress) / maxProgress, "Get new files: {0}", folder.Name);
                this.GetNewFiles(folder, install, filter);
            }
        }

        private void BackupAndInstall(Predicate<UpdateSubNode> filter)
        {
            this.Logger.Debug("Backing up old files and installing new files");

            this.Logger.Trace("Temporary installation directory: {0}", this.installDir);
            this.Logger.Trace("Temporary backup directory: {0}", this.backupDir);

            var progress = 0;
            var maxProgress = this.updateSet.Folders.Count + 2.0;

            foreach (var folder in this.updateSet.Folders)
            {
                var original = Path.Combine(this.InstallationRoot.FullName, folder.Name);
                var backup = Path.Combine(this.backupDir, folder.Name);
                var install = Path.Combine(this.installDir, folder.Name);

                this.SectionProgress((++progress) / maxProgress, "Backup old files: {0}", original);
                this.BackupOldFiles(folder, original, backup, filter);

                this.SectionProgress((++progress) / maxProgress, "Install new files: {0}", original);
                this.InstallNewFiles(folder, install, original, filter);
            }
        }

        private void InstallSystemManager(
            IInstallationHost host,
            IEnumerable<ApplicationInfo> apps,
            ApplicationInfo systemManager,
            Predicate<UpdateSubNode> filter)
        {
            this.Logger.Info("Installing update of System Manager");
            this.GetNewFiles(filter);

            var toExit = new List<ApplicationInfo>(apps);
            toExit.Remove(systemManager);
            toExit.RemoveAll(i => host.ExecutablePath.Equals(i.Path, StringComparison.InvariantCultureIgnoreCase));
            this.ExitApplications(host, toExit, "Updating System Manager");

            // the above updates the progress (1 section)
            this.progressOffset += ProgressSection;

            var systemManagerProcess = ProcessFinder.FindProcess(systemManager.Path);

            var progress = 0.0;
            this.SectionProgress(progress, "Exiting System Manager");
            host.ExitApplication(systemManager, "Updating System Manager");
            this.exitedSystemManager = systemManager;

            if (systemManagerProcess == null)
            {
                this.Logger.Warn("Couldn't find System Manager in list of running processes: {0}", systemManager.Path);
            }
            else
            {
                for (int i = 0; i < 24 && !systemManagerProcess.HasExited; i++)
                {
                    progress += 0.04;
                    this.SectionProgress(progress, "Waiting for System Manager exit");
                    this.SleepSeconds(5);
                }

                this.Logger.Debug("System Manager has exited");
            }

            this.BackupAndInstall(filter);

            this.RelaunchSystemManagerAndExit(systemManager, host);
        }

        private void Cleanup()
        {
            this.Logger.Debug("Final Cleanup...");
            try
            {
                this.FileUtility.DeleteDirectory(this.installDir);
            }
            catch (Exception ex)
            {
                Logger.Trace(ex, $"Couldn't cleanup '{this.installDir}'");
                throw new InstallationCancelledException($"Couldn't cleanup '{this.installDir}'", ex);
            }

            try
            {
                this.FileUtility.DeleteDirectory(this.backupDir);
            }
            catch (Exception ex)
            {
                Logger.Trace(ex, $"Couldn't cleanup '{this.backupDir}'");
            }
        }

        private void RelaunchSystemManagerAndExit(ApplicationInfo systemManager, IInstallationHost host)
        {
            this.Logger.Info("Relaunching System Manager: {0}", systemManager.Path);
            var directory = Path.GetDirectoryName(systemManager.Path);
            Debug.Assert(directory != null, "directory can't be null");

            var args = string.Format("/waitforexit {0}", Process.GetCurrentProcess().Id);
            var start = new ProcessStartInfo(systemManager.Path, args);
            start.WorkingDirectory = directory;
            host.StartProcess(start);

            this.Cleanup();

            host.ForceExit();
        }

        private void ExitApplications(
            IInstallationHost host, IList<ApplicationInfo> touchedApplications, string reason)
        {
            var observers = new List<IApplicationStateObserver>(touchedApplications.Count);
            foreach (var app in touchedApplications)
            {
                var observer = host.CreateApplicationStateObserver(app);
                if (IsExited(observer))
                {
                    this.Logger.Debug("Application '{0}' has already exited ({1})", app.Name, observer.State);
                    observer.Dispose();
                    continue;
                }

                observers.Add(observer);
                this.exitedApplications.Add(app);
                host.ExitApplication(app, reason);
            }

            if (observers.Count == 0)
            {
                return;
            }

            // TODO: this should be asynchronous
            // total wait time max: 120 seconds
            for (int i = 0; i < 24; i++)
            {
                var running = this.CheckAllExited(observers);
                this.SectionProgress(
                    (observers.Count - running) / (double)observers.Count, "Exiting applications: {0} left", running);
                if (running == 0)
                {
                    break;
                }

                this.SleepSeconds(5);
            }

            // TODO: perhaps we should once more try to exit the application if it didn't exit???
            foreach (var observer in observers)
            {
                observer.Dispose();
            }
        }

        private void SleepSeconds(int seconds)
        {
            // "sleep" for n seconds, but checking the cancelled flag every second
            for (int i = 0; i < seconds; i++)
            {
                this.CheckCancelled();
                Thread.Sleep(1000);
            }
        }

        private int CheckAllExited(IEnumerable<IApplicationStateObserver> observers)
        {
            int running = 0;
            foreach (var observer in observers)
            {
                if (IsExited(observer))
                {
                    continue;
                }

                this.Logger.Trace(
                    "Application '{0}' has not yet exited ({1})", observer.ApplicationName, observer.State);
                running++;
            }

            return running;
        }

        private IList<ApplicationInfo> FindTouchedApplications(IList<ApplicationInfo> apps)
        {
            var touched = new List<ApplicationInfo>(apps.Count);

            foreach (var app in apps)
            {
                if (this.IsAppTouched(app))
                {
                    touched.Add(app);
                }
            }

            return touched;
        }

        private bool IsAppTouched(ApplicationInfo app)
        {
            if (app.Path.StartsWith(this.InstallationRoot.FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                var appSubPath = app.Path.Substring(this.InstallationRoot.FullName.Length);
                foreach (var rootFolder in this.updateSet.Folders)
                {
                    foreach (var item in rootFolder.Items)
                    {
                        if (appSubPath.StartsWith(
                            string.Format("Progs{1}{0}{1}", item.Name, Path.DirectorySeparatorChar),
                            StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var dependency in this.RestartApplications.Dependencies)
            {
                if (dependency.ExecutablePaths.Find(
                    p => p.Equals(app.Path, StringComparison.InvariantCultureIgnoreCase)) == null)
                {
                    continue;
                }

                var path = Path.GetDirectoryName(dependency.Path);
                if (path == null ||
                    !path.StartsWith(this.InstallationRoot.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Logger.Warn("Invalid dependency path configured: {0}", dependency.Path);
                    continue;
                }

                path = path.Substring(this.InstallationRoot.FullName.Length);
                var folder = TreeHelper.FindFolder(this.updateSet, path);
                if (folder == null)
                {
                    continue;
                }

                var file = Path.GetFileName(dependency.Path);
                if (string.IsNullOrEmpty(file))
                {
                    return true;
                }

                if (folder.Items.Find(i => i.Name.Equals(file, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void SectionProgress(double value, string format, params object[] args)
        {
            this.Progress((value * ProgressSection) + this.progressOffset, string.Format(format, args));
        }
    }
}