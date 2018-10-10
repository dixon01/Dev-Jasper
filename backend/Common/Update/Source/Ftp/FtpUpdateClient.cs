// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="FtpUpdateClient.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.Multicast.Core;

    using Math = System.Math;

    /// <summary>
    ///     Class to handle update of system via FTP
    /// </summary>
    public class FtpUpdateClient : UpdateClientBase<FtpUpdateClientConfig>
    {
        private const int LockTimeout = 60000;

        private const long MaximumIntermediateFeedbackCount = 10; // maximum 10 feedbacks per download

        private const long MinimumIntermediateFeedbackSize = 20 * 1024; // 20 kB

        private const string RemotenetworkinfoXml = @"..\..\remotenetworkInfo.xml";

        private readonly ITimer pollTimer;

        private readonly object uploadsInProgress = new object();

        private CompressionFactory compressionFactory;

        private FtpHandler ftpHandler;

        private RepositoryConfig lastRepoConfig;

        private long lastRepoConfigDownload;

        private DateTime? lastTimedDownloads;

        private MulticastManager multicastManager;

        private FtpUpdateClientConfig originalFtpUpdateClientConfig;

        private string temporaryDirectory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FtpUpdateClient" /> class.
        /// </summary>
        public FtpUpdateClient()
        {
            this.pollTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
            this.pollTimer.AutoReset = false;
            this.pollTimer.Elapsed += this.PollTimerOnElapsed;
            this.NetworkUpdateZoneConnected = false;
            this.MulticastIPAddressFound = false;
            this.multicastManager = null;
            this.ConnectedToMulticastFtp = false;

            // this.SyncResources();
        }

        /// <summary>The network availability changed.</summary>
        public event EventHandler<NetworkConnectionMessageEventsArgs> NetworkConnectionChanged;

        /// <summary>Gets the last network connected time.</summary>
        public DateTime? LastNetworkConnectedTime { get; private set; }

        /// <summary>Gets or sets a value indicating whether multicast ip address found.</summary>
        public bool MulticastIPAddressFound { get; set; }

        /// <summary>Gets a value indicating whether network connection connected.</summary>
        public bool NetworkUpdateZoneConnected { get; private set; }

        private bool AreFtpConfigCredentialsValid
        {
            get
            {
                return this.Config != null && FtpUpdateClientConfig.IsFtpCredentialsValid(this.Config);
            }
        }

        private bool ConnectedToMulticastFtp { get; set; }

        private string LocalCommandsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(this.Config?.LocalFtpHomePath))
                {
                    return string.Empty;
                }

                return Path.Combine(this.Config.LocalFtpHomePath, "Commands").Replace('\\', '/');
            }
        }

        private string RepositoryConfigPath
        {
            get
            {
                return this.Config != null
                           ? Path.Combine(this.Config.RepositoryBasePath, RepositoryConfig.RepositoryXmlFileName)
                               .Replace('\\', '/')
                           : string.Empty;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="FtpUpdateClient"/> class.</summary>
        /// <param name="config">The config.</param>
        /// <param name="updateContext">The update context.</param>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="ArgumentException">Invalid FtpClient Config</exception>
        public override void Configure(UpdateClientConfigBase config, IUpdateContext updateContext)
        {
            base.Configure(config, updateContext);
            this.pollTimer.Interval = this.Config.PollInterval;
            this.temporaryDirectory = Path.Combine(this.Context.TemporaryDirectory, Path.Combine("FTP", this.Name));
            var ftpUpdateClientConfig = config as FtpUpdateClientConfig;
            if (ftpUpdateClientConfig == null)
            {
                throw new ArgumentException("Invalid FtpClient Configuration");
            }

            // save the original ftp client config off - LTG
            this.originalFtpUpdateClientConfig = new FtpUpdateClientConfig
                                                     {
                                                         Host = ftpUpdateClientConfig.Host,
                                                         Username = ftpUpdateClientConfig.Username,
                                                         Password = ftpUpdateClientConfig.Password,
                                                         LocalFtpHomePath = ftpUpdateClientConfig.LocalFtpHomePath,
                                                         EnableMulticastIP = ftpUpdateClientConfig.EnableMulticastIP,
                                                         ShowVisualization = ftpUpdateClientConfig.ShowVisualization,
                                                         PollInterval = ftpUpdateClientConfig.PollInterval,
                                                         RepositoryBasePath = ftpUpdateClientConfig.RepositoryBasePath,
                                                         RequireWifiNetworkConnection = ftpUpdateClientConfig.RequireWifiNetworkConnection,
                                                         Port = ftpUpdateClientConfig.Port
                                                     };

            // initialize on startup with the current ftp client config settings
            this.ftpHandler = this.CreateFtpHandler(ftpUpdateClientConfig);

            this.compressionFactory = new CompressionFactory(this.temporaryDirectory);

            if (ftpUpdateClientConfig.EnableMulticastIP)
            {
                // allow getting the FTP IP and login credentials from the Multicast manager vs using the xml config file
                this.CreateMulticastManager();
            }

            this.Logger.Debug(
                "FtpUpdateClient Configure.pollTimer.Interval={0}, requireWifiConnection={1}",
                this.pollTimer.Interval,
                ftpUpdateClientConfig.RequireWifiNetworkConnection);

            MessageDispatcher.Instance.Subscribe<NetworkChangedMessage>(
                (sender, args) =>
                    {
                        var networkConnectionMessage = args.Message;
                        this.NetworkUpdateZoneConnected = networkConnectionMessage.WiFiConnected;

                        this.Logger.Info(
                            "*** NetworkUpdateZoneConnected Changed via Medi Message Connection = {0} ***",
                            this.NetworkUpdateZoneConnected);

                        this.NetworkConnectionChanged?.Invoke(
                            this,
                            new NetworkConnectionMessageEventsArgs(this.NetworkUpdateZoneConnected));

                        if (this.NetworkUpdateZoneConnected)
                        {
                            var elaspedTimeSpan = this.LastNetworkConnectedTime.HasValue
                                                      ? DateTime.Now.TimeOfDay.Subtract(
                                                          this.LastNetworkConnectedTime.Value.TimeOfDay)
                                                      : TimeSpan.Zero;
                            var elamsedTimeMinutes = elaspedTimeSpan.Minutes;

                            // use the poll time span interval to throttle us
                            var timerPollInterval = this.pollTimer.Interval.Minutes;

                            // If we want to limit how often case we receive multiple events
                            if (elamsedTimeMinutes == 0 || elamsedTimeMinutes > timerPollInterval)
                            {
                                this.LastNetworkConnectedTime = DateTime.Now;
                                this.DoTimedDownloads();
                            }
                            else
                            {
                                this.Logger.Info(
                                    "Already did a download on {0}, Ignored for now",
                                    this.LastNetworkConnectedTime);
                            }
                        }

                        this.Logger.Trace(
                            "FtpUpdateClient received Network Connection changed event Connected={0}",
                            this.NetworkUpdateZoneConnected);
                        Debug.WriteLine(
                            "FtpUpdateClient  Test Received Message Dispatched for NetworkConnection value = "
                            + networkConnectionMessage.WiFiConnected);
                    });
        }

        /// <summary>
        /// Restart apps, currently does nothing.
        /// </summary>
        public void RestartApps()
        {
            // this.Logger.Warn("Restarting Update Manager after Update for completed Update process");
            // var updateApp = ServiceLocator.Current.GetInstance<UpdateApplication>();
            // updateApp?.Relaunch("Relaunch For MCU Update");

            // Natraj - Below calls did not restart the hardware manager
            // var hardwareMgrApp = ServiceLocator.Current.GetInstance<HardwareManagerApplication>();
            // hardwareMgrApp?.Relaunch("Relaunch For MCU Update");
            // var sysApp = ServiceLocator.Current.GetInstance<SystemManagerApplication>();
            // sysApp.Controller.RequestReboot("MCU Update");
        }

        /// <summary>Sends feedback back to the source.</summary>
        /// <param name="logFiles">The log files to upload.</param>
        /// <param name="stateInfos">The state information objects to upload.</param>
        /// <param name="progressMonitor">The progress monitor that observes the upload of update feedback and log files.</param>
        /// <exception cref="UpdateException">Couldn't upload feedback</exception>
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
            states.ForEach(
                x => this.Logger.Info(
                    $"Feedback {nameof(x.State)} : {x.State}, " + $" {nameof(x.Description)}:{x.Description},"
                    + $" {x.UnitId}," + $" {x.UpdateSource}," + $" {x.ErrorReason}," + $" {x.UpdateId},"
                    + $" {x.Version} "));

            var maxProgress = logs.Count + states.Count + 1.0;
            var progress = 0;
            try
            {
                var currentConfig = this.GetRemoteRepositoryConfiguration(this.Config);

                if (currentConfig == null)
                {
                    return;
                }

                var feedbackPath = Path.Combine(this.Config.RepositoryBasePath, currentConfig.FeedbackDirectory);
                this.Logger.Trace("FeedbackPath = {0}", feedbackPath);
                this.ftpHandler.CreateDirectory(feedbackPath, false);
                foreach (var log in logs)
                {
                    progressMonitor.Progress(++progress / maxProgress, log.FileName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    this.UploadLogFile(
                        feedbackPath,
                        log,
                        currentConfig.Compression,
                        new FtpProgressPart(log.FileName, progressMonitor, progress, maxProgress));
                }

                foreach (var state in states)
                {
                    progressMonitor.Progress(++progress / maxProgress, state.UnitId.UnitName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    this.Logger.Info(
                        $" UploadState : {state} , UnitName : {state.UnitId.UnitName}, Current Progress : {progress}, Max Progress: {maxProgress}");
                    this.UploadStateFile(
                        feedbackPath,
                        state,
                        currentConfig.Compression,
                        new FtpProgressPart(state.UnitId.UnitName, progressMonitor, progress, maxProgress));
                }

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                progressMonitor.Complete("Couldn't upload feedback: " + ex.Message + ex.InnerException?.Message, null);
                throw new UpdateException("Couldn't upload feedback");
            }
        }

        /// <summary>
        /// Upload files to the remote server.
        /// </summary>
        /// <param name="uploadFiles">
        /// The log files to upload.
        /// </param>
        /// <exception cref="UpdateException">
        /// Couldn't upload files
        /// </exception>
        public override void UploadFiles(IList<IReceivedLogFile> uploadFiles)
        {
            if (uploadFiles.Count == 0)
            {
                return;
            }

            var locked = Monitor.TryEnter(this.uploadsInProgress, LockTimeout);
            if (!locked)
            {
                return;
            }

            if (!this.IsAvailable)
            {
                throw new DirectoryNotFoundException("Couldn't find " + this.RepositoryConfigPath);
            }

            var progressMonitor =
                this.Context.CreateProgressMonitor(
                    UpdateStage.UploadingFiles,
                    false); // We never show the progress bar for FTPs.
            progressMonitor.Start();

            var files = new List<IReceivedLogFile>(uploadFiles);
            this.Logger.Info($"Uploading {files.Count} files");

            var maxProgress = files.Count + 1.0;
            var progress = 0;

            try
            {
                // Respect the wifi configuration
                var currentConfig = this.GetRemoteRepositoryConfiguration(this.Config);

                if (currentConfig == null)
                {
                    return;
                }

                if (string.IsNullOrEmpty(currentConfig.UploadsDirectory))
                {
                    this.Logger.Trace("Remote repository upload path not defined.");
                    return;
                }

                string remoteUploadsDirectory = Path.Combine(this.Config.RepositoryBasePath, currentConfig.UploadsDirectory);
                this.Logger.Trace($"Remote base upload path = {remoteUploadsDirectory}");

                this.ftpHandler.CreateDirectory(remoteUploadsDirectory, false);

                foreach (var file in files)
                {
                    progressMonitor.Progress(++progress / maxProgress, file.FileName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    var uploadFile = file as FileReceivedLogFile;
                    if (uploadFile == null)
                    {
                        continue;
                    }

                    // Create the full directory structure (with TFT subfolder last) on the server.
                    string fullRemotePath = Path.Combine(remoteUploadsDirectory, this.GetDestinationSubdirectories(uploadFile));
                    this.ftpHandler.CreateDirectory(fullRemotePath, true);

                    this.Logger.Trace($"Uploading {uploadFile.FileName} to {fullRemotePath}");

                    try
                    {
                        this.UploadLogFile(
                            fullRemotePath,
                            uploadFile,
                            currentConfig.Compression,
                            new FtpProgressPart(uploadFile.FileName, progressMonitor, progress, maxProgress));

                        uploadFile.Delete();
                    }
                    catch (Exception e)
                    {
                        this.Logger.Trace(
                            $"Unable to upload {uploadFile.FilePath} to {fullRemotePath}, skipping. {e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                progressMonitor.Complete($"Couldn\'t upload file(s): {ex.Message}{ex.InnerException?.Message}", null);
                throw new UpdateException("Couldn't upload file(s)");
            }
            finally
            {
                this.Logger.Trace("Upload complete.");
                progressMonitor.Complete(null, null);
                Monitor.Exit(this.uploadsInProgress);
            }
        }

        /// <summary>
        ///     Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        ///     True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            if (this.ftpHandler != null)
            {
                // make ftp call to check for folder to test the ftp connection
                this.Logger.Info($"Verifying repository config exists on FTP server at {this.RepositoryConfigPath}");
                var checkAvailable = this.ftpHandler.FileExists(this.RepositoryConfigPath);
                this.Logger.Info($"{this.RepositoryConfigPath} found? {checkAvailable}");
                return checkAvailable;
            }
            
            this.Logger.Error("Invalid ftp handler. FTP server not available.");
            return false;
        }

        /// <summary>
        ///     Implementation of the <see cref="UpdateClientBase{TConfig}.Start" /> method.
        /// </summary>
        protected override void DoStart()
        {
            if (this.IsAvailable)
            {
                this.DownloadUpdates();
            }

            this.pollTimer.Enabled = true;
        }

        /// <summary>
        ///     Stops the update client
        /// </summary>
        protected override void DoStop()
        {
            this.pollTimer.Enabled = false;
            if (this.ftpHandler != null)
            {
                this.ftpHandler.Dispose();
                this.ftpHandler = null;
            }

            if (this.multicastManager != null)
            {
                this.multicastManager.Stop();
                this.multicastManager = null;
            }
        }

        /// <summary>
        /// The current ftp update client configuration
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        protected override void UpdateConfig(FtpUpdateClientConfig config)
        {
            if (config != null)
            {
                base.UpdateConfig(config);
                this.Logger.Info("FtpUpdateClientConfig Updated IP:{0}", config.Host);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not we can process downloads via FTP
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanFtpDownloadsProcess()
        {
            this.Logger.Trace("Checking if FTP Downloads can process.");
            if (this.Config == null)
            {
                this.Logger.Error("FTPUpdate client configuration is not valid!");
                return false;
            }

            this.Logger.Trace($"Enable Multicast IP: {this.Config.EnableMulticastIP}");
            this.Logger.Trace($"Can Updates Run For Multicast: {this.CanUpdatesRunForMulticast()}");
            this.Logger.Trace($"Require Wifi Network Connection: {this.Config.RequireWifiNetworkConnection}");
            this.Logger.Trace($"Network Update Zone Connected: {this.NetworkUpdateZoneConnected}");

            // First check the config if the Multicast feature to change the Host IP is enabled, ignore if not
            if (this.Config.EnableMulticastIP)
            {
                this.Logger.Debug("Multicast IP enabled, checking updates can run for Multicast.");
                if (this.CanUpdatesRunForMulticast())
                {
                    this.Logger.Debug("Updates for multicast can run.");
                    return true;
                }
            }

            if (!this.Config.RequireWifiNetworkConnection)
            {
                this.Logger.Debug("Wifi network connection not required, able to process ftp downloads.");
                return true;
            }

            this.Logger.Debug("Wifi connection required, checking connection status.");

            if (this.NetworkUpdateZoneConnected)
            {
                this.Logger.Trace("Connected, able to process ftp downloads.");
                return true;
            }

            this.Logger.Debug("Not connected, unable to process ftp downloads.");
            return false;
        }

        /// <summary>
        /// Verifies we can create a valid FTP Handler that can perform FTP operations.
        /// </summary>
        /// <param name="config">The current ftp update client configuration</param>
        /// <returns>Whether or not we the ftp update client can perform ftp operations</returns>
        private bool CanProcessTransferRequests(FtpUpdateClientConfig config)
        {
            if (config != null)
            {
                this.ftpHandler = this.CreateFtpHandler(config);
            }

            if (this.ftpHandler == null)
            {
                this.Logger.Error("DownloadUpdates() FtpHandler = null, Updated ignored");
                return false;
            }

            // are ftp downloads or uploads allowed ?
            if (this.CanFtpDownloadsProcess() == false)
            {
                this.Logger.Info("Ftp Updates Ignored, not allowed");
                if (this.Config.RequireWifiNetworkConnection && this.NetworkUpdateZoneConnected == false)
                {
                    this.Logger.Info("Ftp Updates not in WiFi Update Zone Not set!");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether or not we can can process updates via Multicast
        /// Flag when true use to allow the updates to continue for the multicast cast of changing out the ftp handler to a new host
        /// Check the state flag MulticastIPAddressFound and if the current Config has the feature enabled.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanUpdatesRunForMulticast()
        {
            this.Logger.Trace($"Multicast IP address found? {this.MulticastIPAddressFound}");
            this.Logger.Trace($"Is Ftp Config Credentials Valid? {this.AreFtpConfigCredentialsValid}");

            // we check if our flag was set externally and we have a valid Config
            bool canFtpDownloadsForMulticastProcess = false;
            if (this.MulticastIPAddressFound && this.Config != null && this.Config.EnableMulticastIP)
            {
                // verify the config is valid
                canFtpDownloadsForMulticastProcess = this.AreFtpConfigCredentialsValid;
            }

            return canFtpDownloadsForMulticastProcess;
        }

        // Store a Command File to the \Commands folder on the home ftp root folder
        private void CopyCommandFileToLocalFtpCommandFolder(string commandFile)
        {
            var gucFile = Path.GetFileNameWithoutExtension(commandFile);

            // For Infotransite copy this local GUC to the root ftp folder \ Commands sub folder
            try
            {
                // preserve a copy of the last commands file - LTG
                // Delete the target folder of all GUCs then copy this new guc to target folder
                var targetFolder = this.LocalCommandsFolder;
                this.Logger.Info("Keeping copy of GUC file {0}", gucFile);
                var targetFile = Path.Combine(targetFolder, gucFile);
                this.Logger.Info($"Copy Commands(guc) file {commandFile} to target folder {targetFile}");
                this.CopyFileToLocalRootFtpFolder(commandFile, targetFile);
            }
            catch (Exception ex)
            {
                this.Logger.Error("Failed to Copy Commands file {0}", ex.Message);
            }
            finally
            {
                // last delete the command file
                File.Delete(commandFile);
            }
        }

        // Copy the file to the Ftp root when the Ftp local root path is defined - LTG
        private void CopyFileToLocalRootFtpFolder(string sourceFileName, string targetFileName)
        {
            var targetPath = this.Config.LocalFtpHomePath;
            if (string.IsNullOrEmpty(sourceFileName) || string.IsNullOrEmpty(targetPath))
            {
                return;
            }

            if (string.IsNullOrEmpty(targetFileName))
            {
                targetFileName = Path.GetFileName(sourceFileName);
            }

            // set default target folder if not provided
            var dir = Path.GetDirectoryName(targetFileName);
            if (string.IsNullOrEmpty(dir))
            {
                targetFileName = Path.Combine(targetPath, Path.GetFileName(targetFileName));
            }

            try
            {
                dir = Path.GetDirectoryName(targetFileName);
                if (Directory.Exists(dir) == false)
                {
                    this.Logger.Debug("Creating missing folder {0}", dir);
                    Directory.CreateDirectory(dir);
                }

                File.Copy(sourceFileName, targetFileName, true);
                this.Logger.Info("Copied File {0} to {1}", sourceFileName, targetFileName);
            }
            catch (Exception ex)
            {
                this.Logger.Error("Failed to copy File {0} to {1}, {2}", sourceFileName, targetFileName, ex.Message);
            }
        }

        private List<DownloadProgress> CreateDownloadProgresses(
            Dictionary<string, List<UpdateCommand>> resourceHashes,
            List<UpdateCommand> commands,
            Dictionary<string, long> downloads)
        {
            var downloadProgresses = new List<DownloadProgress>(commands.Count);
            foreach (var command in commands)
            {
                var downloadProgress = new DownloadProgress(command);
                downloadProgresses.Add(downloadProgress);
                foreach (var resourcePair in resourceHashes)
                {
                    var hash = resourcePair.Key;
                    if (resourcePair.Value.Contains(command) && downloads.TryGetValue(hash, out long size))
                    {
                        downloadProgress.AddResource(hash, size);
                    }
                }
            }

            return downloadProgresses;
        }

        private bool CreateFtpFile(string fileName, string data = "", IPartProgressMonitor progressMonitor = null)
        {
            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} - Enter");
            if (this.ftpHandler != null && !string.IsNullOrEmpty(fileName))
            {
                var fileNameOnly = Path.GetFileName(fileName);
                var destFileName = Path.Combine(this.Config.RepositoryBasePath, fileNameOnly).Replace('\\', '/');
                try
                {
                    using (var fs = File.Open(fileName, FileMode.OpenOrCreate))
                    {
                        if (!string.IsNullOrEmpty(data))
                        {
                            var buf = Encoding.ASCII.GetBytes(data);
                            fs.Write(buf, 0, buf.Length);
                            fs.Flush();
                        }

                        fs.Seek(0, SeekOrigin.Begin);

                        this.ftpHandler.UploadStream(
                            fs,
                            destFileName,
                            fs.Length,
                            progressMonitor ?? new NullProgressMonitor());
                        this.Logger.Info("Created Ftp File {0} on stream", destFileName);
                    }

                    return this.ftpHandler.FileExists(destFileName);
                }
                catch (WebException ex)
                {
                    this.Logger.Error("Failed to create remote Ftp File {0} {1}", destFileName, ex.Message);
                }
            }

            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} - Exiting with false.");
            return false;
        }

        /// <summary>
        /// Create the Ftp Handler with the config settings 
        /// </summary>
        /// <param name="config">The Ftp Client Config settings</param>
        /// <returns>The FTP handler utility class</returns>
        private FtpHandler CreateFtpHandler(FtpUpdateClientConfig config)
        {
            lock (this.uploadsInProgress)
            {
                this.ftpHandler?.Dispose();
                this.Logger.Debug("CreateFtpHandler() Host={0}", config.Host);
                return new FtpHandler(config);
            }
        }

        private void CreateMulticastManager()
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            try
            {
                this.multicastManager = new MulticastManager();
                this.multicastManager.StartMulticast();
                this.multicastManager.RemoteNetworkInfoChanged += this.MulticastManagerOnRemoteNetworkInfoChanged;
                this.multicastManager.MultiCastCommand += this.MulticastManagerMultiCastCommand;
            }
            catch (Exception ex)
            {
                this.multicastManager = null;
                this.Logger.Warn(
                    "Failed to create MulticastManager, feature will be ignored" + ex.Message
                    + (ex.InnerException?.Message ?? string.Empty));
            }
        }

        private bool CreateStatusFile(MulticastUpdateClientState state, string data = "")
        {
            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} - {state} ");
            string fileName = string.Empty;
            switch (state)
            {
                case MulticastUpdateClientState.Started:
                    fileName = $"Connect-{MessageDispatcher.Instance.LocalAddress.Unit}.txt";

                    break;

                case MulticastUpdateClientState.Finished:
                    if (this.multicastManager.MulticastConfig.CreateCompleteStatus)
                    {
                        fileName = $"Complete-{MessageDispatcher.Instance.LocalAddress.Unit}.txt";
                    }
                    else
                    {
                        this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} Not creating complete as it was set to FALSE MulticastConfig.");
                        return true;
                    }

                    break;
                default:
                    this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} Should not be here {nameof(state)} : {state}.");
                    break;
            }

            var statusFile = this.CreateFtpFile(fileName, data);
            this.Logger.Info(!statusFile ? $" could not create ftp file {fileName}" : $"  ftp file {fileName} created sucessfully");
            return statusFile;
        }

        private void DeleteCommandFiles(IEnumerable<string> commandFiles)
        {
            foreach (var commandFile in commandFiles)
            {
                this.Logger.Trace("Deleting command file on ftp server {0}", commandFile);
                try
                {
                    this.ftpHandler.DeleteFile(commandFile);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't delete command file: " + commandFile);
                    continue;
                }

                var localTempFile = this.GetTempCommandPath(commandFile);

                this.Logger.Trace("Deleting temp commands file {0}", localTempFile);
                File.Delete(localTempFile);
            }
        }

        private void DoFtpUpdateFromMulticast(FtpUpdateClientConfig ftpUpdateClientConfig)
        {
            if (ftpUpdateClientConfig != null && this.multicastManager != null
                && FtpUpdateClientConfig.IsFtpCredentialsValid(ftpUpdateClientConfig))
            {
                var originalConfig = this.Config;
                try
                {
                    this.Logger.Info("DoUpdates Successful Multicast settings received - Stopping Multicast");
                    this.MulticastIPAddressFound = true;
                    this.multicastManager?.StopMulticast();
                    this.lastRepoConfigDownload = 0; // So we don't keep using the cached (previous) config.
                    this.UpdateConfig(ftpUpdateClientConfig); // Because a lot of things use the Config, not just DownloadUpdates()

                    // kick off the updates and use new configuration
                    this.DownloadUpdates(ftpUpdateClientConfig);
                    this.UploadFiles();
                }
                finally
                {
                    this.lastRepoConfigDownload = 0;
                    this.RestoreConfig(originalConfig);
                    this.MulticastIPAddressFound = false;
                    this.Logger.Info("DoUpdates complete restarting Multicast Manager (from finally)");

                    if (File.Exists(RemotenetworkinfoXml))
                    {
                        File.Delete(RemotenetworkinfoXml);
                    }

                    this.multicastManager?.StartMulticast();
                }
            }
        }

        private void DoTimedDownloads()
        {
            try
            {
                this.pollTimer.Enabled = false;
                this.Logger.Debug(
                    "FtpUpdateClient.DoTimedDownloads() Enter, Last Download was {0}",
                    this.lastTimedDownloads.HasValue ? this.lastTimedDownloads.ToString() : "N/A");

                this.lastTimedDownloads = DateTime.Now;
                this.Logger.Debug(
                    "FtpUpdateClient Location is available: {0}:{1}/{2}",
                    this.Config.Host,
                    this.Config.Port,
                    this.RepositoryConfigPath);
                this.DownloadUpdates();
            }
            finally
            {
                this.pollTimer.Enabled = true;
                this.Logger.Debug("FtpUpdateClient.DoTimedDownloads() Exit");
            }
        }

        private UpdateCommand DownloadCommand(
            string commandFile,
            CompressionAlgorithm compression,
            bool deleteTempFile = true)
        {
            var tempFile = this.GetTempCommandPath(commandFile);
            var updateCommand = this.DownloadXmlFile<UpdateCommand>(commandFile, tempFile, compression, deleteTempFile);
            if (deleteTempFile == false && string.IsNullOrEmpty(this.LocalCommandsFolder) == false)
            {
                // keep local copy and delete the temp file - LTG
                this.CopyCommandFileToLocalFtpCommandFolder(tempFile);
            }

            return updateCommand;
        }

        private List<UpdateCommand> DownloadCommands(IEnumerable<string> commandFiles, CompressionAlgorithm compression)
        {
            var commands = new List<UpdateCommand>();
            foreach (var commandFile in commandFiles)
            {
                this.Logger.Trace("Downloading command: {0}", commandFile);
                try
                {
                    var command = this.DownloadCommand(commandFile, compression, false);
                    commands.Add(command);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't download Command File {0}", commandFile);
                }
            }

            return commands;
        }

        private string DownloadFile(
            string remoteFile,
            string tempFile,
            long size,
            CompressionAlgorithm compression,
            IPartProgressMonitor progressMonitor)
        {
            this.Logger.Debug("Downloading file: {0} => {1} ({2} bytes)", remoteFile, tempFile, size);
            var tempInfo = new FileInfo(tempFile);
            if (tempInfo.Exists)
            {
                if (tempInfo.Length == size)
                {
                    this.Logger.Debug("Temporary download file exists with entire contents: {0}", tempFile);
                    return this.compressionFactory.GetDecompressedFile(tempFile, compression);
                }

                if (tempInfo.Length < size)
                {
                    this.ftpHandler.ContinueDownloadFile(remoteFile, tempFile, size, progressMonitor);
                    return this.compressionFactory.GetDecompressedFile(tempFile, compression);
                }

                this.Logger.Warn("Temporary download file exists but is too big: {0}", tempFile);
                tempInfo.Delete();
            }

            this.ftpHandler.DownloadFile(remoteFile, tempFile, size, progressMonitor);
            return this.compressionFactory.GetDecompressedFile(tempFile, compression);
        }

        private string DownloadResource(
            string resourceFile,
            long size,
            string hash,
            CompressionAlgorithm compression,
            IPartProgressMonitor progressMonitor)
        {
            var tempFile = Path.Combine(
                this.temporaryDirectory,
                hash + FileDefinitions.ResourceFileExtension + FileDefinitions.TempFileExtension);

            return this.DownloadFile(resourceFile, tempFile, size, compression, progressMonitor);
        }

        private void DownloadResources(
            Dictionary<string, List<UpdateCommand>> resourceHashes,
            string resourcePath,
            List<UpdateCommand> commands,
            CompressionAlgorithm compression,
            IProgressMonitor progressMonitor)
        {
            var progress = 0;
            var maxProgress = (resourceHashes.Count * 2) + 1.0;
            var downloads = new Dictionary<string, long>();
            this.Logger.Info($" {MethodBase.GetCurrentMethod().Name} : Total of Resource Pairs - {resourceHashes.Count}");
            foreach (var resourcePair in resourceHashes)
            {
                var hash = resourcePair.Key;
                var resourceFile = this.GetResourceFilePath(resourcePath, hash);
                this.Logger.Info(
                    $"  {MethodBase.GetCurrentMethod().Name} : Looking for resource {hash} in the path {resourcePath}. File being looked for is {resourceFile}");
                progressMonitor.Progress(++progress / maxProgress, $"Checking {hash}");
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                if (this.ResourceExists(hash))
                {
                    continue;
                }

                long size;
                try
                {
                    size = this.ftpHandler.GetFileSize(resourceFile);
                }
                catch (WebException)
                {
                    // remove all commands that would require this resource
                    this.Logger.Warn("Couldn't find resource '{0}', removing all related commands", resourceFile);
                    commands.RemoveAll(c => resourcePair.Value.Contains(c));
                    this.NotifyFailedStatus(resourcePair.Value, "Couldn't find resource " + hash);
                    continue;
                }

                downloads.Add(hash, size);
            }

            var downloadProgresses = this.CreateDownloadProgresses(resourceHashes, commands, downloads);
            maxProgress = resourceHashes.Count + downloads.Count + 1.0;
            foreach (var downloadPair in downloads)
            {
                this.SendIntermediateFeedback(downloadProgresses, false);

                var hash = downloadPair.Key;
                var size = downloadPair.Value;
                var resourceFile = this.GetResourceFilePath(resourcePath, hash);

                progressMonitor.Progress(++progress / maxProgress, $"Downloading {hash}");
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                var tempFile = this.DownloadResource(
                    resourceFile,
                    size,
                    hash,
                    compression,
                    new FtpProgressPart(hash, progressMonitor, progress, maxProgress));
                this.Logger.Debug("Adding resource {0}: {1}", hash, resourceFile);
                this.Context.ResourceProvider.AddResource(hash, tempFile, true);
                foreach (var downloadProgress in downloadProgresses)
                {
                    downloadProgress.SetResourceDownloaded(hash);
                }
            }

            this.SendIntermediateFeedback(downloadProgresses, true);
        }

        private void DownloadUpdates(FtpUpdateClientConfig config = null)
        {
            this.Logger.Trace(MethodBase.GetCurrentMethod().Name + " Entering ...");
            var locked = Monitor.TryEnter(this.uploadsInProgress, LockTimeout);
            try
            {
                if (locked)
                {
                    var repositoryConfig = this.GetRemoteRepositoryConfiguration(config);

                    if (repositoryConfig == null)
                    {
                        return;
                    }

                    var repositoryBasePath = this.Config.RepositoryBasePath;

                    List<string> commandFiles;
                    try
                    {
                        var commandsPath = Path.Combine(repositoryBasePath, repositoryConfig.CommandsDirectory);
                        if (!this.ftpHandler.DirectoryExists(commandsPath))
                        {
                            this.Logger.Warn("DownloadUpdates() DirectoryExists({0}) == false", commandsPath);
                            return;
                        }

                        commandFiles = this.FindCommandFiles(commandsPath);
                        this.Logger.Debug("Found {0} commands in {1}", commandFiles.Count, commandsPath);
                        if (commandFiles.Count == 0)
                        {
                            if (this.MulticastIPAddressFound)
                            {
                                this.Logger.Warn(
                                    "Found {0} commands in {1}. Exiting Download Updates ...",
                                    commandFiles.Count,
                                    commandsPath);
                            }

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex, "Couldn't get command file paths");
                        return;
                    }

                    this.EmptyLocalCommandsFolder(); // for 3rdparty to use local GUC file for LTG - LAM 

                    var progressMonitor = this.Context.CreateProgressMonitor(
                        UpdateStage.ReceivingUpdate,
                        this.Config.ShowVisualization);
                    progressMonitor.Start();
                    progressMonitor.Progress(0, "Downloading Commands");
                    try
                    {
                        var commands = this.DownloadCommands(commandFiles, repositoryConfig.Compression);
                        this.Logger.Debug("Downloaded {0} commands", commands.Count);
                        if (commands.Count == 0)
                        {
                            progressMonitor.Complete(null, null);
                            return;
                        }

                        var resourceHashes = this.GetResourceHashes(commands);

                        var resourcePath = Path.Combine(repositoryBasePath, repositoryConfig.ResourceDirectory);
                        if (!this.ftpHandler.DirectoryExists(resourcePath))
                        {
                            var msg = $"Couldn't find resource path on ftp server: {resourcePath}";
                            this.Logger.Warn(msg);
                            progressMonitor.Complete(msg, null);
                            this.NotifyFailedStatus(commands, "Couldn't find resource path");
                            return;
                        }

                        this.Logger.Debug("Found {0} resources for {1} commands", resourceHashes.Count, commands.Count);

                        this.DownloadResources(
                            resourceHashes,
                            resourcePath,
                            commands,
                            repositoryConfig.Compression,
                            progressMonitor);
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
                    }
                }
            }
            finally
            {
                if (locked)
                {
                    this.Logger.Info("Cleanup after Update");
                    this.Logger.Trace(MethodBase.GetCurrentMethod().Name + " Exiting ...");

                    // this.SyncResources();
                    this.RestoreFromMulticastUpdate();
                    Monitor.Exit(this.uploadsInProgress);
                }
            }
        }

        private T DownloadXmlFile<T>(
            string remoteFile,
            string localTempFile,
            CompressionAlgorithm compression,
            bool deleteTempFile = true)
        {
            var size = this.ftpHandler.GetFileSize(remoteFile);
            localTempFile = this.DownloadFile(
                remoteFile,
                localTempFile,
                size,
                CompressionAlgorithm.None,
                new NullProgressMonitor());
            T obj;
            using (var fileStream = File.OpenRead(localTempFile))
            {
                var serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(
                    this.compressionFactory.CreateDecompressionStream(fileStream, compression));
            }

            if (deleteTempFile)
            {
                File.Delete(localTempFile);
            }

            return obj;
        }

        private void EmptyLocalCommandsFolder()
        {
            var targetFolder = this.LocalCommandsFolder;
            if (string.IsNullOrEmpty(targetFolder) == false)
            {
                try
                {
                    if (Directory.Exists(targetFolder))
                    {
                        this.Logger.Debug("Deleting all files from folder {0}", targetFolder);
                        var directoryInfo = new DirectoryInfo(targetFolder);
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Error("EmptyLocalCommandsFolder Exception {0}", ex.Message);
                }
            }
        }

        private List<string> FindCommandFiles(string commandsPath)
        {
            var commands = new List<string>();
            try
            {
                var directories = this.ftpHandler.GetEntries(commandsPath);
                foreach (var unitDirectory in directories)
                {
                    var unitName = Path.GetFileName(unitDirectory);

                    if (!this.ShouldDownloadForUnit(unitName))
                    {
                        this.Logger.Debug("Not downloading commands for unit {0}", unitName);
                        continue;
                    }

                    var files = this.ftpHandler.GetEntries(unitDirectory);

                    var sortedListOfFiles = new List<string>(files);

                    sortedListOfFiles.Sort((a, b) => string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase));
                    foreach (var commandFile in sortedListOfFiles)
                    {
                        if (!commandFile.EndsWith(FileDefinitions.UpdateCommandExtension))
                        {
                            continue;
                        }

                        this.Logger.Debug("Found command: {0}", commandFile);
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

        private RepositoryVersionConfig GetCurrentRepoConfig(bool forceDownload = false)
        {
            var timeout = this.lastRepoConfigDownload + (this.pollTimer.Interval.TotalMilliseconds / 2);
            if (!forceDownload && this.lastRepoConfig != null && TimeProvider.Current.TickCount < timeout)
            {
                // prevent downloading the repo config too often
                this.Logger.Trace("Using cached config file.");
                return this.lastRepoConfig.GetCurrentConfig();
            }

            this.Logger.Trace("Retrieving current config file.");
            var tempFile = Path.Combine(this.temporaryDirectory, Guid.NewGuid() + FileDefinitions.TempFileExtension);
            var repositoryConfig = this.DownloadXmlFile<RepositoryConfig>(
                this.RepositoryConfigPath,
                tempFile,
                CompressionAlgorithm.None,
                false);
            this.lastRepoConfig = repositoryConfig;
            this.lastRepoConfigDownload = TimeProvider.Current.TickCount;

            // Keep a local copy of the repository.xml for 3rdparty ftp operations LTG
            this.CopyFileToLocalRootFtpFolder(tempFile, "repository.xml");
            File.Delete(tempFile);
            return repositoryConfig.GetCurrentConfig();
        }

        /// <summary>
        /// For a file to upload from the Uploads location, get the subdirectories to create on the remote server.
        /// </summary>
        /// <param name="file">The file being uploaded</param>
        /// <returns>The remote directory path</returns>
        private string GetDestinationSubdirectories(FileReceivedLogFile file)
        {
            // Given a file with the local path like: D:\ftproot\Uploads\TestData\Round1TestData\TestFile.txt,
            // this would return \TestData\Round1TestData
            var localUploadPath = PathManager.Instance.CreatePath(
                FileType.Data,
                "Uploads" + Path.DirectorySeparatorChar);

            // Remove the /Uploads part.
            string destinationFile = file.FilePath.Replace(localUploadPath, string.Empty);

            // Get the remaining directory structure (without the file name)
            string destinationPath = Path.GetDirectoryName(destinationFile);
            return destinationPath;
        }

        /// <summary>
        /// Verifies the remote server is available, and attempts to retrieve the remote server's repository configuration.
        /// </summary>
        /// <param name="config">The ftp configuration for accessing the remote server</param>
        /// <param name="forceDownload">Force download the latest remote repository configuration file</param>
        /// <returns>The remote repository configuration</returns>
        private RepositoryVersionConfig GetRemoteRepositoryConfiguration(
            FtpUpdateClientConfig config = null,
            bool forceDownload = true)
        {
            // Are we able to process FTP requests?
            if (!this.CanProcessTransferRequests(config))
            {
                return null;
            }

            // Can we see the server repository config exists?
            if (!this.IsServerAvailable())
            {
                return null;
            }

            try
            {
                return this.GetCurrentRepoConfig(forceDownload);
            }
            catch (Exception e)
            {
                this.Logger.Error(e, "Couldn't retrieve repository configuration from the server.");
                return null;
            }
        }

        private string GetResourceFilePath(string resourcePath, string hash)
        {
            var resourceFile = Path.Combine(resourcePath, hash + FileDefinitions.ResourceFileExtension);
            return resourceFile;
        }

        private string GetTempCommandPath(string remoteCommandFile)
        {
            var unitDirectory = Path.GetFileName(Path.GetDirectoryName(remoteCommandFile)) ?? "-";
            var fileName = Path.GetFileName(remoteCommandFile);
            var tempFile = Path.Combine(
                Path.Combine(this.temporaryDirectory, unitDirectory),
                fileName + FileDefinitions.TempFileExtension);
            return tempFile;
        }

        /// <summary>
        /// Verifies the FTP server is available for transfers.
        /// Checks to see if the repository configuration file is available on the server, and
        /// if using a multicast ftp server set the update state to Started.
        /// </summary>
        /// <returns>Whether or not the remote server is available</returns>
        private bool IsServerAvailable()
        {
            this.Logger.Trace($"Checking if FTP server is available.");
            bool isAvailableResult;
            int numberOfTrys = this.MulticastIPAddressFound ? 10 : 1;
            while ((isAvailableResult = this.CheckAvailable()) == false && numberOfTrys-- > 0)
            {
                Thread.Sleep(1000);
                this.Logger.Info($"Check available {numberOfTrys} ");
            }

            // is the Ftp root found
            if (isAvailableResult == false)
            {
                this.Logger.Warn("Ftp transfers Ignored ftp server not found");
                return false;
            }

            // Start the Update process
            if (this.MulticastIPAddressFound)
            {
                // only create when we are ftp connected due to a multi cast ftp use case
                this.ConnectedToMulticastFtp = this.CreateStatusFile(MulticastUpdateClientState.Started);
                this.Logger.Trace(MethodBase.GetCurrentMethod().Name + " Multicast Ftp transfers Starting ...");
            }

            return true;
        }

        private void MulticastManagerMultiCastCommand(object sender, EventArgs e)
        {
            var multicastCommandEventArgs = e as MulticastCommandEventArgs;
            if (multicastCommandEventArgs == null)
            {
                return;
            }
            
            this.Logger.Info($"We received the command {multicastCommandEventArgs.CommandToExecute} from Multicast");
            
            if (multicastCommandEventArgs.CommandToExecute == MulticastCommands.ShowDisplay)
            {
                this.Logger.Info($"We broadcast the message {multicastCommandEventArgs.CommandToExecute}");
                var splashScreenMessage = new SplashScreenMessage();
                if (this.multicastManager.IsMasterUnit())
                {
                    MessageDispatcher.Instance.Broadcast(splashScreenMessage);
                }
            }
        }

        /// <summary>
        /// Process the change in the Network info for a Multicast Network case
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="remoteNetworkInfo">The remote network information</param>
        private void MulticastManagerOnRemoteNetworkInfoChanged(object sender, RemoteNetworkInfo remoteNetworkInfo)
        {
            this.Logger.Info("Received new Multicast Changed event");

            // set the flag to indicate to the Downloads that we are executing ftp operations do to this change
            var host = remoteNetworkInfo.RemoteIpAddress.ToString();
            IPAddress ip;

            // take our original settings as a base, change out the Host, Username and password to use vs the original content from the XML Settings on disk
            if (IPAddress.TryParse(host, out ip))
            {
                var config = new FtpUpdateClientConfig
                                 {
                                     Host = host,
                                     Username = remoteNetworkInfo.Username,
                                     Password = remoteNetworkInfo.Password,
                                     ShowVisualization = true,
                                     EnableMulticastIP = true,
                                     LocalFtpHomePath =
                                         this.originalFtpUpdateClientConfig.LocalFtpHomePath,
                                     RepositoryBasePath =
                                         this.originalFtpUpdateClientConfig.RepositoryBasePath,
                                     Port = this.originalFtpUpdateClientConfig.Port
                                 };
                remoteNetworkInfo.Save(RemotenetworkinfoXml);
                this.DoFtpUpdateFromMulticast(config);
            }
            else
            {
                this.Logger.Warn("Invalid Multicast Ftp Settings, Ignored!");
            }
        }

        private void PollTimerOnElapsed(object sender, EventArgs e)
        {
            // #if DEBUG
            if (File.Exists(RemotenetworkinfoXml))
            {
                RemoteNetworkInfo remoteNetworkInfo = new RemoteNetworkInfo();
                remoteNetworkInfo.Load(RemotenetworkinfoXml);
                this.Logger.Info(
                    $" {MethodBase.GetCurrentMethod().Name} Loaded RemoteNetworkInfo {remoteNetworkInfo}- Set the Network to Connect to Multicast Device and Complete the Update Process");
            }

            // #endif
            this.DoTimedDownloads();
            this.UploadFiles();
        }

        private bool ResourceExists(string hash)
        {
            try
            {
                this.Logger.Trace("ResourceExists Checking if resource {0} already exists", hash);
                this.Context.ResourceProvider.GetResource(hash);
                return true;
            }
            catch (UpdateException ex)
            {
                this.Logger.Trace(ex, "Couldn't find resource " + hash);
                return false;
            }
        }

        private void RestoreFromMulticastUpdate()
        {
            // clear our flags, restore the config to original case changed
            if (this.MulticastIPAddressFound)
            {
                // only create when we are ftp connected due to a multi cast ftp use case
                if (this.ConnectedToMulticastFtp)
                {
                    this.CreateStatusFile(MulticastUpdateClientState.Finished);
                    this.ConnectedToMulticastFtp = false;
                }

                this.MulticastIPAddressFound = false;

                this.Logger.Info("Restoring the original ftp Host={0}", this.originalFtpUpdateClientConfig.Host);

                // revert back to the original ftp handler with the original ftp client config settings
                this.ftpHandler = this.CreateFtpHandler(this.originalFtpUpdateClientConfig);
            }
        }

        private void SendIntermediateFeedback(List<DownloadProgress> downloadProgresses, bool final)
        {
            var feedbacks = new List<UpdateStateInfo>(downloadProgresses.Count);
            foreach (var downloadProgress in downloadProgresses)
            {
                var feedback = downloadProgress.CreateFeedback(final);
                if (feedback != null)
                {
                    feedbacks.Add(feedback);
                }
            }

            if (feedbacks.Count > 0)
            {
                this.SendFeedback(new IReceivedLogFile[0], feedbacks, null);
            }
        }

        [Obsolete(
            "SyncResources is obsolete. It was added due to 'D:\\Resources' was set as the Resource Directory in "
            + "Update\\medi.config manually. This is different from what the Admin Tool generates ( D:\\Data\\Update\\Medi\\Resources) "
            + "This caused the update process to fail during verification of resources and looking at the wrong location.")]
        private void SyncResources()
        {
            try
            {
                var mediResourcesPath = "D:\\Data\\Update\\Medi\\Resources";
                if (!Directory.Exists(mediResourcesPath))
                {
                    Directory.CreateDirectory(mediResourcesPath);
                    this.Logger.Info($" Created Medi Directory as it was missing: {mediResourcesPath}");
                }

                if (Directory.Exists(mediResourcesPath))
                {
                    this.Logger.Info($" Successfully Created Medi Directory as it was missing: {mediResourcesPath}");
                    var sourceDirName = "D:\\Resources\\";
                    if (Directory.EnumerateFileSystemEntries(mediResourcesPath).Count()
                        != Directory.EnumerateFileSystemEntries(sourceDirName).Count())
                    {
                        DirectoryCopy(sourceDirName, mediResourcesPath, true);
                        this.Logger.Trace(" Medi Data Resources Store was empty...");
                    }
                    else
                    {
                        this.Logger.Trace(" Medi Data Resources were available...");
                    }
                }
            }
            catch (Exception e)
            {
                this.Logger.Info($" Medi Resources Could not be created : {e.Message}{e.InnerException?.Message}");
            }
        }

        private void UploadLogFile(
            string feedbackPath,
            IReceivedLogFile log,
            CompressionAlgorithm compression,
            IPartProgressMonitor progressMonitor)
        {
            var feedbackDir = Path.Combine(feedbackPath, log.UnitName);
            this.ftpHandler.CreateDirectory(feedbackDir, false);
            var fileName = log.FileName;
            var logPath = Path.Combine(feedbackDir, fileName);
            var tempPath = Path.Combine(
                feedbackDir,
                fileName + "." + ApplicationHelper.MachineName + FileDefinitions.TempFileExtension);

            if (this.ftpHandler.FileExists(logPath))
            {
                this.Logger.Debug("Log file already exists, not uploading: {0}", logPath);
                return;
            }

            this.Logger.Trace("Uploading log file to {0}", tempPath);

            using (var input = this.compressionFactory.CreateCompressedStream(log.OpenRead(), compression))
            {
                this.ftpHandler.UploadStream(input, tempPath, input.Length, progressMonitor);
            }

            this.Logger.Trace("Renaming log file to {0}", logPath);
            try
            {
                this.ftpHandler.MoveFile(tempPath, logPath);
            }
            catch (WebException ex)
            {
                if (this.ftpHandler.FileExists(logPath))
                {
                    this.Logger.Warn(ex, "Couldn't rename file, assuming it was already uploaded: " + logPath);
                }
                else
                {
                    throw;
                }
            }
        }

        private void UploadStateFile(
            string feedbackPath,
            UpdateStateInfo state,
            CompressionAlgorithm compression,
            IPartProgressMonitor progressMonitor)
        {
            this.Logger.Trace($" {MethodBase.GetCurrentMethod().Name} : Enter");
            var feedbackDir = Path.Combine(feedbackPath, state.UnitId.UnitName);
            this.ftpHandler.CreateDirectory(feedbackDir, false);
            var index = 0;
            var fileNameBase =
                $"{state.UpdateId.BackgroundSystemGuid}-{state.UpdateId.UpdateIndex:####0000}-{(int)state.State:00}-{state.State}";
            this.Logger.Trace("Uploading state file to base {0}", fileNameBase);
            var fileName = fileNameBase + FileDefinitions.UpdateStateInfoExtension;
            this.Logger.Trace("Uploading state file to filename {0}", fileName);
            var stateFilePath = Path.Combine(feedbackDir, fileName);
            this.Logger.Trace("Uploading state file to full path {0}", stateFilePath);
            var tempFilePath = stateFilePath + "." + ApplicationHelper.MachineName + FileDefinitions.TempFileExtension;
            this.Logger.Trace("Uploading state file to {0}", tempFilePath);

            var memory = new MemoryStream();
            var serializer = new XmlSerializer(typeof(UpdateStateInfo));
            using (var compressed =
                this.compressionFactory.CreateCompressionStream(new NonClosingStream(memory), compression))
            {
                serializer.Serialize(compressed, state);
            }

            memory.Seek(0, SeekOrigin.Begin);
            this.ftpHandler.UploadStream(memory, tempFilePath, memory.Length, progressMonitor);

            while (this.ftpHandler.FileExists(stateFilePath))
            {
                fileName = $"{fileNameBase}.{++index}{FileDefinitions.UpdateStateInfoExtension}";
                stateFilePath = Path.Combine(feedbackDir, fileName);
            }

            this.Logger.Trace("Renaming state file to {0}", stateFilePath);
            this.ftpHandler.MoveFile(tempFilePath, stateFilePath);
        }

        private class DownloadProgress
        {
            private readonly UpdateCommand command;

            private readonly Dictionary<string, long> resources = new Dictionary<string, long>();

            private int downloadedCount;

            private long downloadedSize;

            private bool finalFeedbackSent;

            private long nextFeedback;

            private int totalCount;

            private long totalSize;

            /// <summary>Initializes a new instance of the <see cref="DownloadProgress"/> class.</summary>
            /// <param name="command">The command.</param>
            public DownloadProgress(UpdateCommand command)
            {
                this.command = command;
            }

            /// <summary>The add resource.</summary>
            /// <param name="hash">The hash.</param>
            /// <param name="size">The size.</param>
            public void AddResource(string hash, long size)
            {
                this.resources.Add(hash, size);
                this.totalSize += size;
                this.totalCount++;
            }

            /// <summary>The create feedback.</summary>
            /// <param name="final">The final.</param>
            /// <returns>The <see cref="UpdateStateInfo"/>.</returns>
            public UpdateStateInfo CreateFeedback(bool final)
            {
                var feedbackSize = Math.Max(
                    this.totalSize / MaximumIntermediateFeedbackCount,
                    MinimumIntermediateFeedbackSize);
                if (this.nextFeedback <= 0)
                {
                    this.nextFeedback = feedbackSize;
                }

                if ((!final || this.finalFeedbackSent) && this.nextFeedback > this.downloadedSize)
                {
                    return null;
                }

                while (this.nextFeedback <= this.downloadedSize)
                {
                    this.nextFeedback += feedbackSize;
                }

                this.finalFeedbackSent = this.downloadedCount == this.totalCount;
                return new UpdateStateInfo
                           {
                               UpdateId = this.command.UpdateId,
                               UnitId = this.command.UnitId,
                               UpdateSource = "FTP client on " + ApplicationHelper.MachineName,
                               State = UpdateState.Transferring,
                               TimeStamp = TimeProvider.Current.UtcNow,
                               Description = $"{this.downloadedCount}/{this.totalCount} files ({this.downloadedSize}/{this.totalSize} bytes) transferred"
                           };
            }

            /// <summary>The set resource downloaded.</summary>
            /// <param name="hash">The hash.</param>
            public void SetResourceDownloaded(string hash)
            {
                if (this.resources.TryGetValue(hash, out long size))
                {
                    this.resources.Remove(hash);
                    this.downloadedSize += size;
                    this.downloadedCount++;
                }
            }
        }
    }
}