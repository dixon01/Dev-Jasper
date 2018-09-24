// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupDynamicDataController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupDynamicDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The controller responsible to generate dynamic data depending on what was configured
    /// in the <see cref="UpdatePart.DynamicContent"/> for a certain Update Group.
    /// For each Update Group in the system, there is a <see cref="UpdateGroupDynamicDataController"/>
    /// observing the included parts and all providers and generating new update parts if needed.
    /// </summary>
    internal class UpdateGroupDynamicDataController
    {
        private static readonly TimeSpan PreviewTime = TimeSpan.FromMinutes(1);
        private readonly Logger logger;

        private readonly IDeadlineTimer updateTimer;

        private readonly List<DynamicContentProviderBase> providers = new List<DynamicContentProviderBase>();

        private GroupUpdate currentUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGroupDynamicDataController"/> class.
        /// </summary>
        /// <param name="updateGroup">
        /// The update group for which this controller is responsible.
        /// </param>
        public UpdateGroupDynamicDataController(UpdateGroupReadableModel updateGroup)
        {
            this.logger = LogManager.GetLogger(string.Format("{0}-{1}", this.GetType().FullName, updateGroup.Id));
            this.UpdateGroup = updateGroup;

            this.updateTimer = TimerFactory.Current.CreateDeadlineTimer(this.GetType().Name + "-" + updateGroup.Name);
            this.updateTimer.TriggerIfPassed = true;
        }

        /// <summary>
        /// Gets the update group for which this controller is responsible.
        /// </summary>
        public UpdateGroupReadableModel UpdateGroup { get; private set; }

        /// <summary>
        /// Starts this controller.
        /// </summary>
        public async void Start()
        {
            try
            {
                await this.UpdateGroup.LoadNavigationPropertiesAsync();
                this.logger.Debug(
                    "Starting for Update Group '{0}' of Tenant '{1}'",
                    this.UpdateGroup.Name,
                    this.UpdateGroup.Tenant.Name);
                this.UpdateGroup.UpdateParts.CollectionChanged += this.UpdatePartsOnCollectionChanged;
                await this.RestartProvidersAsync();
                this.logger.Debug("Started");
            }
            catch (Exception ex)
            {
                this.logger.Error("Couldn't start providers {0}", ex);
            }
        }

        /// <summary>
        /// Stops this controller.
        /// </summary>
        public void Stop()
        {
            this.StopProviders();
        }

        private async Task RestartProvidersAsync()
        {
            this.updateTimer.Enabled = false;

            var now = TimeProvider.Current.UtcNow + PreviewTime;
            var update = await this.GetCurrentUpdatesAsync(now);
            if (update == null)
            {
                this.StopProviders();
                return;
            }

            if (update.StartTime > now)
            {
                this.logger.Debug("Currently no Update Part is active, waiting until {0}", update.StartTime);
                this.updateTimer.UtcDeadline = update.StartTime - PreviewTime;
                this.updateTimer.Enabled = true;
                this.StopProviders();
                return;
            }

            this.StartProviders(update);
            if (!update.EndTime.HasValue)
            {
                this.logger.Debug("No next Update Part is scheduled");
                return;
            }

            this.logger.Debug("Next Update Part is scheduled for {0}", update.EndTime.Value);
            this.updateTimer.UtcDeadline = update.EndTime.Value - PreviewTime;
            this.updateTimer.Enabled = true;
        }

        private async Task<GroupUpdate> GetCurrentUpdatesAsync(DateTime utcNow)
        {
            var updateParts =
                this.UpdateGroup.UpdateParts.Where(p => p.Type != UpdatePartType.AutoPresentation).ToList();
            if (updateParts.Count == 0)
            {
                this.logger.Debug("Couldn't find any update parts for {0}", this.UpdateGroup.Name);
                return null;
            }

            var times =
                updateParts.Select(p => p.Start)
                    .Union(updateParts.Select(p => p.End))
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
            var index = times.FindIndex(t => t > utcNow);
            if (index < 0)
            {
                this.logger.Debug("Couldn't find any valid updates for {0}", this.UpdateGroup.Name);
                return null;
            }

            if (index > 0)
            {
                // we want the time before "now"
                index--;
            }

            var time = times[index];
            index++;
            var update = new GroupUpdate(time, times.Count >= index ? times[index] : default(DateTime?));
            foreach (var part in
                updateParts.Where(p => time >= p.Start && time < p.End)
                    .OrderByDescending(p => p.Id)
                    .GroupBy(p => p.Type)
                    .Select(g => g.First()))
            {
                await part.LoadXmlPropertiesAsync();
                update.IncludedParts.Add(part);
            }

            return update;
        }

        private void StartProviders(GroupUpdate update)
        {
            if (this.currentUpdate != null
                && this.currentUpdate.IncludedParts.OrderBy(p => p.Id)
                       .SequenceEqual(update.IncludedParts.OrderBy(p => p.Id)))
            {
                this.logger.Trace("Still the same Update Parts, nothing to do");
                return;
            }

            this.StopProviders();

            this.currentUpdate = update;
            var parts =
                this.currentUpdate.IncludedParts.Select(p => p.DynamicContent.Deserialize())
                    .OfType<DynamicContentInfo>()
                    .SelectMany(i => i.Parts);

            var feedProviders =
                parts.Select(p => DynamicContentProviderBase.Create(this.UpdateGroup, p)).Where(p => p != null);
            foreach (var provider in feedProviders)
            {
                this.logger.Trace("Created new provider: {0}", provider);
                this.providers.Add(provider);
                provider.ContentUpdated += this.ProviderOnContentUpdated;
                provider.Start();
            }
        }

        private async Task GenerateUpdatePartAsync()
        {
            var structure = new UpdateFolderStructure();
            foreach (var provider in this.providers.ToList())
            {
                structure.Include(provider.CreateFolderStructure());
            }

            var updatePart = new UpdatePart();
            updatePart.Type = UpdatePartType.AutoPresentation;
            updatePart.UpdateGroup = this.UpdateGroup.ToDto();
            updatePart.Start = this.currentUpdate.StartTime;
            updatePart.End = this.currentUpdate.EndTime ?? new DateTime(2100, 12, 31);
            updatePart.Structure = new XmlData(structure);

            this.logger.Debug("Created new Update Part valid from {0} until {1}", updatePart.Start, updatePart.End);

            var updatePartDataService = DependencyResolver.Current.Get<IUpdatePartDataService>();
            await updatePartDataService.AddAsync(updatePart);

            var updateService = DependencyResolver.Current.Get<IUpdateService>();
            await updateService.CreateUpdateCommandsForUpdateGroupAsync(this.UpdateGroup.Id);
        }

        private void StopProviders()
        {
            foreach (var provider in this.providers)
            {
                provider.Stop();
            }

            this.providers.Clear();
        }

        private async void ProviderOnContentUpdated(object sender, EventArgs e)
        {
            try
            {
                this.logger.Trace("Received content update from {0}", sender);
                await this.GenerateUpdatePartAsync();
            }
            catch (Exception ex)
            {
                this.logger.Warn("Couldn't generate update part {0}", ex);
            }
        }

        private async void UpdatePartsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems.OfType<UpdatePartReadableModel>()
                                .All(p => p.Type == UpdatePartType.AutoPresentation))
                        {
                            // ignore updates if the parts are from us
                            return;
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems.OfType<UpdatePartReadableModel>()
                                .All(p => p.Type == UpdatePartType.AutoPresentation))
                        {
                            // ignore updates if the parts are from us
                            return;
                        }

                        break;
                    case NotifyCollectionChangedAction.Move:
                        // order doesn't matter for us
                        return;
                }

                await this.RestartProvidersAsync();
            }
            catch (Exception ex)
            {
                this.logger.Warn("Couldn't restart providers for update parts change {0}", ex);
            }
        }

        private class GroupUpdate
        {
            public GroupUpdate(DateTime startTime, DateTime? endTime)
            {
                this.StartTime = startTime;
                this.EndTime = endTime;
                this.IncludedParts = new List<UpdatePartReadableModel>();
            }

            public DateTime StartTime { get; private set; }

            public DateTime? EndTime { get; private set; }

            public List<UpdatePartReadableModel> IncludedParts { get; private set; }
        }
    }
}