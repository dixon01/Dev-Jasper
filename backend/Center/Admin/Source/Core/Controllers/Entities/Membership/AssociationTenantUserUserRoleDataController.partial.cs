// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationTenantUserUserRoleDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssociationTenantUserUserRoleDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="AssociationTenantUserUserRoleDataController"/>.
    /// </summary>
    public partial class AssociationTenantUserUserRoleDataController
    {
        private User currentUser;

        private bool hasWildcardAuthorization;

        // ReSharper disable RedundantAssignment
        partial void Filter(ref Func<AssociationTenantUserUserRoleReadableModel, Task<bool>> asyncMethod)
        {
            asyncMethod = this.FilterAsync;
        }

        partial void FilterResults(
            ref
                Func<IEnumerable<AssociationTenantUserUserRoleReadableModel>,
                Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>>> asyncMethod)
        {
            asyncMethod = this.FilterResultsAsync;
        }

        private async Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>> FilterResultsAsync(
            IEnumerable<AssociationTenantUserUserRoleReadableModel> models)
        {
            var result = new List<AssociationTenantUserUserRoleReadableModel>();
            foreach (var model in models)
            {
                // FilterAsync is complex, so let's use it to filter the models to show
                if (await this.FilterAsync(model))
                {
                    result.Add(model);
                }
            }

            return result;
        }

        partial void PostCreateEntity(AssociationTenantUserUserRoleDataViewModel dataViewModel)
        {
            var currentTenant = this.ApplicationState.CurrentTenant;
            if (currentTenant == null)
            {
                return;
            }

            if (dataViewModel.Tenant.Entities.Any(t => t.Id == currentTenant.Id))
            {
                dataViewModel.Tenant.SelectedEntity = this.Factory.CreateReadOnly(currentTenant);
            }
        }

        partial void PostSetupReferenceProperties(
            ref Func<AssociationTenantUserUserRoleDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        private async Task<bool> FilterAsync(AssociationTenantUserUserRoleReadableModel readableModel)
        {
            if (readableModel.Tenant == null)
            {
                return await this.GetHasWildcardAuthorizationAsync();
            }

            return readableModel.Tenant.Id.Equals(this.ApplicationState.CurrentTenant.Id);
        }

        private async Task<bool> GetHasWildcardAuthorizationAsync()
        {
            if (object.Equals(this.currentUser, this.ApplicationState.CurrentUser))
            {
                return this.hasWildcardAuthorization;
            }

            this.hasWildcardAuthorization = false;
            var user = this.ConnectionController.UserChangeTrackingManager.Wrap(this.ApplicationState.CurrentUser);
            await user.LoadNavigationPropertiesAsync();
            var associations = user.AssociationTenantUserUserRoles.Where(a => a.Tenant == null).ToList();
            foreach (var association in associations)
            {
                await association.LoadReferencePropertiesAsync();
                await association.UserRole.LoadNavigationPropertiesAsync();
                if (
                    association.UserRole.Authorizations.Any(
                        a => a.DataScope == DataScope.AccessControl && a.Permission == Permission.Read))
                {
                    this.hasWildcardAuthorization = true;
                    break;
                }
            }

            this.currentUser = this.ApplicationState.CurrentUser;
            return this.hasWildcardAuthorization;
        }

        private async Task PostSetupReferencePropertiesAsync(AssociationTenantUserUserRoleDataViewModel dataViewModel)
        {
            var permissions = ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;

            // filter tenants (only tenants for which we can create associations (User->Create) are allowed)
            await this.DataController.Tenant.AwaitAllDataAsync();
            var tenants =
                this.DataController.Tenant.All.Where(
                    t => permissions.HasPermission(t.ReadableModel, Permission.Create, DataScope.User));
            dataViewModel.Tenant.Entities =
                new ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>(
                    new ObservableCollection<TenantReadOnlyDataViewModel>(tenants));

            // decide if null-tenant is allowed
            dataViewModel.Tenant.IsRequired = !(await this.GetHasWildcardAuthorizationAsync());

            // filter user roles (only roles that have equal or less rights are allowed)
            await this.DataController.UserRole.AwaitAllDataAsync();
            var allUserRoles = new List<UserRoleReadOnlyDataViewModel>(this.DataController.UserRole.All);
            foreach (var userRole in allUserRoles)
            {
                await userRole.ReadableModel.LoadNavigationPropertiesAsync();
            }

            dataViewModel.UserRole.Entities = new DynamicUserRolesCollection(allUserRoles, dataViewModel.Tenant);
        }

        private class DynamicUserRolesCollection : ReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel>
        {
            private readonly ObservableCollection<UserRoleReadOnlyDataViewModel> filteredUserRoles;

            private readonly List<UserRoleReadOnlyDataViewModel> allUserRoles;

            private readonly EntityReference<TenantReadOnlyDataViewModel> tenantReference;

            public DynamicUserRolesCollection(
                List<UserRoleReadOnlyDataViewModel> allUserRoles,
                EntityReference<TenantReadOnlyDataViewModel> tenantReference)
                : this(new ObservableCollection<UserRoleReadOnlyDataViewModel>())
            {
                this.allUserRoles = allUserRoles;
                this.tenantReference = tenantReference;
                this.tenantReference.PropertyChanged += this.TenantReferenceOnPropertyChanged;

                this.UpdateFilteredItems();
            }

            private DynamicUserRolesCollection(ObservableCollection<UserRoleReadOnlyDataViewModel> filteredUserRoles)
                : base(filteredUserRoles)
            {
                this.filteredUserRoles = filteredUserRoles;
            }

            private void UpdateFilteredItems()
            {
                var appController = ServiceLocator.Current.GetInstance<IAdminApplicationController>();
                var permissions = appController.PermissionController;

                var tenant = this.tenantReference.SelectedEntity != null
                                 ? this.tenantReference.SelectedEntity.ReadableModel
                                 : appController.ShellController.Shell.AdminApplicationState.CurrentTenant;
                var newRoles =
                    this.allUserRoles.Where(
                        u =>
                        u.ReadableModel.Authorizations.All(
                            a => permissions.HasPermission(tenant, a.Permission, a.DataScope)))
                            .OrderBy(ur => ur.Name, StringComparer.CurrentCultureIgnoreCase)
                            .ToList();

                // remove unavailable roles
                for (var i = this.filteredUserRoles.Count - 1; i >= 0; i--)
                {
                    if (!newRoles.Contains(this.filteredUserRoles[i]))
                    {
                        this.filteredUserRoles.RemoveAt(i);
                    }
                }

                // add newly added roles
                // valid for sorted lists, honors order
                var index = 0;
                foreach (var userRole in newRoles)
                {
                    if (index >= this.filteredUserRoles.Count || !this.filteredUserRoles[index].Equals(userRole))
                    {
                        this.filteredUserRoles.Insert(index, userRole);
                    }

                    index++;
                }
            }

            private void TenantReferenceOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "SelectedEntity")
                {
                    this.UpdateFilteredItems();
                }
            }
        }
    }
}