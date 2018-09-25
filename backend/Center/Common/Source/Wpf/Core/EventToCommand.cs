// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventToCommand.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventToCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Helper class to bind commands to events.
    /// </summary>
    /// <remarks>
    /// Taken from <see href="http://krishnabhargav.blogspot.ch/2011/01/wpf-events-to-command-redirection-using.html"/>.
    /// </remarks>
    public class EventToCommand : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Dependency property for the command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(EventToCommand), new UIPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty as the backing store for CommandParameter.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter", typeof(object), typeof(EventToCommand), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }

            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>
        /// The command parameter.
        /// </value>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }

            if (this.Command is RoutedCommand)
            {
                var rc = this.Command as RoutedCommand;
                if (rc.CanExecute(this.CommandParameter, this.AssociatedObject))
                {
                    rc.Execute(this.CommandParameter, this.AssociatedObject);
                }
            }
            else
            {
                if (this.Command.CanExecute(this.CommandParameter))
                {
                    this.Command.Execute(this.CommandParameter);
                }
            }
        }
    }
}