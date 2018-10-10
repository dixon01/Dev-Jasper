// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TenantValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="TenantValidator"/>.
    /// </summary>
    public partial class TenantValidator
    {
        partial void ValidateName(TenantDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.Tenant.All.Any(t => t.Id != dvm.Id && t.Name.Equals(dvm.Name)));
        }
    }
}
