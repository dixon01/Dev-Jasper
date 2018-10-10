// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectedApplicationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ConnectedApplicationState interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// The ConnectedApplicationState interface.
    /// </summary>
    public interface IConnectedApplicationState : IApplicationState
    {
        /// <summary>
        /// Gets or sets the recent connection URLs.
        /// </summary>
        ObservableCollection<string> RecentServers { get; set; }

        /// <summary>
        /// Gets or sets the user name of the last login.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the last connected server.
        /// </summary>
        string LastServer { get; set; }

        /// <summary>
        /// Gets or sets the authentication for the current user.
        /// This is used to automatically re-login a user after an update.
        /// </summary>
        string Authentication { get; set; }

        /// <summary>
        /// Gets the authorized tenants.
        /// </summary>
        ObservableCollection<TenantReadableModel> AuthorizedTenants { get; }

        /// <summary>
        /// Gets or sets the current tenant.
        /// </summary>
        TenantReadableModel CurrentTenant { get; set; }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets or sets the last user id.
        /// </summary>
        int LastUserId { get; set; }

        /// <summary>
        /// Gets or sets the last tenant id.
        /// </summary>
        int LastTenantId { get; set; }

        /// <summary>
        /// Gets or sets the background system maintenance mode configuration.
        /// </summary>
        MaintenanceModeSettings MaintenanceMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether used domain login.
        /// </summary>
        bool UsedDomainLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is in offline mode.
        /// </summary>
        bool IsOfflineMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is checking in a project.
        /// </summary>
        bool IsCheckingIn { get; set; }

        /// <summary>
        /// Gets or sets the check in progress.
        /// </summary>
        int CheckInProgress { get; set; }

        /// <summary>
        /// Gets or sets the total check in progress.
        /// </summary>
        int TotalCheckInProgress { get; set; }
    }
}
