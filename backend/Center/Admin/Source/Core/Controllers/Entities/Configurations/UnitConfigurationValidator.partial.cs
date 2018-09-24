// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigurationValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigurationValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Configurations
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="UnitConfigurationValidator"/>.
    /// </summary>
    public partial class UnitConfigurationValidator
    {
        partial void ValidateProductType(UnitConfigurationDataViewModel dvm)
        {
            dvm.ChangeError(
                "ProductType",
                AdminStrings.Errors_NoItemSelected,
                !dvm.IsReadOnlyProductType && dvm.ProductType.SelectedEntity == null);
        }

        partial void Validate(string propertyName, UnitConfigurationDataViewModel dvm)
        {
            if (propertyName != "Name" && propertyName != null)
            {
                return;
            }

            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.Document.All.Any(
                    d => !d.Equals(dvm.Document.SelectedEntity) && d.Name.Equals(dvm.Name)));
        }
    }
}
