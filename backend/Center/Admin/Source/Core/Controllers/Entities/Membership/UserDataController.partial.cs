// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.Wpf.Client.Controllers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UserDataController"/>.
    /// </summary>
    public partial class UserDataController
    {
        // ReSharper disable RedundantAssignment
        private IPermissionController PermissionController
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;
            }
        }

        partial void CreateFilter(ref UserQuery query)
        {
            // include associations' user roles since they are required for the display string
            query =
                UserQuery.Create()
                    .IncludeAssociationTenantUserUserRoles(
                        AssociationTenantUserUserRoleFilter.Create().IncludeUserRole());
        }

        partial void PostCreateEntity(UserDataViewModel dataViewModel)
        {
            dataViewModel.IsEnabled = true;
        }

        partial void FilterResults(
            ref Func<IEnumerable<UserReadableModel>, Task<IEnumerable<UserReadableModel>>> asyncMethod)
        {
            asyncMethod = this.FilterResultsAsync;
        }

        partial void Filter(ref Func<UserReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void PostSetupReferenceProperties(ref Func<UserDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        partial void PreDeleteEntity(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreDeleteEntityAsync;
        }

        partial void PrePopulateEntityDetails(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PrePopulateEntityDetailsAsync;
        }

        private Task<IEnumerable<UserReadableModel>> FilterResultsAsync(IEnumerable<UserReadableModel> users)
        {
            var permissions = this.PermissionController;
            return
                Task.FromResult(
                    users.Where(u => permissions.HasPermission(u.OwnerTenant, Permission.Read, DataScope.User)));
        }

        private Task<bool> FilterAsync(UserReadableModel user)
        {
            return
                Task.FromResult(
                    this.PermissionController.HasPermission(user.OwnerTenant, Permission.Read, DataScope.User));
        }

        private async Task PostSetupReferencePropertiesAsync(UserDataViewModel dataViewModel)
        {
            dataViewModel.CultureSelection = new SelectionProperty<UserDataViewModel, string, CultureInfo>(
                dataViewModel,
                dvm => dvm.Culture,
                (dvm, value) => dvm.Culture = value,
                CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .OrderBy(c => c.EnglishName, StringComparer.CurrentCultureIgnoreCase),
                c => c.Name,
                c => c.EnglishName);
            dataViewModel.CultureSelection.IsRequired = true;

            dataViewModel.TimeZoneSelection = new SelectionProperty<UserDataViewModel, string, TimeZoneInfo>(
                dataViewModel,
                dvm => dvm.TimeZone,
                (dvm, value) => dvm.TimeZone = value,
                TimeZoneInfo.GetSystemTimeZones().OrderBy(z => z.DisplayName, StringComparer.CurrentCultureIgnoreCase),
                z => z.Id,
                z => z.DisplayName);
            dataViewModel.TimeZoneSelection.IsRequired = true;

            var permissions = this.PermissionController;
            await this.DataController.Tenant.AwaitAllDataAsync();
            var tenants =
                this.DataController.Tenant.All.Where(
                    t => permissions.HasPermission(t.ReadableModel, Permission.Create, DataScope.User));
            dataViewModel.OwnerTenant.Entities =
                new ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>(
                    new ObservableCollection<TenantReadOnlyDataViewModel>(tenants));
        }

        private async Task PreDeleteEntityAsync(UserReadOnlyDataViewModel dataViewModel)
        {
            var readableModel = dataViewModel.ReadableModel;
            await readableModel.LoadNavigationPropertiesAsync();

            // remove all Association-Tenant-User-UserRole objects
            foreach (var association in readableModel.AssociationTenantUserUserRoles.ToList())
            {
                await
                    this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager
                        .DeleteAsync(association);
            }

            // update all resources not to reference the given user anymore
            var resources =
                await
                this.ConnectionController.ResourceChangeTrackingManager.QueryAsync(
                    ResourceQuery.Create().WithUploadingUser(readableModel.ToDto()));
            foreach (var resource in resources)
            {
                var writableResource = resource.ToChangeTrackingModel();
                writableResource.UploadingUser = null;
                writableResource.Commit();
            }

            // update all document versions not to reference the given user anymore
            var versions =
                await
                this.ConnectionController.DocumentVersionChangeTrackingManager.QueryAsync(
                    DocumentVersionQuery.Create().WithCreatingUser(readableModel.ToDto()));
            foreach (var version in versions)
            {
                await version.LoadXmlPropertiesAsync();
                var writableResource = version.ToChangeTrackingModel();
                writableResource.CreatingUser = null;
                writableResource.Commit();
            }
        }

        private async Task PrePopulateEntityDetailsAsync(UserReadOnlyDataViewModel dataViewModel)
        {
            foreach (var association in dataViewModel.ReadableModel.AssociationTenantUserUserRoles)
            {
                await association.LoadReferencePropertiesAsync();
            }
        }
    }
}
