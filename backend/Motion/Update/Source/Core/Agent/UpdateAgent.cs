// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="UpdateAgent.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Motion.Update.Core.Persistence;
    using Gorba.Motion.Update.Core.Utility;

    using Microsoft.Practices.ServiceLocation;

#if WindowsCE
    using FileSystemEventArgs = OpenNETCF.IO.FileSystemEventArgs;

    // using FileSystemMonitor instead of FileSystemWatcher since it doesn't require a UI
    using FileSystemWatcher = OpenNETCF.IO.FileSystemMonitor;
    using NotifyFilters = OpenNETCF.IO.NotifyFilters;
#endif

    /// <summary>
    ///     Class to execute an update
    /// </summary>
    public class UpdateAgent : UpdateSinkBase, IInstallationHost
    {
        #region Static Fields

        private static readonly string InstallQueueFileFormat = "next-{0}" + FileDefinitions.UpdateCommandExtension;

        private static readonly TimeSpan ProcessFailureTimeout = TimeSpan.FromSeconds(10);

        #endregion

        #region Fields

        private readonly string currentUpdateFile;

        private readonly IWritableFileSystem fileSystem;

        private readonly FileUtility fileUtility;

        private readonly ITimer fileWatcherTimeoutTimer;

        private readonly ManualResetEvent installationEndWait = new ManualResetEvent(true);

        private readonly object installationLock = new object();

        private readonly IApplicationRegistration registration;

        private readonly UpdateValet updateValet;

        private AgentConfig agentConfig;

        private ProgressWrapper installationProgress;

        private IWritableDirectoryInfo installationRoot;

        private IPersistenceContext<UpdateAgentPersistence> persistenceContext;

        private bool running;

        private FileSystemWatcher watcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateAgent" /> class.
        /// </summary>
        public UpdateAgent()
        {
            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
            this.fileUtility = new FileUtility(this.fileSystem);
            this.currentUpdateFile = PathManager.Instance.CreatePath(FileType.Data, "current" + FileDefinitions.UpdateCommandExtension);

            this.registration = ServiceLocator.Current.GetInstance<IApplicationRegistration>("Update");

            this.SetUpPersistence();

            this.fileWatcherTimeoutTimer = TimerFactory.Current.CreateTimer("FileWatcherTimeout");
            this.fileWatcherTimeoutTimer.Elapsed += this.FileWatcherTimerOnElapsed;
            this.fileWatcherTimeoutTimer.Interval = TimeSpan.FromSeconds(10);
            this.fileWatcherTimeoutTimer.AutoReset = false;

            this.ConfigureLogFileWatcher();

            this.updateValet = new UpdateValet(this.persistenceContext.Value.ParkedUpdateCommands);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the list of handled units. One of the unit name might be a wildcard
        ///     (<see cref="UpdateComponentBase.UnitWildcard" />) to tell the user of this
        ///     class that the sink is interested in all updates.
        /// </summary>
        public override IEnumerable<string> HandledUnits
        {
            get
            {
                yield return this.UnitName;
            }
        }

        /// <summary>
        ///     Gets the unique name of this agent.
        /// </summary>
        public override string Name
        {
            get
            {
                // the class name is unique since we only have a single instance of
                // UpdateAgent in each update controller
                return "$UpdateAgent";
            }
        }

        /// <summary>
        ///     Gets the name of this unit (usually the PC name).
        /// </summary>
        public string UnitName { get; private set; }

        #endregion

        #region Explicit Interface Properties

        string IInstallationHost.ExecutablePath
        {
            get
            {
                return ApplicationHelper.GetEntryAssemblyLocation();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the update context.
        ///     This property is valid after calling <see cref="Configure" />.
        /// </summary>
        protected IUpdateContext Context { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Configures the update agent</summary>
        /// <param name="context">The update context.</param>
        /// <param name="config">The update configuration.</param>
        /// <exception cref="DirectoryNotFoundException">
        /// if the given directory can't be found.
        /// </exception>
        public void Configure(IUpdateContext context, AgentConfig config)
        {
            this.Context = context;
            this.agentConfig = config;
            Logger.Debug("config.InstallationRoot=[{0}]", config.InstallationRoot);
            string root;
            try
            {
                root = string.IsNullOrEmpty(config.InstallationRoot) ? PathManager.Instance.GetInstallationRoot() : config.InstallationRoot;
                if (config.Enabled)
                {
                    IWritableDirectoryInfo writableDirectoryInfo;
                    if (!this.fileSystem.TryGetDirectory(root, out writableDirectoryInfo))
                    {
                        Logger.Warn("InstallationRoot directory not found {0}. Creating it now...", root);
                        this.fileSystem.CreateDirectory(root);
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                root = ApplicationHelper.CurrentDirectory;
                Logger.Warn("Config() failed getting installation root path {0}, using default root path = {1}", ex.Message, root);
            }

            this.installationRoot = this.fileSystem.GetDirectory(root);
            this.UnitName = string.IsNullOrEmpty(config.UnitName) ? ApplicationHelper.MachineName : config.UnitName;
        }

        /// <summary>Handles the update commands by forwarding them.</summary>
        /// <param name="commands">The update commands.</param>
        /// <param name="progressMonitor">The progress monitor that observes the upload of the update command.</param>
        public override void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor)
        {
            this.SelectCommandToInstall(commands, true);
        }

        /// <summary>
        ///     Starts the update agent
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.watcher.EnableRaisingEvents = true;
            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} : Starting");

            this.DeleteSelfUpdateDirectories();

            // start the timer to get initial files that might be sitting around when the application starts
            this.fileWatcherTimeoutTimer.Enabled = true;

            this.updateValet.UpdateDeparked += this.UpdateValetOnUpdateDeparked;

            IFileInfo currentFile;
            if (!this.fileSystem.TryGetFile(this.currentUpdateFile, out currentFile))
            {
                this.Logger.Debug("No update in progress found: {0}", this.currentUpdateFile);
                this.IsAvailable = true;
            }
            else
            {
                this.Logger.Debug("Found an update in progress, will run it: {0}", this.currentUpdateFile);
                try
                {
                    var command = this.DeserializeCommand(this.currentUpdateFile);
                    this.StartInstallation(command);
                    return;
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't deserialize {0}", this.currentUpdateFile);
                }
            }

            this.updateValet.Start();
        }

        /// <summary>
        ///     Stops the update agent
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current instance has already been disposed. </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents an infinite time-out. </exception>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.IsAvailable = false;
            this.Logger.Debug("Stopping");
            var progress = this.installationProgress;
            if (progress != null)
            {
                progress.Cancel();
            }

            this.watcher.EnableRaisingEvents = false;
            this.watcher.Dispose();

            this.updateValet.UpdateDeparked -= this.UpdateValetOnUpdateDeparked;
            this.updateValet.Stop();

            // wait a maximum of 180 seconds (3 minutes) for the installation to end
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    if (this.installationEndWait.WaitOne(6 * 1000, false))
                    {
                        this.Logger.Trace("Possible pending installation has completed");
                        break;
                    }
                }
                catch (AbandonedMutexException)
                {
                }

                this.Logger.Trace("Still waiting for installation to be completed");
            }

            this.Logger.Info("Stopped");
        }

#endregion

#region Explicit Interface Methods

        IApplicationStateObserver IInstallationHost.CreateApplicationStateObserver(ApplicationInfo application)
        {
            return SystemManagerClient.Instance.CreateApplicationStateObserver(application);
        }

        void IInstallationHost.Exit(string reason)
        {
            this.Logger.Info("Exiting myself: {0}", reason);
            this.registration.Exit(reason);
        }

        void IInstallationHost.ExitApplication(ApplicationInfo application, string reason)
        {
            this.Logger.Info("Exiting '{0}': {1}", application.Name, reason);
            SystemManagerClient.Instance.ExitApplication(application, reason);
        }

        void IInstallationHost.ForceExit()
        {
            this.Logger.Info("Forcing exit of myself");
            try
            {
                // Stop() below will block until this event is set, so we have to do this manually here
                this.installationEndWait.Set();
                ServiceLocator.Current.GetInstance<UpdateApplication>().Stop();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't exit myself");
                ApplicationHelper.Exit(-1);
            }
        }

        IList<ApplicationInfo> IInstallationHost.GetRunningApplications()
        {
            var result = SystemManagerClient.Instance.BeginGetApplicationInfos(null, null);
            if (!result.AsyncWaitHandle.WaitOne(10 * 1000, false))
            {
                throw new TimeoutException("Didn't get the running applications from System Manager within 10 seconds");
            }

            return SystemManagerClient.Instance.EndGetApplicationInfos(result);
        }

        void IInstallationHost.Relaunch(string reason)
        {
            this.Logger.Info("Relaunching myself: {0}", reason);
            this.registration.Relaunch(reason);
        }

        void IInstallationHost.RelaunchApplication(ApplicationInfo application, string reason)
        {
            this.Logger.Info("Relaunching '{0}': {1}", application.Name, reason);
            SystemManagerClient.Instance.RelaunchApplication(application, reason);
        }

        void IInstallationHost.StartProcess(ProcessStartInfo startInfo)
        {
            this.Logger.Info("Starting '{0}' {1}", startInfo.FileName, startInfo.Arguments);
            var process = Process.Start(startInfo);
            if (process != null && process.WaitForExit((int)ProcessFailureTimeout.TotalMilliseconds))
            {
                // if the process exits within the given timespan, we assume there was an error
                throw new UpdateException(string.Format("Process has unexpectedly exited ({0}): {1}", process.ExitCode, startInfo.FileName));
            }
        }

#endregion

#region Methods

        /// <summary>
        ///     Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        ///     True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            return this.installationProgress == null && this.running;
        }

        private void ConfigureLogFileWatcher()
        {
            // TODO: verify if the path without directory delimiter still works in FX 2.0
            var archivePath = PathManager.Instance.CreatePath(FileType.Log, "Archives");
            this.fileSystem.CreateDirectory(archivePath + Path.DirectorySeparatorChar);

            this.watcher = new FileSystemWatcher
                               {
                                   Path = archivePath, 
                                   Filter = "*.*", 
                                   NotifyFilter =
                                       NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName
                               };
            this.watcher.Created += this.OnLogFileChanged;
            this.watcher.Changed += this.OnLogFileChanged;
        }

        private UpdateStateInfo CreateFeedback(UpdateCommand updateCommand, UpdateState state, string errorReason)
        {
            var factory = new UpdateStateInfoFactory(updateCommand, this.installationRoot);
            return factory.CreateFeedback(state, errorReason);
        }

        private void DeleteLogFile(string fileName)
        {
            try
            {
                FileUtility.DeleteFile(this.fileSystem.GetFile(fileName));
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't delete log file {0}", fileName);
            }
        }

        private void DeleteSelfUpdateDirectories()
        {
            var pathManager = PathManager.Instance;
            var progs = pathManager.GetPath(FileType.Application, string.Empty);
            var config = pathManager.GetPath(FileType.Config, string.Empty);

            var progsRoot = Path.GetDirectoryName(progs);
            var configRoot = Path.GetDirectoryName(config);

            if (progsRoot == null || configRoot == null)
            {
                return;
            }

            var progsName = Path.GetFileName(progs);
            var configName = Path.GetFileName(config);

            foreach (var dir in this.fileSystem.GetDirectory(progsRoot).GetDirectories())
            {
                if (!dir.Name.StartsWith(progsName + "_"))
                {
                    continue;
                }

                for (int i = 0; i < 3; i++)
                {
                    this.Logger.Debug("Deleting temporary self-update application directory: {0}", dir.FullName);
                    try
                    {
                        FileUtility.DeleteDirectory(dir);
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, "Couldn't delete {0} trying again in 10 seconds", dir.FullName);
                    }

                    Thread.Sleep(10 * 1000);
                }
            }

            foreach (var dir in this.fileSystem.GetDirectory(configRoot).GetDirectories())
            {
                if (!dir.Name.StartsWith(configName + "_"))
                {
                    continue;
                }

                this.Logger.Debug("Deleting temporary self-update config directory: {0}", dir.FullName);
                try
                {
                    FileUtility.DeleteDirectory(dir);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't delete {0}", dir.FullName);
                }
            }
        }

        private UpdateCommand DeserializeCommand(string path)
        {
            this.Logger.Trace("Deserializing command from {0}", path);
            var configurator = new Configurator(path);
            return configurator.Deserialize<UpdateCommand>();
        }

        private void EndInstallation(UpdateCommand command, UpdateState errorState, string errorReason)
        {
            UpdateCommand nextCommand = null;
            try
            {
                if (errorState > UpdateState.Installed)
                {
                    this.SendFeedback(this.CreateFeedback(command, errorState, errorReason));
                }

                this.Logger.Info("Installation completed: {0} {1}", errorState, errorReason);
                this.installationProgress.Complete(errorReason, null);
                this.installationProgress = null;

                // check if we have a next-xxx.guc file which needs to be installed next
                var path = PathManager.Instance.GetPath(FileType.Data, string.Empty);
                var queueFileRegex = new Regex(string.Format(InstallQueueFileFormat, "[0-9]+"));
                var enqueuedUpdates = new List<string>();
                foreach (var file in this.fileSystem.GetDirectory(path).GetFiles())
                {
                    if (queueFileRegex.IsMatch(file.Name))
                    {
                        enqueuedUpdates.Add(file.FullName);
                    }
                }

                if (enqueuedUpdates.Count == 0)
                {
                    this.IsAvailable = true;
                    this.updateValet.Start();
                    return;
                }

                enqueuedUpdates.Sort(StringComparer.InvariantCultureIgnoreCase);

                this.Logger.Debug("Found enqueued command: {0}", enqueuedUpdates[0]);
                nextCommand = this.DeserializeCommand(enqueuedUpdates[0]);
                this.fileUtility.DeleteFile(enqueuedUpdates[0]);
                this.InstallCommand(nextCommand);
            }
            finally
            {
                if (nextCommand == null)
                {
                    // only notify if we are not installing the next command
                    this.installationEndWait.Set();
                }
            }
        }

        private void FileWatcherTimerOnElapsed(object sender, EventArgs e)
        {
            lock (this.fileWatcherTimeoutTimer)
            {
                var files = Directory.GetFiles(this.watcher.Path);
                if (files.Length == 0)
                {
                    this.Logger.Trace("No log files found in {0}", this.watcher.Path);
                    return;
                }

                var receivedFiles = new List<IReceivedLogFile>(files.Length);
                foreach (var logFile in files)
                {
                    receivedFiles.Add(new FileReceivedLogFile(this.UnitName, logFile));
                    this.Logger.Trace("Log file sent to queue: {0}", logFile);
                }

                //TODO : Handle uploaded files?
                var eventArgs = new FeedbackEventArgs(receivedFiles.ToArray(), new UpdateStateInfo[0], new IReceivedLogFile[0]);
                this.RaiseFeedbackReceived(eventArgs);

                foreach (var logFile in files)
                {
                    this.DeleteLogFile(logFile);
                }
            }
        }

        private void InstallCommand(UpdateCommand command)
        {
            this.updateValet.Stop();

            lock (this.installationLock)
            {
                this.Logger.Debug("Got valid command to install: {0} of {1}", command.UpdateId.UpdateIndex, command.UpdateId.BackgroundSystemGuid);
                this.persistenceContext.Value.BackgroundId = command.UpdateId.BackgroundSystemGuid;
                this.persistenceContext.Value.UpdateIndex = command.UpdateId.UpdateIndex;

                this.SaveCurrentCommandFile(command);

                this.StartInstallation(command);
            }
        }

        private void OnLogFileChanged(object source, FileSystemEventArgs e)
        {
            this.fileWatcherTimeoutTimer.Enabled = false;
            this.fileWatcherTimeoutTimer.Enabled = true;
        }

        private void ResourcesVerified(IAsyncResult ar)
        {
            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} - Trying to verify resources");
            var installer = (UpdateInstaller)ar.AsyncState;
            IList<ResourceInfo> resources;
            try
            {
                resources = installer.EndVerifyResources(ar);
               
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't finish verification of resources");
                this.EndInstallation(installer.Command, UpdateState.InstallationFailed, "Couldn't verify resources: " + ex.Message);
                return;
            }

            this.Logger.Debug("Verified all resources");
            ThreadPool.QueueUserWorkItem(st => this.RunInstallation(installer, resources));
        }

        private void Rollback(IInstallationEngine engine)
        {
            if (engine == null)
            {
                return;
            }

            this.Logger.Info("Rolling back {0}", engine.GetType().Name);
            try
            {
                engine.Rollback(this);
                this.Logger.Info("Rollback completed");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't rollback installation");
            }
        }

        private void RunCommands(UpdateCommand command, RunCommands runCommands, IList<ResourceInfo> resources)
        {
            if (runCommands == null || runCommands.Items.Count == 0)
            {
                return;
            }

            var runner = new CommandsRunner(runCommands);
            runner.CommandsUpdated += (s, e) => this.SaveCurrentCommandFile(command);
            runner.Run(resources);
        }

        private void RunInstallation(UpdateInstaller installer, IList<ResourceInfo> resources)
        {
            IInstallationEngine engine = null;
            try
            {
                this.Logger.Debug("Running pre-installation commands");
                this.RunCommands(installer.Command, installer.Command.PreInstallation, resources);

                this.Logger.Debug("Creating installation engine");
                engine = installer.CreateInstallationEngine(resources, this.agentConfig.RestartApplications);
                engine.StateChanged += (s, e) => this.SendFeedback(this.CreateFeedback(installer.Command, engine.State, null));

                this.Logger.Debug("Starting installation");
                if (!engine.Install(this))
                {
                    // notify that we are done for now
                    this.Logger.Debug("Installation partially completed");
                    this.installationEndWait.Set();
                    return;
                }

                this.Logger.Debug("Running post-installation commands");
                this.RunCommands(installer.Command, installer.Command.PostInstallation, resources);

                this.fileUtility.DeleteFile(this.currentUpdateFile);
                this.EndInstallation(installer.Command, UpdateState.Installed, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't install update");
                this.Rollback(engine);
                this.fileUtility.DeleteFile(this.currentUpdateFile);
                this.EndInstallation(
                    installer.Command, 
                    engine == null ? UpdateState.InstallationFailed : UpdateState.PartiallyInstalled, 
                    "Couldn't completely install update: " + ex.Message);
            }
        }

        private void SaveCurrentCommandFile(UpdateCommand command)
        {
            this.SerializeCommand(command, this.currentUpdateFile);
        }

        private void SelectCommandToInstall(IEnumerable<UpdateCommand> commands, bool sendTransferredState)
        {
            lock (this.installationLock)
            {
                var selector = new UpdateSelector(commands);
                selector.Search(
                    new UpdateId(this.persistenceContext.Value.BackgroundId, this.persistenceContext.Value.UpdateIndex), 
                    this.updateValet.StoredParkedUpdateCommands, 
                    sendTransferredState);
                this.SendFeedback(new List<UpdateStateInfo>(selector.Feedback).ToArray());
                var parkedcommands = new List<UpdateCommand>();
                foreach (var validatedParkedUpdateCommand in selector.ValidatedParkedUpdateCommands)
                {
                    parkedcommands.Add(validatedParkedUpdateCommand);
                }

                this.updateValet.ParkUpdates(parkedcommands);

                var install = selector.CommandsToInstall;
                if (install.Count == 0)
                {
                    this.Logger.Debug("No commands found");
                    return;
                }

                // ignore the first one
                for (int i = 1; i < install.Count; i++)
                {
                    var path = PathManager.Instance.CreatePath(FileType.Data, string.Format(InstallQueueFileFormat, i.ToString("000")));
                    this.Logger.Debug("Enqueueing command: {0}", path);
                    this.SerializeCommand(install[i], path);
                }

                this.InstallCommand(install[0]);
            }
        }

        private void SendFeedback(params UpdateStateInfo[] updateStateInfos)
        {
            if (updateStateInfos == null || updateStateInfos.Length == 0)
            {
                return;
            }

            var eventArgs = new FeedbackEventArgs(new IReceivedLogFile[0], updateStateInfos, new IReceivedLogFile[0]);
            this.RaiseFeedbackReceived(eventArgs);
        }

        private void SerializeCommand(UpdateCommand command, string path)
        {
            this.Logger.Trace("Serializing command to {0}", path);
            var configurator = new Configurator(path);
            configurator.Serialize(command);
        }

        private void SetUpPersistence()
        {
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            this.persistenceContext = persistenceService.GetContext<UpdateAgentPersistence>();
            if (this.persistenceContext.Value == null || !this.persistenceContext.Valid)
            {
                this.persistenceContext.Value = new UpdateAgentPersistence();
            }
        }

        private void StartInstallation(UpdateCommand command)
        {
            this.installationProgress =
                new ProgressWrapper(this.Context.CreateProgressMonitor(UpdateStage.Installing, this.agentConfig.ShowVisualization));
            this.IsAvailable = false;
            this.installationEndWait.Reset();

            this.Logger.Info(
                "Starting installation of update command {0} of {1}", 
                command.UpdateId.UpdateIndex, 
                command.UpdateId.BackgroundSystemGuid);

            this.installationProgress.Start();

            if (this.Logger.IsTraceEnabled && File.Exists(this.currentUpdateFile))
            {
                using (var reader = File.OpenText(this.currentUpdateFile))
                {
                    this.Logger.Trace(reader.ReadToEnd());
                }
            }

            var installer = new UpdateInstaller(command, this.installationRoot, this.installationProgress);
            this.installationProgress.Progress(0, "Verifying resources");
            installer.BeginVerifyResources(this.ResourcesVerified, installer);
        }

        private void UpdateValetOnUpdateDeparked(object sender, UpdateCommandsEventArgs e)
        {
            this.SelectCommandToInstall(e.Commands, false);
        }

#endregion

        private class ProgressPartWrapper : IPartProgressMonitor
        {
#region Fields

            private readonly IProgressMonitor parent;

            private readonly IPartProgressMonitor partProgress;

#endregion

#region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="ProgressPartWrapper"/> class.</summary>
            /// <param name="parent">The parent.</param>
            /// <param name="partProgress">The part progress.</param>
            public ProgressPartWrapper(IProgressMonitor parent, IPartProgressMonitor partProgress)
            {
                this.parent = parent;
                this.partProgress = partProgress;
            }

#endregion

#region Public Properties

            /// <summary>Gets a value indicating whether is cancelled.</summary>
            public bool IsCancelled
            {
                get
                {
                    return this.partProgress.IsCancelled || this.parent.IsCancelled;
                }
            }

#endregion

#region Public Methods and Operators

            /// <summary>The progress.</summary>
            /// <param name="value">The value.</param>
            /// <param name="note">The note.</param>
            public void Progress(double value, string note)
            {
                this.partProgress.Progress(value, note);
            }

#endregion
        }

        private class ProgressWrapper : IProgressMonitor
        {
#region Fields

            private readonly IProgressMonitor progressMonitor;

            private bool isCancelled;

#endregion

#region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="ProgressWrapper"/> class.</summary>
            /// <param name="progressMonitor">The progress monitor.</param>
            public ProgressWrapper(IProgressMonitor progressMonitor)
            {
                this.progressMonitor = progressMonitor;
            }

#endregion

#region Public Properties

            /// <summary>Gets a value indicating whether is cancelled.</summary>
            public bool IsCancelled
            {
                get
                {
                    return this.isCancelled || this.progressMonitor.IsCancelled;
                }
            }

#endregion

#region Public Methods and Operators

            /// <summary>The cancel.</summary>
            public void Cancel()
            {
                this.isCancelled = true;
            }

            /// <summary>The complete.</summary>
            /// <param name="errorMessage">The error message.</param>
            /// <param name="successMessage">The success message.</param>
            public void Complete(string errorMessage, string successMessage)
            {
                this.progressMonitor.Complete(errorMessage, successMessage);
            }

            /// <summary>The create part.</summary>
            /// <param name="startValue">The start value.</param>
            /// <param name="endValue">The end value.</param>
            /// <returns>The <see cref="IPartProgressMonitor"/>.</returns>
            public IPartProgressMonitor CreatePart(double startValue, double endValue)
            {
                return new ProgressPartWrapper(this, this.progressMonitor.CreatePart(startValue, endValue));
            }

            /// <summary>The progress.</summary>
            /// <param name="value">The value.</param>
            /// <param name="note">The note.</param>
            public void Progress(double value, string note)
            {
                this.progressMonitor.Progress(value, note);
            }

            /// <summary>The start.</summary>
            public void Start()
            {
                this.progressMonitor.Start();
            }

#endregion
        }
    }
}