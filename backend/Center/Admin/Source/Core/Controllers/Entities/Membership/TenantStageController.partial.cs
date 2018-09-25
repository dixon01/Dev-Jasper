// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantStageController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TenantController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="TenantStageController"/>.
    /// </summary>
    public partial class TenantStageController
    {
        /// <summary>
        /// Checks if the given data view model can be deleted.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to delete.
        /// </param>
        /// <returns>
        /// True if it can be deleted, false otherwise.
        /// </returns>
        public override bool CanDeleteEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            var tenant = dataViewModel as TenantReadOnlyDataViewModel;
            if (tenant == null)
            {
                return false;
            }

            var state = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
            return state.CurrentTenant != null && tenant.Id != state.CurrentTenant.Id
                   && tenant.Name != CommonNames.AdminTenantName;
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
            this.UpdatePermissions(DataScope.Tenant);
        }
    }
}