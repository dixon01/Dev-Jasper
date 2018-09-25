// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Update;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UpdateGroupDataController"/>.
    /// </summary>
    public partial class UpdateGroupDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UpdateGroupQuery query)
        {
            // unit configuration & media configuration documents are needed for display strings
            query = UpdateGroupQuery.Create().WithTenant(this.ApplicationState.CurrentTenant.ToDto())
                .IncludeUnitConfiguration(UnitConfigurationFilter.Create().IncludeDocument())
                .IncludeMediaConfiguration(MediaConfigurationFilter.Create().IncludeDocument());
        }

        partial void Filter(ref Func<UpdateGroupReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PostCreateEntity(UpdateGroupDataViewModel dataViewModel)
        {
            dataViewModel.Tenant.SelectedEntity = this.Factory.CreateReadOnly(this.ApplicationState.CurrentTenant);
        }

        partial void PostSetupReferenceProperties(ref Func<UpdateGroupDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        partial void PreDeleteEntity(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreDeleteEntityAsync;
        }

        private async Task<bool> FilterAsync(UpdateGroupReadableModel readableModel)
        {
            if (!readableModel.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id))
            {
                return false;
            }

            if (readableModel.UnitConfiguration != null)
            {
                await readableModel.UnitConfiguration.LoadReferencePropertiesAsync();
            }

            if (readableModel.MediaConfiguration != null)
            {
                await readableModel.MediaConfiguration.LoadReferencePropertiesAsync();
            }

            return true;
        }

        private async Task PreDeleteEntityAsync(UpdateGroupReadOnlyDataViewModel dataViewModel)
        {
            var readableModel = dataViewModel.ReadableModel;
            await readableModel.LoadNavigationPropertiesAsync();

            // remove the update group from all units
            foreach (var unit in readableModel.Units.ToList())
            {
                var writableUnit = unit.ToChangeTrackingModel();
                writableUnit.UpdateGroup = null;
                writableUnit.Commit();
            }

            // remove all update parts (but first remove all related commands from it)
            using (var updateParts = this.ConnectionController.CreateChannelScope<IUpdatePartDataService>())
            {
                // updating many-to-many relationships is only possible with data services
                foreach (var updatePartId in readableModel.UpdateParts.Select(u => u.Id).ToList())
                {
                    var updatePart = (await updateParts.Channel.QueryAsync(
                        UpdatePartQuery.Create().WithId(updatePartId).IncludeRelatedCommands())).First();

                    // clear the list (it is read-only, so updatePart.RelatedCommands.Clear() won't work)
                    updatePart.RelatedCommands = new List<UpdateCommand>();
                    await updateParts.Channel.UpdateAsync(updatePart);
                    await updateParts.Channel.DeleteAsync(updatePart);
                }
            }
        }

        private async Task PostSetupReferencePropertiesAsync(UpdateGroupDataViewModel dataViewModel)
        {
            var permissions = ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;
            await this.DataController.Tenant.AwaitAllDataAsync();
            var tenants =
                this.DataController.Tenant.All.Where(
                    t => permissions.HasPermission(t.ReadableModel, Permission.Create, DataScope.Update));
            dataViewModel.Tenant.Entities =
                new ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>(
                    new ObservableCollection<TenantReadOnlyDataViewModel>(tenants));
        }
    }
}