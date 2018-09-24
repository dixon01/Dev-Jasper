// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICsvMappingController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICsvMappingController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The TextReplacementController interface.
    /// </summary>
    public interface ICsvMappingController
    {
        /// <summary>
        /// Gets the command registry.
        /// </summary>
        ICommandRegistry CommandRegistry { get; }

        /// <summary>
        /// Gets the project controller.
        /// </summary>
        IMediaShellController ParentController { get; }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        IMediaShell MediaShell { get; }
    }
}
