// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="">
//   
// </copyright>
// <summary>
//   The delegate command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.Common.MVVM.ViewModelHelpers
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute)
            : this(execute, null) { }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this._execute = execute;
            this._canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter) { return this._canExecute == null ? true : this._canExecute(parameter); }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) { this._execute(parameter); }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}