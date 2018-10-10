// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMediaShellController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMediaShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The Media Shell Controller
    /// </summary>
    public interface IMediaShellController : IWindowController, IShellController
    {
        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        IMediaApplicationController ParentController { get; }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        new IMediaShell Shell { get; }

        /// <summary>
        /// Gets the project controller.
        /// </summary>
        /// <value>
        /// The project controller.
        /// </value>
        IProjectController ProjectController { get; }

        /// <summary>
        /// Gets the physical screen controller.
        /// </summary>
        IPhysicalScreenController PhysicalScreenController { get; }

        /// <summary>
        /// Gets the formula controller.
        /// </summary>
        /// <value>
        /// The formula controller.
        /// </value>
        IFormulaController FormulaController { get; }

        /// <summary>
        /// Gets or sets the main menu prompt.
        /// </summary>
        MainMenuPrompt MainMenuPrompt { get; set; }

        /// <summary>
        /// Gets or sets the export controller.
        /// </summary>
        IExportController ExportController { get; set; }

        /// <summary>
        /// Gets or sets the import controller.
        /// </summary>
        IImportController ImportController { get; set; }

        /// <summary>
        /// Gets the text replacement controller.
        /// </summary>
        ITextReplacementController TextReplacementController { get; }

        /// <summary>
        /// Gets the text replacement controller.
        /// </summary>
        ICsvMappingController CsvMappingController { get; }

        /// <summary>
        /// Gets the resource controller.
        /// </summary>
        IResourceController ResourceController { get; }

        /// <summary>
        /// Gets or sets the change history controller.
        /// </summary>
        IChangeHistoryController ChangeHistoryController { get; set; }
    }
}