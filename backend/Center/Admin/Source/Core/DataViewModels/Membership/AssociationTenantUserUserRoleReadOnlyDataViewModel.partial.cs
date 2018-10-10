// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationTenantUserUserRoleReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssociationTenantUserUserRoleReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Membership
{
    /// <summary>
    /// Partial implementation of <see cref="AssociationTenantUserUserRoleReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class AssociationTenantUserUserRoleReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0}: {1}", this.Tenant, this.UserRole);
        }
    }
}
