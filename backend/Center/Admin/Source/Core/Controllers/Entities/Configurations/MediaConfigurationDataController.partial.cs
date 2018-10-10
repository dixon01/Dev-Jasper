// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfigurationDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaConfigurationDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Configurations
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="MediaConfigurationDataController"/>.
    /// </summary>
    public partial class MediaConfigurationDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref MediaConfigurationQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query = MediaConfigurationQuery.Create().IncludeDocument(DocumentFilter.Create().WithTenant(currentTenant));
        }

        partial void Filter(ref Func<MediaConfigurationReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        private async Task<bool> FilterAsync(MediaConfigurationReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.Document.LoadReferencePropertiesAsync();
            return readableModel.Document.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }
    }
}