// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaShellWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Views;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using SystemCommands = Microsoft.Windows.Shell.SystemCommands;

    /// <summary>
    /// Interaction logic for MediaShellWindow.xaml
    /// </summary>
    public partial class MediaShellWindow : IShellWindowView
    {
        private readonly GridLength[] sidebarGridRowHeights;

        private bool cycleNavigationIsExpanded;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaShellWindow" /> class.
        /// </summary>
        public MediaShellWindow()
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
            this.CommandBindings.Add(
                new CommandBinding(SystemCommands.ShowFileMenuCommand, this.ShowFileMenuCommand));
            this.CommandBindings.Add(
                new CommandBinding(SystemCommands.ShowEditMenuCommand, this.ShowEditMenuCommand));
            this.CommandBindings.Add(
                new CommandBinding(SystemCommands.ShowViewMenuCommand, this.ShowViewMenuCommand));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Undo,
                    this.UndoEditMenuCommand,
                    this.OnCanUndo));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Redo,
                    this.RedoEditMenuCommand,
                    this.OnCanRedo));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Cut,
                    this.CutEditMenuCommand,
                    (sender, args) => { args.CanExecute = true; }));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Copy,
                    this.CopyEditMenuCommand,
                    (sender, args) => { args.CanExecute = true; }));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Paste,
                    this.PasteEditMenuCommand,
                    (sender, args) => { args.CanExecute = true; }));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.Delete,
                    this.DeleteEditMenuCommand,
                    (sender, args) => { args.CanExecute = true; }));

            this.CommandBindings.Add(new CommandBinding(
                    ApplicationCommands.SelectAll,
                    this.SelectAllEditMenuCommand,
                    (sender, args) => { args.CanExecute = true; }));
            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.SaveAs, this.SaveAsCommand, this.CanSaveAs));
            DwmDropShadow.DropShadowToWindow(this);

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, this.SaveCommand, this.CanSave));

            this.sidebarGridRowHeights = new GridLength[this.SidebarGrid.RowDefinitions.Count];
            this.sidebarGridRowHeights[0] = this.SidebarGrid.RowDefinitions[0].Height;
            this.sidebarGridRowHeights[2] = this.SidebarGrid.RowDefinitions[2].Height;
            this.LayerEditorWindow.IsVisibleChanged += this.LayerEditorWindowVisibleChanged;
        }

        /// <summary>
        /// opens the cycle navigation
        /// </summary>
        public void OpenCycleNavigation()
        {
            this.OnOpenCycleNavigation(null);
            this.CycleNavigation.SelectCycleNavigationItem();
        }

        /// <summary>
        /// opens the section navigation
        /// </summary>
        public void OpenSectionNavigation()
        {
            this.OnOpenSectionNavigation(null);
            this.CycleNavigation.SelectSectionNavigationItem();
        }

        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var context = (MediaShell)this.DataContext;
            if (context == null || !context.PermissionController.PermissionTrap(
                Permission.Create,
                DataScope.MediaConfiguration))
            {
                return;
            }

            var menuNavigationParameters = new MenuNavigationParameters
                                           {
                                               Root = MenuNavigationParameters.MainMenuEntries.FileSaveAs
                                           };
            context.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                .Execute(menuNavigationParameters);
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var context = (MediaShell)this.DataContext;
            if (context == null)
            {
                return;
            }

            context.SaveProjectCommand.Execute(null);
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            var context = (MediaShell)this.DataContext;
            if (context != null)
            {
                e.CanExecute = context.SaveProjectCommand.CanExecute(null);
            }
        }

        private void CanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            var context = (MediaShell)this.DataContext;
            if (context != null
                && context.PermissionController.HasPermission(Permission.Create, DataScope.MediaConfiguration)
                && context.MediaApplicationState.CurrentProject != null)
            {
                e.CanExecute = true;
                return;
            }

            e.CanExecute = false;
        }

        private void UndoEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).UndoCommand.Execute(e);
        }

        private void RedoEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).RedoCommand.Execute(e);
        }

        private void CutEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).CutCommand.Execute(e);
        }

        private void CopyEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).CopyCommand.Execute(e);
        }

        private void PasteEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).PasteCommand.Execute(e);
        }

        private void DeleteEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).DeleteCommand.Execute(e);
        }

        private void SelectAllEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).SelectAllCommand.Execute(e);
        }

        private void ShowFileMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).ShowMainMenuCommand.Execute(e);
        }

        private void ShowEditMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).ShowEditMenuCommand.Execute(e);
        }

        private void ShowViewMenuCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ((MediaShell)this.DataContext).ShowViewMenuCommand.Execute(e);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
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

        private void OnCanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MediaShell)this.DataContext).UndoCommand.CanExecute(e);
        }

        private void OnCanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((MediaShell)this.DataContext).RedoCommand.CanExecute(e);
        }

        private void ExpandSlideout(bool expand)
        {
            AnimateGridLengthAnimation.AnimateGridRowExpandCollapse(
                this.CycleNavigationSlideOutRowDefinition,
                expand,
                300,
                60,
                60,
                TimeSpan.FromMilliseconds(200));

            this.cycleNavigationIsExpanded = expand;
        }

        private void OnOpenCyclePackageNavigation(CycleNavigation obj)
        {
            if (!this.cycleNavigationIsExpanded)
            {
                this.ExpandSlideout(true);

                Mouse.AddPreviewMouseUpHandler(this, this.OnGlobalMouseUp);
            }
        }

        private void OnOpenCycleNavigation(CycleNavigation obj)
        {
            if (!this.cycleNavigationIsExpanded)
            {
                this.ExpandSlideout(true);

                Mouse.AddPreviewMouseUpHandler(this, this.OnGlobalMouseUp);
            }
        }

        private void OnOpenSectionNavigation(CycleNavigation obj)
        {
            if (!this.cycleNavigationIsExpanded)
            {
                this.ExpandSlideout(true);

                Mouse.AddPreviewMouseUpHandler(this, this.OnGlobalMouseUp);
            }
        }

        private void OnGlobalMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            var cycleNavigationPosition = this.CycleNavigation.TransformToAncestor(this).Transform(new Point(0, 0));
            if (mousePosition.Y < cycleNavigationPosition.Y
                && mousePosition.X < this.CycleDetailsNavigator.ActualWidth)
            {
                Mouse.RemovePreviewMouseUpHandler(this, this.OnGlobalMouseUp);

                this.ExpandSlideout(false);

                ((MediaShell)this.DataContext).CycleNavigator.SelectedNavigation = CycleNavigationSelection.None;
            }
        }

        private void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var left = e.KeyboardDevice.IsKeyDown(Key.Left);
            var right = e.KeyboardDevice.IsKeyDown(Key.Right);
            var up = e.KeyboardDevice.IsKeyDown(Key.Up);
            var down = e.KeyboardDevice.IsKeyDown(Key.Down);
            if (left || right || up || down)
            {
                e.Handled = true;
            }
        }

        private void LayerEditorWindowVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var popupWindow = sender as PopupWindow;
            if (popupWindow == null)
            {
                return;
            }

            var rowIndex = Grid.GetRow(popupWindow);
            var row = this.SidebarGrid.RowDefinitions[rowIndex];
            if (this.LayerEditorWindow.IsVisible)
            {
                row.Height = this.sidebarGridRowHeights[rowIndex];
            }
            else
            {
                this.sidebarGridRowHeights[rowIndex] = row.Height;
                row.Height = GridLength.Auto;
            }
        }
    }
}
