// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdminShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAdminShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers
{
    using System;

    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The icenter.admin Shell Controller
    /// </summary>
    public interface IAdminShellController : IWindowController, IShellController, IDisposable
    {
        /// <summary>
        /// Gets the shell.
        /// </summary>
        new IAdminShell Shell { get; }
    }
}