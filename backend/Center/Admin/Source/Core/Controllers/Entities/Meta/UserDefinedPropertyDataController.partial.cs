// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDefinedPropertyDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserDefinedPropertyDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Membership;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Meta;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="UserDefinedPropertyDataController"/>.
    /// </summary>
    public partial class UserDefinedPropertyDataController
    {
        // ReSharper disable once RedundantAssignment
        partial void PostSetupReferenceProperties(ref Func<UserDefinedPropertyDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        private async Task PostSetupReferencePropertiesAsync(UserDefinedPropertyDataViewModel dataViewModel)
        {
            await this.DataController.Tenant.AwaitAllDataAsync();
            dataViewModel.Tenant.Entities = new DynamicTenantsCollection(this.DataController.Tenant.All, dataViewModel);
        }

        private class DynamicTenantsCollection : ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>
        {
            private readonly ObservableCollection<TenantReadOnlyDataViewModel> filteredTenants;

            private readonly List<TenantReadOnlyDataViewModel> allTenants;

            private readonly UserDefinedPropertyDataViewModel dataViewModel;

            public DynamicTenantsCollection(
                IEnumerable<TenantReadOnlyDataViewModel> allTenants,
                UserDefinedPropertyDataViewModel dataViewModel)
                : this(new ObservableCollection<TenantReadOnlyDataViewModel>())
            {
                this.allTenants = allTenants.ToList();
                this.dataViewModel = dataViewModel;
                this.dataViewModel.PropertyChanged += this.DataViewModelOnPropertyChanged;

                this.UpdateFilteredItems();
            }

            private DynamicTenantsCollection(ObservableCollection<TenantReadOnlyDataViewModel> filteredTenants)
                : base(filteredTenants)
            {
                this.filteredTenants = filteredTenants;
            }

            private void UpdateFilteredItems()
            {
                var hasTenants = this.HasTenants();
                if (!hasTenants)
                {
                    this.filteredTenants.Clear();
                    this.dataViewModel.Tenant.SelectedEntity = null;
                    return;
                }

                var permissions =
                    ServiceLocator.Current.GetInstance<IAdminApplicationController>().PermissionController;
                this.filteredTenants.Clear();
                var tenants = new List<TenantReadOnlyDataViewModel>();
                switch (this.dataViewModel.OwnerEntity)
                {
                    case UserDefinedPropertyEnabledEntity.Unit:
                        tenants =
                            this.allTenants.Where(
                                t => permissions.HasPermission(t.ReadableModel, Permission.Write, DataScope.Unit))
                                .ToList();
                        break;
                    case UserDefinedPropertyEnabledEntity.UpdateGroup:
                        tenants =
                            this.allTenants.Where(
                                t => permissions.HasPermission(t.ReadableModel, Permission.Write, DataScope.Update))
                                .ToList();
                        break;
                }

                tenants.ForEach(this.filteredTenants.Add);
            }

            private bool HasTenants()
            {
                switch (this.dataViewModel.OwnerEntity)
                {
                    case UserDefinedPropertyEnabledEntity.Unit:
                    case UserDefinedPropertyEnabledEntity.UpdateGroup:
                        return true;
                    default:
                        return false;
                }
            }

            private void DataViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "OwnerEntity")
                {
                    this.UpdateFilteredItems();
                }
            }
        }
    }
}
