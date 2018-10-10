// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentVersionController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="DocumentVersionStageController"/>.
    /// </summary>
    public partial class DocumentVersionStageController
    {
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
            this.StageViewModel.CanRead =
                applicationController.PermissionController.HasPermission(
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