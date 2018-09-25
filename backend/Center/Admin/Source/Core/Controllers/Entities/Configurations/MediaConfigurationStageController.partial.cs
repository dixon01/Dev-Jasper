// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfigurationStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaConfigurationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Configurations
{
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="MediaConfigurationStageController"/>.
    /// </summary>
    public partial class MediaConfigurationStageController
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
            this.UpdatePermissions(DataScope.MediaConfiguration);

            // we will never be able to create or delete icenter.media configurations through icenter.admin
            this.StageViewModel.CanCreate = false;
            this.StageViewModel.CanDelete = false;
        }
    }
}