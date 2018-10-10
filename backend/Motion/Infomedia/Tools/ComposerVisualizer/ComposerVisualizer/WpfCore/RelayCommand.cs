// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RelayCommand type.
// </summary>
// <remarks>
//   Taken from msdn article: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Defines a command which wraps delegates to execute.
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        /// <summary>
        /// Factory method to get <see cref="Nop"/> commands.
        /// </summary>
        public static readonly Func<ICommand> NopFactory = () => new RelayCommand(() => { }, parameter => false);

        /// <summary>
        /// Fake command that can't be executed and doesn't do anything.
        /// </summary>
        public static readonly ICommand Nop;

        static RelayCommand()
        {
            Nop = new RelayCommand(() => { }, parameter => false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public RelayCommand(Action execute)
            : base(o => execute(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public RelayCommand(Action<object> execute)
            : base(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The predicate to evaluate the possibility to execute the method.</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
            : base(o => execute(), canExecute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The predicate to evaluate the possibility to execute the method.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {
        }
    }
}
