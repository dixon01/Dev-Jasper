// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using System.Windows;

    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="DocumentStageController"/>.
    /// </summary>
    public partial class DocumentStageController
    {
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
            var applicationController = ServiceLocator.Current.GetInstance<IAdminApplicationController>();
            this.StageViewModel.CanCreate = false;
            this.StageViewModel.CanRead = applicationController.PermissionController.HasPermission(
                Permission.Read,
                DataScope.UnitConfiguration)
                                          || applicationController.PermissionController.HasPermission(
                                              Permission.Read,
                                              DataScope.MediaConfiguration);
            this.StageViewModel.CanWrite = applicationController.PermissionController.HasPermission(
                Permission.Write,
                DataScope.UnitConfiguration)
                                          || applicationController.PermissionController.HasPermission(
                                              Permission.Write,
                                              DataScope.MediaConfiguration);
            this.StageViewModel.CanDelete = applicationController.PermissionController.HasPermission(
                Permission.Delete,
                DataScope.UnitConfiguration)
                                          || applicationController.PermissionController.HasPermission(
                                              Permission.Delete,
                                              DataScope.MediaConfiguration);
        }
    }
}
