// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Resources
{
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="ResourceStageController"/>.
    /// </summary>
    public partial class ResourceStageController
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
            this.UpdatePermissions(DataScope.Resource);

            // resources are never edited by icenter.admin directly (but they can be deleted)
            this.StageViewModel.CanCreate = false;
            this.StageViewModel.CanWrite = false;
        }
    }
}
