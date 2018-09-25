// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PermissionController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implementation of <see cref="IPermissionController"/>.
    /// </summary>
    public class PermissionController : IPermissionController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AsyncLock locker = new AsyncLock();

        private readonly IConnectionController connectionController;

        private readonly ITimer updateTimer;

        private readonly Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>> tenantAuthorizations =
            new Dictionary<TenantReadableModel, IList<AuthorizationReadableModel>>();

        private AuthorizationReadableModel[] currentAuthorizations;

        private UserReadableModel currentUser;

        private DataScope applicationDataScope;

        private List<DataScope> allDataScopes;

        private TenantReadableModel removeTenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionController"/> class.
        /// </summary>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        public PermissionController(IConnectionController connectionController)
        {
            this.connectionController = connectionController;
            this.currentAuthorizations = new AuthorizationReadableModel[0];

            this.updateTimer = TimerFactory.Current.CreateTimer("PermissionUpdate");
            this.updateTimer.Interval = TimeSpan.FromSeconds(0.5);
            this.updateTimer.AutoReset = false;
            this.updateTimer.Elapsed += this.UpdateTimerOnElapsed;
        }

        private IConnectedApplicationState ApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IConnectedApplicationState>();
            }
        }

        /// <summary>
        /// Loads all permissions with the given <paramref name="allowedDataScopes"/>
        /// for the given <paramref name="user"/>.
        /// This method also ensures that the application state is up-to-date and that it is updated
        /// when something related to permissions changes.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="appDataScope">
        /// The data scope that represents this application.
        /// This is used to figure out if the user has the right to use this application for a given tenant.
        /// </param>
        /// <param name="allowedDataScopes">
        /// Only permissions with allowed data scope are used, never null.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        public async Task LoadPermissionsAsync(User user, DataScope appDataScope, IList<DataScope> allowedDataScopes)
        {
            this.StopListening();

            this.applicationDataScope = appDataScope;
            this.allDataScopes = allowedDataScopes.ToList();
            if (!this.allDataScopes.Contains(appDataScope))
            {
                this.allDataScopes.Add(appDataScope);
            }

            await this.UpdateAuthorizedTenantsAsync(user);

            this.ApplicationState.CurrentUser = user;
            this.UpdateCurrentAuthorizations();
            await this.StartListeningAsync(user);
        }

        /// <summary>
        /// Checks if the current tenant of the application has the <paramref name="desiredPermission"/>
        /// within the <paramref name="scope"/>.
        /// </summary>
        /// <param name="desiredPermission">
        /// The desired permission.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <returns>
        /// <c>true</c> if the permission is granted; <c>false</c> otherwise.
        /// </returns>
        public bool HasPermission(Permission desiredPermission, DataScope scope)
        {
            return this.HasPermission(this.ApplicationState.CurrentTenant, desiredPermission, scope);
        }

        /// <summary>
        /// Checks if the <paramref name="tenant"/> has the <paramref name="desiredPermission"/>
        /// within the <paramref name="scope"/>.
        /// </summary>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <param name="desiredPermission">
        /// The desired permission.
        /// </param>
        /// <param name="scope">
        /// The data scope.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <paramref name="tenant"/> has permission; <c>false</c> otherwise.
        /// </returns>
        public bool HasPermission(TenantReadableModel tenant, Permission desiredPermission, DataScope scope)
        {
            if (this.tenantAuthorizations == null || tenant == null)
            {
                Logger.Trace(
                   "Can't check the permission '{0}' because either the TenantAuthorizations"
                   + " or the tenant is null.",
                   desiredPermission);
                return false;
            }

            IList<AuthorizationReadableModel> authorizations;
            if (tenant.Equals(this.ApplicationState.CurrentTenant))
            {
                // to prevent looking at changed authorizations for the current tenant, just use the one stored
                authorizations = this.currentAuthorizations;
            }
            else
            {
                this.tenantAuthorizations.TryGetValue(tenant, out authorizations);

                if (authorizations == null)
                {
                    Logger.Trace("No authorizations found for tenant '{0}'.", tenant.Name);
                    return false;
                }
            }

            if (authorizations.Any(a => a.Permission == desiredPermission && a.DataScope == scope))
            {
                Logger.Trace(
                    "Permission '{0}' on data scope '{1}' granted for tenant '{2}'",
                    desiredPermission,
                    scope,
                    tenant.Name);
                return true;
            }

            Logger.Trace(
               "Permission '{1}' on data scope '{1}' not granted for tenant '{2}'",
               desiredPermission,
               scope,
               tenant.Name);
            return false;
        }

        /// <summary>
        /// The permission trap.
        /// </summary>
        /// <param name="desiredPermission">
        /// The desired permission.
        /// </param>
        /// <param name="scope">
        /// The scope.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if permission granted else displays an error dialog.
        /// </returns>
        public bool PermissionTrap(Permission desiredPermission, DataScope scope)
        {
            if (!this.HasPermission(desiredPermission, scope))
            {
                Logger.Error("Permission '{0}' requested which was not granted.", desiredPermission);

                MessageBox.Show(
                    Strings.Permission_NotGranted,
                    Strings.Permission_NotGrantedTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private async Task StartListeningAsync(User user)
        {
            this.ApplicationState.PropertyChanged += this.ApplicationStateOnPropertyChanged;

            this.currentUser = this.connectionController.UserChangeTrackingManager.Wrap(user);
            await this.currentUser.LoadNavigationPropertiesAsync();

            this.connectionController.TenantChangeTrackingManager.Added += this.TenantChangeTrackingManagerOnChanged;
            this.connectionController.TenantChangeTrackingManager.Removed += this.TenantChangeTrackingManagerOnChanged;

            this.currentUser.AssociationTenantUserUserRoles.CollectionChanged +=
                this.UserAssociationsOnCollectionChanged;
            foreach (var association in this.currentUser.AssociationTenantUserUserRoles)
            {
                await this.ObserveAssociationAsync(association);
            }
        }

        private void StopListening()
        {
            this.ApplicationState.PropertyChanged -= this.ApplicationStateOnPropertyChanged;
            if (this.currentUser == null)
            {
                return;
            }

            this.connectionController.TenantChangeTrackingManager.Added -= this.TenantChangeTrackingManagerOnChanged;
            this.connectionController.TenantChangeTrackingManager.Removed -= this.TenantChangeTrackingManagerOnChanged;

            this.currentUser.AssociationTenantUserUserRoles.CollectionChanged -=
                this.UserAssociationsOnCollectionChanged;
            foreach (var association in this.currentUser.AssociationTenantUserUserRoles)
            {
                this.UnobserveAssociation(association);
            }

            this.currentUser = null;
        }

        private void UnobserveAssociation(AssociationTenantUserUserRoleReadableModel association)
        {
            association.PropertyChanged -= this.AssociationOnPropertyChanged;
            this.UnobserveUserRole(association.UserRole);
        }

        private async Task ObserveAssociationAsync(AssociationTenantUserUserRoleReadableModel association)
        {
            association.PropertyChanged += this.AssociationOnPropertyChanged;
            await association.LoadReferencePropertiesAsync();
            await this.ObserveUserRoleAsync(association.UserRole);
        }

        private async Task ObserveUserRoleAsync(UserRoleReadableModel userRole)
        {
            await userRole.LoadNavigationPropertiesAsync();
            userRole.Authorizations.CollectionChanged += this.AuthorizationsOnCollectionChanged;
            foreach (var authorization in userRole.Authorizations)
            {
                this.ObserveAuthorization(authorization);
            }
        }

        private void UnobserveUserRole(UserRoleReadableModel userRole)
        {
            userRole.Authorizations.CollectionChanged -= this.AuthorizationsOnCollectionChanged;
            foreach (var authorization in userRole.Authorizations)
            {
                this.UnobserveAuthorization(authorization);
            }
        }

        private void UnobserveAuthorization(AuthorizationReadableModel authorization)
        {
            authorization.PropertyChanged -= this.AuthorizationOnPropertyChanged;
        }

        private void ObserveAuthorization(AuthorizationReadableModel authorization)
        {
            authorization.PropertyChanged += this.AuthorizationOnPropertyChanged;
        }

        private void RestartUpdateTimer()
        {
            this.updateTimer.Enabled = false;
            this.updateTimer.Enabled = true;
        }

        private async Task UpdateAuthorizedTenantsAsync(User user)
        {
            using (await this.locker.LockAsync())
            {
                this.tenantAuthorizations.Clear();

                var associations =
                    await
                    this.connectionController.AssociationTenantUserUserRoleChangeTrackingManager.QueryAsync(
                        AssociationTenantUserUserRoleQuery.Create()
                        .IncludeTenant()
                        .IncludeUserRole(UserRoleFilter.Create().IncludeAuthorizations())
                        .WithUser(user)).ConfigureAwait(false);

                IList<TenantReadableModel> allTenants = null;
                foreach (
                    var association in associations.Where(association => association.UserRole.Authorizations != null))
                {
                    // association is valid for all tenants
                    if (association.Tenant == null)
                    {
                        Logger.Trace("Tenant of an association is null.");

                        if (allTenants == null)
                        {
                            allTenants =
                                (await
                                 this.connectionController.TenantChangeTrackingManager.QueryAsync()
                                     .ConfigureAwait(false)).ToList();
                        }

                        foreach (var tenant in allTenants)
                        {
                            this.AddAuthorizations(tenant, association.UserRole.Authorizations);
                        }
                    }
                    else
                    {
                        this.AddAuthorizations(association.Tenant, association.UserRole.Authorizations);
                    }
                }

                var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
                dispatcher.Dispatch(this.UpdateAuthorizedTenants);
            }
        }

        private void UpdateAuthorizedTenants()
        {
            var tenants =
                this.tenantAuthorizations.Where(
                    ta =>
                    ta.Value.Any(a => a.DataScope == this.applicationDataScope && a.Permission == Permission.Interact))
                    .Select(ta => ta.Key)
                    .OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
                    .ToList();

            this.removeTenant = null;

            // remove all tenants that we don't have anymore
            var state = this.ApplicationState;
            for (int i = state.AuthorizedTenants.Count - 1; i >= 0; i--)
            {
                var tenant = state.AuthorizedTenants[i];
                if (!tenants.Contains(tenant))
                {
                    if (tenant.Equals(state.CurrentTenant))
                    {
                        // don't remove this tenant for now, it should stay in the list
                        this.removeTenant = tenant;
                    }
                    else
                    {
                        state.AuthorizedTenants.RemoveAt(i);
                    }
                }
            }

            if (this.removeTenant != null)
            {
                tenants.Add(this.removeTenant);
                tenants.Sort((a, b) => StringComparer.CurrentCultureIgnoreCase.Compare(a.Name, b.Name));
            }

            // add all newly added tenants
            var index = 0;
            foreach (var tenant in tenants)
            {
                if (index >= state.AuthorizedTenants.Count || !state.AuthorizedTenants[index].Equals(tenant))
                {
                    state.AuthorizedTenants.Insert(index, tenant);
                }

                index++;
            }
        }

        private void AddAuthorizations(
            TenantReadableModel tenant,
            IEnumerable<AuthorizationReadableModel> authorizations)
        {
            IList<AuthorizationReadableModel> authorizationList;
            if (!this.tenantAuthorizations.TryGetValue(tenant, out authorizationList))
            {
                authorizationList = new List<AuthorizationReadableModel>();
                this.tenantAuthorizations[tenant] = authorizationList;
            }

            foreach (var authorization in authorizations)
            {
                if (this.allDataScopes.Contains(authorization.DataScope)
                    && !authorizationList.Any(
                        a => a.DataScope == authorization.DataScope && a.Permission == authorization.Permission))
                {
                    authorizationList.Add(authorization);
                }
            }
        }

        private void UpdateCurrentAuthorizations()
        {
            var tenant = this.ApplicationState.CurrentTenant;
            IList<AuthorizationReadableModel> authorizations;
            if (tenant != null && this.tenantAuthorizations != null
                && this.tenantAuthorizations.TryGetValue(tenant, out authorizations))
            {
                this.currentAuthorizations = authorizations.ToArray();
                return;
            }

            this.currentAuthorizations = new AuthorizationReadableModel[0];
        }

        private async void UpdateTimerOnElapsed(object sender, EventArgs e)
        {
            if (this.currentUser == null)
            {
                return;
            }

            try
            {
                await this.UpdateAuthorizedTenantsAsync(this.currentUser.ToDto());
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't update authorizations");
            }
        }

        private void TenantChangeTrackingManagerOnChanged(object sender, ReadableModelEventArgs<TenantReadableModel> e)
        {
            this.RestartUpdateTimer();
        }

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentTenant")
            {
                return;
            }

            this.UpdateCurrentAuthorizations();
            if (this.removeTenant == null || this.ApplicationState.CurrentTenant.Equals(this.removeTenant))
            {
                return;
            }

            // remove the tenant only when we deselected it
            this.ApplicationState.AuthorizedTenants.Remove(this.removeTenant);
            this.removeTenant = null;
        }

        private async void UserAssociationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var association = (AssociationTenantUserUserRoleReadableModel)e.NewItems[0];
                    await this.ObserveAssociationAsync(association);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var association = (AssociationTenantUserUserRoleReadableModel)e.OldItems[0];
                    this.UnobserveAssociation(association);
                }

                this.RestartUpdateTimer();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't handle user association change");
            }
        }

        private void AuthorizationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var authorization = (AuthorizationReadableModel)e.NewItems[0];
                    this.ObserveAuthorization(authorization);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var authorization = (AuthorizationReadableModel)e.OldItems[0];
                    this.UnobserveAuthorization(authorization);
                }

                this.RestartUpdateTimer();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't handle user role authorization change");
            }
        }

        private void AuthorizationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RestartUpdateTimer();
        }

        private async void AssociationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var association = sender as AssociationTenantUserUserRoleReadableModel;
            if (association == null)
            {
                return;
            }

            if (e.PropertyName == "Tenant")
            {
                this.RestartUpdateTimer();
                return;
            }

            if (e.PropertyName != "UserRole")
            {
                return;
            }

            try
            {
                // TODO: how can we de-register from the old one (we don't have any reference anymore)?
                await association.LoadReferencePropertiesAsync();
                await this.ObserveUserRoleAsync(association.UserRole);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Coulnd't handle association property change");
            }

            this.RestartUpdateTimer();
        }
    }
}
