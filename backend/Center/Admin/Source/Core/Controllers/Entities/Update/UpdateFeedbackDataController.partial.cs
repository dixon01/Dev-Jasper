// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFeedbackDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFeedbackDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Update;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UpdateFeedbackDataController"/>.
    /// </summary>
    public partial class UpdateFeedbackDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref UpdateFeedbackQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query =
                UpdateFeedbackQuery.Create().IncludeUpdateCommand(
                    UpdateCommandFilter.Create().IncludeUnit(UnitFilter.Create().WithTenant(currentTenant)));
        }

        partial void Filter(ref Func<UpdateFeedbackReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        private async Task<bool> FilterAsync(UpdateFeedbackReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.UpdateCommand.LoadReferencePropertiesAsync();
            await readableModel.UpdateCommand.Unit.LoadReferencePropertiesAsync();
            return readableModel.UpdateCommand.Unit.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }
    }
}