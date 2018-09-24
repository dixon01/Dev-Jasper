// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IClientApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The ClientApplicationController interface.
    /// </summary>
    public interface IClientApplicationController : IApplicationController
    {
        /// <summary>
        /// Gets the login controller.
        /// </summary>
        ILoginController LoginController { get; }

        /// <summary>
        /// Gets the tenant selection controller.
        /// </summary>
        ITenantSelectionController TenantSelectionController { get; }

        /// <summary>
        /// Gets the update progress controller.
        /// </summary>
        IUpdateProgressController UpdateProgressController { get; }

        /// <summary>
        /// Gets the connection controller.
        /// </summary>
        IConnectionController ConnectionController { get; }

        /// <summary>
        /// Gets the permission controller.
        /// </summary>
        IPermissionController PermissionController { get; }

        /// <summary>
        /// The logout.
        /// </summary>
        void Logout();
    }
}