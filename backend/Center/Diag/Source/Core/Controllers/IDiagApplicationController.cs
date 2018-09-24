// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IDiagApplicationController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Client.Controllers;

    /// <summary>
    /// Defines the application controller specific for the icenter.diag application.
    /// </summary>
    public interface IDiagApplicationController : IClientApplicationController
    {
        /// <summary>
        /// Gets or sets the shell controller.
        /// </summary>
        /// <value>
        /// The shell controller.
        /// </value>
        IDiagShellController ShellController { get; set; }
    }
}