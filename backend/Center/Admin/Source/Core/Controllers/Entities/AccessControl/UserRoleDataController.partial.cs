// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserRoleDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserRoleDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.AccessControl
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;

    /// <summary>
    /// Special implementation for <see cref="UserRoleDataController"/> that fills
    /// and stores the authorizations from <see cref="UserRoleDataViewModel.Authorizations"/>.
    /// </summary>
    public partial class UserRoleDataController
    {
        // ReSharper disable RedundantAssignment
        partial void PostSetupReferenceProperties(ref Func<UserRoleDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        partial void PostSaveEntity(
            ref Func<UserRoleDataViewModel, UserRoleReadableModel, Task<UserRoleReadableModel>> asyncMethod)
        {
            asyncMethod = this.PostSaveEntityAsync;
        }

        private async Task PostSetupReferencePropertiesAsync(UserRoleDataViewModel dataViewModel)
        {
            dataViewModel.Authorizations = new AuthorizationMatrixDataViewModel();
            if (dataViewModel.Model.ReadableModel == null)
            {
                // this is a new user role, so we don't have any authorizations yet
                return;
            }

            var authorizations =
                await this.ConnectionController.AuthorizationChangeTrackingManager.QueryAsync(
                    AuthorizationQuery.Create().WithUserRole(dataViewModel.Model.ReadableModel.ToDto()));
            foreach (var authorization in authorizations)
            {
                var auth =
                    dataViewModel.Authorizations.Authorizations.SelectMany(l => l)
                        .First(a => a.DataScope == authorization.DataScope && a.Permission == authorization.Permission);
                auth.IsChecked = true;
            }
        }

        private async Task<UserRoleReadableModel> PostSaveEntityAsync(
            UserRoleDataViewModel dataViewModel, UserRoleReadableModel readableModel)
        {
            if (dataViewModel.Authorizations == null)
            {
                // oops, something went wrong, let's ignore this model and not save any authorizations
                return readableModel;
            }

            var changeTracking = this.ConnectionController.AuthorizationChangeTrackingManager;
            var authorizations =
                (await changeTracking.QueryAsync(
                    AuthorizationQuery.Create().WithUserRole(readableModel.ToDto()))).ToList();
            foreach (var authorization in dataViewModel.Authorizations.Authorizations.SelectMany(l => l))
            {
                var found =
                    authorizations.FirstOrDefault(
                        a => a.DataScope == authorization.DataScope && a.Permission == authorization.Permission);
                if (!authorization.IsChecked)
                {
                    if (found == null)
                    {
                        // ok, it's not checked and it doesn't exist
                        continue;
                    }

                    // we need to delete this authorization, it's not checked any more
                    await changeTracking.DeleteAsync(found);
                    continue;
                }

                if (found != null)
                {
                    // ok, it's already there
                    continue;
                }

                // this authorization is missing, let's create it
                var auth = changeTracking.Create();
                auth.DataScope = authorization.DataScope;
                auth.Permission = authorization.Permission;
                auth.UserRole = readableModel;
                await changeTracking.CommitAndVerifyAsync(auth);
            }

            return readableModel;
        }
    }
}
