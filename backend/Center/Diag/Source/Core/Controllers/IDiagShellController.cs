// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IDiagShellController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Controllers
{
    using System;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// The icenter.diag Shell Controller
    /// </summary>
    public interface IDiagShellController : IWindowController, IShellController, IDisposable
    {
        /// <summary>
        /// Gets the shell.
        /// </summary>
        new IDiagShell Shell { get; }

        /// <summary>
        /// Gets a value indicating whether the application is in offline mode.
        /// </summary>
        bool IsOfflineMode { get; }

        /// <summary>
        /// Gets a value indicating whether the current user can interact with a Unit.
        /// </summary>
        bool UserCanInteract { get; }
    }
}