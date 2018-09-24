// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdminApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdminApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// Defines the application controller specific for the icenter.admin application.
    /// </summary>
    public interface IAdminApplicationController : IClientApplicationController
    {
        /// <summary>
        /// Gets or sets the shell controller.
        /// </summary>
        /// <value>
        /// The shell controller.
        /// </value>
        IAdminShellController ShellController { get; set; }
    }
}
