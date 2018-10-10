// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenGateContentProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenGateContentProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.ScreenGate
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Common.Protocols.ScreenGate.Data;
    using Gorba.Common.Protocols.ScreenGate.Init;
    using Gorba.Common.Protocols.ScreenGate.MultiStatus;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The dynamic content provider for ScreenGate.
    /// </summary>
    internal class ScreenGateContentProvider : DynamicContentProviderBase
    {
        private const string ResourceDownloadDescriptionFormat = "Downloaded from {0} (ETag={1})";

        private static readonly TimeSpan RefreshTimeout = TimeSpan.FromSeconds(20);

        private static new readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Uri, ResourceState> resourceStates = new Dictionary<Uri, ResourceState>();

        private readonly ScreenGateDynamicContentPart partConfig;

        private readonly ITimer refreshTimer;

        private readonly JsonDownloader<InitialConfig> initialConfig;

        private readonly XmlDownloader<MultiStatusConfiguration> multiStatus;

        private readonly JsonDownloader<PlayerData> playerData;

        private readonly string tempDirectory;

        private CancellationTokenSource cancelAll;

        private UpdateFolderStructure currentStructure;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenGateContentProvider"/> class.
        /// </summary>
        /// <param name="partConfig">
        /// The dynamic part config.
        /// </param>
        public ScreenGateContentProvider(ScreenGateDynamicContentPart partConfig)
        {
            this.partConfig = partConfig;

            this.refreshTimer = TimerFactory.Current.CreateTimer("ScreenGateRefresh");
            this.refreshTimer.AutoReset = false;
            this.refreshTimer.Interval = RefreshTimeout;
            this.refreshTimer.Elapsed += (s, e) => this.Refresh();

            this.initialConfig = new JsonDownloader<InitialConfig>();
            this.initialConfig.Credentials = new NetworkCredential(this.partConfig.Username, this.partConfig.Password);
            this.multiStatus = new XmlDownloader<MultiStatusConfiguration>();
            this.playerData = new JsonDownloader<PlayerData>();

            this.tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            this.currentStructure = new UpdateFolderStructure();
        }

        /// <summary>
        /// Creates the folder structure for this content provider.
        /// This method should never return null, but can return an empty object.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateFolderStructure"/>.
        /// </returns>
        public override UpdateFolderStructure CreateFolderStructure()
        {
            return this.currentStructure;
        }

        /// <summary>
        /// Implementation of <see cref="DynamicContentProviderBase.Start"/>.
        /// </summary>
        protected override void DoStart()
        {
            this.cancelAll = new CancellationTokenSource();
            Directory.CreateDirectory(this.tempDirectory);

            this.Refresh();
        }

        /// <summary>
        /// Implementation of <see cref="DynamicContentProviderBase.Stop"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.refreshTimer.Enabled = false;

            this.cancelAll.Cancel();
            try
            {
                Directory.Delete(this.tempDirectory, true);
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't delete temporary directory {0}", ex);
            }
        }

        private async void Refresh()
        {
            try
            {
                await this.initialConfig.UpdateAsync(new Uri(this.partConfig.ConfigUrl));
                this.CheckCancelled();

                var multiStatusChanged =
                    await this.multiStatus.UpdateAsync(new Uri(this.initialConfig.LastValue.XmlUrl));
                this.CheckCancelled();

                var dataResponse = this.multiStatus.LastValue.MultiStatuses[0].GetResponses().First();
                var playerDataChanged = await this.playerData.UpdateAsync(dataResponse.Key);
                this.CheckCancelled();

                if (multiStatusChanged || playerDataChanged)
                {
                    var resources = this.multiStatus.LastValue.MultiStatuses[1].GetResponses();
                    await this.UpdateDynamicDataAsync(resources);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't refresh content {0}", ex);
            }

            this.refreshTimer.Enabled = this.IsRunning;
        }

        private void CheckCancelled()
        {
            this.cancelAll.Token.ThrowIfCancellationRequested();
        }

        private async Task UpdateDynamicDataAsync(IDictionary<Uri, string> resources)
        {
            if (this.playerData.LastValue.BroadcastLocations.Count > 1)
            {
                Logger.Debug(
                    "Got player data with {0} broadcast locations, only using the first",
                    this.playerData.LastValue.BroadcastLocations.Count);
            }

            var broadcastLocation = this.playerData.LastValue.BroadcastLocations.First();
            if (broadcastLocation.Surfaces.Count > 1)
            {
                Logger.Debug(
                    "Got broadcast location with {0} surfaces, only using the first",
                    broadcastLocation.Surfaces.Count);
            }

            var surface = broadcastLocation.Surfaces.First();
            var config = new WebmediaConfig();
            var cycle = new WebmediaCycleConfig { Enabled = true, Name = "ScreenGate" };
            config.Cycles.Add(cycle);

            var requiredResources = new List<string>();
            foreach (var playlistElement in surface.Playlist.Elements)
            {
                try
                {
                    cycle.Elements.Add(this.Convert(playlistElement, requiredResources));
                }
                catch (Exception ex)
                {
                    Logger.Debug("Couldn't add playlist element {0} - {1}", playlistElement.Id, ex);
                }
            }

            this.currentStructure = await this.UploadResourcesAsync(resources, requiredResources, config);
            this.RaiseContentUpdated();
        }

        private async Task<UpdateFolderStructure> UploadResourcesAsync(
            IDictionary<Uri, string> resources,
            List<string> requiredResources,
            WebmediaConfig config)
        {
            var path = Path.GetDirectoryName(this.partConfig.Wm2FilePath);

            var presentation = new FolderUpdate { Name = "Presentation" };
            var folder = presentation;
            if (path != null)
            {
                path = path.Trim('\\', '/');
                if (path.Length > 0)
                {
                    foreach (var subfolder in path.Split('\\', '/').Select(part => new FolderUpdate { Name = part }))
                    {
                        folder.Items.Add(subfolder);
                        folder = subfolder;
                    }
                }
            }

            var structure = new UpdateFolderStructure { Folders = { presentation } };

            var baseUri = this.playerData.LastValue.Config.RemoteResourceUrl.TrimEnd('/');
            var resourceUris = requiredResources.ConvertAll(r => new Uri(baseUri + "/" + r.TrimStart('/')));
            var downloadResources =
                resourceUris.Distinct()
                    .Where(u => !this.resourceStates.ContainsKey(u) || this.resourceStates[u].ETag != resources[u]);
            await Task.WhenAll(downloadResources.Select(r => this.DownloadResourceAsync(r, resources[r])));

            foreach (var resource in resourceUris)
            {
                folder.Items.Add(
                    new FileUpdate
                    {
                        Hash = this.resourceStates[resource].Hash,
                        Name = Path.GetFileName(resource.LocalPath)
                    });
            }

            var tempFile = Path.Combine(this.tempDirectory, "ScreenGate.wm2");
            using (var writer = new StreamWriter(tempFile, false, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(config.GetType());
                serializer.Serialize(writer, config);
            }

            var hash = ResourceHash.Create(tempFile);
            this.UploadResource(tempFile, hash);

            folder.Items.Add(new FileUpdate { Name = Path.GetFileName(this.partConfig.Wm2FilePath), Hash = hash });
            return structure;
        }

        private async Task DownloadResourceAsync(Uri uri, string etag)
        {
            string hash;
            var description = string.Format(ResourceDownloadDescriptionFormat, uri, etag);
            var resourceDataService = DependencyResolver.Current.Get<IResourceDataService>();

            // we use the description to speed up the downloading of files:
            // if they were downloaded from the same URI with the same ETag before, we don't try to download them again
            var existing =
                (await resourceDataService.QueryAsync(ResourceQuery.Create().WithDescription(description)))
                    .FirstOrDefault();
            if (existing != null)
            {
                hash = existing.Hash;
            }
            else
            {
                var tempFile = Path.Combine(this.tempDirectory, Path.GetFileName(uri.LocalPath));

                using (var client = new WebClient())
                {
                    Logger.Debug("Downloading resource {0} (ETag={1}) to {2}", uri, etag, tempFile);
                    await client.DownloadFileTaskAsync(uri, tempFile);
                }

                hash = ResourceHash.Create(tempFile);

                Logger.Trace("Resource {0} (ETag={1}) has hash {2}", uri, etag, hash);
                this.UploadResource(tempFile, hash);
                var resource =
                    (await resourceDataService.QueryAsync(ResourceQuery.Create().WithHash(hash))).Single();
                resource.Description = description;
                await resourceDataService.UpdateAsync(resource);

                File.Delete(tempFile);
            }

            lock (this.resourceStates)
            {
                this.resourceStates[uri] = new ResourceState(etag, hash);
            }
        }

        private void UploadResource(string fileName, string hash)
        {
            // we use the IResourceProvider because it is more efficient (just moving the file instead of stream-copy)
            var resourceProvider = DependencyResolver.Current.Get<IResourceProvider>();
            resourceProvider.AddResource(hash, fileName, true);
        }

        private LayoutWebmediaElement Convert(PlaylistElement playlistElement, List<string> requiredResources)
        {
            var content = this.playerData.LastValue.Contents.FirstOrDefault(c => c.Id == playlistElement.ContentId);
            if (content == null)
            {
                throw new KeyNotFoundException("Couldn't find content " + playlistElement.ContentId);
            }

            var planning = playlistElement.BroadcastPlanning;
            var dayOfWeek = new DayOfWeekEval();
            var conditions = new OrEval { Conditions = { dayOfWeek } };
            this.UpdateSchedule(planning.Monday, (d, v) => d.Monday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Tuesday, (d, v) => d.Tuesday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Wednesday, (d, v) => d.Wednesday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Thursday, (d, v) => d.Thursday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Friday, (d, v) => d.Friday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Saturday, (d, v) => d.Saturday = v, dayOfWeek, conditions);
            this.UpdateSchedule(planning.Sunday, (d, v) => d.Sunday = v, dayOfWeek, conditions);

            var element = new LayoutWebmediaElement
            {
                Duration = TimeSpan.FromSeconds(playlistElement.Duration),
                Name = content.Name,
                EnabledProperty = new DynamicProperty(conditions)
            };

            var backgroundImage = new ImageElement
            {
                X = 0,
                Y = 0,
                Width = content.Width,
                Height = content.Height,
                Filename = Path.GetFileName(content.RasterUrl),
                Scaling = ElementScaling.Stretch,
                Visible = true,
                ZIndex = -100
            };
            requiredResources.Add(content.RasterUrl);
            element.Elements.Add(backgroundImage);

            foreach (var module in content.Modules)
            {
                switch (module.Kind)
                {
                    case "video":
                        var video = new VideoElement
                        {
                            X = module.X,
                            Y = module.Y,
                            Width = module.Width,
                            Height = module.Height,
                            ZIndex = module.Z,
                            Replay = false,
                            VideoUri = Path.GetFileName(module.ResourceMp4Url),
                            Visible = true
                        };
                        if ((module.ResourceHeight.HasValue && module.ResourceHeight.Value < module.Height)
                            || (module.ResourceWidth.HasValue && module.ResourceWidth.Value < module.Width))
                        {
                            // scale up only if configured to do so
                            video.Scaling = module.ScaleToFit ? ElementScaling.Scale : ElementScaling.Fixed;
                        }
                        else
                        {
                            // always scale down
                            video.Scaling = ElementScaling.Scale;
                        }

                        element.Elements.Add(video);
                        requiredResources.Add(module.ResourceMp4Url);
                        break;
                    default:
                        Logger.Debug("Module kind not yet supported: {0}", module.Kind);
                        break;
                }
            }

            return element;
        }

        private void UpdateSchedule(
            List<string[]> schedule, Action<DayOfWeekEval, bool> func, DayOfWeekEval dayOfWeek, OrEval conditions)
        {
            if (schedule == null || schedule.Count == 0)
            {
                // simple case: no scheduling is set, set the flag to false
                func(dayOfWeek, false);
                return;
            }

            if (schedule.Count == 1 && schedule[0][0] == "00:00" && schedule[0][1] == "23:59")
            {
                // simple case: we have an "all day" schedule, set the flag to true
                func(dayOfWeek, true);
                return;
            }

            // complex case: we have a special schedule of this day
            // 1. set the flag on the global DayOfWeekEval to false
            func(dayOfWeek, false);

            var times = new OrEval();
            foreach (var pair in schedule)
            {
                var begin = TimeSpan.ParseExact(pair[0], "hh\\:mm", CultureInfo.InvariantCulture);
                var end = TimeSpan.ParseExact(pair[1], "hh\\:mm", CultureInfo.InvariantCulture);
                times.Conditions.Add(new TimeEval { Begin = begin, End = end });
            }

            // 2. create an "and" of the day of week (called inner) and all times (see above)
            var innerDayOfWeek = new DayOfWeekEval();
            func(innerDayOfWeek, true);
            conditions.Conditions.Add(new AndEval { Conditions = { innerDayOfWeek, times } });
        }

        private class ResourceState
        {
            public ResourceState(string etag, string hash)
            {
                this.ETag = etag;
                this.Hash = hash;
            }

            public string ETag { get; private set; }

            public string Hash { get; private set; }
        }
    }
}
