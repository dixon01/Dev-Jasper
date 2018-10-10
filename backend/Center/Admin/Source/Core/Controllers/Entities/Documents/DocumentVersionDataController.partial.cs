// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentVersionDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using System;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Documents;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="DocumentVersionDataController"/>.
    /// </summary>
    public partial class DocumentVersionDataController
    {
        // ReSharper disable RedundantAssignment
        partial void CreateFilter(ref DocumentVersionQuery query)
        {
            var currentTenant = this.ApplicationState.CurrentTenant.ToDto();
            query = DocumentVersionQuery.Create().IncludeDocument(DocumentFilter.Create().WithTenant(currentTenant));
        }

        partial void Filter(ref Func<DocumentVersionReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PostCreateEntity(DocumentVersionDataViewModel dataViewModel)
        {
            dataViewModel.Content.XmlData = new XmlData(new UnitConfigData());
        }

        private async Task<bool> FilterAsync(DocumentVersionReadableModel readableModel)
        {
            await readableModel.LoadReferencePropertiesAsync();
            await readableModel.Document.LoadReferencePropertiesAsync();
            return readableModel.Document.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }
    }
}