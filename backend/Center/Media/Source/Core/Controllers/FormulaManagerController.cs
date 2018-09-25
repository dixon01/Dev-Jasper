// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaManagerController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaManagerController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The FormulaManagerController.
    /// </summary>
    public class FormulaManagerController : IFormulaManagerController
    {
        private readonly IMediaShellController shellController;

        private readonly Lazy<IMediaApplicationState> lazyApplicationState =
            new Lazy<IMediaApplicationState>(GetApplicationState);

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaManagerController"/> class.
        /// </summary>
        /// <param name="shellController">The shell controller.</param>
        /// <param name="mediaShell">The media shell.</param>
        /// <param name="commandRegistry">The command registry.</param>
        public FormulaManagerController(
            IMediaShellController shellController, IMediaShell mediaShell, ICommandRegistry commandRegistry)
        {
            this.shellController = shellController;
            this.MediaShell = mediaShell;
            this.CommandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CreatePredefinedFormula, new RelayCommand(this.CreatePredefinedFormula));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeletePredefinedFormula,
                new RelayCommand<EvaluationConfigDataViewModel>(this.DeletePredefinedFormula));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.UpdatePredefinedFormula,
                new RelayCommand<UpdateEntityParameters>(this.UpdatePredefinedFormula));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.RenamePredefinedFormula,
                new RelayCommand<RenameReusableEntityParameters>(this.RenamePredefinedFormula));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.ClonePredefinedFormula,
                new RelayCommand<EvaluationConfigDataViewModel>(this.ClonePredefinedFormula));
        }

        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        protected ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        protected IMediaApplicationState ApplicationState
        {
            get
            {
                return this.lazyApplicationState.Value;
            }
        }

        private static IMediaApplicationState GetApplicationState()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
        }

        /// <summary>
        /// Creates a new Predefined Formula
        /// </summary>
        private void CreatePredefinedFormula()
        {
            var predefinedFormulas = this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations;

            var newNumber = 1;
            var newName = MediaStrings.ProjectController_NewPredefinedFormulaName + newNumber;
            while (predefinedFormulas.Any(pf => pf.Name.Value == newName))
            {
                newNumber += 1;
                newName = MediaStrings.ProjectController_NewPredefinedFormulaName + newNumber;
            }

            var predefinedFormula = new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = new DataValue<string>(newName),
                DisplayText = newName,
                IsInEditMode = true,
            };

            var historyEntry = new CreatePredefinedFormulaHistoryEntry(
                predefinedFormula,
                predefinedFormulas,
                MediaStrings.ProjectController_CreatePredefinedFormulaHistoryEntry);

            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Update a Predefined Formula
        /// </summary>
        /// <param name="parameters">the parameters</param>
        private void UpdatePredefinedFormula(UpdateEntityParameters parameters)
        {
            var oldElements = parameters.OldElements.ToList();
            var newElements = parameters.NewElements.ToList();
            var elementsContainer = parameters.ElementsContainerReference;

            Action doCallBack = () =>
                {
                    var elements = this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations;
                    var currentElement =
                        elements.FirstOrDefault(
                            e => e.Name.Value == ((EvaluationConfigDataViewModel)newElements.First()).Name.Value);
                    this.shellController.MainMenuPrompt.FormulaManagerPrompt.CurrentEvaluation =
                        currentElement;
                };
            Action undoCallBack = () =>
                {
                    var elements = this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations;
                    var currentElement =
                        elements.FirstOrDefault(
                            e => e.Name.Value == ((EvaluationConfigDataViewModel)oldElements.First()).Name.Value);
                    this.shellController.MainMenuPrompt.FormulaManagerPrompt.CurrentEvaluation = currentElement;
                };
            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                doCallBack,
                undoCallBack,
                MediaStrings.FormulaManagerController_UpdatePredefinedFormulaHistoryEntryLabel,
                this.CommandRegistry);

            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Delete a Predefined Formula
        /// </summary>
        /// <param name="formula">the formula to delete</param>
        private void DeletePredefinedFormula(EvaluationConfigDataViewModel formula)
        {
            var formulas = this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations;

            if (formula.ReferencesCount > 0)
            {
                MessageBox.Show(MediaStrings.FormulaManager_CanNotRemoveFormula);
                return;
            }

            var historyEntry = new DeletePredefinedFormulaHistoryEntry(
                formula,
                formulas,
                MediaStrings.ProjectController_DeletePredefinedFormulaHistoryEntry);

            this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Rename a Predefined Formula
        /// </summary>
        /// <param name="parameters">the parameters</param>
        private void RenamePredefinedFormula(RenameReusableEntityParameters parameters)
        {
            var evaluation = parameters.Entity as EvaluationConfigDataViewModel;
            if (evaluation != null && parameters.NewName != evaluation.Name.Value)
            {
                var historyEntry = new RenamePredefinedFormulaHistoryEntry(
                    evaluation,
                    parameters.NewName,
                    MediaStrings.FormulaManagerController_RenameEvaluationHistoryEntry,
                    this.OnAfterPredefinedFormulaNameChange);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
            }
        }

        /// <summary>
        /// Clone a Predefined Formula
        /// </summary>
        /// <param name="formula">the formula to clone</param>
        private void ClonePredefinedFormula(EvaluationConfigDataViewModel formula)
        {
            var formulas = this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Evaluations;

                var historyEntry = new ClonePredefinedFormulaHistoryEntry(
                    formula,
                    this.MediaShell,
                    formulas,
                    MediaStrings.FormulaManagerController_ClonePredefinedFormulaHistoryEntry);
                this.shellController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void OnAfterPredefinedFormulaNameChange(EvaluationConfigDataViewModel predefinedFormula)
        {
            InteractionManager<UpdateCycleDetailsPrompt>.Current.Raise(new UpdateCycleDetailsPrompt());
            InteractionManager<UpdateLayoutDetailsPrompt>.Current.Raise(new UpdateLayoutDetailsPrompt());
            InteractionManager<UpdateSectionDetailsPrompt>.Current.Raise(new UpdateSectionDetailsPrompt());
            InteractionManager<UpdateResolutionDetailsPrompt>.Current.Raise(new UpdateResolutionDetailsPrompt());
        }
    }
}