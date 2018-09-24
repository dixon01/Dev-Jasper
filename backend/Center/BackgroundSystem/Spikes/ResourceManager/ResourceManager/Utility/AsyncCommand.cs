// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCommand.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AsyncCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Utility
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Defines an async command.
    /// </summary>
    public class AsyncCommand : ICommand, IDisposable
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

        private readonly Func<bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        /// <param name="completed">
        /// The completed.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public AsyncCommand(
            Action action,
            Func<bool> canExecute = null,
            Action<object> completed = null,
            Action<Exception> error = null)
        {
            this.backgroundWorker.DoWork += (s, e) =>
            {
                CommandManager.InvalidateRequerySuggested();
                action();
            };

            this.backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                if (completed != null && e.Error == null)
                {
                    completed(e.Result);
                }

                if (error != null && e.Error != null)
                {
                    error(e.Error);
                }

                CommandManager.InvalidateRequerySuggested();
            };

            this.canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Cancels this command.
        /// </summary>
        public void Cancel()
        {
            if (!this.backgroundWorker.IsBusy)
            {
                return;
            }

            this.backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null
                       ? !this.backgroundWorker.IsBusy
                       : !this.backgroundWorker.IsBusy && this.canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            this.backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.Dispose();
            }
        }
    }
}