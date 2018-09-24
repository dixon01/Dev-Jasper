// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainUnitUpdateCommandUpdater.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainUnitUpdateCommandUpdater type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Qube.Configuration;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.EPaper.MainUnit;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Component that creates the update commands.
    /// </summary>
    public class MainUnitUpdateCommandUpdater : IMainUnitUpdateCommandUpdater, IDisposable
    {
        private const string EmptyFileHash64 = "99e9d85137db46ef";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IProducerConsumerQueue<EnqueuedUpdate> updates;

        private readonly object locker = new object();

        private bool isStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainUnitUpdateCommandUpdater"/> class.
        /// </summary>
        public MainUnitUpdateCommandUpdater()
        {
            this.updates = ProducerConsumerQueueFactory<EnqueuedUpdate>.Current.Create(
                "MainUnitUpdateCommandUpdater",
                this.Dequeue,
                1000);
        }

        /// <summary>
        /// Enqueues the update with the given hash for the specified unit id and the display unit with the  given
        /// index.
        /// </summary>
        /// <param name="mainUnitId">
        /// The main unit id.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        public void EnqueueDisplayContent(int mainUnitId, int index, string hash)
        {
            this.updates.Enqueue(new EnqueuedUpdate { MainUnitId = mainUnitId, Hash = hash, Index = index });
        }

        /// <summary>
        /// Starts the component.
        /// </summary>
        public void Start()
        {
            if (this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (this.isStarted)
                {
                    return;
                }

                this.updates.StartConsumer();
                this.isStarted = true;
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (!this.isStarted)
            {
                return;
            }

            lock (this.locker)
            {
                if (!this.isStarted)
                {
                    return;
                }

                this.updates.StopConsumer();
                this.isStarted = false;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();
        }

        private async void Dequeue(EnqueuedUpdate update)
        {
            try
            {
                var unitService = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
                var unit =
                    (await
                     unitService.QueryAsync(
                         UnitQuery.Create()
                         .WithId(update.MainUnitId)
                         .IncludeUpdateGroup(UpdateGroupFilter.Create().IncludeUpdateParts()))).Single();
                var updatePart = unit.UpdateGroup.UpdateParts.OrderByDescending(p => p.Id).FirstOrDefault();
                var configuration = this.GetConfiguration(updatePart.ToDto());
                var newConfiguration = await this.CreateNewConfigurationAsync(configuration, update.Index, update.Hash);
                await this.StoreNewCommandAsync(unit.Id, newConfiguration);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while handling a queued update");
            }
        }

        private async Task StoreNewCommandAsync(int id, string hash)
        {
            var updateFolder = new FolderUpdate
                                   {
                                       Name = "Root"
                                   };
            updateFolder.Items.Add(new FileUpdate
                                       {
                                           Name = hash,
                                           Hash = hash
                                       });
            var unitService = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
            var unit = await unitService.GetAsync(id);
            var updateservice = DependencyResolver.Current.Get<IUpdateCommandChangeTrackingManager>();
            var writable = updateservice.Create();
            writable.Command = new XmlData(updateFolder);
            writable.Unit = unit;
            await updateservice.AddAsync(writable);
        }

        private MainUnitConfig GetConfiguration(UpdatePart updatePart)
        {
            var files = (UpdateFolderStructure)updatePart.Structure.Deserialize();
            var configDir = files.Folders[0];
            var configFile = (FolderUpdate)configDir.Items[0];
            var main = (FileUpdate)configFile.Items[0];
            var resourceService = DependencyResolver.Current.Get<IResourceProvider>();
            var mainConfigResource = resourceService.GetResource(main.Hash);
            using (var x = mainConfigResource.OpenRead())
            {
                var manager = new ConfigManager<MainUnitConfig> { Configurator = new Configurator(x) };
                return manager.Config;
            }
        }

        private async Task<string> CreateNewConfigurationAsync(MainUnitConfig config, int index, string hash)
        {
            // the unit index starts from 1, but in the collection we need 0-based indexes
            var arrayIndex = index - 1;
            if (config.DisplayUnits.Count > arrayIndex)
            {
                config.DisplayUnits.RemoveAt(arrayIndex);
            }

            config.DisplayUnits.Insert(
                arrayIndex,
                new DisplayUnitConfig { FirmwareHash = EmptyFileHash64, ContentHash = hash });

            var configurationSerializer = DependencyResolver.Current.Get<IConfigurationSerializer>();
            var configuration = await configurationSerializer.SerializeAsync(config);

            var contentService = DependencyResolver.Current.Get<IContentResourceService>();
            var configurationHash = ContentResourceHash.Create(configuration, HashAlgorithmTypes.xxHash64);
            configuration.Position = 0;
            var uploadRequest = new ContentResourceUploadRequest
                                    {
                                        Content = configuration,
                                        Resource = new ContentResource
                                                       {
                                                           Hash = configurationHash,
                                                           Length = configuration.Length,
                                                           MimeType = "application/octet-stream",
                                                           OriginalFilename = "configuration.bin",
                                                           HashAlgorithmType = HashAlgorithmTypes.xxHash64
                                                       }
                                    };
            var response = await contentService.UploadAsync(uploadRequest);
            if (response == null)
            {
                Logger.Trace("The configuration witgh has '{0}' already exists", configurationHash);
            }

            return configurationHash;
        }

        /// <summary>
        /// The enqueued update.
        /// </summary>
        public class EnqueuedUpdate
        {
            /// <summary>
            /// Gets or sets the main unit id.
            /// </summary>
            public int MainUnitId { get; set; }

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets or sets the hash.
            /// </summary>
            public string Hash { get; set; }
        }
    }
}