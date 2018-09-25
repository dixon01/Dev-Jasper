// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectListScreen.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ProjectListScreen.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Resources;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.GridView;

    /// <summary>
    /// Interaction logic for ProjectListScreen.xaml
    /// </summary>
    public partial class ProjectListScreen
    {
        /// <summary>
        /// The Position Changed Event
        /// </summary>
        public static readonly RoutedEvent ProjectOpenedEvent = EventManager.RegisterRoutedEvent(
            "ProjectOpened",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ProjectListScreen));

        private bool versionSelectionLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectListScreen"/> class.
        /// </summary>
        public ProjectListScreen()
        {
            this.InitializeComponent();
            this.Loaded += (sender, args) => this.RefreshRecentProjects();
        }

        /// <summary>
        /// The Project Opened Event Accessor
        /// </summary>
        public event RoutedEventHandler ProjectOpened
        {
            add => this.AddHandler(ProjectOpenedEvent, value);

            remove => this.RemoveHandler(ProjectOpenedEvent, value);
        }

        /// <summary>
        /// Gets the open project command wrapper which raises an event after executing the command.
        /// </summary>
        public ICommand OpenProjectCommandWrapper => new RelayCommand(this.OnOpenProject);

        private void OnOpenProject(object obj)
        {
            if (!(this.DataContext is ProjectListPrompt context))
            {
                return;
            }

            if (obj is RecentProjectDataViewModel recentProject)
            {
                var recentMediaConfiguration =
                    context.Shell.MediaApplicationState.ExistingProjects.FirstOrDefault(
                        p => p.Name.Equals(recentProject.ProjectName));
                context.HighlightedProject = recentMediaConfiguration;
            }

            if (context.HighlightedProject != null)
            {
                context.OpenProjectCommand.Execute(context.HighlightedProject);
                this.RaiseProjectOpenedEvent(null, null);
            }
        }

        private void RaiseProjectOpenedEvent(object sender, EventArgs e)
        {
            var context = (ProjectListPrompt)this.DataContext;
            context.HighlightedProject.Loaded -= this.RaiseProjectOpenedEvent;
            var newEventArgs = new RoutedEventArgs(ProjectOpenedEvent);
            this.RaiseEvent(newEventArgs);
        }

        private void RefreshRecentProjects()
        {
            if (!(this.TryFindResource("FilteredRecentProjects") is CollectionViewSource collection))
            {
                return;
            }

            if (collection.Source == null)
            {
                if (this.DataContext is ProjectListPrompt context)
                {
                    collection.Source = context.Shell.MediaApplicationState.RecentProjects;
                }
            }

            collection.View?.Refresh();
        }

        private void FilterRecentProjects(object sender, FilterEventArgs e)
        {
            if (!(e.Item is RecentProjectDataViewModel recentProjectViewModel))
            {
                return;
            }

            var context = (ProjectListPrompt)this.DataContext;

            if (context.Shell.MediaApplicationState.CurrentTenant == null)
            {
                return;
            }

            var serverValid = recentProjectViewModel.ServerName == null
                              || recentProjectViewModel.ServerName.Equals(
                                  context.Shell.MediaApplicationState.LastServer,
                                  StringComparison.InvariantCultureIgnoreCase);
            e.Accepted = recentProjectViewModel.TenantId == context.Shell.MediaApplicationState.CurrentTenant.Id
                         && serverValid;
        }

        private void GridViewOnDataLoaded(object sender, EventArgs e)
        {
            if (!(sender is RadGridView grid))
            {
                return;
            }

            if (!(this.DataContext is ProjectListPrompt viewModel))
            {
                return;
            }

            // workaround for bug with Telerik: when we get the DataLoaded event,
            // the data context hasn't completely changed yet, so let's invoke
            // this a little later
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        grid.Focus();
                        if (viewModel.HighlightedProject == null)
                        {
                            if (grid.Items.Count > 0)
                            {
                                grid.SelectedItem = grid.Items[0];
                            }
                        }
                        else
                        {
                            grid.SelectedItem = viewModel.HighlightedProject;
                        }

                        if (grid.SelectedItem != null)
                        {
                            grid.ScrollIntoView(grid.SelectedItem);
                        }

                        grid.Columns["ProjectName"].IsReadOnly = true;

                        // Todo: Replace previous line with this when reenabling the rename function!
                        ////grid.Columns["ProjectName"].IsReadOnly =
                        ////    !viewModel.Shell.MediaApplicationState.HasPermission(Permission.Write);
                    }));
        }

        private void GridViewOnCellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "ProjectName")
            {
                if (!(this.DataContext is ProjectListPrompt context))
                {
                    return;
                }

                var newValue = e.NewValue.ToString();
                if (string.IsNullOrEmpty(newValue)
                    || (!newValue.Equals(context.HighlightedProject.Name, StringComparison.InvariantCultureIgnoreCase)
                        && context.Shell.MediaApplicationState.ExistingProjects.Any(
                            p => p.Name.Equals(newValue, StringComparison.InvariantCultureIgnoreCase))))
                {
                    e.IsValid = false;
                    e.ErrorMessage = MediaStrings.MainMenu_DuplicateProjectName;
                }
            }
        }

        private void OnDescriptionTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ProjectsGridView.Focus();
        }

        private async void ProjectsGridView_OnLoadingRowDetails(object sender, GridViewRowDetailsEventArgs e)
        {
            if (e.Row.Item is MediaConfigurationDataViewModel project)
            {
                await project.LoadMediaProjectDataViewModelAsync();
            }
        }

        private void VersionSelectionGridViewOnDataLoaded(object sender, EventArgs e)
        {
            var viewModel = (ProjectListPrompt)this.DataContext;
            if (!(sender is RadGridView grid))
            {
                return;
            }

            // workaround for bug with Telerik: when we get the DataLoaded event,
            // the data context hasn't completely changed yet, so let's invoke this a little later
            this.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        grid.Focus();
                        if (viewModel.HighlightedProject == null)
                        {
                            return;
                        }

                        grid.SelectedItem = viewModel.HighlightedProject.CurrentVersion;
                        if (grid.SelectedItem != null)
                        {
                            grid.ScrollIntoView(grid.SelectedItem);
                        }

                        this.versionSelectionLoaded = true;
                    }));
        }

        private async void VersionSelectionGridView_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var grid = (RadGridView)sender;

            // workaround for bug with Telerik: when the selected item changes from code behind, the grid within
            // the dropdown content is not refreshed. We need to refresh it manually
            grid.Rebind();
            if (this.versionSelectionLoaded)
            {
                if (grid.DataContext is MediaConfigurationDataViewModel project)
                {
                    InteractionAction.SkipNextMouseUp = true;
                    project.IsVersionSelectionOpen = false;
                    await project.LoadMediaProjectDataViewModelAsync();
                }
            }
        }

        private async void ProjectsGridViewOnRowDetailsVisibilityChanged(object sender, GridViewRowDetailsEventArgs e)
        {
            if (e.Visibility.HasValue && e.Visibility.Value == Visibility.Visible)
            {
                var prompt = (ProjectListPrompt)this.DataContext;
                await prompt.HighlightedProject.LoadMediaProjectDataViewModelAsync();
            }
        }
    }
}
