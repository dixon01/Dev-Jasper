// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageVersionValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Software
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Specific implementation of <see cref="PackageVersionValidator"/>.
    /// </summary>
    public partial class PackageVersionValidator
    {
        partial void ValidatePackage(PackageVersionDataViewModel dvm)
        {
            dvm.ChangeError("Package", AdminStrings.Errors_NoItemSelected, dvm.Package.SelectedEntity == null);
        }

        partial void ValidateSoftwareVersion(PackageVersionDataViewModel dvm)
        {
            dvm.ChangeError(
                "SoftwareVersion",
                AdminStrings.Errors_DuplicateValue,
                this.DataController.PackageVersion.All.Any(
                    d => d.Id != dvm.Id
                         && d.SoftwareVersion == dvm.SoftwareVersion
                         && dvm.Package.SelectedEntity != null
                         && d.Package.Id == dvm.Package.SelectedEntity.Id));

            Version version;
            dvm.ChangeError(
                "SoftwareVersion",
                AdminStrings.Errors_ValidVersion,
                !ParserUtil.TryParse(dvm.SoftwareVersion, out version));
        }
    }
}