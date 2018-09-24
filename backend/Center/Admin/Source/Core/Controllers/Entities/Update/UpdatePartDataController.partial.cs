// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePartDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdatePartDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UpdatePartDataController"/>.
    /// </summary>
    public partial class UpdatePartDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UpdatePartQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query = UpdatePartQuery.Create().IncludeUpdateGroup(UpdateGroupFilter.Create().WithTenant(currentTenant));
        }

        partial void Filter(ref Func<UpdatePartReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PrePopulateEntityDetails(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PrePopulateEntityDetailsAsync;
        }

        private async Task<bool> FilterAsync(UpdatePartReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.UpdateGroup.LoadReferencePropertiesAsync();
            return readableModel.UpdateGroup.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }

        private async Task PrePopulateEntityDetailsAsync(UpdatePartReadOnlyDataViewModel dataViewModel)
        {
            foreach (var updateCommand in dataViewModel.ReadableModel.RelatedCommands)
            {
                await updateCommand.LoadReferencePropertiesAsync();
            }
        }
    }
}