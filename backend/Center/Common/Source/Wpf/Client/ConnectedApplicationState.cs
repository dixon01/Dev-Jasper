// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectedApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   ApplicationState that contains the last connection information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Framework.Model;

    /// <summary>
    /// ApplicationState that contains the last connection information.
    /// </summary>
    [DataContract]
    public class ConnectedApplicationState : ApplicationState, IConnectedApplicationState
    {
        private ObservableCollection<string> recentServers;

        private TenantReadableModel currentTenant;

        private User currentUser;

        private MaintenanceModeSettings isMaintenanceMode;

        private bool usedDomainLogin;

        private ObservableCollection<TenantReadableModel> authorizedTenants;

        private bool isOfflineMode;

        private bool isCheckingIn;

        private int checkInProgress;

        private int totalCheckInProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedApplicationState"/> class.
        /// </summary>
        public ConnectedApplicationState()
        {
            this.RecentServers = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets or sets the recent connection URLs.
        /// </summary>
        [DataMember]
        public ObservableCollection<string> RecentServers
        {
            get
            {
                return this.recentServers;
            }

            set
            {
                this.SetProperty(ref this.recentServers, value, () => this.RecentServers);
            }
        }

        /// <summary>
        /// Gets or sets the user name of the last login.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the last connected server.
        /// </summary>
        [DataMember]
        public string LastServer { get; set; }

        /// <summary>
        /// Gets or sets the authentication for the current user.
        /// This is used to automatically re-login a user after an update.
        /// </summary>
        [DataMember]
        public string Authentication { get; set; }

        /// <summary>
        /// Gets or sets the last user id.
        /// </summary>
        [DataMember]
        public int LastUserId { get; set; }

        /// <summary>
        /// Gets or sets the last tenant id.
        /// </summary>
        [DataMember]
        public int LastTenantId { get; set; }

        /// <summary>
        /// Gets the authorized tenants.
        /// </summary>
        public ObservableCollection<TenantReadableModel> AuthorizedTenants
        {
            get
            {
                return this.authorizedTenants
                       ?? (this.authorizedTenants = new ObservableCollection<TenantReadableModel>());
            }
        }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                if (this.SetProperty(ref this.currentUser, value, () => this.CurrentUser) && value != null)
                {
                    this.LastUserId = value.Id;
                    this.UserName = value.Username;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current tenant.
        /// </summary>
        public TenantReadableModel CurrentTenant
        {
            get
            {
                return this.currentTenant;
            }

            set
            {
                this.ConfirmTenantChange(
                    value,
                    () =>
                    {
                        if (this.currentTenant != null)
                        {
                            this.LastTenantId = this.currentTenant.Id;
                        }

                        this.SetProperty(ref this.currentTenant, value, () => this.CurrentTenant);
                    });
            }
        }

        /// <summary>
        /// Gets or sets the background system maintenance mode configuration.
        /// </summary>
        public MaintenanceModeSettings MaintenanceMode
        {
            get
            {
                return this.isMaintenanceMode;
            }

            set
            {
                this.SetProperty(ref this.isMaintenanceMode, value, () => this.MaintenanceMode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether used domain login.
        /// </summary>
        public bool UsedDomainLogin
        {
            get
            {
                return this.usedDomainLogin;
            }

            set
            {
                this.SetProperty(ref this.usedDomainLogin, value, () => this.UsedDomainLogin);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is in offline mode.
        /// </summary>
        public bool IsOfflineMode
        {
            get
            {
                return this.isOfflineMode;
            }

            set
            {
                this.SetProperty(ref this.isOfflineMode, value, () => this.IsOfflineMode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is checking in a document.
        /// </summary>
        public bool IsCheckingIn
        {
            get
            {
                return this.isCheckingIn;
            }

            set
            {
                this.SetProperty(ref this.isCheckingIn, value, () => this.IsCheckingIn);
            }
        }

        /// <summary>
        /// Gets or sets the check in progress.
        /// </summary>
        public int CheckInProgress
        {
            get
            {
                return this.checkInProgress;
            }

            set
            {
                this.SetProperty(ref this.checkInProgress, value, () => this.CheckInProgress);
            }
        }

        /// <summary>
        /// Gets or sets the total check in progress.
        /// </summary>
        public int TotalCheckInProgress
        {
            get
            {
                return this.totalCheckInProgress;
            }

            set
            {
                this.SetProperty(ref this.totalCheckInProgress, value, () => this.TotalCheckInProgress);
            }
        }

        /// <summary>
        /// Confirms that the tenant can be changed.
        /// </summary>
        /// <param name="tenant">
        /// The tenant to change to.
        /// </param>
        /// <param name="continuation">
        /// The continuation.
        /// </param>
        /// <remarks>
        /// If not overridden, this method always returns <c>true</c>.
        /// </remarks>
        protected virtual void ConfirmTenantChange(TenantReadableModel tenant, Action continuation)
        {
            continuation();
        }
    }
}
