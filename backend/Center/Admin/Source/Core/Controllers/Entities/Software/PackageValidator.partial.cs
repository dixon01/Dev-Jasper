// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Software
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="PackageValidator"/>.
    /// </summary>
    public partial class PackageValidator
    {
        partial void ValidatePackageId(PackageDataViewModel dvm)
        {
            dvm.ChangeError("PackageId", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.PackageId));
            dvm.ChangeError(
                "PackageId",
                AdminStrings.Errors_DuplicateName,
                this.DataController.Package.All.Any(d => d.Id != dvm.Id && d.PackageId.Equals(dvm.PackageId)));
        }

        partial void ValidateProductName(PackageDataViewModel dvm)
        {
            dvm.ChangeError(
                "ProductName", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.ProductName));
            dvm.ChangeError(
                "ProductName",
                AdminStrings.Errors_DuplicateName,
                this.DataController.Package.All.Any(d => d.Id != dvm.Id && d.ProductName.Equals(dvm.ProductName)));
        }
    }
}
