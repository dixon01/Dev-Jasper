// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the interface to handle permissions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;

    /// <summary>
    /// Defines the interface to handle permissions.
    /// </summary>
    public interface IPermissionController
    {
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
        bool HasPermission(Permission desiredPermission, DataScope scope);

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
        bool HasPermission(TenantReadableModel tenant, Permission desiredPermission, DataScope scope);

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
        bool PermissionTrap(Permission desiredPermission, DataScope scope);

        /// <summary>
        /// Loads all permissions with the given <paramref name="allowedDataScopes"/>
        /// for the given <paramref name="user"/>.
        /// This method also ensures that the application state is up-to-date and that it is updated
        /// when something related to permissions changes.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="applicationDataScope">
        /// The data scope that represents this application.
        /// This is used to figure out if the user has the right to use this application for a given tenant.
        /// </param>
        /// <param name="allowedDataScopes">
        /// Only permissions with allowed data scope are used, never null.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        Task LoadPermissionsAsync(User user, DataScope applicationDataScope, IList<DataScope> allowedDataScopes);
    }
}