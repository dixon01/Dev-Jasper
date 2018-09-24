// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateDataObserver.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateDataObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;

    using NLog;

    /// <summary>
    /// This class is responsible to observe changes in the database (through change tracking)
    /// and then create the necessary update commands for units that changed.
    /// Currently this class tracks Units and their Update Group.
    /// </summary>
    internal class UpdateDataObserver : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly UpdateTransferController updateTransferController;

        private readonly IUnitChangeTrackingManager unitsManager;

        private readonly IUpdatePartChangeTrackingManager updatePartsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDataObserver"/> class.
        /// </summary>
        /// <param name="updateTransferController">
        /// The update transfer controller.
        /// </param>
        public UpdateDataObserver(UpdateTransferController updateTransferController)
        {
            this.updateTransferController = updateTransferController;

            this.unitsManager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
            this.unitsManager.Added += this.UnitsManagerOnAdded;
            this.unitsManager.Removed += this.UnitsManagerOnRemoved;

            this.updatePartsManager = DependencyResolver.Current.Get<IUpdatePartChangeTrackingManager>();
            this.updatePartsManager.Removed += this.UpdatePartsManagerOnRemoved;

            this.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.unitsManager.Added -= this.UnitsManagerOnAdded;
            this.unitsManager.Removed -= this.UnitsManagerOnRemoved;
            this.updatePartsManager.Removed -= this.UpdatePartsManagerOnRemoved;
        }

        private async void Start()
        {
            try
            {
                foreach (var unit in await this.unitsManager.QueryAsync())
                {
                    this.ObserveUnit(unit);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't start observer");
            }
        }

        private void ObserveUnit(UnitReadableModel unit)
        {
            Logger.Trace("Observing unit {0}", unit.Name);
            unit.PropertyChanged += this.UnitOnPropertyChanged;
        }

        private async void CreateUpdateCommands(UnitReadableModel unit)
        {
            Logger.Trace("Creating necessary update commands for unit {0}", unit.Name);
            try
            {
                await unit.LoadReferencePropertiesAsync();
                if (unit.UpdateGroup == null)
                {
                    return;
                }

                Logger.Debug(
                    "Unit '{0}' was added/modified, sending new commands for Update Group '{1}'",
                    unit.Name,
                    unit.UpdateGroup.Name);
                await this.updateTransferController.CreateUpdateCommandsForUnitAsync(unit.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't send update commands for changed unit " + unit.Name);
            }
        }

        private void UnitsManagerOnAdded(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            this.ObserveUnit(e.Model);
            this.CreateUpdateCommands(e.Model);
        }

        private void UnitsManagerOnRemoved(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            e.Model.PropertyChanged -= this.UnitOnPropertyChanged;
        }

        private void UnitOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "UpdateGroup")
            {
                return;
            }

            this.CreateUpdateCommands((UnitReadableModel)sender);
        }

        private async void UpdatePartsManagerOnRemoved(object sender, ReadableModelEventArgs<UpdatePartReadableModel> e)
        {
            try
            {
                await e.Model.LoadReferencePropertiesAsync();
                if (e.Model.UpdateGroup == null)
                {
                    return;
                }

                await e.Model.UpdateGroup.LoadNavigationPropertiesAsync();
                foreach (var unit in e.Model.UpdateGroup.Units)
                {
                    this.CreateUpdateCommands(unit);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't send update commands for removed update part " + e.Model.Id);
            }
        }
    }
}