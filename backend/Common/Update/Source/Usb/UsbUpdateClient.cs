// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbUpdateClient.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Usb
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// Class to handle update of system via USB
    /// </summary>
    public class UsbUpdateClient : UpdateClientBase<UsbUpdateClientConfig>
    {
        private readonly object uploadsInProgress = new object();
        
        private readonly IWritableFileSystem fileSystem;

        private readonly ITimer usbDetectionTimeoutTimer;

        private readonly UsbStickDetector usbStickDetector;

        private ITimer pollTimer;

        private bool usbDetected;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbUpdateClient"/> class.
        /// </summary>
        public UsbUpdateClient()
        {
            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;

            this.usbDetectionTimeoutTimer = TimerFactory.Current.CreateTimer("UsbDetectionTimeout");
            this.usbDetectionTimeoutTimer.Elapsed += this.UsbTimeoutTimerOnElapsed;
            this.usbDetectionTimeoutTimer.AutoReset = false;

            this.usbStickDetector = new UsbStickDetector();
            this.usbStickDetector.Inserted += this.UsbStickDetectorOnInserted;
        }

        private string RepositoryConfigPath
        {
            get
            {
                return Path.Combine(this.Config.RepositoryBasePath, RepositoryConfig.RepositoryXmlFileName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbUpdateClient"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public override void Configure(UpdateClientConfigBase config, IUpdateContext updateContext)
        {
            base.Configure(config, updateContext);

            var pollInterval = this.Config.PollInterval;
            if (pollInterval.HasValue)
            {
                this.pollTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
                this.pollTimer.AutoReset = true;
                this.pollTimer.Elapsed += this.PollTimerOnElapsed;
                this.pollTimer.Interval = pollInterval.Value;
            }

            this.usbDetectionTimeoutTimer.Interval = this.Config.UsbDetectionTimeOut;
        }

        /// <summary>
        /// Gets all files in the local upload directory and in any of its subdirectories.
        /// </summary>
        /// <param name="repositoryConfig">The current repository configuration</param>
        /// <returns>The list of paths of files which will be uploaded</returns>
        public List<string> GetLocalFilesToUpload(RepositoryVersionConfig repositoryConfig)
        {
            List<string> uploadFiles = new List<string>();

            try
            {
                var localUploadPath = Path.Combine(this.Config.RepositoryBasePath, repositoryConfig.UploadsDirectory);
                Directory.CreateDirectory(localUploadPath);
                var localUploadFiles = Directory.GetFiles(localUploadPath, "*.*", SearchOption.AllDirectories);
                return localUploadFiles.ToList();
            }
            catch (Exception e)
            {
                this.Logger.Error(e, "Unable to retrieve files from the local upload folder.");
                return uploadFiles;
            }
        }

        /// <summary>
        /// Sends feedback back to the source.
        /// </summary>
        /// <param name="logFiles">
        /// The log files to upload.
        /// </param>
        /// <param name="stateInfos">
        /// The state information objects to upload.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor that observes the upload of update feedback and log files.
        /// </param>
        public override void SendFeedback(
            IEnumerable<IReceivedLogFile> logFiles,
            IEnumerable<UpdateStateInfo> stateInfos,
            IProgressMonitor progressMonitor)
        {
            if (!this.IsAvailable)
            {
                throw new DirectoryNotFoundException("Couldn't find " + this.RepositoryConfigPath);
            }

            var logs = new List<IReceivedLogFile>(logFiles);
            var states = new List<UpdateStateInfo>(stateInfos);
            progressMonitor = progressMonitor ?? this.Context.CreateProgressMonitor(
                                  UpdateStage.SendingFeedback,
                                  this.Config.ShowVisualization);
            progressMonitor.Start();
            this.Logger.Info("Sending feedback: {0} logs, {1} states", logs.Count, states.Count);

            var maxProgress = logs.Count + states.Count + 1.0;
            var progress = 0;
            try
            {
                var root = this.Config.RepositoryBasePath;

                var currentConfig = this.GetCurrentRepoConfig();
                if (currentConfig.Compression != CompressionAlgorithm.None)
                {
                    throw new NotSupportedException(
                        "Compression of repository is not supported: " + currentConfig.Compression);
                }

                var feedbackPath = Path.Combine(root, currentConfig.FeedbackDirectory);
                Directory.CreateDirectory(feedbackPath);
                foreach (var log in logs)
                {
                    progressMonitor.Progress((++progress) / maxProgress, log.FileName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    this.CopyFeedbackFile(feedbackPath, log);
                }

                foreach (var state in states)
                {
                    progressMonitor.Progress((++progress) / maxProgress, state.UnitId.UnitName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    this.CreateStateFile(feedbackPath, state);
                }

                progressMonitor.Complete(null, "Remove USB stick");
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't upload feedback");
                progressMonitor.Complete("Couldn't upload feedback: " + ex.Message, "Remove USB stick");
            }
        }

        /// <summary>
        /// Sends feedback back to the source.
        /// </summary>
        /// <param name="uploadFiles">
        /// The log files to upload.
        /// </param>
        public override void UploadFiles(IList<IReceivedLogFile> uploadFiles)
        {
            if (uploadFiles.Count == 0)
            {
                return;
            }
            
            var locked = Monitor.TryEnter(this.uploadsInProgress, 60000);
            if (!locked)
            {
                return;
            }

            if (!this.IsAvailable)
            {
                throw new DirectoryNotFoundException("Couldn't find " + this.RepositoryConfigPath);
            }
            
            var logs = new List<IReceivedLogFile>(uploadFiles);
            IProgressMonitor progressMonitor = this.Context.CreateProgressMonitor(
                UpdateStage.UploadingFiles,
                this.Config.ShowVisualization);
            progressMonitor.Start();
            this.Logger.Info($"Uploading {logs.Count} files");
            var maxProgress = logs.Count + 1.0;
            var progress = 0;

            try
            {
                var currentConfig = this.GetCurrentRepoConfig();

                if (string.IsNullOrEmpty(currentConfig.UploadsDirectory))
                {
                    this.Logger.Trace("Remote repository upload path not defined.");
                    return;
                }

                if (currentConfig.Compression != CompressionAlgorithm.None)
                {
                    throw new NotSupportedException(
                        "Compression of repository is not supported: " + currentConfig.Compression);
                }

                var destinationDirectory = Path.Combine(this.Config.RepositoryBasePath, currentConfig.UploadsDirectory);

                foreach (var log in logs)
                {
                    progressMonitor.Progress(++progress / maxProgress, log.FileName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    var uploadFile = log as FileReceivedLogFile;
                    if (uploadFile == null)
                    {
                        continue;
                    }

                    // Make sure the remote full directory structure exists on the server, for this file
                    string remoteDirectory = Path.Combine(destinationDirectory, this.GetDestinationSubdirectories(uploadFile));
                    Directory.CreateDirectory(remoteDirectory);

                    this.Logger.Trace($"Uploading {log.FileName} to {remoteDirectory}");

                    try
                    {
                        this.CopyFeedbackFile(remoteDirectory, log);
                        uploadFile.Delete();
                    }
                    catch (Exception e)
                    {
                        this.Logger.Trace($"Unable to upload {uploadFile.FilePath} to {remoteDirectory}, skipping. {e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't upload files");
                progressMonitor.Complete($"Couldn\'t upload files: {ex.Message}", "Remove USB stick");
            }
            finally
            {
                this.Logger.Trace("Upload complete.");
                progressMonitor.Complete(null, "Remove USB stick");
                Monitor.Exit(this.uploadsInProgress);
            }
        }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            return File.Exists(this.RepositoryConfigPath);
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateClientBase{TConfig}.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            if (this.IsAvailable)
            {
                this.DownloadUpdates();
            }

            try
            {
                this.usbStickDetector.Start();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't start USB stick detector");
            }

            this.EnablePollTimer(true);
        }

        /// <summary>
        /// Stops the update client
        /// </summary>
        protected override void DoStop()
        {
            this.usbStickDetector.Stop();

            this.EnablePollTimer(false);
        }

        private void CopyFeedbackFile(string feedbackPath, IReceivedLogFile log)
        {
            var feedbackDir = Path.Combine(feedbackPath, log.UnitName);
            Directory.CreateDirectory(feedbackDir);
            var index = 0;
            var fileName = log.FileName;
            string logPath;
            while (File.Exists(logPath = Path.Combine(feedbackDir, fileName)))
            {
                fileName = Path.ChangeExtension(log.FileName, $".{++index}{FileDefinitions.LogFileExtension}");
            }

            this.Logger.Trace("Copying feedback file to {0}", logPath);
            var tempFile = Path.ChangeExtension(logPath, FileDefinitions.TempFileExtension);
            log.CopyTo(tempFile);
            this.MoveFile(tempFile, logPath);
        }

        private void CreateStateFile(string feedbackPath, UpdateStateInfo state)
        {
            var feedbackDir = Path.Combine(feedbackPath, state.UnitId.UnitName);
            Directory.CreateDirectory(feedbackDir);
            var index = 0;
            var fileNameBase = $"{state.UpdateId.BackgroundSystemGuid}-{state.UpdateId.UpdateIndex:####0000}-{(int)state.State:00}-{state.State}";
            var fileName = fileNameBase + FileDefinitions.UpdateStateInfoExtension;
            string stateFilePath;
            while (File.Exists(stateFilePath = Path.Combine(feedbackDir, fileName)))
            {
                fileName = $"{fileNameBase}.{++index}{FileDefinitions.UpdateStateInfoExtension}";
            }

            this.Logger.Trace("Creating state file at {0}", stateFilePath);
            var tempFile = Path.ChangeExtension(stateFilePath, FileDefinitions.TempFileExtension);
            var configurator = new Configurator(tempFile);
            configurator.Serialize(state);
            this.MoveFile(tempFile, stateFilePath);
        }

        private void DeleteCommandFiles(IEnumerable<IWritableFileInfo> commandFiles)
        {
            foreach (var commandFile in commandFiles)
            {
                this.Logger.Trace("Deleting {0}", commandFile.FullName);
                try
                {
                    commandFile.Attributes = FileAttributes.Normal;
                    commandFile.Delete();
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't delete command file: " + commandFile);
                }
            }
        }

        private List<UpdateCommand> DownloadCommands(IEnumerable<IWritableFileInfo> commandFiles)
        {
            var commands = new List<UpdateCommand>();
            foreach (var commandFile in commandFiles)
            {
                this.Logger.Trace("Deserializing command: {0}", commandFile.FullName);
                try
                {
                    var configurator = new Configurator(commandFile.FullName);
                    commands.Add(configurator.Deserialize<UpdateCommand>());
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't deserialize " + commandFile);
                }
            }

            return commands;
        }

        private IEnumerable<UpdateStateInfo> DownloadResources(
            Dictionary<string, List<UpdateCommand>> resourceHashes,
            string resourcePath,
            IProgressMonitor progressMonitor,
            List<UpdateCommand> commands)
        {
            var progress = 0;
            var maxProgress = resourceHashes.Count + 1.0;

            foreach (var resourcePair in resourceHashes)
            {
                var resourceFile = Path.Combine(resourcePath, resourcePair.Key + FileDefinitions.ResourceFileExtension);
                if (!File.Exists(resourceFile))
                {
                    // remove all commands that would require this resource
                    this.Logger.Warn("Couldn't find resource '{0}', removing all related commands", resourceFile);
                    commands.RemoveAll(c => resourcePair.Value.Contains(c));
                    this.NotifyFailedStatus(resourcePair.Value, "Couldn't find resource " + resourcePair.Key);
                }
            }

            var calculations = commands.ConvertAll(c => new TransferringFeedbackCalculation(c));

            foreach (var resourcePair in resourceHashes)
            {
                var resourceFile = Path.Combine(resourcePath, resourcePair.Key + FileDefinitions.ResourceFileExtension);

                progressMonitor.Progress((++progress) / maxProgress, resourcePair.Key);
                if (progressMonitor.IsCancelled)
                {
                    return new UpdateStateInfo[0];
                }

                if (!this.ResourceExists(resourcePair.Key))
                {
                    this.Logger.Debug("Adding resource {0}: {1}", resourcePair.Key, resourceFile);
                    this.Context.ResourceProvider.AddResource(resourcePair.Key, resourceFile, false);

                    var fileInfo = new FileInfo(resourceFile);
                    foreach (var calculation in calculations)
                    {
                        if (resourcePair.Value.Contains(calculation.Command))
                        {
                            calculation.AddTransferredFile(fileInfo);
                        }
                    }
                }
            }

            return calculations.ConvertAll(c => c.CreateFeedback());
        }

        private void DownloadUpdates()
        {
            RepositoryVersionConfig currentConfig;
            List<IWritableFileInfo> commandFiles;
            try
            {
                currentConfig = this.GetCurrentRepoConfig();
                if (currentConfig.Compression != CompressionAlgorithm.None)
                {
                    this.Logger.Error("Compression of repository is not supported: {0}", currentConfig.Compression);
                    return;
                }

                var commandsPath = Path.Combine(this.Config.RepositoryBasePath, currentConfig.CommandsDirectory);
                if (!Directory.Exists(commandsPath))
                {
                    return;
                }

                commandFiles = this.FindCommandFiles(commandsPath);
                this.Logger.Debug("Found {0} commands", commandFiles.Count);
                if (commandFiles.Count == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't find commands in " + this.Config.RepositoryBasePath);
                return;
            }

            this.DownloadUpdates(currentConfig, commandFiles);
        }

        private void DownloadUpdates(RepositoryVersionConfig config, List<IWritableFileInfo> commandFiles)
        {
            var progressMonitor = this.Context.CreateProgressMonitor(
                UpdateStage.ReceivingUpdate,
                this.Config.ShowVisualization);
            progressMonitor.Start();
            progressMonitor.Progress(0, "Downloading Commands");
            IEnumerable<UpdateStateInfo> feedback;
            try
            {
                var commands = this.DownloadCommands(commandFiles);
                this.Logger.Debug("Downloaded {0} commands", commands.Count);
                if (commands.Count == 0)
                {
                    progressMonitor.Complete(null, null);
                    return;
                }

                var resourceHashes = this.GetResourceHashes(commands);

                var resourcePath = Path.Combine(this.Config.RepositoryBasePath, config.ResourceDirectory);
                if (!Directory.Exists(resourcePath))
                {
                    var msg = $"Couldn't find resource path: {resourcePath}";
                    this.Logger.Warn(msg);
                    progressMonitor.Complete(msg, null);
                    this.NotifyFailedStatus(commands, "Couldn't find resource path");
                    return;
                }

                this.Logger.Debug("Found {0} resources for {1} commands", resourceHashes.Count, commands.Count);

                feedback = this.DownloadResources(resourceHashes, resourcePath, progressMonitor, commands);
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                if (commands.Count == 0)
                {
                    this.Logger.Warn("No commands left to handle");
                    progressMonitor.Complete("No commands left to handle", null);
                    return;
                }

                this.RaiseCommandReceived(new UpdateCommandsEventArgs(commands.ToArray()));

                this.DeleteCommandFiles(commandFiles);
                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't download updates");
                progressMonitor.Complete("Couldn't download updates: " + ex.Message, null);
                return;
            }

            this.SendFeedback(new IReceivedLogFile[0], feedback, null);
        }

        private void EnablePollTimer(bool enable)
        {
            if (this.pollTimer != null)
            {
                this.pollTimer.Enabled = enable;
            }
        }

        private List<IWritableFileInfo> FindCommandFiles(string commandsPath)
        {
            var commands = new List<IWritableFileInfo>();
            try
            {
                foreach (var unitDirectory in this.fileSystem.GetDirectory(commandsPath).GetDirectories())
                {
                    var unitName = unitDirectory.Name;

                    if (!this.ShouldDownloadForUnit(unitName))
                    {
                        this.Logger.Debug("Not downloading commands for unit {0}", unitName);
                        continue;
                    }

                    var files = unitDirectory.GetFiles();
                    Array.Sort(
                        files,
                        (a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var commandFile in files)
                    {
                        if (!commandFile.Name.EndsWith(
                                FileDefinitions.UpdateCommandExtension,
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        this.Logger.Debug("Found command: {0}", commandFile.FullName);
                        commands.Add(commandFile);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't search for commands");
            }

            return commands;
        }

        private RepositoryVersionConfig GetCurrentRepoConfig()
        {
            var configurator = new Configurator(this.RepositoryConfigPath, RepositoryConfig.Schema);
            var repositoryConfig = configurator.Deserialize<RepositoryConfig>();

            return repositoryConfig.GetCurrentConfig();
        }

        /// <summary>
        /// For a file to upload from the Uploads location, get the subdirectories to create on the remote server.
        /// </summary>
        /// <param name="file">The file we are uploading</param>
        /// <returns>The remote directory we are uploading the file to</returns>
        private string GetDestinationSubdirectories(FileReceivedLogFile file)
        {
            var localUploadPath = PathManager.Instance.CreatePath(FileType.Data, "Uploads" + Path.DirectorySeparatorChar);

            string destinationFile = file.FilePath.Replace(localUploadPath, string.Empty);
            string destinationPath = Path.GetDirectoryName(destinationFile);
            return destinationPath;
        }

        private void MoveFile(string sourcePath, string destinationPath)
        {
            var file = this.fileSystem.GetFile(sourcePath).MoveTo(destinationPath);
            file.Attributes = FileAttributes.Normal;
        }

        private void PollTimerOnElapsed(object sender, EventArgs e)
        {
            if (!this.IsAvailable)
            {
                this.Logger.Trace("Polling, but not available: {0}", this.RepositoryConfigPath);
                return;
            }

            this.Logger.Trace("Polling: found {0}", this.RepositoryConfigPath);
            this.DownloadUpdates();
        }

        private bool ResourceExists(string hash)
        {
            try
            {
                this.Logger.Trace("Checking if resource {0} already exists", hash);
                this.Context.ResourceProvider.GetResource(hash);
                return true;
            }
            catch (UpdateException ex)
            {
                this.Logger.Trace(ex, "Couldn't find resource " + hash);
                return false;
            }
        }

        private void UsbStickDetectorOnInserted(object sender, EventArgs e)
        {
            this.Logger.Debug("Received USB insertion event (already detected: {0})", this.usbDetected);
            if (this.usbDetected)
            {
                return;
            }

            this.usbDetected = true;
            this.usbDetectionTimeoutTimer.Enabled = true;
        }

        private void UsbTimeoutTimerOnElapsed(object sender, EventArgs e)
        {
            this.usbDetectionTimeoutTimer.Enabled = false;
            if (!this.IsAvailable)
            {
                this.usbDetected = false;
                return;
            }

            this.EnablePollTimer(false);
            try
            {
                this.DownloadUpdates();
            }
            finally
            {
                this.EnablePollTimer(true);
                this.usbDetected = false;
            }
        }

        private class TransferringFeedbackCalculation
        {
            private int totalCount;

            private long totalSize;

            public TransferringFeedbackCalculation(UpdateCommand command)
            {
                this.Command = command;
            }

            public UpdateCommand Command { get; }

            public void AddTransferredFile(FileInfo fileInfo)
            {
                this.totalSize += fileInfo.Length;
                this.totalCount++;
            }

            public UpdateStateInfo CreateFeedback()
            {
                return new UpdateStateInfo
                           {
                               UpdateId = this.Command.UpdateId,
                               UnitId = this.Command.UnitId,
                               UpdateSource = "USB",
                               State = UpdateState.Transferring,
                               TimeStamp = TimeProvider.Current.UtcNow,
                               ErrorReason = string.Empty,
                               Description = $"{this.totalCount} files ({this.totalSize} bytes) transferred"
                           };
            }
        }
    }
}