// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Documents;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="DocumentValidator"/>.
    /// </summary>
    public partial class DocumentValidator
    {
        partial void ValidateName(DocumentDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.Document.All.Any(d => d.Id != dvm.Id && d.Name.Equals(dvm.Name)));
        }

        partial void ValidateTenant(DocumentDataViewModel dvm)
        {
            dvm.ChangeError("Tenant", AdminStrings.Errors_NoItemSelected, dvm.Tenant.SelectedEntity == null);
        }
    }
}
