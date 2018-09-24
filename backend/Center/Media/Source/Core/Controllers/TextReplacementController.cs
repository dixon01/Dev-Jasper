// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextReplacementController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextReplacementController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The text replacement controller.
    /// </summary>
    public class TextReplacementController : ITextReplacementController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextReplacementController"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media Shell.
        /// </param>
        /// <param name="parentController">
        /// The parent Controller.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public TextReplacementController(
            IMediaShell mediaShell,
            IMediaShellController parentController,
            ICommandRegistry commandRegistry)
        {
            this.MediaShell = mediaShell;
            this.ParentController = parentController;
            this.CommandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
                 CommandCompositionKeys.Project.CreateReplacement, new RelayCommand(this.CreateReplacement));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeleteReplacement,
                new RelayCommand<TextualReplacementDataViewModel>(this.DeleteReplacement));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.UpdateReplacement,
                new RelayCommand<UpdateEntityParameters>(this.UpdateReplacement));
        }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; private set; }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; private set; }

        /// <summary>
        /// Creates a new replacement for code conversion.
        /// </summary>
        private void CreateReplacement()
        {
            var replacements = this.MediaShell.MediaApplicationState.CurrentProject.Replacements;

            if (replacements.Count > 98)
            {
                MessageBox.Show(
                    MediaStrings.ProjectController_TooManyReplacements,
                    MediaStrings.ProjectController_TooManyReplacementsTitle);
                return;
            }

            var newNumber = 1;
            while (replacements.Any(r => r.Number.Value == newNumber))
            {
                newNumber += 1;
            }

            var replacement = new TextualReplacementDataViewModel(this.MediaShell, this.CommandRegistry)
            {
                Code = new DataValue<string>(Settings.Default.CodeConversion_DefaultCode),
                Number = new DataValue<int>(newNumber),
            };

            var historyEntry = new CreateReplacementHistoryEntry(replacement, replacements, "Create replacement.");
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Deletes a code conversion replacement
        /// </summary>
        /// <param name="parameter">the parameter</param>
        private void DeleteReplacement(TextualReplacementDataViewModel parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            var replacements = this.MediaShell.MediaApplicationState.CurrentProject.Replacements;
            var historyEntry = new DeleteReplacementHistoryEntry(parameter, replacements, "Delete replacement.");
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Updates a code conversion replacement
        /// </summary>
        /// <param name="parameters">the parameters</param>
        private void UpdateReplacement(UpdateEntityParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var oldElements = parameters.OldElements.ToList();
            var newElements = parameters.NewElements.ToList();
            var elementsContainer = parameters.ElementsContainerReference;

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                MediaStrings.ProjectController_UpdateTextReplacementHistoryEntryLabel,
                this.CommandRegistry);

            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }
    }
}