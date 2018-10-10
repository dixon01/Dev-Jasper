// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="UpdateGroupValidator"/>.
    /// </summary>
    public partial class UpdateGroupValidator
    {
        partial void ValidateName(UpdateGroupDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.UpdateGroup.All.Any(ug => ug.Id != dvm.Id && ug.Name.Equals(dvm.Name)));
        }

        partial void ValidateTenant(UpdateGroupDataViewModel dvm)
        {
            dvm.ChangeError("Tenant", AdminStrings.Errors_NoItemSelected, dvm.Tenant.SelectedEntity == null);
        }
    }
}
