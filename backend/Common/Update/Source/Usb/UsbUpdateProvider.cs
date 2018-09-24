// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsbUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Usb
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    /// <summary>
    /// An update provider that stores files on a USB stick.
    /// </summary>
    public class UsbUpdateProvider : UpdateProviderBase<UsbUpdateProviderConfig>, IManualUpdateProvider
    {
        private readonly IWritableFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbUpdateProvider"/> class.
        /// </summary>
        public UsbUpdateProvider()
        {
            this.fileSystem = (IWritableFileSystem)FileSystemManager.Local;
        }

        /// <summary>
        /// Gets the list of handled units. One of the unit name might be a wildcard
        /// (<see cref="UpdateComponentBase.UnitWildcard"/>) to tell the user of this
        /// class that the sink is interested in all updates.
        /// </summary>
        public override IEnumerable<string> HandledUnits
        {
            get
            {
                yield return UpdateComponentBase.UnitWildcard;
            }
        }

        private string RepositoryConfigPath
        {
            get
            {
                return Path.Combine(this.Config.RepositoryBasePath, RepositoryConfig.RepositoryXmlFileName);
            }
        }

        /// <summary>
        /// Gets the repository configuration for this provider.
        /// This can be the default configuration or the actual configuration
        /// from the remote repository.
        /// </summary>
        /// <returns>
        /// The <see cref="RepositoryConfig"/>.
        /// </returns>
        public RepositoryConfig GetRepositoryConfig()
        {
            var path = this.RepositoryConfigPath;
            if (!File.Exists(path))
            {
                return this.CreateDefaultRepositoryConfig();
            }

            var configurator = new Configurator(path, RepositoryConfig.Schema);
            return configurator.Deserialize<RepositoryConfig>();
        }

        /// <summary>
        /// Handles the update commands by forwarding them.
        /// </summary>
        /// <param name="commands">
        ///     The update commands.
        /// </param>
        /// <param name="progressMonitor">
        ///     The progress monitor that observes the upload of the update command.
        /// </param>
        public override void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor)
        {
            if (!this.IsAvailable)
            {
                throw new DirectoryNotFoundException("Couldn't find root of " + this.Config.RepositoryBasePath);
            }

            progressMonitor = progressMonitor ?? this.Context.CreateProgressMonitor(
                UpdateStage.ForwardingUpdate, this.Config.ShowVisualization);

            try
            {
                int count;
                var all = this.GetAllResources(commands, out count);

                progressMonitor.Start();

                var maxProgress = count + all.Count + 2.0;
                var progress = 0;

                progressMonitor.Progress((progress++) / maxProgress, RepositoryConfig.RepositoryXmlFileName);

                var repoConfig = this.CreateRepository();

                foreach (var pair in all)
                {
                    var command = pair.Key;
                    foreach (var hash in pair.Value)
                    {
                        if (progressMonitor.IsCancelled)
                        {
                            return;
                        }

                        try
                        {
                            var resourceName = string.Format("{0}{1}", hash, FileDefinitions.ResourceFileExtension);
                            progressMonitor.Progress(
                                (progress++) / maxProgress,
                                command.UnitId.UnitName + ": " + resourceName);

                            var resourcePath = this.GetRepositoryPath(repoConfig.ResourceDirectory, resourceName);
                            this.CopyResource(resourcePath, hash);
                        }
                        catch (Exception ex)
                        {
                            throw new UpdateException("Couldn't copy resource " + hash, ex);
                        }
                    }

                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    var commandDir = this.GetRepositoryPath(repoConfig.CommandsDirectory, command.UnitId.UnitName);
                    var commandName = string.Format(
                        "{0}-{1:####0000}{2}",
                        command.UpdateId.BackgroundSystemGuid,
                        command.UpdateId.UpdateIndex,
                        FileDefinitions.UpdateCommandExtension);
                    progressMonitor.Progress((progress++) / maxProgress, command.UnitId.UnitName + ": " + commandName);
                    this.CreateUpdateCommandFile(commandDir, commandName, command);
                }

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't upload commands");
                progressMonitor.Complete("Couldn't upload commands: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Forces this update provider to immediately check if there is feedback available and
        /// download it if so.
        /// This method won't do anything if <see cref="IUpdateSink.IsAvailable"/> returns false.
        /// </summary>
        /// <param name="progress">
        /// The progress monitor.
        /// </param>
        /// <exception cref="UpdateException">if there was an error while getting the feedback.</exception>
        public void CheckForFeedback(IProgressMonitor progress)
        {
            try
            {
                this.DoCheckForFeedback(progress);
            }
            catch (UpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UpdateException("Couldn't check for feedback", ex);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateProviderBase{TConfig}.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            this.CheckForFeedback(null);
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateProviderBase{TConfig}.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
        }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            try
            {
                return new DirectoryInfo(this.Config.RepositoryBasePath).Root.Exists;
            }
            catch (Exception ex)
            {
                Logger.Error("CheckAvailable() Exception {0}", ex.ToString());
                throw;
            }
        }

        private void DoCheckForFeedback(IProgressMonitor progressMonitor)
        {
            if (!this.IsAvailable)
            {
                return;
            }

            progressMonitor = progressMonitor ?? this.Context.CreateProgressMonitor(
                UpdateStage.ForwardingFeedback, this.Config.ShowVisualization);
            progressMonitor.Start();
            progressMonitor.Progress(0, "Searching for feedback");
            try
            {
                var repoConfig = this.CreateRepository();
                var logFiles = new List<KeyValuePair<string, IWritableFileInfo>>();
                var stateFiles = new List<IWritableFileInfo>();
                var feedbackDir = this.fileSystem.GetDirectory(this.GetRepositoryPath(repoConfig.FeedbackDirectory));
                foreach (var unitDirectory in feedbackDir.GetDirectories())
                {
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    var unitName = unitDirectory.Name;
                    foreach (var file in unitDirectory.GetFiles())
                    {
                        if (file.Name.EndsWith(
                            FileDefinitions.LogFileExtension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            logFiles.Add(new KeyValuePair<string, IWritableFileInfo>(unitName, file));
                        }
                        else if (file.Name.EndsWith(
                            FileDefinitions.UpdateStateInfoExtension, StringComparison.InvariantCultureIgnoreCase))
                        {
                            stateFiles.Add(file);
                        }
                    }
                }

                var progress = 0;
                var maxProgress = stateFiles.Count + 1.0;

                var updateStates = new List<UpdateStateInfo>(stateFiles.Count);

                foreach (var stateFile in stateFiles)
                {
                    progressMonitor.Progress((++progress) / maxProgress, stateFile.Name);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    try
                    {
                        var configurator = new Configurator(stateFile.FullName);
                        updateStates.Add(configurator.Deserialize<UpdateStateInfo>());
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, "Couldn't deserialize state from " + stateFile.FullName);
                    }
                }

                var eventArgs =
                    new FeedbackEventArgs(
                        logFiles.ConvertAll(f => (IReceivedLogFile)new FileReceivedLogFile(f.Key, f.Value.FullName))
                                .ToArray(), updateStates.ToArray(), new IReceivedLogFile[0]);
                this.RaiseFeedbackReceived(eventArgs);

                this.DeleteFiles(logFiles, stateFiles);

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't download feedback");
                progressMonitor.Complete("Couldn't download feedback: " + ex.Message, null);
            }
        }

        private void DeleteFiles(
            IEnumerable<KeyValuePair<string, IWritableFileInfo>> logFiles, IEnumerable<IWritableFileInfo> stateFiles)
        {
            foreach (var logFile in logFiles)
            {
                this.DeleteFile(logFile.Value);
            }

            foreach (var stateFile in stateFiles)
            {
                this.DeleteFile(stateFile);
            }
        }

        private void DeleteFile(IWritableFileInfo file)
        {
            try
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't delete file " + file.FullName);
            }
        }

        private RepositoryVersionConfig CreateRepository()
        {
            Directory.CreateDirectory(this.Config.RepositoryBasePath);

            var repoConfig = this.GetRepositoryConfig();
            var configurator = new Configurator(this.RepositoryConfigPath, RepositoryConfig.Schema);
            configurator.Serialize(repoConfig);

            var versionConfig = repoConfig.GetCurrentConfig();

            Directory.CreateDirectory(this.GetRepositoryPath(versionConfig.ResourceDirectory));
            Directory.CreateDirectory(this.GetRepositoryPath(versionConfig.CommandsDirectory));
            Directory.CreateDirectory(this.GetRepositoryPath(versionConfig.FeedbackDirectory));
            Directory.CreateDirectory(this.GetRepositoryPath(versionConfig.UploadsDirectory));

            return versionConfig;
        }

        private string GetRepositoryPath(params string[] pathElements)
        {
            var path = this.Config.RepositoryBasePath;
            foreach (var element in pathElements)
            {
                path = Path.Combine(path, element);
            }

            return path;
        }

        private List<KeyValuePair<UpdateCommand, List<string>>> GetAllResources(
            IEnumerable<UpdateCommand> commands, out int count)
        {
            var all = new List<KeyValuePair<UpdateCommand, List<string>>>();
            var allResources = new SortedList<string, string>();
            count = 0;
            foreach (var command in commands)
            {
                var resources = new List<string>();
                all.Add(new KeyValuePair<UpdateCommand, List<string>>(command, resources));
                foreach (var hash in this.GetAllResourceHashes(command))
                {
                    if (allResources.ContainsKey(hash))
                    {
                        continue;
                    }

                    count++;
                    resources.Add(hash);
                    allResources.Add(hash, hash);
                }
            }

            return all;
        }

        private void CopyResource(string resourcePath, string hash)
        {
            IWritableFileInfo file;
            if (this.fileSystem.TryGetFile(resourcePath, out file))
            {
                var calculatedHash = ResourceHash.Create(resourcePath);
                if (calculatedHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Logger.Trace("Resource {0} exists: {1}", hash, resourcePath);
                    return;
                }

                this.Logger.Info("Invalid resource {0} exists, deleting it: {1}", hash, resourcePath);
                file.Delete();
            }
            else
            {
                this.Logger.Trace("Downloading resource {0} to {1}", hash, resourcePath);
            }

            var resource = this.Context.ResourceProvider.GetResource(hash);
            var tempResourcePath = Path.ChangeExtension(resourcePath, FileDefinitions.TempFileExtension);
            resource.CopyTo(tempResourcePath);
            this.MoveFile(tempResourcePath, resourcePath);
        }

        private void CreateUpdateCommandFile(string directory, string fileName, UpdateCommand command)
        {
            Directory.CreateDirectory(directory);
            var commandPath = Path.Combine(directory, fileName);
            IWritableFileInfo file;
            if (this.fileSystem.TryGetFile(commandPath, out file))
            {
                this.DeleteFile(file);
            }

            var tempCommandPath = Path.ChangeExtension(commandPath, FileDefinitions.TempFileExtension);
            var configurator = new Configurator(tempCommandPath);
            configurator.Serialize(command);
            this.MoveFile(tempCommandPath, commandPath);
        }

        private void MoveFile(string sourcePath, string destinationPath)
        {
            var file = this.fileSystem.GetFile(sourcePath).MoveTo(destinationPath);
            file.Attributes = FileAttributes.Normal;
        }
    }
}
