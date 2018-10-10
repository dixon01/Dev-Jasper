// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelfUpdateController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelfUpdateController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.SelfUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.SystemManagement.ServiceModel;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.OSWrappers.Process;
    using Gorba.Motion.Update.Core.Utility;

    using NLog;

    /// <summary>
    /// Controller responsible for installing Update.exe (i.e. updating itself).
    /// </summary>
    public class SelfUpdateController : IUpdateController
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SelfUpdateController>();

        private static readonly int GetApplicationListTimeout = 10 * 1000;
        private static readonly int GetApplicationListRetries = 5;

        private readonly string updateCommandFile;

        private readonly string targetPath;

        private readonly int waitForExitProcessId;

        private readonly IWritableFileSystem fileSystem;
        private readonly FileUtility fileUtility;

        private readonly ManualResetEvent relaunchWait = new ManualResetEvent(true);

        private string tempProgs;
        private string tempConfig;

        private string targetProgs;
        private string targetConfig;

        private string backupProgs;
        private string backupConfig;

        private bool rollbackPerformed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfUpdateController"/> class.
        /// </summary>
        /// <param name="updateCommandFile">
        /// The full path to the update command file.
        /// </param>
        /// <param name="targetPath">
        /// The target path where to install Update.exe.
        /// </param>
        /// <param name="waitForExitProcessId">
        /// The ID of the process to wait for before continuing the installation.
        /// </param>
        public SelfUpdateController(string updateCommandFile, string targetPath, int waitForExitProcessId)
        {
            this.updateCommandFile = updateCommandFile;
            this.targetPath = targetPath;
            this.waitForExitProcessId = waitForExitProcessId;

            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
            this.fileUtility = new FileUtility(this.fileSystem);
        }

        /// <summary>
        /// Starts the update controller
        /// </summary>
        /// <param name="app">
        /// The <see cref="UpdateApplication"/> starting this controller.
        /// </param>
        /// <exception cref="AbandonedMutexException">The wait completed because a thread exited without releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.</exception>
        public void Start(UpdateApplication app)
        {
            Logger.Info("Executing self-update of '{0}' to '{1}'", this.updateCommandFile, this.targetPath);
            this.relaunchWait.Set();

            try
            {
                var getAppInfos = SystemManagerClient.Instance.BeginGetApplicationInfos(null, null);
                try
                {
                    var configurator = new Configurator(this.updateCommandFile);
                    var command = configurator.Deserialize<UpdateCommand>();

                    getAppInfos = SystemManagerClient.Instance.BeginGetApplicationInfos(null, null);
                    this.WaitForParentProcess();
                    this.CopyFiles(command);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Couldn't execute self-update");
                    this.Rollback();
                }

                try
                {
                    this.fileUtility.DeleteFile(this.updateCommandFile);

                    // finally we delete the backup directories
                    Logger.Debug("Deleting backup directories");
                    this.fileUtility.DeleteDirectory(this.backupProgs);
                    this.fileUtility.DeleteDirectory(this.backupConfig);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Couldn't delete self-update files");
                }

                this.RelaunchUpdate(getAppInfos);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to execute self-update");
            }
            finally
            {
                app.Exit("Self-update done");
                // Bad SystemManagerClient.Instance.Reboot("Self-update completed");
                //SystemManagerClient.Instance.RelaunchApplication(//TODo);
                // Wait one minute for the event signaled
                for (var i = 0; !this.relaunchWait.WaitOne(1000, false) && i < 60; i++)
                {
                    Logger.Info("Waiting for launch of Update.exe");
                }
            }
        }

        /// <summary>
        /// Stops the update controller
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();
        }

        private void WaitForParentProcess()
        {
            Process parentProcess;
            try
            {
                parentProcess = this.waitForExitProcessId > 0
                                    ? Process.GetProcessById(this.waitForExitProcessId)
                                    : ProcessHelper.GetParentProcess();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't get parent process, assuming it already exited");
                return;
            }

            if (parentProcess == null)
            {
                Logger.Info("Couldn't find parent process, assuming it already exited");
                return;
            }

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; !parentProcess.HasExited && i < 60; i++)
                {
                    Logger.Info(
                        "Waiting for parent process {0} ({1}) to exit...",
                        ProcessFinder.GetName(parentProcess),
                        parentProcess.Id);
                    Thread.Sleep(1000);
                }

                if (parentProcess.HasExited)
                {
                    break;
                }

                Logger.Warn("Parent process did not exit, killing it");
                parentProcess.Kill();
            }

            Logger.Info("Parent process has exited");
        }

        private void CopyFiles(UpdateCommand command)
        {
            Logger.Debug(
                "Self-update of {0} ({1})", command.UpdateId.BackgroundSystemGuid, command.UpdateId.UpdateIndex);
            var sourceProgs = PathManager.Instance.GetPath(FileType.Application, string.Empty);
            var sourceConfig = PathManager.Instance.GetPath(FileType.Config, string.Empty);
            Logger.Debug("Source application directory: {0}", sourceProgs);
            Logger.Debug("Source configuration directory: {0}", sourceConfig);

            var targetApp = Path.GetFullPath(this.targetPath);
            if (targetApp.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                targetApp = Path.GetDirectoryName(targetApp);
            }

            var tempApp = string.Format("{0}_{1}", targetApp, Guid.NewGuid());
            var tempManager = PathManager.Instance.GetPathManager(Path.GetFileName(tempApp));
            this.tempProgs = tempManager.CreatePath(FileType.Application, string.Empty);
            this.tempConfig = tempManager.CreatePath(FileType.Config, string.Empty);
            Logger.Debug("Temporary application directory: {0}", this.tempProgs);
            Logger.Debug("Temporary configuration directory: {0}", this.tempConfig);

            var targetManager = PathManager.Instance.GetPathManager(Path.GetFileName(targetApp));
            this.targetProgs = targetManager.CreatePath(FileType.Application, string.Empty);
            this.targetConfig = targetManager.CreatePath(FileType.Config, string.Empty);
            Logger.Debug("Target application directory: {0}", this.targetProgs);
            Logger.Debug("Target configuration directory: {0}", this.targetConfig);

            var backupApp = targetApp + FileDefinitions.BackupFileExtension;
            var backupManager = PathManager.Instance.GetPathManager(Path.GetFileName(backupApp));
            this.backupProgs = backupManager.CreatePath(FileType.Application, string.Empty);
            this.backupConfig = backupManager.CreatePath(FileType.Config, string.Empty);
            Logger.Debug("Backup application directory: {0}", this.backupProgs);
            Logger.Debug("Backup configuration directory: {0}", this.backupConfig);

            // first we copy ourselves to the temp directories
            Logger.Debug("Copying source to temporary directories");
            this.fileUtility.CopyDirectory(sourceProgs, this.tempProgs);
            this.fileUtility.CopyDirectory(sourceConfig, this.tempConfig);

            // we need to delete the temporary EXE file if we are not called "Update.exe"
            this.DeleteTemporaryExe(this.tempProgs);

            // then we rename the target directories to *.bak
            Logger.Debug("Moving target to backup directories");
            this.fileUtility.MoveDirectory(this.targetProgs, this.backupProgs);
            this.fileUtility.MoveDirectory(this.targetConfig, this.backupConfig);

            // now we move the temp directories to the taget paths
            Logger.Debug("Moving temporary to target directories");
            this.fileUtility.MoveDirectory(this.tempProgs, this.targetProgs);
            this.fileUtility.MoveDirectory(this.tempConfig, this.targetConfig);
        }

        private void DeleteTemporaryExe(string appDirectory)
        {
            var currentExe = ApplicationHelper.GetEntryAssemblyLocation();
            var exeName = Path.GetFileName(currentExe);
            if (exeName == null || exeName.StartsWith("Update.", StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.Debug("There is no copy of temporary EXE");
                return;
            }

            var tempExe = Path.Combine(appDirectory, exeName);
            IWritableFileInfo tempExeFile;
            if (!this.fileSystem.TryGetFile(tempExe, out tempExeFile))
            {
                Logger.Debug("Copy of temporary EXE doesn't exist");
                return;
            }

            Logger.Debug("Deleting temporary EXE: {0}", tempExeFile.FullName);
            tempExeFile.Delete();
        }

        private void RelaunchUpdate(IAsyncResult getAppInfos)
        {
            for (int i = 0; i < GetApplicationListRetries; i++)
            {
                if (getAppInfos.AsyncWaitHandle.WaitOne(GetApplicationListTimeout, false))
                {
                    break;
                }

                Logger.Warn(
                    "Couldn't get list of applications from System Manager, retrying ({0}/{1})",
                    i + 1,
                    GetApplicationListRetries);
                getAppInfos = SystemManagerClient.Instance.BeginGetApplicationInfos(null, null);
            }

            if (getAppInfos.AsyncWaitHandle.WaitOne(GetApplicationListTimeout, false))
            {
                // we relaunch all applications in the given target path
                var apps = SystemManagerClient.Instance.EndGetApplicationInfos(getAppInfos);
                if (this.RelaunchUpdate(apps))
                {
                    return;
                }

                Logger.Warn("Couldn't find Update in list of applications");
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("Searching for {0} in:", this.targetProgs);
                    foreach (var app in apps)
                    {
                        Logger.Trace(" - {0}: {1}", app.Name, app.Path);
                    }
                }
            }
            else
            {
                Logger.Warn(
                    "Couldn't get list of applications from System Manager, trying to find application manually");
            }

            var directory = this.fileSystem.GetDirectory(this.targetProgs);
            foreach (var file in directory.GetFiles())
            {
                if (file.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase))
                {
                    Logger.Debug("Manually launching {0}", file.FullName);
                    var start = new ProcessStartInfo(file.FullName, string.Empty);
                    start.WorkingDirectory = directory.FullName;
                    Process.Start(start);
                    return;
                }
            }

            Logger.Error("Couldn't find Update.exe to start, rolling back update");
            this.Rollback();
            getAppInfos = SystemManagerClient.Instance.BeginGetApplicationInfos(null, null);
            this.RelaunchUpdate(getAppInfos);
        }

        private bool RelaunchUpdate(IEnumerable<ApplicationInfo> apps)
        {
            foreach (var appInfo in apps)
            {
                var path = appInfo.Path;
                if (!path.StartsWith(this.targetProgs, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                this.relaunchWait.Reset();
                var observer = SystemManagerClient.Instance.CreateApplicationStateObserver(appInfo);
                observer.StateChanged += (s, e) =>
                    {
                        if (observer.State == ApplicationState.AwaitingLaunch
                            || observer.State == ApplicationState.Launching)
                        {
                            Logger.Debug("{0} is now in state {1}, we can exit", path, observer.State);
                            this.relaunchWait.Set();
                            observer.Dispose();
                        }
                    };

                Logger.Info("Relaunching {0} with System Manager", path);
                SystemManagerClient.Instance.RelaunchApplication(appInfo, "Self-update completed");
                return true;
            }

            return false;
        }

        private void Rollback()
        {
            if (this.rollbackPerformed)
            {
                throw new UpdateException("Rollback can't be performed twice");
            }

            this.rollbackPerformed = true;
            Logger.Info("Rollback of update");
            if (this.targetProgs == null || this.targetConfig == null)
            {
                Logger.Debug("Target paths not defined, nothing to do.");
                return;
            }

            if (this.backupProgs == null || this.backupConfig == null)
            {
                Logger.Debug("Backup paths not defined, nothing to do.");
                return;
            }

            Logger.Debug("Deleting target directories");
            this.fileUtility.DeleteDirectory(this.targetProgs);
            this.fileUtility.DeleteDirectory(this.targetConfig);

            Logger.Debug("Moving backup to target directories");
            this.fileUtility.MoveDirectory(this.backupProgs, this.targetProgs);
            this.fileUtility.MoveDirectory(this.backupConfig, this.targetConfig);
        }
    }
}