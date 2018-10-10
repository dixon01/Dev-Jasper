// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="DocumentDataController"/>.
    /// </summary>
    public partial class DocumentDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref DocumentQuery query)
        {
            query = DocumentQuery.Create().WithTenant(this.ApplicationState.CurrentTenant.ToDto());
        }

        partial void Filter(ref Func<DocumentReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PostCreateEntity(DocumentDataViewModel dataViewModel)
        {
            dataViewModel.Tenant.SelectedEntity = this.Factory.CreateReadOnly(this.ApplicationState.CurrentTenant);
        }

        private Task<bool> FilterAsync(DocumentReadableModel readableModel)
        {
            return Task.FromResult(readableModel.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id));
        }
    }
}