// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Units
{
    using System.Windows;

    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UnitStageController"/>.
    /// </summary>
    public partial class UnitStageController
    {
        /// <summary>
        /// Updates the property display parameters for the given property.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to be modified.
        /// </param>
        public override void UpdatePropertyDisplay(PropertyDisplayParameters parameters)
        {
            if (parameters.PropertyName == "IsConnected")
            {
                parameters.IsVisible = false;
            }
        }

        /// <summary>
        /// Gets the initial column visibility if the user hasn't chosen the visibility yet.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="Visibility"/>.
        /// The following return values have the given meaning:
        /// <see cref="Visibility.Visible"/>: the column is visible and can be hidden by the user.
        /// <see cref="Visibility.Collapsed"/>: the column is hidden and can be shown by the user.
        /// <see cref="Visibility.Hidden"/>: the column is completely removed and can't be selected by the user.
        /// </returns>
        protected override Visibility GetInitialColumnVisibility(string columnName)
        {
            if (columnName == "Tenant")
            {
                // we never want to show the tenant (since we are already filtered by it)
                return Visibility.Hidden;
            }

            return base.GetInitialColumnVisibility(columnName);
        }

        /// <summary>
        /// Updates the permissions of the <see cref="EntityStageControllerBase.StageViewModel"/> with
        /// the permission controller.
        /// Subclasses should check the data scope to set the stage's:
        /// - <see cref="EntityStageViewModelBase.CanCreate"/>
        /// - <see cref="EntityStageViewModelBase.CanRead"/>
        /// - <see cref="EntityStageViewModelBase.CanWrite"/>
        /// - <see cref="EntityStageViewModelBase.CanDelete"/>
        /// </summary>
        protected override void UpdatePermissions()
        {
            this.UpdatePermissions(DataScope.Unit);
        }
    }
}
