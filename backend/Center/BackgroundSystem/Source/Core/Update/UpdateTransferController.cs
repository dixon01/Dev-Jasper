// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateTransferController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateTransferController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Update.Azure;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Update.Ftp;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// The update transfer controller.
    /// This class is responsible for transferring update commands to the FTP server when needed.
    /// </summary>
    internal class UpdateTransferController : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AsyncReaderWriterLock feedbackLock = new AsyncReaderWriterLock();

        private readonly Dictionary<int, AsyncLock> updateGroupLocks = new Dictionary<int, AsyncLock>();

        private readonly IUpdateContext updateContext = new UpdateContext();

        private readonly Dictionary<string, ProducerConsumerQueue<Tuple<FtpUpdateProviderConfig, UpdateCommand[]>>>
            uploadQueues =
                new Dictionary<string, ProducerConsumerQueue<Tuple<FtpUpdateProviderConfig, UpdateCommand[]>>>();

        private readonly Dictionary<FtpUpdateProviderConfig, FtpUpdateProvider> updateProviders =
            new Dictionary<FtpUpdateProviderConfig, FtpUpdateProvider>(new CompareProviders());

        private readonly UpdateCommandManager updateCommandManager = new UpdateCommandManager();

        private readonly UnitLogFileManager logFileManager = new UnitLogFileManager();
        
        private readonly UnitUploadFileManager uploadFileManager = new UnitUploadFileManager((IWritableFileSystem)FileSystemManager.Local);

        private readonly IUpdateGroupDataService updateGroupDataService =
            DependencyResolver.Current.Get<IUpdateGroupDataService>();

        private readonly ISystemConfigDataService systemConfigDataService =
            DependencyResolver.Current.Get<ISystemConfigDataService>();

        private readonly IUpdateCommandDataService updateCommandDataService =
            DependencyResolver.Current.Get<IUpdateCommandDataService>();

        private readonly IUnitDataService unitDataService = DependencyResolver.Current.Get<IUnitDataService>();

        private AzureUpdateProvider azureUpdateProvider;

        private string backgroundSystemGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateTransferController"/> class.
        /// </summary>
        public UpdateTransferController()
        {
            this.StartProviders();
        }

        /// <summary>
        /// Asynchronously creates all update commands for all units in the given update group.
        /// </summary>
        /// <param name="updateGroupId">
        /// The id of an update group (<see cref="Common.ServiceModel.Update.UpdateGroup.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for all units in the given update group.
        /// </returns>
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUpdateGroupAsync(int updateGroupId)
        {
            using (await this.feedbackLock.ReaderLockAsync())
            {
                using (await this.LockUpdateGroupAsync(updateGroupId))
                {
                    var updateGroup =
                        (await
                         this.updateGroupDataService.QueryAsync(
                             UpdateGroupQuery.Create()
                             .IncludeUpdateParts(
                                UpdatePartFilter.Create().IncludeStructure().IncludeInstallInstructions())
                             .IncludeUnits(UnitFilter.Create().IncludeProductType())
                             .WithId(updateGroupId))).Single();
                    var createdCommands = await this.updateCommandManager.CreateUpdateCommandsAsync(updateGroup);

                    var uploadCommands = new List<UpdateCommand>();
                    foreach (var unit in updateGroup.Units)
                    {
                        uploadCommands.AddRange(await this.GetAllUploadCommandsAsync(unit));
                    }

                    await this.UploadCommandsAsync(uploadCommands);

                    return createdCommands.ToList();
                }
            }
        }

        /// <summary>
        /// Asynchronously creates all update commands the given unit.
        /// </summary>
        /// <param name="unitId">
        /// The id of an unit (<see cref="Common.ServiceModel.Units.Unit.Id"/>).
        /// </param>
        /// <returns>
        /// The list of update commands for the given unit.
        /// </returns>
        public async Task<List<UpdateCommand>> CreateUpdateCommandsForUnitAsync(int unitId)
        {
            using (await this.feedbackLock.ReaderLockAsync())
            {
                var unit =
                    (await
                     this.unitDataService.QueryAsync(
                         UnitQuery.Create()
                         .WithId(unitId)
                         .IncludeUpdateGroup(
                             UpdateGroupQuery.Create().IncludeUpdateParts(
                                UpdatePartFilter.Create().IncludeStructure().IncludeInstallInstructions()))
                         .IncludeProductType()))
                        .First();
                if (unit.UpdateGroup == null)
                {
                    throw new ArgumentException("Given unit doesn't have an update group " + unitId);
                }

                using (await this.LockUpdateGroupAsync(unit.UpdateGroup.Id))
                {
                    var createdCommands = (await this.updateCommandManager.CreateUpdateCommandsAsync(unit)).ToList();

                    await this.UploadCommandsAsync(await this.GetAllUploadCommandsAsync(unit));

                    return createdCommands;
                }
            }
        }

        /// <summary>
        /// Asynchronously adds update feedback from a unit to the background system database.
        /// </summary>
        /// <param name="stateInfos">
        /// The state information.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        public async Task AddFeedbacksAsync(UpdateStateInfo[] stateInfos)
        {
            using (await this.feedbackLock.WriterLockAsync())
            {
                await this.updateCommandManager.AddFeedbacksAsync(stateInfos);
            }
        }

        /// <summary>
        /// Asynchronously saves a log file to the background system database.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="unitId">
        /// The unit id.
        /// </param>
        /// <returns>
        /// The task to wait on.
        /// </returns>
        public Task SaveLogFileAsync(string filename, Stream content, int unitId)
        {
            return this.logFileManager.SaveLogFileAsync(filename, content, unitId);
        }

        /// <summary>
        /// Asynchronously saves an uploaded file to the backround system file system.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="contents">
        /// The contents.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task SaveUploadFileAsync(string fileName, Stream contents)
        {
            return this.uploadFileManager.SaveUploadFileAsync(fileName, contents);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var uploadQueue in this.uploadQueues.Values)
            {
                uploadQueue.StopConsumer();
            }

            this.uploadQueues.Clear();

            foreach (var updateProvider in this.updateProviders.Values)
            {
                updateProvider.Stop();
            }

            this.updateProviders.Clear();

            this.logFileManager.Dispose();
        }

        private async Task<IEnumerable<UpdateCommand>> GetAllUploadCommandsAsync(Unit unit)
        {
            var allCommands = new List<UpdateCommand>();

            if (unit.ProductType.UnitType == UnitTypes.EPaper)
            {
                return allCommands;
            }

            var commands =
                await
                this.updateCommandDataService.QueryAsync(
                    UpdateCommandQuery.Create().WithUnit(unit).IncludeCommand().OrderByUpdateIndexDescending());
            var lastActivateTime = DateTime.MaxValue;
            foreach (var command in commands.TakeWhile(c => !c.WasTransferred))
            {
                var commandMsg = (UpdateCommand)command.Command.Deserialize();
                if (commandMsg.ActivateTime >= lastActivateTime)
                {
                    break;
                }

                lastActivateTime = commandMsg.ActivateTime;
                allCommands.Add(commandMsg);
            }

            return allCommands;
        }

        private async Task UploadCommandsAsync(IEnumerable<UpdateCommand> updateCommands)
        {
            var settings = await this.GetBackgroundSystemSettings();
            var commands = updateCommands.ToArray();
            settings.FtpUpdateProviders.ForEach(p => this.QueueCommandsUpload(commands, p));
            if (settings.AzureUpdateProvider == null)
            {
                Logger.Trace("AzureUpdateProvider not enabled");
                return;
            }

            if (this.azureUpdateProvider == null)
            {
                Logger.Info("Creating AzureUpdateProvider");
                this.azureUpdateProvider = new AzureUpdateProvider(await this.GetBackgroundSystemIdAsync());
                this.azureUpdateProvider.FeedbackReceived += this.ProviderOnFeedbackReceived;
                this.azureUpdateProvider.Configure(settings.AzureUpdateProvider, this.updateContext);
                this.azureUpdateProvider.Start();
            }

            this.azureUpdateProvider.HandleCommands(commands, null);
        }

        private async Task<BackgroundSystemSettings> GetBackgroundSystemSettings()
        {
            var systemConfig = (await this.systemConfigDataService.QueryAsync(SystemConfigQuery.Create().IncludeSettings())).First();
            var settings = (BackgroundSystemSettings)systemConfig.Settings.Deserialize();
            return settings;
        }

        private async Task<string> GetBackgroundSystemIdAsync()
        {
            if (this.backgroundSystemGuid != null)
            {
                return this.backgroundSystemGuid;
            }

            var config = (await this.systemConfigDataService.QueryAsync()).Single();
            return this.backgroundSystemGuid = config.SystemId.ToString();
        }

        private void QueueCommandsUpload(UpdateCommand[] updateCommands, FtpUpdateProviderConfig providerConfig)
        {
            ProducerConsumerQueue<Tuple<FtpUpdateProviderConfig, UpdateCommand[]>> queue;
            lock (this.uploadQueues)
            {
                if (!this.uploadQueues.TryGetValue(providerConfig.Host, out queue))
                {
                    Logger.Debug("Creating queue for FTP update commands on {0}", providerConfig.Host);
                    queue =
                        new ProducerConsumerQueue<Tuple<FtpUpdateProviderConfig, UpdateCommand[]>>(
                            c => this.UploadCommands(c.Item2, c.Item1),
                            100000);
                    queue.StartConsumer();
                    this.uploadQueues.Add(providerConfig.Host, queue);
                }
            }

            queue.Enqueue(new Tuple<FtpUpdateProviderConfig, UpdateCommand[]>(providerConfig, updateCommands));
        }

        private async void StartProviders()
        {
            try
            {
                var updateGroups =
                    await
                    this.updateGroupDataService.QueryAsync(
                        UpdateGroupQuery.Create().IncludeUnits(UnitFilter.Create().IncludeProductType()));

                var uploadCommands = new List<UpdateCommand>();
                foreach (var unit in updateGroups.SelectMany(g => g.Units))
                {
                    uploadCommands.AddRange(await this.GetAllUploadCommandsAsync(unit));
                }

                await this.UploadCommandsAsync(uploadCommands);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't start update providers");
            }
        }

        private void UploadCommands(UpdateCommand[] updateCommands, FtpUpdateProviderConfig providerConfig)
        {
            FtpUpdateProvider provider;
            lock (this.updateProviders)
            {
                if (!this.updateProviders.TryGetValue(providerConfig, out provider))
                {
                    Logger.Debug(
                        "Creating FTP update provider for {0}@{1}:{2}/{3}",
                        providerConfig.Username,
                        providerConfig.Host,
                        providerConfig.Port,
                        providerConfig.RepositoryBasePath);

                    provider = new FtpUpdateProvider();
                    provider.Configure(providerConfig, this.updateContext);
                    provider.FeedbackReceived += this.ProviderOnFeedbackReceived;
                    provider.Start();
                    this.updateProviders.Add(providerConfig, provider);
                }
            }

            if (updateCommands.Length == 0)
            {
                return;
            }

            // TODO: handle unavailable FTP server!!!
            Logger.Info(
                "Uploading {4} update commands to {0}@{1}:{2}/{3}",
                providerConfig.Username,
                providerConfig.Host,
                providerConfig.Port,
                providerConfig.RepositoryBasePath,
                updateCommands.Length);
            provider.HandleCommands(updateCommands, null);
        }

        private async Task<IDisposable> LockUpdateGroupAsync(int updateGroupId)
        {
            AsyncLock asyncLock;
            lock (this.updateGroupLocks)
            {
                if (!this.updateGroupLocks.TryGetValue(updateGroupId, out asyncLock))
                {
                    asyncLock = new AsyncLock();
                    this.updateGroupLocks.Add(updateGroupId, asyncLock);
                }
            }

            return await asyncLock.LockAsync();
        }

        private async Task HandleFeedbackReceivedAsync(FeedbackEventArgs e)
        {
            await this.AddFeedbacksAsync(e.ReceivedUpdateStates);

            foreach (var logFile in e.ReceivedLogFiles)
            {
                var units = await this.unitDataService.QueryAsync(UnitQuery.Create().WithName(logFile.UnitName));
                var unit = units.FirstOrDefault();
                
                if (unit != null)
                {
                    await this.SaveLogFileAsync(logFile.FileName, logFile.OpenRead(), unit.Id);
                }
            }
            
            // var settings = await this.GetBackgroundSystemSettings(); // TODO: This accidentally made it in for changes to download from FTP to app server. Not complete.
            // var localUploadsFolder = settings.UploadsPath;
            
            // this.MoveReceivedUploadFilesToUploadsDirectoryAsync(e.UploadedFiles, localUploadsFolder);

            // Move the received files to an uploads location.
        }

        /// <summary>
        /// The move received upload files to uploads directory async.
        /// </summary>
        /// <param name="uploadedFiles">
        /// The uploaded files.
        /// </param>
        /// <param name="localUploadsFolder">
        /// The local uploads folder where uploaded files will be copied to.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public void MoveReceivedUploadFilesToUploadsDirectoryAsync(IReceivedLogFile[] uploadedFiles, string localUploadsFolder)
        {
            // TODO: This accidentally made it in from changes to download files from FTP to app server. NOT COMPLETE.
            // Copy files to local uploads folder. That is, a file in D:\temp\Uploads\TFT-01-02-03\TestFile.csv.tmp
            // should be placed in something like D:\Uploads\TFT-01-02-03\TestFile.csv
            foreach (var uploadedFile in uploadedFiles)
            {
                string[] directoryNames = uploadedFile.FileName.Split(Path.DirectorySeparatorChar);
                string invalidPrefix = directoryNames[0] + Path.DirectorySeparatorChar + directoryNames[1];
                var cleanSourcePath = uploadedFile.FileName.Replace(invalidPrefix, string.Empty);
                string targetFilePath = Path.Combine(localUploadsFolder, cleanSourcePath);

                Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
                uploadedFile.CopyTo(targetFilePath);
            }
        }

        private void ProviderOnFeedbackReceived(object sender, FeedbackEventArgs e)
        {
            try
            {
                // IMPORTANT: this handler can't be an "async void". The method must return synchronously because any
                // temporary file will be deleted once the event was completed
                this.HandleFeedbackReceivedAsync(e).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't add feedback");
            }
        }

        private class UpdateContext : IUpdateContext
        {
            public UpdateContext()
            {
                this.TemporaryDirectory = Path.Combine(Path.GetTempPath(), "FtpUpdate");
                Directory.CreateDirectory(this.TemporaryDirectory);
            }

            public IResourceProvider ResourceProvider
            {
                get
                {
                    return DependencyResolver.Current.Get<IResourceProvider>();
                }
            }

            public string TemporaryDirectory { get; private set; }

            public IEnumerable<IUpdateSink> Sinks
            {
                get
                {
                    return new IUpdateSink[0];
                }
            }

            public IProgressMonitor CreateProgressMonitor(UpdateStage stage, bool showVisualization)
            {
                return new NullProgressMonitor();
            }
        }

        private class CompareProviders : IEqualityComparer<FtpUpdateProviderConfig>
        {
            public bool Equals(FtpUpdateProviderConfig x, FtpUpdateProviderConfig y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }

                return x.Host.Equals(y.Host) && string.Equals(x.Username, y.Username)
                       && string.Equals(x.Password, y.Password) && x.Port == y.Port
                       && string.Equals(x.RepositoryBasePath, y.RepositoryBasePath);
            }

            public int GetHashCode(FtpUpdateProviderConfig obj)
            {
                return obj.Host.GetHashCode();
            }
        }
    }
}