// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayerEditorController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayerEditorController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The layer editor controller.
    /// </summary>
    public class LayerEditorController : ILayerEditorController
    {
        private readonly MediaShellController mediaShellController;
        private readonly ICommandRegistry commandRegistry;

        private IMediaShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerEditorController"/> class.
        /// </summary>
        /// <param name="mediaShellController">
        /// The media shell controller.
        /// </param>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public LayerEditorController(
            MediaShellController mediaShellController,
            IMediaShell shell,
            ICommandRegistry commandRegistry)
        {
            this.mediaShellController = mediaShellController;
            this.shell = shell;
            this.commandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
               CommandCompositionKeys.Shell.Layout.RenameLayoutElement,
               new RelayCommand<UpdateEntityParameters>(this.RenameLayoutElement));
        }

        /// <summary>
        /// The rename layout element.
        /// </summary>
        /// <param name="parameters">
        /// The parameter.
        /// </param>
        public void RenameLayoutElement(UpdateEntityParameters parameters)
        {
            var oldElements = parameters.OldElements.ToList();
            var newElements = parameters.NewElements.ToList();
            var elementsContainer = parameters.ElementsContainerReference;

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                MediaStrings.LayerEditor_RenameLayoutElementHistoryEntry,
                this.commandRegistry);

            this.mediaShellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }
    }
}