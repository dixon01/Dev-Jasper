// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicDataManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicDataManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Update;

    using NLog;

    /// <summary>
    /// The manager responsible to generate all dynamic data depending on what was configured
    /// in the <see cref="UpdatePart.DynamicContent"/>.
    /// </summary>
    public class DynamicDataManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<UpdateGroupDynamicDataController> controllers =
            new List<UpdateGroupDynamicDataController>();

        private IUpdateGroupChangeTrackingManager updateGroupChangeTrackingManager;

        /// <summary>
        /// Starts this manager.
        /// </summary>
        public void Start()
        {
            if (this.controllers.Count > 0)
            {
                return;
            }

            this.DoStart();
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        public void Stop()
        {
            this.updateGroupChangeTrackingManager.Added -= this.UpdateGroupsOnAdded;
            this.updateGroupChangeTrackingManager.Removed -= this.UpdateGroupsOnRemoved;

            lock (this.controllers)
            {
                foreach (var controller in this.controllers)
                {
                    controller.Stop();
                }

                this.controllers.Clear();
            }

            Logger.Info("Stopped");
        }

        private async void DoStart()
        {
            try
            {
                this.updateGroupChangeTrackingManager =
                    DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
                foreach (var updateGroup in await this.updateGroupChangeTrackingManager.QueryAsync(
                    UpdateGroupQuery.Create().IncludeUnits().IncludeUpdateParts()))
                {
                    lock (this.controllers)
                    {
                        this.AddController(updateGroup);
                    }
                }

                this.updateGroupChangeTrackingManager.Added += this.UpdateGroupsOnAdded;
                this.updateGroupChangeTrackingManager.Removed += this.UpdateGroupsOnRemoved;
                Logger.Info("Started");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't start");
            }
        }

        private void AddController(UpdateGroupReadableModel updateGroup)
        {
            var controller = new UpdateGroupDynamicDataController(updateGroup);
            this.controllers.Add(controller);
            controller.Start();
        }

        private void UpdateGroupsOnAdded(object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            lock (this.controllers)
            {
                if (this.controllers.Any(c => c.UpdateGroup.Equals(e.Model)))
                {
                    return;
                }

                this.AddController(e.Model);
            }
        }

        private void UpdateGroupsOnRemoved(object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            lock (this.controllers)
            {
                foreach (var controller in this.controllers.Where(c => c.UpdateGroup.Equals(e.Model)).ToList())
                {
                    controller.Stop();
                    this.controllers.Remove(controller);
                }
            }
        }
    }
}