// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationTenantUserUserRoleValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssociationTenantUserUserRoleValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="AssociationTenantUserUserRoleValidator"/>.
    /// </summary>
    public partial class AssociationTenantUserUserRoleValidator
    {
        partial void ValidateUser(AssociationTenantUserUserRoleDataViewModel dvm)
        {
            dvm.ChangeError("User", AdminStrings.Errors_NoItemSelected, dvm.User.SelectedEntity == null);
        }

        partial void ValidateUserRole(AssociationTenantUserUserRoleDataViewModel dvm)
        {
            dvm.ChangeError("UserRole", AdminStrings.Errors_NoItemSelected, dvm.UserRole.SelectedEntity == null);
        }

        partial void ValidateTenant(AssociationTenantUserUserRoleDataViewModel dvm)
        {
            // special case: tenant is sometimes required, sometimes not (depending on rights)
            dvm.ChangeError(
                "Tenant",
                AdminStrings.Errors_NoItemSelected,
                dvm.Tenant.SelectedEntity == null && dvm.Tenant.IsRequired);
        }
    }
}