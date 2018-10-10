// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagShellWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DiagShellWindow.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Views;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    using Microsoft.Windows.Shell;

    using Telerik.Windows.Controls.Docking;

    /// <summary>
    /// Interaction logic for DiagShellWindow.xaml
    /// </summary>
    public partial class DiagShellWindow : IShellWindowView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagShellWindow" /> class.
        /// </summary>
        public DiagShellWindow()
        {
            this.MenuItems.Clear();
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, this.OnCloseWindow));
            this.CommandBindings.Add(
                new CommandBinding(
                    SystemCommands.MaximizeWindowCommand, this.OnMaximizeWindow, this.OnCanResizeWindow));
            this.CommandBindings.Add(
                new CommandBinding(
                    SystemCommands.MinimizeWindowCommand, this.OnMinimizeWindow, this.OnCanMinimizeWindow));
            this.CommandBindings.Add(
                new CommandBinding(SystemCommands.RestoreWindowCommand, this.OnRestoreWindow, this.OnCanResizeWindow));

            DwmDropShadow.DropShadowToWindow(this);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == System.Windows.ResizeMode.CanResize
                           || this.ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != System.Windows.ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void RadDockingOnClose(object sender, StateChangeEventArgs e)
        {
            foreach (var tab in e.Panes.Select(p => p.Content).OfType<UnitTab>())
            {
                tab.Shell.DisconnectUnitCommand.Execute(tab.Unit);
            }
        }
    }
}
