// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;

    /// <summary>
    /// Defines the controller used to export the presentation.
    /// </summary>
    public interface IExportController
    {
        /// <summary>
        /// The exported event.
        /// </summary>
        event EventHandler Exported;

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; }
    }
}