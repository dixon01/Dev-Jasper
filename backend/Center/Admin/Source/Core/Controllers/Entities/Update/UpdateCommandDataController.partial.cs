// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UpdateCommandDataController"/>.
    /// </summary>
    public partial class UpdateCommandDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UpdateCommandQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query = UpdateCommandQuery.Create().IncludeUnit(UnitFilter.Create().WithTenant(currentTenant));
        }

        partial void Filter(ref Func<UpdateCommandReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PrePopulateEntityDetails(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PrePopulateEntityDetailsAsync;
        }

        private async Task<bool> FilterAsync(UpdateCommandReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.Unit.LoadReferencePropertiesAsync();
            return readableModel.Unit.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }

        private async Task PrePopulateEntityDetailsAsync(UpdateCommandReadOnlyDataViewModel dataViewModel)
        {
            foreach (var part in dataViewModel.ReadableModel.IncludedParts)
            {
                await part.LoadReferencePropertiesAsync();
            }
        }
    }
}