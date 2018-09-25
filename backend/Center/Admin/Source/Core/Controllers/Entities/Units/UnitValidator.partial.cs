// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Units
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.ServiceModel.Filters.Units;

    using NLog;

    /// <summary>
    /// Specific implementation of <see cref="UnitValidator"/>.
    /// </summary>
    public partial class UnitValidator
    {
        private static readonly Regex UnitNameRegex = new Regex("^TFT-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}$");

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        partial void ValidateTenant(UnitDataViewModel dvm)
        {
            dvm.ChangeError("Tenant", AdminStrings.Errors_NoItemSelected, dvm.Tenant.SelectedEntity == null);
        }

        partial void ValidateProductType(UnitDataViewModel dvm)
        {
            dvm.ChangeError("ProductType", AdminStrings.Errors_NoItemSelected, dvm.ProductType.SelectedEntity == null);
        }

        async partial void ValidateName(UnitDataViewModel dvm)
        {
            var nameValid = dvm.Name != null && UnitNameRegex.IsMatch(dvm.Name);
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_ValidUnitName,
                !nameValid);

            dvm.RemoveError("Name", AdminStrings.Errors_DuplicateName);
            if (nameValid)
            {
                try
                {
                    await this.CheckNameIsUniqueAsync(dvm);
                }
                catch (Exception ex)
                {
                    dvm.AddError("Name", AdminStrings.Errors_DuplicateName);
                    Logger.Warn(ex, "Couldn't check unit name");
                }
            }
        }

        private async Task CheckNameIsUniqueAsync(UnitDataViewModel dvm)
        {
            var query = UnitQuery.Create().WithName(dvm.Name);
            var units = await this.DataController.ConnectionController.UnitChangeTrackingManager.QueryAsync(query);
            if (units.Any(u => u.Id != dvm.Id))
            {
                // if it isn't our id, then somebody else has that unit name already
                dvm.AddError("Name", AdminStrings.Errors_DuplicateName);
            }
        }
    }
}