// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPaperContentProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic.EPaper
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;

    using Gorba.Center.BackgroundSystem.Core.Qube;
    using Gorba.Center.BackgroundSystem.Core.Qube.Content;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Utils;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The e-paper content provider dynamically updates the <see cref="UpdatePart"/>s with content taken from an
    /// external URL.
    /// </summary>
    internal class EPaperContentProvider : DynamicContentProviderBase
    {
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(1);

        private readonly ITimer refreshTimer; // used to pool the news feed

        private readonly EPaperDynamicContentPart configPart;

        private readonly UpdateFolderStructure currentStructure;

        private CancellationTokenSource cancelAll;

        private string lastContentFromUrlHash;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPaperContentProvider"/> class.
        /// </summary>
        /// <param name="updateGroup">
        /// The update group.
        /// </param>
        /// <param name="configPart">
        /// The config part.
        /// </param>
        public EPaperContentProvider(UpdateGroupReadableModel updateGroup, EPaperDynamicContentPart configPart)
        {
            this.configPart = configPart;
            if (!configPart.IsPersistentFile)
            {
                this.refreshTimer = TimerFactory.Current.CreateTimer("EPaper_" + updateGroup.Name);
                this.refreshTimer.Interval = RefreshInterval;
                this.refreshTimer.AutoReset = true;
                this.refreshTimer.Elapsed += this.OnRefreshIntervalElapsed;
            }

            this.currentStructure = new UpdateFolderStructure();
        }

        /// <summary>
        /// The create folder structure.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateFolderStructure"/>.
        /// </returns>
        public override UpdateFolderStructure CreateFolderStructure()
        {
            return this.currentStructure;
        }

        /// <summary>
        /// The do start.
        /// </summary>
        protected override void DoStart()
        {
            this.cancelAll = new CancellationTokenSource();
            this.cancelAll.Token.Register(this.DisposeTimer);
            if (this.configPart.IsPersistentFile)
            {
                this.AddPersistentContentResource();
                return;
            }

            this.OnRefreshIntervalElapsed(null, null);
            this.refreshTimer.Enabled = true;
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            if (!this.configPart.IsPersistentFile)
            {
                this.DisposeTimer();
            }

            this.cancelAll.Cancel();
        }

        private void DisposeTimer()
        {
            if (this.refreshTimer == null)
            {
                return;
            }

            this.refreshTimer.Enabled = false;
            this.refreshTimer.Dispose();
        }

        private void AddPersistentContentResource()
        {
            try
            {
                var updater = DependencyResolver.Current.Get<IMainUnitUpdateCommandUpdater>();
                updater.EnqueueDisplayContent(
                    this.configPart.MainUnitId,
                    this.configPart.DisplayUnitIndex,
                    this.configPart.StaticFileSourceHash);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error adding a content resource.");
            }
        }

        private async void OnRefreshIntervalElapsed(object sender, EventArgs eventArgs)
        {
            try
            {
                if (this.cancelAll.Token.IsCancellationRequested)
                {
                    return;
                }

                var uri = new Uri(this.configPart.Url);
                var memoryStream = new MemoryStream();
                using (var client = new HttpClient())
                {
                    var stream = await client.GetStreamAsync(uri);
                    await stream.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;

                var hash = ContentResourceHash.Create(memoryStream, HashAlgorithmTypes.xxHash64);
                memoryStream.Position = 0;

                if (string.Equals(hash, this.lastContentFromUrlHash, StringComparison.OrdinalIgnoreCase))
                {
                    // The content didn't change
                    return;
                }

                var conversion = TimeTableContentConverter.Create(EPaperFormat.TimeTable);
                var conversionResult = await conversion.ProcessAsync(memoryStream);
                memoryStream.Position = 0;
                var contentResourceService = DependencyResolver.Current.Get<IContentResourceService>();
                var request = new ContentResourceUploadRequest
                                  {
                                      Content = conversionResult.Output.Epd.Content,
                                      Resource = conversionResult.Output.Epd.Resource
                                  };
                var result = await contentResourceService.UploadAsync(request);
                if (result == null)
                {
                    this.Logger.Trace("The resource '{0}' already stored", conversionResult.Output.Epd.Resource.Hash);
                }

                var updater = DependencyResolver.Current.Get<IMainUnitUpdateCommandUpdater>();
                updater.EnqueueDisplayContent(
                    this.configPart.MainUnitId,
                    this.configPart.DisplayUnitIndex,
                    conversionResult.Output.Epd.Resource.Hash);

                // we store the hash here, where the content has been handled
                // TODO: verify also against the database. There could have been problems in handling the enqueued item!
                this.lastContentFromUrlHash = hash;
            }
            catch (Exception exception)
            {
                this.Logger.Warn("Error while updating content {0}", exception);
            }
        }
    }
}