// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AuthorizationValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.AccessControl
{
    using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="AuthorizationValidator"/>.
    /// </summary>
    public partial class AuthorizationValidator
    {
        partial void ValidateUserRole(AuthorizationDataViewModel dvm)
        {
            dvm.ChangeError("UserRole", AdminStrings.Errors_NoItemSelected, dvm.UserRole.SelectedEntity == null);
        }
    }
}
