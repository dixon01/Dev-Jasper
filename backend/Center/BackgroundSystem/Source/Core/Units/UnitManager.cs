// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Dynamic;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Network;

    using NLog;

    /// <summary>
    /// The unit manager responsible for handling units and their connection to the Background System.
    /// </summary>
    public class UnitManager : IUnitManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TaskCompletionSource<bool> controllersLoaded = new TaskCompletionSource<bool>();

        private readonly IUnitChangeTrackingManager unitChangeTrackingManager;

        private readonly Dictionary<string, UnitController> controllers = new Dictionary<string, UnitController>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitManager"/> class.
        /// </summary>
        public UnitManager()
        {
            this.unitChangeTrackingManager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
        }

        /// <summary>
        /// Starts this manager.
        /// </summary>
        public void Start()
        {
            this.DoStart();
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        public void Stop()
        {
            this.unitChangeTrackingManager.Added -= this.OnUnitAdded;
            this.unitChangeTrackingManager.Removed -= this.OnUnitRemoved;
            MessageDispatcher.Instance.RoutingTable.Updated -= this.RoutingTableOnUpdated;
        }

        /// <summary>
        /// Sends a live update to all units assigned to the specified update group.
        /// </summary>
        /// <param name="queuedMessage">
        /// The live update to send.
        /// </param>
        public async void SendLiveUpdate(QueuedMessage queuedMessage)
        {
            try
            {
                if (!(await this.controllersLoaded.Task))
                {
                    return;
                }

                var controllersToUpdate =
                    this.controllers.Values.Where(
                        controller =>
                        controller.Model.UpdateGroup != null
                        && controller.Model.UpdateGroup.Id == queuedMessage.UpdateGroupId).ToList();
                foreach (var controller in controllersToUpdate)
                {
                    controller.EnqueueLiveUpdate(queuedMessage);
                }
            }
            catch (Exception exception)
            {
                Logger.Warn("error while sending live update {0}", exception);
            }
        }

        private async void DoStart()
        {
            try
            {
                var units = await this.unitChangeTrackingManager.QueryAsync(UnitQuery.Create().IncludeUpdateGroup());
                lock (this.controllers)
                {
                    foreach (var controller in units.Select(model => new UnitController(model)))
                    {
                        this.controllers.Add(controller.Model.Name, controller);
                    }
                }

                this.controllersLoaded.TrySetResult(true);
                this.unitChangeTrackingManager.Added += this.OnUnitAdded;
                this.unitChangeTrackingManager.Removed += this.OnUnitRemoved;

                var connectedUnits =
                    MessageDispatcher.Instance.RoutingTable.GetEntries(i => true)
                        .Select(e => e.Address.Unit)
                        .Distinct()
                        .ToList();
                await this.SetConnectedAsync(connectedUnits, true);

                MessageDispatcher.Instance.RoutingTable.Updated += this.RoutingTableOnUpdated;
            }
            catch (Exception ex)
            {
                this.controllersLoaded.TrySetResult(false);
                Logger.Error("Couldn't start Cause:{0}", ex);
            }
        }

        private async void OnUnitAdded(object sender, ReadableModelEventArgs<UnitReadableModel> readableModelEventArgs)
        {
            try
            {
                UnitController controller;
                lock (this.controllers)
                {
                    if (this.controllers.ContainsKey(readableModelEventArgs.Model.Name))
                    {
                        return;
                    }

                    controller = new UnitController(readableModelEventArgs.Model);
                    this.controllers.Add(
                        readableModelEventArgs.Model.Name,
                        controller);
                }

                await readableModelEventArgs.Model.LoadReferencePropertiesAsync();
                await this.InitControllerAsync(controller);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while adding unit");
            }
        }

        private void OnUnitRemoved(object sender, ReadableModelEventArgs<UnitReadableModel> readableModelEventArgs)
        {
            lock (this.controllers)
            {
                if (!this.controllers.ContainsKey(readableModelEventArgs.Model.Name))
                {
                    return;
                }

                this.controllers[readableModelEventArgs.Model.Name].Dispose();
                this.controllers.Remove(readableModelEventArgs.Model.Name);
            }
        }

        private async void RoutingTableOnUpdated(object sender, RouteUpdatesEventArgs e)
        {
            var addedUnits = e.Updates.Where(u => u.Added).Select(u => u.Address.Unit).Distinct();
            var removedUnits = e.Updates.Where(u => !u.Added).Select(u => u.Address.Unit).Distinct().ToList();

            if (removedUnits.Count > 0)
            {
                var connectedUnits =
                    MessageDispatcher.Instance.RoutingTable.GetEntries(i => true)
                        .Select(u => u.Address.Unit)
                        .Distinct()
                        .ToList();
                removedUnits.RemoveAll(connectedUnits.Contains);
            }

            try
            {
                await this.SetConnectedAsync(addedUnits, true);
                await this.SetConnectedAsync(removedUnits, false);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't update unit connection status", ex);
            }
        }

        private async Task SetConnectedAsync(IEnumerable<string> unitNames, bool isConnected)
        {
            var tasks = new List<Task>();
            lock (this.controllers)
            {
                foreach (var unitName in unitNames)
                {
                    if (!this.controllers.ContainsKey(unitName))
                    {
                        Logger.Debug("Controller not found for unit {0}", unitName);
                        continue;
                    }

                    var controller = this.controllers[unitName];
                    tasks.Add(this.SetConnectedAsync(controller, isConnected));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task SetConnectedAsync(UnitController controller, bool isConnected)
        {
            if (controller.Model.IsConnected == isConnected)
            {
                Logger.Trace("IsConnected flag already set {0} for controler {1}", isConnected, controller.Model.Name);
                return;
            }

            var writableModel = controller.Model.ToChangeTrackingModel();
            writableModel.IsConnected = isConnected;
            await this.unitChangeTrackingManager.CommitAndVerifyAsync(writableModel);
        }

        private async Task InitControllerAsync(UnitController controller)
        {
            var connectedUnit =
                MessageDispatcher.Instance.RoutingTable.GetEntries(i => true)
                    .SingleOrDefault(e => e.Address.Unit == controller.Model.Name);
            if (connectedUnit == null)
            {
                return;
            }

            await this.SetConnectedAsync(controller, true);
        }
    }
}