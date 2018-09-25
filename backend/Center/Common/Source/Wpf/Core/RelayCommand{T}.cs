// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines a command which wraps delegates to execute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Defines a command which wraps typed delegates to execute.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the argument to the <see cref="Execute"/> and <see cref="CanExecute"/> methods.
    /// </typeparam>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// Stores the action to be executed.
        /// </summary>
        private readonly Action<T> execute;

        /// <summary>
        /// Stores the function to evaluate to decide if the command can be executed.
        /// </summary>
        private readonly Predicate<T> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The predicate to evaluate the possibility to execute the method.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            Contract.Requires(execute != null, "The action to be executed can't be null.");

            this.execute = execute;
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
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command.  If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        [DebuggerStepThrough]
        public bool CanExecute(T parameter)
        {
            var result = this.canExecute == null || this.canExecute(parameter);
            return result;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        public void Execute(T parameter)
        {
            this.execute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        [DebuggerStepThrough]
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute((T)parameter);
        }
    }
}
