// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminShellWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AdminShellWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Views;

    using SystemCommands = Microsoft.Windows.Shell.SystemCommands;

    /// <summary>
    /// Interaction logic for AdminShellWindow.xaml
    /// </summary>
    public partial class AdminShellWindow : IShellWindowView
    {
        private GridLength editorWidth = GridLength.Auto;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminShellWindow"/> class.
        /// </summary>
        public AdminShellWindow()
        {
            this.MenuItems.Clear();
            this.InitializeComponent();

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
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize
                           || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
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

        private void GridSplitterOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var gridSplitter = sender as GridSplitter;
            if (gridSplitter == null)
            {
                return;
            }

            if (!gridSplitter.IsVisible)
            {
                this.editorWidth = this.Grid.ColumnDefinitions[3].Width;
                this.Grid.ColumnDefinitions[3].Width = GridLength.Auto;
            }
            else
            {
                this.Grid.ColumnDefinitions[3].Width = this.editorWidth;
            }
        }
    }
}
