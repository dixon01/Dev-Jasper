// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRoleValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserRoleValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.AccessControl
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="UserRoleValidator"/>.
    /// </summary>
    public partial class UserRoleValidator
    {
        partial void ValidateName(UserRoleDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.UserRole.All.Any(ur => ur.Id != dvm.Id && ur.Name.Equals(dvm.Name)));
        }
    }
}
