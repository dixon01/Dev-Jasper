// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImportController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the controller used to import a project file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    /// <summary>
    /// Defines the controller used to import a project file.
    /// </summary>
    public interface IImportController
    {
        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        IMediaShellController ParentController { get; set; }
    }
}