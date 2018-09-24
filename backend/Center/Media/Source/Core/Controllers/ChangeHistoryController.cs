// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistoryController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation of the <see cref="IChangeHistoryController"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.History;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.ViewModels;

    using NLog;

    /// <summary>
    /// Implementation of the <see cref="IChangeHistoryController"/>.
    /// </summary>
    public class ChangeHistoryController : IChangeHistoryController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IChangeHistory changeHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeHistoryController"/> class.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ChangeHistoryController(
            IMediaShellController parentController,
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry)
        {
            this.ParentController = parentController;
            this.MediaShell = mediaShell;
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.Undo,
                new RelayCommand(this.Undo, this.CanUndo));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Default.Redo,
                new RelayCommand(this.Redo, this.CanRedo));
        }

        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        public IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Adds the history entry.
        /// </summary>
        /// <param name="historyEntry">The history entry.</param>
        /// <param name="skipInitialDo">
        /// Value indicating if the history entry should skip the execution of "Do" after adding it. Default is false.
        /// </param>
        public void AddHistoryEntry(IHistoryEntry historyEntry, bool skipInitialDo = false)
        {
            this.EnsureChangeHistory();
            this.changeHistory.Add(historyEntry);

            this.MediaShell.MediaApplicationState.MakeDirty();
            this.MediaShell.MediaApplicationState.CurrentProject.IsCheckedIn = false;
            if (!skipInitialDo)
            {
                historyEntry.Do();
            }

            if (historyEntry is MoveLayoutElementsHistoryEntry
                || historyEntry is ResizeLayoutElementsHistoryEntry)
            {
                return;
            }

            this.ParentController.ProjectController.ConsistencyChecker.Check();
        }

        /// <summary>
        /// Adds a save marker to the undo stack.
        /// </summary>
        public void AddSaveMarker()
        {
            this.EnsureChangeHistory();
            this.changeHistory.AddSaveMarker();
        }

        /// <summary>
        /// Resets the <see cref="ChangeHistory"/>.
        /// </summary>
        public void ResetChangeHistory()
        {
            Logger.Info("Creating new ChangeHistory instance");
            this.changeHistory = ChangeHistoryFactory.Current.Create();
        }

        /// <summary>
        /// Ensures the <see cref="changeHistory"/> field contains a reference to a <see cref="ChangeHistory"/>.
        /// </summary>
        private void EnsureChangeHistory()
        {
            if (this.changeHistory != null)
            {
                return;
            }

            Logger.Debug("ChangeHistory null. Resetting it");
            this.ResetChangeHistory();
        }

        private void Undo()
        {
            this.EnsureChangeHistory();
            this.changeHistory.Undo();

            this.ParentController.ProjectController.ConsistencyChecker.Check();
            if (!this.MediaShell.MediaApplicationState.IsDirty)
            {
                this.ParentController.ProjectController.OnUndoToSaveMark();
            }
        }

        private void Redo()
        {
            this.EnsureChangeHistory();
            this.changeHistory.Redo();

            this.ParentController.ProjectController.ConsistencyChecker.Check();
        }

        private bool CanRedo(object obj)
        {
            this.EnsureChangeHistory();
            return this.changeHistory.RedoStack.Count > 0;
        }

        private bool CanUndo(object obj)
        {
            this.EnsureChangeHistory();
            return this.changeHistory.UndoStack.Count > 0;
        }
    }
}
