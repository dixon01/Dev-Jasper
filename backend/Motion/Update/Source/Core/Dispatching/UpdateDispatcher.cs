// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateDispatcher.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Update.Core.Agent;

    using NLog;

    /// <summary>
    /// Dispatcher that takes commands from update clients and routes them to
    /// update providers and the local update agent (and vice versa for logs and state info).
    /// </summary>
    public class UpdateDispatcher
    {
        private const string ReferenceFileExtension = ".ref";

        private static readonly Logger Logger = LogHelper.GetLogger<UpdateDispatcher>();

        private readonly string poolDirectory;
        private readonly string sourceQueueBaseDirectory;
        private readonly string sinkQueueBaseDirectory;
        private readonly string queueDirectory;

        private readonly IList<UpdateFeedbackQueue> feedbackQueues;
        private readonly IList<UpdateCommandQueue> commandQueues;
        private readonly IList<UploadFileQueue> uploadQueues;
        private readonly UpdateCommandQueue agentCommandQueue;

        private readonly CacheLimitManager cacheLimitManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDispatcher"/> class.
        /// </summary>
        /// <param name="updateClients">
        ///     All update clients in the system.
        /// </param>
        /// <param name="updateProviders">
        ///     All update providers in the system.
        /// </param>
        /// <param name="updateAgent">
        ///     The local update agent.
        /// </param>
        /// <param name="cacheLimitsConfig">
        /// The folder limit config.
        /// </param>
        public UpdateDispatcher(
            List<IUpdateClient> updateClients,
            List<IUpdateProvider> updateProviders,
            UpdateAgent updateAgent,
            CacheLimitsConfig cacheLimitsConfig)
        {
            this.poolDirectory = PathManager.Instance.CreatePath(FileType.Data, "Pool" + Path.DirectorySeparatorChar);
            this.queueDirectory = PathManager.Instance.CreatePath(
                FileType.Data,
                "Queues" + Path.DirectorySeparatorChar);

            string uploadsDirectory = PathManager.Instance.CreatePath(FileType.Data, "Uploads" + Path.DirectorySeparatorChar);
            this.sourceQueueBaseDirectory = PathManager.Instance.CreatePath(
                FileType.Data,
                "Queues" + Path.DirectorySeparatorChar + "Sources" + Path.DirectorySeparatorChar);
            this.sinkQueueBaseDirectory = PathManager.Instance.CreatePath(
                FileType.Data, "Queues" + Path.DirectorySeparatorChar + "Sinks" + Path.DirectorySeparatorChar);

            this.feedbackQueues = updateClients.ConvertAll(c => new UpdateFeedbackQueue(c, this.sourceQueueBaseDirectory, this.poolDirectory));
            this.commandQueues = updateProviders.ConvertAll(p => new UpdateCommandQueue(p, this.sinkQueueBaseDirectory, this.poolDirectory));

            this.uploadQueues = updateClients.ConvertAll(c => new UploadFileQueue(c, uploadsDirectory, string.Empty));

            if (updateAgent != null)
            {
                this.agentCommandQueue = new UpdateCommandQueue(
                    updateAgent, this.sinkQueueBaseDirectory, this.poolDirectory);
            }

            if (cacheLimitsConfig.Enabled)
            {
                this.cacheLimitManager = new CacheLimitManager(
                    cacheLimitsConfig,
                    this.poolDirectory,
                    this.sourceQueueBaseDirectory);
            }
        }

        /// <summary>
        /// Starts this dispatcher.
        /// </summary>
        public void Start()
        {
            this.CleanPoolDirectory();

            Logger.Info("Starting");
            foreach (var updateProvider in this.commandQueues)
            {
                updateProvider.Flush();
                updateProvider.UpdateSink.FeedbackReceived += this.UpdateSinkOnFeedbackReceived;
            }

            if (this.agentCommandQueue != null)
            {
                this.agentCommandQueue.Flush();
                this.agentCommandQueue.UpdateSink.FeedbackReceived += this.UpdateSinkOnFeedbackReceived;
            }

            foreach (var updateClient in this.feedbackQueues)
            {
                updateClient.Flush();
                updateClient.UpdateSource.CommandsReceived += this.UpdateSourceOnCommandsReceived;
            }
            
            // Handle upload queue subscriptions
            foreach (var uploadQueue in this.uploadQueues)
            {
                uploadQueue.Flush();
            }

            Logger.Debug("Started");
        }

        /// <summary>
        /// Stops this dispatcher.
        /// </summary>
        public void Stop()
        {
            Logger.Debug("Stopping");
            foreach (var updateProvider in this.commandQueues)
            {
                updateProvider.UpdateSink.FeedbackReceived -= this.UpdateSinkOnFeedbackReceived;
            }

            if (this.agentCommandQueue != null)
            {
                this.agentCommandQueue.UpdateSink.FeedbackReceived -= this.UpdateSinkOnFeedbackReceived;
            }

            foreach (var updateClient in this.feedbackQueues)
            {
                updateClient.UpdateSource.CommandsReceived -= this.UpdateSourceOnCommandsReceived;
            }

            Logger.Info("Stopped");
        }

        /// <summary>
        /// Checks if the given <see cref="hash"/> is still reference somewhere
        /// inside the given <see cref="directory"/> (recursively).
        /// </summary>
        /// <param name="directory">
        /// The root directory where to search for references.
        /// </param>
        /// <param name="hash">
        /// The hash to search for.
        /// </param>
        /// <returns>
        /// True if a reference is found inside the directory, otherwise false.
        /// </returns>
        internal static bool HasReference(string directory, string hash)
        {
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                if (HasReference(subDir, hash))
                {
                    return true;
                }
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith(hash))
                {
                    Logger.Trace("{0} is still referenced by {1}", hash, file);
                    return true;
                }
            }

            return false;
        }

        private void UpdateSourceOnCommandsReceived(object sender, UpdateCommandsEventArgs e)
        {
            Logger.Debug("Received {0} commands", e.Commands.Length);
            var queues = new Dictionary<string, UpdateCommandQueue>();

            foreach (var command in e.Commands)
            {
                this.AddCommandToQueues(command, queues);
            }

            Logger.Debug("Flushing {0} queues", queues.Count);
            foreach (var queue in queues.Values)
            {
                queue.Flush();
            }
        }

        private void UpdateSinkOnFeedbackReceived(object sender, FeedbackEventArgs e)
        {
            Logger.Debug(
                "Received feedback: {0} log files, {1} states",
                e.ReceivedLogFiles.Length,
                e.ReceivedUpdateStates.Length);

            foreach (var logFile in e.ReceivedLogFiles)
            {
                this.AddLogFilesToQueues(logFile);
            }

            foreach (var updateState in e.ReceivedUpdateStates)
            {
                this.AddUpdateStateToQueues(updateState);
            }

            Logger.Debug("Flushing all queues");
            foreach (var queue in this.feedbackQueues)
            {
                queue.Flush();
            }

            this.cacheLimitManager?.PerformFolderLimitOperation();
        }

        private void AddCommandToQueues(UpdateCommand command, IDictionary<string, UpdateCommandQueue> allQueues)
        {
            try
            {
                var fileName = this.AddToPool(command, FileDefinitions.UpdateCommandExtension, out string hash);
                if (fileName == null)
                {
                    return;
                }

                var queues = new List<UpdateCommandQueue>();
                if (this.CreateCommandReference(this.agentCommandQueue, command, hash))
                {
                    // the update is for us, so we don't have to send it to anybody else
                    Logger.Debug(
                        "Command {0} of {1} is for us ('{2}'), not forwarding to anybody else",
                        command.UpdateId.UpdateIndex,
                        command.UpdateId.BackgroundSystemGuid,
                        command.UnitId.UnitName);
                    queues.Add(this.agentCommandQueue);
                }
                else
                {
                    foreach (var queue in this.commandQueues)
                    {
                        if (this.CreateCommandReference(queue, command, hash))
                        {
                            queues.Add(queue);
                        }
                    }
                }

                if (queues.Count == 0)
                {
                    // nobody wants to know anything about this command, we can delete it
                    Logger.Debug(
                        "Nobody was interested in update command {0} of {1} for {2}",
                        command.UpdateId.UpdateIndex,
                        command.UpdateId.BackgroundSystemGuid,
                        command.UnitId.UnitName);

                    File.Delete(fileName);
                    return;
                }

                foreach (var queue in queues)
                {
                    allQueues[queue.UpdateSink.Name] = queue;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "Couldn't add command to queue: {0} of {1} for {2}",
                    command.UpdateId.UpdateIndex,
                        command.UpdateId.BackgroundSystemGuid,
                        command.UnitId.UnitName);
            }
        }

        private void AddUpdateStateToQueues(UpdateStateInfo updateState)
        {
            try
            {
                var fileName = this.AddToPool(updateState, FileDefinitions.UpdateStateInfoExtension, out string hash);
                if (fileName == null)
                {
                    return;
                }

                foreach (var source in this.feedbackQueues)
                {
                    this.CreateFeedbackReference(
                        source, updateState.UnitId.UnitName, hash, FileDefinitions.UpdateStateInfoExtension);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't add update state: " + updateState.State);
            }
        }

        private void AddLogFilesToQueues(IReceivedLogFile logFile)
        {
            try
            {
                this.AddToPool(logFile.CopyTo, FileDefinitions.LogFileExtension, out string hash);

                foreach (var source in this.feedbackQueues)
                {
                    var fileName = $"{hash}.{{{logFile.FileName}}}";
                    this.CreateFeedbackReference(source, logFile.UnitName, fileName, FileDefinitions.LogFileExtension);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't add log file to queue: " + logFile.FileName);
            }
        }

        private string AddToPool<T>(T obj, string extension, out string hash)
            where T : class, new()
        {
            return this.AddToPool(
                fileName =>
                    {
                        var configurator = new Configurator(fileName);
                        configurator.Serialize(obj);
                    },
                extension,
                out hash);
        }

        private string AddToPool(Action<string> createFile, string extension, out string hash)
        {
            var tempFileName = Path.Combine(this.poolDirectory, Guid.NewGuid() + FileDefinitions.TempFileExtension);
            createFile(tempFileName);

            hash = ResourceHash.Create(tempFileName);
            var fileName = Path.Combine(this.poolDirectory, hash + extension);

            if (File.Exists(fileName))
            {
                // we already have the file here, so no need to enqueue it again
                File.Delete(tempFileName);
                Logger.Trace("File is already in pool: {0}", fileName);
                return null;
            }

            File.Move(tempFileName, fileName);
            Logger.Trace("Added file to pool: {0}", fileName);
            return fileName;
        }

        private bool CreateCommandReference(UpdateCommandQueue queue, UpdateCommand command, string hash)
        {
            if (queue == null || !this.HandlesUnit(queue.UpdateSink, command.UnitId.UnitName))
            {
                return false;
            }

            Logger.Debug(
                "Creating reference to update command {0} of {1} for '{2}' in queue '{3}'",
                command.UpdateId.UpdateIndex,
                command.UpdateId.BackgroundSystemGuid,
                command.UnitId.UnitName,
                queue.UpdateSink.Name);

            var queueDir = Path.Combine(this.sinkQueueBaseDirectory, queue.UpdateSink.Name);
            var filePath = Path.Combine(
                queueDir, hash + FileDefinitions.UpdateCommandExtension + ReferenceFileExtension);

            this.CreateReferenceFile(filePath);
            return true;
        }

        private bool HandlesUnit(IUpdateSink updateSink, string unitName)
        {
            foreach (var unit in updateSink.HandledUnits)
            {
                if (unit == UpdateComponentBase.UnitWildcard
                    || unit.Equals(unitName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateFeedbackReference(UpdateFeedbackQueue queue, string unitName, string hash, string extension)
        {
            Logger.Debug(
                "Creating reference to feedback {0} for '{1}' in queue '{2}'",
                hash,
                unitName,
                queue.UpdateSource.Name);

            var queueDir = Path.Combine(this.sourceQueueBaseDirectory, queue.UpdateSource.Name);
            queueDir = Path.Combine(queueDir, unitName);
            Directory.CreateDirectory(queueDir);
            var filePath = Path.Combine(queueDir, hash + extension + ReferenceFileExtension);
            this.CreateReferenceFile(filePath);
        }

        private void CreateReferenceFile(string filePath)
        {
            File.Create(filePath).Close();
            Logger.Trace("Created reference: {0}", filePath);
        }

        private void CleanPoolDirectory()
        {
            var poolFiles = new DirectoryInfo(this.poolDirectory).GetFiles();
            foreach (var poolFile in poolFiles)
            {
                var hash = this.GetHash(poolFile.FullName);

                if (HasReference(this.queueDirectory, hash))
                {
                    continue;
                }

                File.Delete(poolFile.FullName);
                Logger.Debug("File deleted from pool: {0}", poolFile);
            }
        }

        private string GetHash(string refFileName)
        {
            var fileName = Path.GetFileName(refFileName);
            if (fileName == null)
            {
                throw new UpdateException("Couldn't get hash from reference file: " + refFileName);
            }

            var dot = fileName.IndexOf('.');
            return dot < 0 ? fileName : fileName.Substring(0, dot);
        }
    }
}