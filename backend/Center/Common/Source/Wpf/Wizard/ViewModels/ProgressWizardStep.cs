// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressWizardStep.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProgressWizardStep type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Input;

    using ActiproSoftware.Windows.Controls.Wizard;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the view model for a wizard step that displays the progress of an operation.
    /// </summary>
    /// <typeparam name="T">The type of the view.</typeparam>
    public abstract class ProgressWizardStep<T> : WizardStepBase<T>
        where T : WizardPage, new()
    {
        /// <summary>
        /// The observable action states.
        /// </summary>
        private readonly ObservableCollection<ActionState> actionStates;

        private int completedActions;

        private int totalActionsCount;

        private int currentActionIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressWizardStep&lt;T&gt;"/> class.
        /// </summary>
        protected ProgressWizardStep()
        {
            this.actionStates = new ObservableCollection<ActionState>();
            this.SortCommand = new RelayCommand(this.Sort);
            this.ActionStatesView = CollectionViewSource.GetDefaultView(this.ActionStates);
            this.Reset();
        }

        /// <summary>
        /// Gets the action states already done or in progress.
        /// </summary>
        public ObservableCollection<ActionState> ActionStates
        {
            get
            {
                return this.actionStates;
            }
        }

        /// <summary>
        /// Gets or sets the completed actions.
        /// </summary>
        /// <value>
        /// The completed actions.
        /// </value>
        public int CompletedActions
        {
            get
            {
                return this.completedActions;
            }

            set
            {
                this.SetProperty(ref this.completedActions, value, () => this.CompletedActions);
            }
        }

        /// <summary>
        /// Gets or sets the total number of actions.
        /// </summary>
        public int TotalActionsCount
        {
            get
            {
                return this.totalActionsCount;
            }

            set
            {
                this.SetProperty(ref this.totalActionsCount, value, () => this.TotalActionsCount);
            }
        }

        /// <summary>
        /// Gets the view of action states supporting sorting and filtering.
        /// </summary>
        public ICollectionView ActionStatesView { get; private set; }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public ListSortDirection SortDirection { get; private set; }

        /// <summary>
        /// Adds the specified action state.
        /// </summary>
        /// <param name="actionState">State of the action.</param>
        public void Add(ActionState actionState)
        {
            this.actionStates.Add(actionState);
            this.TotalActionsCount++;
        }

        /// <summary>
        /// Starts the next action.
        /// </summary>
        /// <returns>The state of the new current action, if available; otherwise, <c>null</c>.</returns>
        public ActionState StartNext()
        {
            this.currentActionIndex++;
            if (this.currentActionIndex < this.ActionStates.Count)
            {
                var actionState = this.ActionStates[this.currentActionIndex];
                actionState.State = States.InProgress;
                return actionState;
            }

            return null;
        }

        /// <summary>
        /// Starts the next action.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <returns>The state of the new current action, if available; otherwise, <c>null</c>.</returns>
        public ActionState<TData> StartNext<TData>()
        {
            var actionState = this.StartNext();
            if (actionState == null)
            {
                return null;
            }

            var typedActionState = actionState as ActionState<TData>;
            if (typedActionState == null)
            {
                throw new InvalidDataException();
            }

            return typedActionState;
        }

        /// <summary>
        /// Cancels all the pending actions, including the current one.
        /// </summary>
        public void Cancel()
        {
            while (this.currentActionIndex < this.ActionStates.Count)
            {
                if (this.currentActionIndex >= 0)
                {
                    this.ActionStates[this.currentActionIndex].State = States.Canceled;
                    this.CompletedActions++;
                }

                this.currentActionIndex++;
            }
        }

        /// <summary>
        /// Resets this instance removing all action states.
        /// </summary>
        public void Reset()
        {
            this.currentActionIndex = -1;
            this.ActionStates.Clear();
        }

        /// <summary>
        /// An error occurred during the execution of the current action.
        /// </summary>
        public void Error()
        {
            var state = States.Error;
            while (this.currentActionIndex < this.ActionStates.Count)
            {
                if (this.currentActionIndex >= 0)
                {
                    this.ActionStates[this.currentActionIndex].State = state;
                    state = States.Canceled;
                    this.CompletedActions++;
                }

                this.currentActionIndex++;
            }
        }

        /// <summary>
        /// The current action was successfully completed.
        /// </summary>
        public void Done()
        {
            this.ActionStates[this.currentActionIndex].State = States.Done;
            this.CompletedActions++;
        }

        private void Sort(object parameter)
        {
            var column = parameter as string;
            if (string.IsNullOrWhiteSpace(column))
            {
                // TODO: log
            }

            this.ActionStatesView.SortDescriptions.Clear();
            this.ActionStatesView.SortDescriptions.Add(new SortDescription(column, this.SortDirection));
            switch (this.SortDirection)
            {
                case ListSortDirection.Ascending:
                    this.SortDirection = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    this.SortDirection = ListSortDirection.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}