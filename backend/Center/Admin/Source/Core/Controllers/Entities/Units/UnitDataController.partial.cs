// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Units
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Log;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UnitDataController"/>.
    /// </summary>
    public partial class UnitDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UnitQuery query)
        {
            query = UnitQuery.Create().WithTenant(this.ApplicationState.CurrentTenant.ToDto());
        }

        partial void Filter(ref Func<UnitReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PostCreateEntity(UnitDataViewModel dataViewModel)
        {
            var currentTenant = this.ApplicationState.CurrentTenant;
            if (currentTenant == null)
            {
                return;
            }

            if (dataViewModel.Tenant.Entities.Any(t => t.Id == currentTenant.Id))
            {
                dataViewModel.Tenant.SelectedEntity = this.Factory.CreateReadOnly(this.ApplicationState.CurrentTenant);
            }
        }

        partial void PostSetupReferenceProperties(ref Func<UnitDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        partial void PreDeleteEntity(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreDeleteEntityAsync;
        }

        partial void PrePopulateEntityDetails(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PrePopulateEntityDetailsAsync;
        }

        private Task<bool> FilterAsync(UnitReadableModel readableModel)
        {
            return Task.FromResult(readableModel.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id));
        }

        private async Task PrePopulateEntityDetailsAsync(UnitReadOnlyDataViewModel dataViewModel)
        {
            foreach (var updateCommand in dataViewModel.ReadableModel.UpdateCommands)
            {
                await updateCommand.LoadReferencePropertiesAsync();
            }
        }

        private async Task PreDeleteEntityAsync(UnitReadOnlyDataViewModel dataViewModel)
        {
            var readableModel = dataViewModel.ReadableModel;
            await readableModel.LoadNavigationPropertiesAsync();

            // remove update commands with related feedbacks and parts
            var updateCommands =
                await this.ConnectionController.UpdateCommandChangeTrackingManager.QueryAsync(
                    UpdateCommandQuery.Create().WithUnit(readableModel.ToDto()));
            foreach (var updateCommandReadableModel in updateCommands)
            {
                await updateCommandReadableModel.LoadNavigationPropertiesAsync();
                foreach (var feedback in updateCommandReadableModel.Feedbacks.ToList())
                {
                    await this.ConnectionController.UpdateFeedbackChangeTrackingManager.DeleteAsync(feedback);
                }

                foreach (var includedPart in updateCommandReadableModel.IncludedParts.ToList())
                {
                    await this.ConnectionController.UpdatePartChangeTrackingManager.DeleteAsync(includedPart);
                }

                await
                    this.ConnectionController.UpdateCommandChangeTrackingManager.DeleteAsync(
                        updateCommandReadableModel);
            }

            // delete log entries for the unit
            using (var scope = this.ConnectionController.CreateChannelScope<ILogEntryDataService>())
            {
                await scope.Channel.DeleteAsync(LogEntryFilter.Create().WithUnit(readableModel.ToDto()));
            }
        }

        private async Task PostSetupReferencePropertiesAsync(UnitDataViewModel dataViewModel)
        {
            var permissions = ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;
            await this.DataController.Tenant.AwaitAllDataAsync();
            var tenants =
                this.DataController.Tenant.All.Where(
                    t => permissions.HasPermission(t.ReadableModel, Permission.Create, DataScope.Unit));
            dataViewModel.Tenant.Entities =
                new ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>(
                    new ObservableCollection<TenantReadOnlyDataViewModel>(tenants));
        }
    }
}