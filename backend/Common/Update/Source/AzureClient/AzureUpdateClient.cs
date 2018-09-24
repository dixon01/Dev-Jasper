// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureUpdateClient.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureUpdateClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.AzureClient
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using Math = System.Math;

    /// <summary>
    /// Update client getting commands via Medi and downloading resources via HTTP/HTTPS.
    /// </summary>
    public class AzureUpdateClient : UpdateClientBase<AzureUpdateClientConfig>
    {
        /// <summary>
        /// The virtual application name used for addressing the client.
        /// </summary>
        public static readonly string VirtualApplicationName = "AzureUpdateClient";

        private const int LogUploadBlockLength = 256 * 1024; // 256KB

        private const int MaximumDownloadRetries = 5;

        private const long MaximumIntermediateFeedbackCount = 10; // maximum 10 feedbacks per download

        private const long MinimumIntermediateFeedbackFileCount = 10; // at least 10 files must be downloaded

        private readonly byte[] logUploadBuffer = new byte[LogUploadBlockLength];

        private readonly Dictionary<string, UnitMessageHandler> messageHandlers =
            new Dictionary<string, UnitMessageHandler>();

        private readonly ITimer messageHandlerUpdateTimer;

        private readonly ProducerConsumerQueue<UpdateCommandHandling> updateCommandQueue;

        private int currentLogBlockId;

        private RepositoryConfig lastRepoConfig;

        private long lastRepoConfigDownload;

        private IPersistenceContext<MediAddress> persistence;

        private string temporaryDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureUpdateClient"/> class.
        /// </summary>
        public AzureUpdateClient()
        {
            this.messageHandlerUpdateTimer = TimerFactory.Current.CreateTimer("AzureMessageHandlerUpdates");
            this.messageHandlerUpdateTimer.AutoReset = true;
            this.messageHandlerUpdateTimer.Interval = TimeSpan.FromMinutes(1);
            this.messageHandlerUpdateTimer.Elapsed += (s, e) => this.UpdateMessageProxies();

            this.updateCommandQueue = new ProducerConsumerQueue<UpdateCommandHandling>(this.ProcessUpdateCommand, 100);
        }

        /// <summary>
        /// Gets the Medi address of the update provider sending us updates.
        /// </summary>
        public MediAddress ProviderAddress
        {
            get
            {
                return this.persistence.Value;
            }

            private set
            {
                if (AzureUpdateClient.Equals(this.persistence.Value, value))
                {
                    return;
                }

                this.persistence.Value = value;
                this.RaiseIsAvailableChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        public override void Configure(UpdateClientConfigBase config, IUpdateContext context)
        {
            base.Configure(config, context);

            this.temporaryDirectory = Path.Combine(this.Context.TemporaryDirectory, config.Name);
            Directory.CreateDirectory(this.temporaryDirectory);

            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            this.persistence = persistenceService.GetContext<MediAddress>("AzureProviderAddress");
            this.persistence.Validity = TimeSpan.FromDays(3650);
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
            var progress = 0;
            var logs = new List<IReceivedLogFile>(logFiles);
            var states = new List<UpdateStateInfo>(stateInfos);
            progressMonitor = progressMonitor ?? this.Context.CreateProgressMonitor(
                                  UpdateStage.SendingFeedback,
                                  this.Config.ShowVisualization);
            progressMonitor.Start();
            this.Logger.Info("Sending feedback: {0} logs, {1} states", logs.Count, states.Count);

            var maxProgress = logs.Count + states.Count + 1.0;
            try
            {
                foreach (var updateStateInfo in states)
                {
                    progressMonitor.Progress((++progress) / maxProgress, updateStateInfo.UnitId.UnitName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    if (!this.messageHandlers.TryGetValue(updateStateInfo.UnitId.UnitName, out UnitMessageHandler messageHandler))
                    {
                        this.Logger.Warn("Couldn't find a message handler for {0}", updateStateInfo.UnitId.UnitName);
                        continue;
                    }

                    messageHandler.SendFeedback(updateStateInfo);
                }

                var currentConfig = this.GetCurrentRepoConfig(false);

                var logsPath = currentConfig.FeedbackDirectory;

                foreach (var log in logs)
                {
                    progressMonitor.Progress((++progress) / maxProgress, log.FileName);
                    if (progressMonitor.IsCancelled)
                    {
                        return;
                    }

                    this.UploadLogFile(
                        logsPath,
                        log,
                        new AzureProgressPart(log.FileName, progressMonitor, progress, maxProgress));
                }

                progressMonitor.Complete(null, null);
            }
            catch (Exception ex)
            {
                progressMonitor.Complete("Couldn't upload feedback: " + ex.Message, null);
                throw new UpdateException("Couldn't upload feedback", ex);
            }
        }

        /// <summary>
        /// Upload files via the azure client. Not implemented yet.
        /// </summary>
        /// <param name="uploadFiles">The files to upload to the remote server</param>
        public override void UploadFiles(IList<IReceivedLogFile> uploadFiles)
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
                this.GetCurrentRepoConfig(false);
                return this.ProviderAddress != null;
            }
            catch (Exception ex)
            {
                this.Logger.Debug(ex, "Couldn't reach " + this.Config.RepositoryUrl);
                return false;
            }
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateClientBase{TConfig}.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            var serializer = new XmlSerializer(typeof(UpdateCommand));
            string[] commandFiles;
            try
            {
                commandFiles = Directory.GetFiles(
                    this.temporaryDirectory,
                    "*" + FileDefinitions.UpdateCommandExtension);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't get cached commands from {0}", this.temporaryDirectory);
                commandFiles = new string[0];
            }

            foreach (var commandFile in commandFiles)
            {
                try
                {
                    using (var input = File.OpenRead(commandFile))
                    {
                        var command = (UpdateCommand)serializer.Deserialize(input);
                        this.updateCommandQueue.Enqueue(new UpdateCommandHandling(command));
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't reload command from " + commandFile, ex);
                    File.Delete(commandFile);
                }
            }

            this.updateCommandQueue.StartConsumer();
            this.UpdateMessageProxies();
            this.messageHandlerUpdateTimer.Enabled = true;
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateClientBase{TConfig}.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            this.messageHandlerUpdateTimer.Enabled = false;
            foreach (var messageHandler in this.messageHandlers.Values)
            {
                messageHandler.Dispose();
            }

            this.messageHandlers.Clear();

            this.updateCommandQueue.StopConsumer();
        }

        private bool BlobExists(string path)
        {
            try
            {
                // TODO: is this efficient? (doesn't it try to download the file?)
                this.GetBlobSize(path);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void CommitBlock(string blobUri, int maxBlockId)
        {
            var requestUri = string.Format(CultureInfo.InvariantCulture, "{0}&comp=blockList", blobUri);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = "PUT";

            // contrary to what the API documentation says, the ContentSize header is not required
            using (var writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
            {
                writer.Write(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
                writer.Write("<BlockList>");
                for (int i = 0; i <= maxBlockId; i++)
                {
                    writer.WriteLine("<Latest>{0:d8}</Latest>", i);
                }

                writer.Write("</BlockList>");
            }

            var response = request.GetResponse();
            response.Close();
        }

        private FileInfo ContinueDownloadFile(string url, string localFile, long size)
        {
            var localInfo = new FileInfo(localFile);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("x-ms-range", $"bytes={localInfo.Length}-{size}");
            var dir = Path.GetDirectoryName(localFile);
            if (dir != null)
            {
                Directory.CreateDirectory(dir);
            }

            using (var output = File.OpenWrite(localFile))
            {
                output.Seek(0, SeekOrigin.End);
                using (var response = request.GetResponse())
                {
                    using (var input = response.GetResponseStream())
                    {
                        if (input == null)
                        {
                            throw new WebException("Couldn't get response stream");
                        }

                        var buffer = new byte[4096];
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, read);
                        }
                    }
                }
            }

            return localInfo;
        }

        private long DownloadFile(string url, string localFile)
        {
            this.Logger.Trace("Downloading {0} to {1}", url, localFile);
            var request = WebRequest.Create(url);
            var tempInfo = new FileInfo(localFile);
            if (tempInfo.Exists)
            {
                using (var response = request.GetResponse())
                {
                    var size = response.ContentLength;
                    if (tempInfo.Length == size)
                    {
                        this.Logger.Debug("Temporary download file exists with entire contents: {0}", localFile);
                        return tempInfo.Length;
                    }

                    if (tempInfo.Length < size)
                    {
                        var info = this.ContinueDownloadFile(url, localFile, size);
                        return info.Length;
                    }
                }
            }

            long total = 0;
            using (var response = request.GetResponse())
            {
                var input = response.GetResponseStream();
                if (input == null)
                {
                    throw new WebException("Couldn't get response stream");
                }

                using (var output = File.Create(localFile))
                {
                    var buffer = new byte[4096];
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        total += read;
                    }
                }
            }

            return total;
        }

        private void DownloadResources(UpdateCommand command, string resourceBasePath, IProgressMonitor progressMonitor)
        {
            if (!resourceBasePath.EndsWith("/"))
            {
                resourceBasePath += "/";
            }

            var requiredHashes = new List<string>();
            foreach (var file in this.GetAllFiles(command))
            {
                if (!this.ResourceExists(file.Hash))
                {
                    requiredHashes.Add(file.Hash);
                }
            }

            var progress = 0;
            var reportInterval = Math.Max(
                MinimumIntermediateFeedbackFileCount,
                requiredHashes.Count / MaximumIntermediateFeedbackCount);
            long downloadedBytes = 0;
            foreach (var hash in requiredHashes)
            {
                if (progressMonitor.IsCancelled)
                {
                    return;
                }

                var fileName = hash + FileDefinitions.ResourceFileExtension;
                var tempFile = Path.Combine(this.temporaryDirectory, fileName);
                var url = resourceBasePath + hash;
                this.Logger.Debug(
                    "Downloading new resource from {0} ({1}/{2})",
                    url,
                    progress + 1,
                    requiredHashes.Count);
                progressMonitor.Progress((double)progress / requiredHashes.Count, url);
                downloadedBytes += this.DownloadFile(url, tempFile);
                this.Context.ResourceProvider.AddResource(hash, tempFile, true);
                progress++;
                if (progress % reportInterval == 0)
                {
                    // report the progress
                    this.SendTransferringFeedback(command, progress, requiredHashes.Count, downloadedBytes);
                }
            }

            if (progress % reportInterval != 0)
            {
                // send the final feedback (if it wasn't sent yet)
                this.SendTransferringFeedback(command, requiredHashes.Count, requiredHashes.Count, downloadedBytes);
            }
        }

        private int FillUploadBuffer(Stream input)
        {
            int total = 0;
            int read;
            while (total < this.logUploadBuffer.Length && (read = input.Read(
                                                               this.logUploadBuffer,
                                                               total,
                                                               this.logUploadBuffer.Length - total)) > 0)
            {
                total += read;
            }

            return total;
        }

        private long GetBlobSize(string path)
        {
            var request = WebRequest.Create(path);
            var response = request.GetResponse();
            var size = response.ContentLength;
            response.Close();
            return size;
        }

        private RepositoryVersionConfig GetCurrentRepoConfig(bool forceDownload)
        {
            var timeout = this.lastRepoConfigDownload + (this.messageHandlerUpdateTimer.Interval.TotalMilliseconds / 2);
            if (!forceDownload && this.lastRepoConfig != null && TimeProvider.Current.TickCount < timeout)
            {
                // prevent downloading the repo config too often
                return this.lastRepoConfig.GetCurrentConfig();
            }

            var request = WebRequest.Create(this.Config.RepositoryUrl);
            var serializer = new XmlSerializer(typeof(RepositoryConfig));
            using (var response = request.GetResponse())
            {
                var input = response.GetResponseStream();
                if (input == null)
                {
                    throw new WebException("Couldn't get response stream");
                }

                this.lastRepoConfig = (RepositoryConfig)serializer.Deserialize(input);
                this.lastRepoConfigDownload = TimeProvider.Current.TickCount;
            }

            return this.lastRepoConfig.GetCurrentConfig();
        }

        private void MessageHandlerOnUpdateCommandReceived(object sender, MessageEventArgs<UpdateCommand> e)
        {
            this.ProviderAddress = e.Source;

            var command = e.Message;
            var handling = new UpdateCommandHandling(command);

            var localFile = Path.Combine(this.temporaryDirectory, handling.FileName);
            var serializer = new XmlSerializer(typeof(UpdateCommand));
            using (var output = File.Create(localFile))
            {
                serializer.Serialize(output, command);
            }

            var feedback = new UpdateStateInfo { State = UpdateState.Transferring, Description = "Received command" };
            this.SendFeedback(command, feedback);

            if (!this.updateCommandQueue.Enqueue(handling))
            {
                this.Logger.Warn("Couldn't enqueue new update command");
            }
        }

        private void ProcessUpdateCommand(UpdateCommandHandling handling)
        {
            var command = handling.UpdateCommand;
            var commandName = handling.FileName;
            if (handling.Counter > 0)
            {
                var waitTime = handling.Counter * 5000;
                this.Logger.Trace("Retrying command {0}, waiting {1} ms", commandName, waitTime);
                Thread.Sleep(waitTime);
            }

            var progress = this.Context.CreateProgressMonitor(
                UpdateStage.ReceivingUpdate,
                this.Config.ShowVisualization);
            progress.Start();
            var localFile = Path.Combine(this.temporaryDirectory, commandName);
            try
            {
                progress.Progress(0, "Downloading resources for " + commandName);
                var repoConfig = this.GetCurrentRepoConfig(false);
                this.DownloadResources(command, repoConfig.ResourceDirectory, progress);

                this.RaiseCommandReceived(new UpdateCommandsEventArgs(command));
                progress.Complete(null, null);
            }
            catch (Exception ex)
            {
                // TODO: implement retrying download later
                this.Logger.Warn(ex, "Couldn't download resources for command {0}", commandName);
                progress.Complete(ex.Message, null);

                handling.Counter++;
                if (handling.Counter >= MaximumDownloadRetries)
                {
                    var reason = string.Format("Couldn't download {0} times: " + ex.Message, MaximumDownloadRetries);
                    this.SendFeedback(
                        command,
                        new UpdateStateInfo { State = UpdateState.TransferFailed, ErrorReason = reason });
                    return;
                }

                this.updateCommandQueue.Enqueue(handling);
                return;
            }

            File.Delete(localFile);
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
                this.Logger.Trace(ex, "Couldn't find resource hash = {0}", hash);
                return false;
            }
        }

        private void SendFeedback(UpdateCommand command, UpdateStateInfo feedback)
        {
            feedback.UpdateId = command.UpdateId;
            feedback.UnitId = command.UnitId;
            feedback.UpdateSource = "Azure client on " + ApplicationHelper.MachineName;
            feedback.TimeStamp = TimeProvider.Current.UtcNow;

            this.SendFeedback(new IReceivedLogFile[0], new[] { feedback }, null);
        }

        private void SendTransferringFeedback(UpdateCommand command, int completed, int total, long downloadedBytes)
        {
            this.SendFeedback(
                command,
                new UpdateStateInfo
                    {
                        State = UpdateState.Transferring,
                        Description = $"{completed}/{total} files ({downloadedBytes} bytes) transferred"
                    });
        }

        private void UpdateMessageProxies()
        {
            var unused = new List<string>(this.messageHandlers.Keys);
            foreach (var updateSink in this.Context.Sinks)
            {
                foreach (var unitName in updateSink.HandledUnits)
                {
                    if (unitName == UnitWildcard)
                    {
                        continue;
                    }

                    if (this.messageHandlers.ContainsKey(unitName))
                    {
                        unused.Remove(unitName);
                        continue;
                    }

                    this.Logger.Debug("Adding message handler for {0}", unitName);
                    var messageHandler = new UnitMessageHandler(unitName, this);
                    messageHandler.UpdateCommandReceived += this.MessageHandlerOnUpdateCommandReceived;
                    this.messageHandlers[unitName] = messageHandler;
                }
            }

            foreach (var unitName in unused)
            {
                this.Logger.Debug("Removing message handler for {0}", unitName);
                this.messageHandlers[unitName].Dispose();
                this.messageHandlers.Remove(unitName);
            }
        }

        private void UploadBlock(string blobUri, byte[] buffer, int blockId, int contentSize)
        {
            var requestUri = $"{blobUri}&comp=block&blockid={blockId:d8}";
            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.Method = "PUT";
            request.ContentLength = contentSize;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, contentSize);
            }

            var response = request.GetResponse();
            response.Close();
        }

        private void UploadLogFile(string logsBasePath, IReceivedLogFile logFile, IPartProgressMonitor progressMonitor)
        {
            var fileName = logFile.FileName;
            long totalBytes = 0;
            var queryString = (new Uri(logsBasePath)).Query;
            var containerUri = logsBasePath.Substring(0, logsBasePath.Length - queryString.Length);
            var blobUri = string.Format(
                CultureInfo.InvariantCulture,
                "{0}/{1}/{2}{3}",
                containerUri,
                logFile.UnitName,
                fileName,
                queryString);

            if (this.BlobExists(blobUri))
            {
                this.Logger.Debug("Log file already exists, not uploading: {0}", fileName);
                return;
            }

            this.Logger.Trace("Uploading log file to {0}", blobUri);

            using (var input = logFile.OpenRead())
            {
                var fileSize = input.Length;

                int read;
                var offset = this.currentLogBlockId == 0 ? 0 : this.currentLogBlockId * LogUploadBlockLength;
                if (offset > 0)
                {
                    input.Seek(offset, SeekOrigin.Begin);
                }

                while ((read = this.FillUploadBuffer(input)) > 0)
                {
                    if (progressMonitor.IsCancelled)
                    {
                        throw new IOException("Copy of stream was cancelled");
                    }

                    this.UploadBlock(blobUri, this.logUploadBuffer, this.currentLogBlockId, read);
                    this.currentLogBlockId++;
                    totalBytes += read;
                    progressMonitor.Progress(
                        1.0 * totalBytes / fileSize,
                        $"{totalBytes} of {fileSize} bytes");
                }

                this.CommitBlock(blobUri, this.currentLogBlockId - 1);
                this.currentLogBlockId = 0;
            }

            this.Logger.Trace("Log file upload completed.");
        }

        private class UpdateCommandHandling
        {
            public UpdateCommandHandling(UpdateCommand updateCommand)
            {
                this.UpdateCommand = updateCommand;
                this.FileName =
                    $"{updateCommand.UpdateId.BackgroundSystemGuid}-{updateCommand.UpdateId.UpdateIndex:####0000}{FileDefinitions.UpdateCommandExtension}";
            }

            public int Counter { get; set; }

            public string FileName { get; }

            public UpdateCommand UpdateCommand { get; }
        }
    }
}