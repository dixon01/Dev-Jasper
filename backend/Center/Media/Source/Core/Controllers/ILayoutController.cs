// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILayoutController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutController interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The LayoutController interface. It handles all commands for layouts.
    /// </summary>
    public interface ILayoutController
    {
        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        IMediaShell MediaShell { get; }
    }
}