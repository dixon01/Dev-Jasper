// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RelayCommand type.
// </summary>
// <remarks>
//   Taken from msdn article: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

namespace ProtectedOperationsTester.Core
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Defines a command which wraps delegates to execute.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Stores the action to be executed.
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// Stores the function to evaluate to decide if the command can be executed.
        /// </summary>
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
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
        public bool CanExecute(object parameter)
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
        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}