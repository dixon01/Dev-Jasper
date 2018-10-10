// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// An update provider that uploads files to an FTP server.
    /// </summary>
    public class FtpUpdateProvider : UpdateProviderBase<FtpUpdateProviderConfig>, IManualUpdateProvider
    {
        /// <summary>
        /// The regex to use when checking to see if a directory is a TFT directory
        /// </summary>
        public const string TftRegex = @"TFT(-[a-zA-Z0-9]{2}){3}";

        private readonly object repositoryConfigLock = new object();

        private readonly ITimer pollTimer;

        private string temporaryDirectory;

        private FtpHandler ftpHandler;

        private double lastRepoConfigDownload;

        private RepositoryConfig lastRepoConfig;

        private CompressionFactory compressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpUpdateProvider"/> class.
        /// </summary>
        public FtpUpdateProvider()
        {
            this.pollTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.pollTimer.AutoReset = false;
            this.pollTimer.Elapsed += this.PollTimerOnElapsed;
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
                return Path.Combine(this.Config.RepositoryBasePath, RepositoryConfig.RepositoryXmlFileName).Replace('\\', '/');
            }
        }

        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        public override void Configure(UpdateProviderConfigBase config, IUpdateContext updateContext)
        {
            base.Configure(config, updateContext);

            if (!this.Config.PollInterval.Equals(TimeSpan.Zero))
            {
                this.pollTimer.Interval = this.Config.PollInterval;
            }
            else
            {
                this.Logger.Warn("Invalid update interval for {0}. Zero is not allowed, set it to 5 minutes.", this.Config.Host);
                this.pollTimer.Interval = TimeSpan.FromMinutes(5);
            }

            this.temporaryDirectory = Path.Combine(this.Context.TemporaryDirectory, Path.Combine("FTP", this.Name));
            this.ftpHandler = new FtpHandler(this.Config);
            this.compressionFactory = new CompressionFactory(this.temporaryDirectory);
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
            progressMonitor = progressMonitor ?? this.Context.CreateProgressMonitor(
                UpdateStage.ForwardingUpdate, this.Config.ShowVisualization);
            this.Logger.Info(MethodBase.GetCurrentMethod().Name );
            try
            {
                if (this.ftpHandler != null)
                {
                    this.Logger.Info("HandleCommands Enter: {0}", this.ftpHandler);
                }

                int count;
                var all = this.GetAllResources(commands, out count);

                progressMonitor.Start();

                var maxProgress = count + all.Count + 2.0;
                var progress = 0;

                progressMonitor.Progress(0, RepositoryConfig.RepositoryXmlFileName);

                var repoConfig = this.CreateRepository(true);

                foreach (var pair in all)
                {
                    string progressNote;
                    var command = pair.Key;
                    foreach (var hash in pair.Value)
                    {
                      
                        var resourceName = hash + FileDefinitions.ResourceFileExtension;
                        this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + resourceName);
                        progressNote = command.UnitId.UnitName + ": " + resourceName;
                        this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + progressNote);
                        progressMonitor.Progress((++progress) / maxProgress, progressNote);
                        if (progressMonitor.IsCancelled)
                        {
                            return;
                        }

                        var resourcePath = this.GetRepositoryPath(repoConfig.ResourceDirectory, resourceName);
                        Logger.Info("Copy Resources to path {0}", resourcePath);
                        this.CopyResource(
                            resourcePath,
                            hash,
                            repoConfig.Compression,
                            new FtpProgressPart(progressNote, progressMonitor, progress, maxProgress));
                    }

                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    var commandDir = this.GetRepositoryPath(repoConfig.CommandsDirectory, command.UnitId.UnitName);
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + commandDir);
                    var commandName = $"{command.UpdateId.BackgroundSystemGuid}-{command.UpdateId.UpdateIndex:####0000}{FileDefinitions.UpdateCommandExtension}";
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + commandName);
                    progressNote = command.UnitId.UnitName + ": " + commandName;
                    this.CreateUpdateCommandFile(
                        commandDir,
                        commandName,
                        command,
                        repoConfig.Compression,
                        new FtpProgressPart(progressNote, progressMonitor, ++progress, maxProgress));
                }

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debugger.Launch();
                //System.Diagnostics.Debugger.Break();
               
                this.Logger.Error(ex, "Couldn't upload commands");
                progressMonitor.Complete("Couldn't upload commands: " + ex.Message, null);
            }
        }

        /// <summary>
        /// Finds uploaded files on the FTP server.
        /// </summary>
        /// <param name="remotePath">
        /// The remote path, initially uploads on the ftp server
        /// </param>
        /// <param name="ftpHandler">
        /// The ftp handler to use for FTP operations.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor.
        /// </param>
        /// <returns>
        /// The <see /> of files to upload.
        /// </returns>
        public List<KeyValuePair<string, string>> FindUploadedFiles(string remotePath, IFtpHandler ftpHandler, IProgressMonitor progressMonitor)
        {
            var uploadedFiles = new List<KeyValuePair<string, string>>();

            var remoteEntries = ftpHandler.GetEntries(remotePath);

            foreach (var remoteEntry in remoteEntries)
            {
                if (progressMonitor.IsCancelled)
                {
                    return uploadedFiles;
                }

                // We don't process files unless they are in a TFT directory.
                if (!ftpHandler.DirectoryExists(remoteEntry))
                {
                    continue;
                }

                // If this is not a TFT folder, recurse into IT and check for tft folders.
                if (!Regex.IsMatch(remoteEntry, TftRegex))
                {
                    uploadedFiles.AddRange(this.FindUploadedFiles(remoteEntry, ftpHandler, progressMonitor));
                    continue;
                }

                // It is a TFT folder. Get the files.
                var tftFiles = ftpHandler.GetEntries(remoteEntry);
                var unitName = Path.GetFileName(remoteEntry);

                foreach (var tftFile in tftFiles)
                {
                    // We only add files in the TFT folder, we really never should have misc folders in a TFT Folder.
                    if (!ftpHandler.FileExists(tftFile))
                    {
                        continue;
                    }

                    uploadedFiles.Add(new KeyValuePair<string, string>(unitName, tftFile.Replace(@"/", @"\")));
                }
            }
            return uploadedFiles;
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
            try
            {
                this.CheckForFeedback(null);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't check for feedback at start-up");
            }

            this.pollTimer.Enabled = true;
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateProviderBase{TConfig}.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            this.pollTimer.Enabled = false;
        }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            return this.ftpHandler.DirectoryExists("/");
        }

        /// <summary>
        /// Creates a default repository configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="RepositoryConfig"/> valid for the current version of update.
        /// </returns>
        protected override RepositoryConfig CreateDefaultRepositoryConfig()
        {
            var config = base.CreateDefaultRepositoryConfig();
            var current = config.GetCurrentConfig();
            current.Compression = this.Config.Compression;
            return config;
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
                var repoConfig = this.CreateRepository(false);
                this.FindFeedbackFiles(repoConfig, progressMonitor, out var logFiles, out var stateFiles);
                
                var uploadedFiles = this.FindUploadedFiles(this.GetRepositoryPath(repoConfig.UploadsDirectory), this.ftpHandler, progressMonitor);
                
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                List<UpdateStateInfo> updateStates;
                List<string> tempFiles;
                List<IReceivedLogFile> receivedLogFiles;
                List<IReceivedLogFile> receivedUploadedFiles;
                List<string> uploadTempFiles = new List<string>();
                
                this.DownloadFeedback(stateFiles, logFiles, uploadedFiles, repoConfig.Compression, progressMonitor, out updateStates, out tempFiles, out receivedLogFiles);
                receivedUploadedFiles = this.DownloadUploadedFiles(uploadedFiles, repoConfig.Compression, progressMonitor, this.ftpHandler, uploadTempFiles, this.temporaryDirectory);

                try
                {
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    progressMonitor.Progress(0.99, "Queuing feedback");
                    this.RaiseFeedbackReceived(new FeedbackEventArgs(receivedLogFiles.ToArray(), updateStates.ToArray(), receivedUploadedFiles.ToArray()));

                    foreach (var logFile in logFiles)
                    {
                        this.DeleteRemoteFile(logFile.Value);
                    }

                    foreach (var stateFile in stateFiles)
                    {
                        this.DeleteRemoteFile(stateFile);
                    }

                    foreach (var uploadedFile in uploadedFiles)
                    {
                        this.DeleteRemoteFile(uploadedFile.Value);
                    }
                }
                finally
                {
                    foreach (var tempFile in tempFiles)
                    {
                        this.TryDeleteFile(tempFile);
                    }

                    foreach (var uploadTempFile in uploadTempFiles)
                    {
                        this.TryDeleteFile(uploadTempFile);
                    }
                }

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't download feedback");
                progressMonitor.Complete("Couldn't download feedback: " + ex.Message, null);
            }
        }

        private void DownloadFeedback(List<string> stateFiles, List<KeyValuePair<string, string>> logFiles, List<KeyValuePair<string, string>> uploadedFiles, CompressionAlgorithm compression, IProgressMonitor progressMonitor, out List<UpdateStateInfo> updateStates, out List<string> tempFiles, out List<IReceivedLogFile> receivedLogFiles)
        {
            var progress = 0;
            var maxProgress = stateFiles.Count + logFiles.Count + uploadedFiles.Count + 3.0;

            updateStates = new List<UpdateStateInfo>(stateFiles.Count);
            tempFiles = new List<string>(logFiles.Count);
            receivedLogFiles = new List<IReceivedLogFile>();

            foreach (var stateFile in stateFiles.ToArray())
            {
                var fileName = Path.GetFileName(stateFile);
                progressMonitor.Progress((++progress) / maxProgress, fileName);
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                try
                {
                    updateStates.Add(
                        this.DownloadXmlFile<UpdateStateInfo>(
                            stateFile,
                            compression,
                            new FtpProgressPart(Path.GetFileName(stateFile), progressMonitor, progress, maxProgress)));
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't deserialize state from " + stateFile);
                    stateFiles.Remove(stateFile);
                }
            }
            
            this.DownloadLogFiles(logFiles, compression, progressMonitor, tempFiles, receivedLogFiles, progress, maxProgress);
        }

        /// <summary>
        /// Downloads files uploaded to the FTP server, to the local file system.
        /// </summary>
        /// <param name="uploadedFiles">
        /// The files on the FTP server that were uploaded.
        /// </param>
        /// <param name="compression">
        /// The file compression to use.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor.
        /// </param>
        /// <param name="ftpHandler">
        /// The ftp handler.
        /// </param>
        /// <param name="tempFiles">
        /// The temp files the uploaded files are downloaded to
        /// </param>
        /// <param name="tempDirectory">
        /// The temp directory.
        /// </param>
        /// <returns>
        /// A list of IReceivedFiles containing the original file information.
        /// </returns>
        public List<IReceivedLogFile> DownloadUploadedFiles(
            List<KeyValuePair<string, string>> uploadedFiles,
            CompressionAlgorithm compression,
            IProgressMonitor progressMonitor,
            IFtpHandler ftpHandler,
            List<string> tempFiles,
            string tempDirectory)
        {
            var receivedLogFiles = new List<IReceivedLogFile>();
            var maxProgress = uploadedFiles.Count + 1.0;
            int progress = 0;
            
            foreach (var uploadedKvp in uploadedFiles)
            {
                var unitName = uploadedKvp.Key;
                var filePath = uploadedKvp.Value;
                var fileName = Path.GetFileName(filePath);
                var tempPath = tempDirectory + filePath + FileDefinitions.TempFileExtension;
                progressMonitor.Progress((++progress) / maxProgress, fileName);

                if (progressMonitor.IsCancelled)
                {
                    return receivedLogFiles;
                }

                try
                {
                    tempFiles.Add(tempPath);
                    ftpHandler.DownloadFile(filePath, tempPath, ftpHandler.GetFileSize(filePath), new FtpProgressPart(fileName, progressMonitor, progress, maxProgress));
                    receivedLogFiles.Add(this.CreateReceivedLogFile(unitName, fileName, tempPath, compression));
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't download log from " + filePath);
                }
            }

            return receivedLogFiles;
        }

        /// <summary>
        /// Downloads log files from the FTP server, into a list of received log files.
        /// </summary>
        /// <param name="logFiles"></param>
        /// <param name="compression"></param>
        /// <param name="progressMonitor"></param>
        /// <param name="tempFiles"></param>
        /// <param name="receivedLogFiles">The list of downloaded log files.</param>
        /// <param name="progress"></param>
        /// <param name="maxProgress"></param>
        private void DownloadLogFiles(
            List<KeyValuePair<string, string>> logFiles,
            CompressionAlgorithm compression,
            IProgressMonitor progressMonitor,
            List<string> tempFiles,
            List<IReceivedLogFile> receivedLogFiles,
            int progress,
            double maxProgress)
        {
            foreach (var kvp in logFiles.ToArray())
            {
                var unitName = kvp.Key;
                var filePath = kvp.Value;
                var fileName = Path.GetFileName(filePath);
                var tempPath = Path.Combine(this.temporaryDirectory, Guid.NewGuid() + FileDefinitions.LogFileExtension + FileDefinitions.TempFileExtension);
                progressMonitor.Progress((++progress) / maxProgress, fileName);

                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                try
                {
                    tempFiles.Add(tempPath);
                    this.ftpHandler.DownloadFile(filePath, tempPath, this.ftpHandler.GetFileSize(filePath), new FtpProgressPart(fileName, progressMonitor, progress, maxProgress));
                    receivedLogFiles.Add(this.CreateReceivedLogFile(unitName, fileName, tempPath, compression));
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't download log from " + filePath);
                    logFiles.Remove(kvp);
                }
            }
        }

        private IReceivedLogFile CreateReceivedLogFile(
            string unitName, string fileName, string tempPath, CompressionAlgorithm compression)
        {
            switch (compression)
            {
                case CompressionAlgorithm.None:
                    return new FileReceivedLogFile(unitName, fileName, tempPath);
                case CompressionAlgorithm.GZIP:
                    return new GZipReceivedLogFile(unitName, fileName, tempPath);
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        private T DownloadXmlFile<T>(
            string fileName, CompressionAlgorithm compression, IPartProgressMonitor progressMonitor)
            where T : class, new()
        {
            var tempFile = Path.Combine(this.temporaryDirectory, Guid.NewGuid() + FileDefinitions.TempFileExtension);
            var size = this.ftpHandler.GetFileSize(fileName);
            try
            {
                this.ftpHandler.DownloadFile(fileName, tempFile, size, progressMonitor);
            }
            catch
            {
                this.TryDeleteFile(tempFile);
                throw;
            }

            using (var input = new TemporaryFileStream(tempFile))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(this.compressionFactory.CreateDecompressionStream(input, compression));
            }
        }

        private void TryDeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't delete file: " + fileName);
            }
        }

        private void FindFeedbackFiles(RepositoryVersionConfig repoConfig, IProgressMonitor progressMonitor, out List<KeyValuePair<string, string>> logFiles, out List<string> stateFiles)
        {
            logFiles = new List<KeyValuePair<string, string>>();
            stateFiles = new List<string>();
            
            foreach (var unitDirectory in
                this.ftpHandler.GetEntries(this.GetRepositoryPath(repoConfig.FeedbackDirectory)))
            {
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                if (!this.ftpHandler.DirectoryExists(unitDirectory))
                {
                    continue;
                }

                var unitName = Path.GetFileName(unitDirectory);
                var entries = this.ftpHandler.GetEntries(unitDirectory);
                foreach (var entry in entries)
                {
                    if (!this.ftpHandler.FileExists(entry))
                    {
                        continue;
                    }

                    if (entry.EndsWith(FileDefinitions.LogFileExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        logFiles.Add(new KeyValuePair<string, string>(unitName, entry));
                    }
                    else if (entry.EndsWith(FileDefinitions.UpdateStateInfoExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        stateFiles.Add(entry);
                    }
                }
            }
        }

        private bool TryGetRepositoryConfig(out RepositoryConfig repositoryConfig)
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  Enter");
            var timeout = this.lastRepoConfigDownload + (this.pollTimer.Interval.TotalMilliseconds / 2);
            if (this.lastRepoConfig != null && TimeProvider.Current.TickCount < timeout)
            {
                // prevent downloading the repo config too often
                repositoryConfig = this.lastRepoConfig;
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  #0 Checkpoint" + repositoryConfig);
                return true;
            }
           
            if (!this.ftpHandler.FileExists(this.RepositoryConfigPath))
            {
                repositoryConfig = null;
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  #1 Checkpoint");
                return false;
            }
           
            try
            {
                repositoryConfig = this.DownloadXmlFile<RepositoryConfig>(
                    this.RepositoryConfigPath,
                    CompressionAlgorithm.None,
                    new NullProgressMonitor());
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't download the repository config, ignoring it");
                repositoryConfig = null;
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  #2 Checkpoint");
                return false;
            }
           
            this.lastRepoConfig = repositoryConfig;
            this.lastRepoConfigDownload = TimeProvider.Current.TickCount;
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  Exit");
            return true;
        }

        private void DeleteRemoteFile(string fileName)
        {
            try
            {
                this.ftpHandler.DeleteFile(fileName);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't delete file " + fileName);
            }
        }

        private RepositoryVersionConfig CreateRepository(bool createFolders)
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name +  " Enter");
            lock (this.repositoryConfigLock)
            {
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + " Lock Enter");
                RepositoryConfig repositoryConfig;
                if (!this.TryGetRepositoryConfig(out repositoryConfig))
                {
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + repositoryConfig);
                    repositoryConfig = this.CreateDefaultRepositoryConfig();
                   
                    this.Logger.Trace("Uploading repository config file to {0}", this.RepositoryConfigPath);
                    try
                    {
                        if (this.ftpHandler.FileExists(this.RepositoryConfigPath))
                        {
                            this.ftpHandler.DeleteFile(this.RepositoryConfigPath);
                        }

                        // of course the repository config can't be compressed,
                        // therefore we use CompressionAlgorithm.None
                        this.UploadXmlFile(
                            this.RepositoryConfigPath,
                            repositoryConfig,
                            CompressionAlgorithm.None,
                            new NullProgressMonitor());
                        this.lastRepoConfigDownload = TimeProvider.Current.TickCount;
                        createFolders = true;
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, "Couldn't upload repository.xml");
                    }
                }
               
                var versionConfig = repositoryConfig.GetCurrentConfig();
                if (createFolders)
                {
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  createFolders enter");
                    this.ftpHandler.CreateDirectory(this.Config.RepositoryBasePath, true);
                    this.ftpHandler.CreateDirectory(this.GetRepositoryPath(versionConfig.ResourceDirectory), false);
                    this.ftpHandler.CreateDirectory(this.GetRepositoryPath(versionConfig.CommandsDirectory), false);
                    this.ftpHandler.CreateDirectory(this.GetRepositoryPath(versionConfig.FeedbackDirectory), false);
                    this.ftpHandler.CreateDirectory(this.GetRepositoryPath(versionConfig.UploadsDirectory), false);
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  createFolders exit");
                }
                this.Logger.Info(MethodBase.GetCurrentMethod().Name +  " Exit");
                return versionConfig;
            }
        }

        private void UploadXmlFile<T>(
            string filePath, T obj, CompressionAlgorithm compression, IPartProgressMonitor progressMonitor)
            where T : class, new()
        {
            var tempFilePath = filePath + "." + ApplicationHelper.MachineName + FileDefinitions.TempFileExtension;
            this.Logger.Trace("Uploading file to {0}", tempFilePath);
            var memory = new MemoryStream();
            var serializer = new XmlSerializer(typeof(T));
           
            using (var compressed = this.compressionFactory.CreateCompressionStream(
                new NonClosingStream(memory), compression))
            {
                serializer.Serialize(compressed, obj);
               
            }
           

            memory.Seek(0, SeekOrigin.Begin);
            this.ftpHandler.UploadStream(memory, tempFilePath, memory.Length, progressMonitor);

            try
            {
                if (this.ftpHandler.FileExists(filePath))
                {
                    this.ftpHandler.DeleteFile(filePath);
                }

                this.Logger.Trace("Renaming file to {0}", filePath);
                this.ftpHandler.MoveFile(tempFilePath, filePath);
            }
            catch (Exception)
            {
                try
                {
                    this.ftpHandler.DeleteFile(tempFilePath);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't delete temporary file: " + tempFilePath);
                }

                throw;
            }
        }

        private string GetRepositoryPath(params string[] pathElements)
        {
            var path = this.Config.RepositoryBasePath;
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  basepath= " + path);
            foreach (var element in pathElements)
            {
                path = Path.Combine(path, element);
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + "  each path= " + path);
            }

            return path.Replace('\\', '/');
        }

        private List<KeyValuePair<UpdateCommand, List<string>>> GetAllResources(
            IEnumerable<UpdateCommand> commands, out int resourceCount)
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + "Enter");
            var all = new List<KeyValuePair<UpdateCommand, List<string>>>();
            var allResources = new SortedList<string, string>();
            resourceCount = 0;
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

                    resourceCount++;
                    resources.Add(hash);
                    allResources.Add(hash, hash);
                }
            }
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + " Exit ");
            return all;
        }

        private void CopyResource(
            string resourcePath, string hash, CompressionAlgorithm compression, IPartProgressMonitor progressMonitor)
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + resourcePath);
            if (this.ftpHandler.FileExists(resourcePath))
            {
                // TODO: should we check if the file is valid?
                this.Logger.Debug("Resource already {0} exists on target: {1}", hash, resourcePath);
                return;
            }

            var tempFilePath = resourcePath + "." + ApplicationHelper.MachineName + FileDefinitions.TempFileExtension;
            var resource = this.Context.ResourceProvider.GetResource(hash);
            using (var input = this.compressionFactory.CreateCompressedStream(resource.OpenRead(), compression))
            {
                this.ftpHandler.UploadStream(input, tempFilePath, input.Length, progressMonitor);
            }
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + Environment.NewLine + tempFilePath + Environment.NewLine + resourcePath);
            this.ftpHandler.MoveFile(tempFilePath, resourcePath);
        }

        private void CreateUpdateCommandFile(
            string directory,
            string fileName,
            UpdateCommand command,
            CompressionAlgorithm compression,
            IPartProgressMonitor progressMonitor)
        {
            this.Logger.Info("CreateUpdateCommandFile Creating Directory={0}", directory);
            this.ftpHandler.CreateDirectory(directory, false);
            var commandPath = Path.Combine(directory, fileName);
            this.Logger.Info("CreateUpdateCommandFile UploadXmlFile {0} for UnitId {1}", commandPath, command.UnitId.UnitName);
            this.UploadXmlFile(commandPath, command, compression, progressMonitor);
        }

        private void PollTimerOnElapsed(object sender, EventArgs e)
        {
            try
            {
                this.DoCheckForFeedback(null);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't check for feedback");
            }

            this.pollTimer.Enabled = true;
        }
    }
}
