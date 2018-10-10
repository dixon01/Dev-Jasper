// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationTenantUserUserRoleStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssociationTenantUserUserRoleController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System.Windows;

    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    /// <summary>
    /// Partial implementation of the special behavior of the
    /// <see cref="AssociationTenantUserUserRoleStageController"/>.
    /// </summary>
    public partial class AssociationTenantUserUserRoleStageController
    {
        /// <summary>
        /// Initializes this controller with the given data controller.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.StageViewModel.SingularDisplayName = "User to User Role";
            this.StageViewModel.PluralDisplayName = "Users to User Roles";
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
            if (columnName == "Id")
            {
                return Visibility.Collapsed;
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
            this.UpdatePermissions(DataScope.User);
        }
    }
}
