// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDefinedPropertyValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDefinedPropertyValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Meta
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="UserDefinedPropertyValidator"/>.
    /// </summary>
    public partial class UserDefinedPropertyValidator
    {
        partial void ValidateName(UserDefinedPropertyDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateValue,
                this.DataController.UserDefinedProperty.All.Any(
                    udp => udp.Id != dvm.Id && udp.Name == dvm.Name && udp.OwnerEntity == dvm.OwnerEntity));
        }
    }
}
