// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Configurations;
    using Gorba.Center.Common.ServiceModel.Filters.Documents;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Common.Wpf.Framework.Services;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Options;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the <see cref="IApplicationController"/> specific for Media.
    /// </summary>
    [Export(typeof(IMediaApplicationController))]
    [Export]
    internal class MediaApplicationController : ClientApplicationControllerBase, IMediaApplicationController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool stateSaved;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaApplicationController"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// the command registry
        /// </param>
        [ImportingConstructor]
        public MediaApplicationController(ICommandRegistry commandRegistry)
            : base(commandRegistry, "icenter.media", DataScope.CenterMedia)
        {
            commandRegistry.RegisterCommand(
                ClientCommandCompositionKeys.Application.Exit,
                new RelayCommand(this.Shutdown));
        }

        /// <summary>
        /// The project added.
        /// </summary>
        public event EventHandler<EventArgs> ProjectAdded;

        /// <summary>
        /// The projects loaded.
        /// </summary>
        public event EventHandler<EventArgs> ProjectsLoaded;

        /// <summary>
        /// Gets or sets the shell controller.
        /// </summary>
        /// <value>
        /// The shell controller.
        /// </value>
        [Import]
        public IMediaShellController ShellController { get; set; }

        /// <summary>
        /// Gets the application icon shown on the login and tenant selection dialogs.
        /// </summary>
        protected override ImageSource ApplicationIcon
        {
            get
            {
                return
                    new BitmapImage(
                        new Uri(@"pack://application:,,,/Gorba.Center.Media.Core;component/Resources/media.ico"));
            }
        }

        /// <summary>
        /// Gets the data scopes that are allowed in this application.
        /// This list should be fixed and never change over the runtime of an application.
        /// It is used to determine which data scopes have an influence on the selectable tenants.
        /// </summary>
        protected override DataScope[] AllowedDataScopes
        {
            get
            {
                return new[] { DataScope.MediaConfiguration };
            }
        }

        /// <summary>
        /// Runs the controller logic until completed or until the <see cref="ApplicationController.Shutdown"/>.
        /// </summary>
        public override void Run()
        {
            Logger.Info("Running the Media application");
            this.InitializeApplicationState();
            this.CleanupResources();
            InteractionManager<OpenFileDialogInteraction>.SetCurrent(
                new OpenFileDialogInteractionManager(
                    this.ShellController.Shell.MediaApplicationState.LastUsedDirectories));
            InteractionManager<SaveFileDialogInteraction>.SetCurrent(
                new SaveFileDialogInteractionManager(
                    this.ShellController.Shell.MediaApplicationState.LastUsedDirectories));

            this.InitializeLayoutEditorControllers();
            this.InitializeCycleNavigation();
            this.InitializeProjectController();
            this.RunLoginAsync().ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        Logger.ErrorException(
                            "Couldn't run login",
                            t.Exception == null ? null : t.Exception.Flatten());
                        this.Shutdown();
                        return;
                    }

                    if (t.Result == null)
                    {
                        return;
                    }

                    var mediaShell = this.ShellController as MediaShellController;
                    if (mediaShell == null)
                    {
                        throw new Exception("No media shell controller.");
                    }

                    this.RegisterEvents();
                    this.ShellController.Shell.SetMainScreen(this.ConnectedApplicationState);
                    this.ShellController.Show();
                    t.Result.Close();
                    this.SetProjectListBusyIndicator();
                    var parameters = new MenuNavigationParameters
                    {
                        Root = MenuNavigationParameters.MainMenuEntries.FileOpen
                    };
                    this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                        .Execute(parameters);
                    this.GetExistingProjectsAsync().ContinueWith(
                        task =>
                        {
                            this.ShellController.Shell.IsBusy = false;
                            this.ShellController.MainMenuPrompt.ProjectListScreen.ClearBusy();
                            if (task.IsFaulted)
                            {
                                HandleGetProjectsException(task);
                            }
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Gets the existing update groups for the selected tenant.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the update groups could be loaded; <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> GetExistingUpdateGroupsAsync()
        {
            var exportScreen = this.ShellController.MainMenuPrompt.ExportScreen;
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();

            var groups =
                await
                    this.ConnectionController.UpdateGroupChangeTrackingManager.QueryAsync(
                        UpdateGroupQuery.Create().WithTenant(this.ConnectedApplicationState.CurrentTenant.ToDto()))
                        .ConfigureAwait(false);
            var groupsList = groups.ToList();

            var tasks = groupsList.Select(g => g.LoadNavigationPropertiesAsync());
            await Task.WhenAll(tasks);

            dispatcher.Dispatch(() =>
                {
                    exportScreen.UpdateGroups.Clear();
                    foreach (var group in groupsList)
                    {
                        exportScreen.UpdateGroups.Add(new UpdateGroupItemViewModel(group));
                    }
                });

            return true;
        }

        /// <summary>
        /// Gets the existing projects for the selected tenant.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the projects could be loaded; <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> GetExistingProjectsAsync()
        {
            var mediaApplicationState = this.ShellController.Shell.MediaApplicationState;
            if (mediaApplicationState.AllExistingProjects == null)
            {
                return false;
            }

            mediaApplicationState.IsExistingProjectsLoaded = false;
            mediaApplicationState.AllExistingProjects.Clear();
            this.ShellController.MainMenuPrompt.ProjectListScreen.IsRecentProjectListBusy = true;
            if (this.ConnectionController == null)
            {
                return false;
            }

            Logger.Info(
               "Getting existing projects from server {0}",
               ((MediaApplicationState)mediaApplicationState).LastServer);
            var projects =
                (await
                 this.ConnectionController.MediaConfigurationChangeTrackingManager.QueryAsync(
                     MediaConfigurationQuery.Create().IncludeDocument(
                        DocumentFilter.Create().IncludeTenant())).ConfigureAwait(false)).ToList();
            foreach (var tenantReadableModel in mediaApplicationState.AuthorizedTenants)
            {
                mediaApplicationState.AllExistingProjects.Add(
                    tenantReadableModel.Id,
                    new ObservableCollection<MediaConfigurationDataViewModel>());
            }

            var availableProjects = GetAvailableProjects(mediaApplicationState, projects).ToList();
            Logger.Debug("Got all available projects including documents");

            var alreadyLoaded = await this.GetRecentProjectsAsync(projects, mediaApplicationState);
            this.ShellController.MainMenuPrompt.ProjectListScreen.IsRecentProjectListBusy = false;
            Logger.Trace("Loading all remaining projects");
            var distinctProjects = availableProjects.Except(alreadyLoaded);
            await this.GroupProjectsPerTenant(distinctProjects, mediaApplicationState);

            if (mediaApplicationState.CurrentTenant != null
                && mediaApplicationState.AllExistingProjects.ContainsKey(mediaApplicationState.CurrentTenant.Id))
            {
                mediaApplicationState.ExistingProjects =
                    mediaApplicationState.AllExistingProjects[mediaApplicationState.CurrentTenant.Id];
            }

            this.ShellController.MainMenuPrompt.ProjectListScreen.IsBusy = false;
            mediaApplicationState.IsExistingProjectsLoaded = true;
            Logger.Info("All existing projects loaded");
            this.RaiseProjectsLoaded(EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Requests the shutdown of this controller.
        /// </summary>
        public override void Shutdown()
        {
            this.CancellationTokenSource.Cancel();

            this.ShellControllerOnWindowClosing(null, new CancelEventArgs());
        }

        /// <summary>
        /// The logout.
        /// </summary>
        public async override void Logout()
        {
            var messageBox = MessageBox.Show(
                MediaStrings.Connection_LogoutConfirmation,
                MediaStrings.Connection_LogoutConfirmationTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (messageBox != MessageBoxResult.Yes)
            {
                return;
            }

            var success = await this.ShellController.ProjectController.EnsureCanExitAsync();
            if (success)
            {
                this.OnLogoutAfterCheckinCompleted();
            }
        }

        /// <summary>
        /// Initializes the layout editor controller.
        /// </summary>
        public void InitializeLayoutEditorControllers()
        {
            var mediaShell = this.ShellController as MediaShellController;
            if (mediaShell != null)
            {
                mediaShell.PostUndoController.Initialize();
                mediaShell.GeneralEditorController.Initialize();
                mediaShell.TftEditorController.Initialize();
                mediaShell.LedEditorController.Initialize();
                mediaShell.AudioEditorController.Initialize();
                return;
            }

            Logger.Warn("MediaShellController not found for initialization of the editor controller.");
        }

        /// <summary>
        /// The initialize project controller.
        /// </summary>
        public void InitializeProjectController()
        {
            var mediaShell = this.ShellController as MediaShellController;
            if (mediaShell != null)
            {
                mediaShell.ProjectController.Initialize();
                return;
            }

            Logger.Warn("MediaShellController not found for initialization of the project controller.");
        }

        /// <summary>
        /// Initializes the cycle navigation view model.
        /// </summary>
        public void InitializeCycleNavigation()
        {
            var mediaShell = this.ShellController.Shell;
            if (mediaShell != null)
            {
                mediaShell.CycleNavigator.Initialize();
            }
        }

        /// <summary>
        /// Saves the application state.
        /// </summary>
        /// <param name="force">
        /// Forces the application to save the state even if it is not dirty.
        /// </param>
        public void SaveState(bool force = false)
        {
            if (this.stateSaved && !force)
            {
                return;
            }

            ApplicationStateManager.Current.Save(
                "Media",
                "MediaApplication",
                this.ShellController.Shell.MediaApplicationState);
            this.stateSaved = true;
        }

        /// <summary>
        /// Initializes the options for media.
        /// </summary>
        /// <param name="state">
        /// The application state.
        /// </param>
        protected override void InitializeApplicationStateOptions(IApplicationState state)
        {
            base.InitializeApplicationStateOptions(state);

            if (state.Options.Categories.Count == 1)
            {
                Logger.Warn(
                    "Only common options found in ApplicationState. Getting default ones from MediaConfiguration.xml.");
                GetMediaApplicationOptionsFromConfiguration(state.Options);
            }
        }

        private static IEnumerable<MediaConfigurationReadableModel> GetAvailableProjects(
            IMediaApplicationState mediaApplicationState,
            IEnumerable<MediaConfigurationReadableModel> projects)
        {
            var authorizedTenantsIds = mediaApplicationState.AuthorizedTenants.Select(model => model.Id).ToList();
            return
                projects.Where(
                    model => model.Document.Tenant != null && authorizedTenantsIds.Contains(model.Document.Tenant.Id));
        }

        private static void HandleGetProjectsException(Task<bool> task)
        {
            var message = MediaStrings.MediaApplicationController_GetProjectsErrorMessage;

            if (task.Exception != null)
            {
                Logger.ErrorException(message, task.Exception == null ? null : task.Exception.Flatten());
                var errorprompt = new ConnectionExceptionPrompt(
                    task.Exception.Flatten(),
                    message,
                    MediaStrings.MediaApplicationController_GetProjectsErrorMessageTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(errorprompt);
            }
        }

        private static void GetMediaApplicationOptionsFromConfiguration(ApplicationOptions options)
        {
            var mediaConfiguration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            var localResourceGroup = new LocalResourceOptionGroup
            {
                RemoveLocalResourcesAfter =
                    mediaConfiguration.ResourceSettings
                    .RemoveLocalResourceAfter
            };
            var resourceCategory = options.Categories.FirstOrDefault(c => c is LocalResourceOptionCategory);
            if (resourceCategory == null)
            {
                resourceCategory = new LocalResourceOptionCategory();
                options.Categories.Add(resourceCategory);
            }

            resourceCategory.Groups.Add(localResourceGroup);
            var rendererCategory = options.Categories.FirstOrDefault(c => c is RendererOptionCategory);
            if (rendererCategory == null)
            {
                rendererCategory = new RendererOptionCategory();
                options.Categories.Add(rendererCategory);
            }

            var rendererGroup = new RendererOptionGroup
            {
                TextMode =
                    mediaConfiguration.DirectXRendererConfig.Text.TextMode
                    .ToString(),
                FontQuality =
                    mediaConfiguration.DirectXRendererConfig.Text
                    .FontQuality.ToString(),
                VideoMode =
                    mediaConfiguration.DirectXRendererConfig.Video.VideoMode
                    .ToString()
            };
            rendererCategory.Groups.Add(rendererGroup);
        }

        private void SetProjectListBusyIndicator()
        {
            var projectListPrompt = this.ShellController.MainMenuPrompt.ProjectListScreen;
            projectListPrompt.IsBusyIndeterminate = true;
            projectListPrompt.RecentProjectBusyContent = MediaStrings.ProjectList_LoadingRecentProjects;
            projectListPrompt.IsRecentProjectListBusy = true;
            projectListPrompt.BusyContentTextFormat = MediaStrings.ProjectList_LoadingServerProjects;
            projectListPrompt.IsBusy = true;
        }

        private async Task<IEnumerable<MediaConfigurationReadableModel>> GetRecentProjectsAsync(
          List<MediaConfigurationReadableModel> projects,
          IMediaApplicationState mediaApplicationState)
        {
            var alreadyLoaded = new List<MediaConfigurationReadableModel>();
            var recentProjects =
              mediaApplicationState.RecentProjects.Where(
                  p =>
                  p.TenantId == mediaApplicationState.CurrentTenant.Id
                  && p.ServerName.Equals(
                      mediaApplicationState.LastServer,
                      StringComparison.InvariantCultureIgnoreCase));

            Logger.Trace("Loading Recent projects");
            foreach (var recentProjectDataViewModel in recentProjects)
            {
                var project =
                    projects.FirstOrDefault(
                        p =>
                        p.Document.Name.Equals(
                            recentProjectDataViewModel.ProjectName,
                            StringComparison.InvariantCultureIgnoreCase));
                if (project == null)
                {
                    continue;
                }

                if (!mediaApplicationState.AllExistingProjects.ContainsKey(project.Document.Tenant.Id))
                {
                    mediaApplicationState.AllExistingProjects.Add(
                        project.Document.Tenant.Id,
                        new ObservableItemCollection<MediaConfigurationDataViewModel>());
                }

                await project.Document.LoadNavigationPropertiesAsync();
                var configuration = new MediaConfigurationDataViewModel(
                    project,
                    this.ShellController.Shell,
                    this.CommandRegistry);
                alreadyLoaded.Add(project);
                mediaApplicationState.AllExistingProjects[project.Document.Tenant.Id].Add(configuration);
                mediaApplicationState.ExistingProjects =
                    mediaApplicationState.AllExistingProjects[project.Document.Tenant.Id];
            }

            var recentProject =
                mediaApplicationState.RecentProjects.FirstOrDefault(
                    p =>
                    !p.IsCheckedIn && p.TenantId == mediaApplicationState.CurrentTenant.Id
                    && p.ServerName.Equals(
                        mediaApplicationState.LastServer,
                        StringComparison.InvariantCultureIgnoreCase));
            if (recentProject != null)
            {
                this.ShellController.Shell.IsBusy = true;
                this.ShellController.Shell.IsBusyIndeterminate = true;
                this.ShellController.Shell.BusyContentTextFormat = MediaStrings.Shell_LoadingPendingProject;
                await this.ShellController.ProjectController.OpenLocalProjectAsync(recentProject.ProjectName);
                this.ShellController.Shell.ClearBusy();
            }

            return alreadyLoaded;
        }

        private async Task GroupProjectsPerTenant(
           IEnumerable<MediaConfigurationReadableModel> availableProjects,
            IMediaApplicationState mediaApplicationState)
        {
            foreach (var project in availableProjects)
            {
                var tenantId = project.Document.Tenant.Id;
                ObservableCollection<MediaConfigurationDataViewModel> list;
                if (!mediaApplicationState.AllExistingProjects.TryGetValue(tenantId, out list))
                {
                    list = new ObservableCollection<MediaConfigurationDataViewModel>();
                    mediaApplicationState.AllExistingProjects.Add(tenantId, list);
                }

                var existing = list.FirstOrDefault(p => p.ReadableModel.Id == project.Id);
                if (existing != null)
                {
                    list.Remove(existing);
                }

                await project.Document.LoadNavigationPropertiesAsync();
                MediaConfigurationDataViewModel configuration;
                if (mediaApplicationState.CurrentTenant != null && tenantId == mediaApplicationState.CurrentTenant.Id)
                {
                    configuration = new MediaConfigurationDataViewModel(
                        project,
                        this.ShellController.Shell,
                        this.CommandRegistry);
                    Logger.Debug("Document version loaded");
                }
                else
                {
                    configuration = new MediaConfigurationDataViewModel(
                        project,
                        this.ShellController.Shell,
                        this.CommandRegistry);
                }

                list.Add(configuration);
            }
        }

        private void OnLogoutAfterCheckinCompleted()
        {
            var stateInfo = this.ShellController.Shell.MediaApplicationState.CurrentProjectState;

            if (stateInfo == ProjectStates.ProjectStates.Modified)
            {
                return;
            }

            if (stateInfo == ProjectStates.ProjectStates.Saved)
            {
                MessageBox.Show(
                    MediaStrings.Project_SavedLocallyMessage,
                    MediaStrings.Project_SavedLocallyTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }

            this.ShellController.Shell.MediaApplicationState.CurrentProject = null;
            base.Logout();

            this.CloseMainWindow();
            this.Run();
        }

        private void RaiseProjectAdded(EventArgs args)
        {
            var handler = this.ProjectAdded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RaiseProjectsLoaded(EventArgs args)
        {
            var handler = this.ProjectsLoaded;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void RegisterEvents()
        {
            this.ConnectionController.MediaConfigurationChangeTrackingManager.Added += this.OnProjectAddedAsync;
            this.ConnectionController.MediaConfigurationChangeTrackingManager.Removed += this.OnProjectRemoved;
            this.ConnectionController.UpdateGroupChangeTrackingManager.Added += this.OnUpdateGroupAdded;
            this.ConnectionController.UpdateGroupChangeTrackingManager.Removed += this.OnUpdateGroupRemovedAsync;
            this.ShellController.WindowClosing += this.ShellControllerOnWindowClosing;
            this.ShellController.WindowClosed += this.ShellControllerOnWindowClosed;
        }

        private void OnProjectRemoved(
            object sender, ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
            if (e.Model == null)
            {
                return;
            }

            var projects = this.ShellController.Shell.MediaApplicationState.AllExistingProjects;
            var projectKeyValuePair =
                projects.FirstOrDefault(
                    p => p.Value != null && p.Value.Any(project => project.ReadableModel.Id == e.Model.Id));

            // Check for default value of key value pair
            if (
                projectKeyValuePair.Equals(
                    new KeyValuePair<int, ObservableCollection<MediaConfigurationDataViewModel>>()))
            {
                return;
            }

            var projectToRemove = projects[projectKeyValuePair.Key].FirstOrDefault(
                p => p.ReadableModel.Id == e.Model.Id);
            if (projectToRemove != null)
            {
                projects[projectKeyValuePair.Key].Remove(projectToRemove);
            }
        }

        private async void OnProjectAddedAsync(object sender, ReadableModelEventArgs<MediaConfigurationReadableModel> e)
        {
            try
            {
                var model = e;
                await model.Model.LoadNavigationPropertiesAsync();
                await model.Model.Document.LoadNavigationPropertiesAsync();
                if (
                    this.ShellController.Shell.MediaApplicationState.AllExistingProjects.ContainsKey(
                        e.Model.Document.Tenant.Id))
                {
                    var addedProject = new MediaConfigurationDataViewModel(
                        model.Model,
                        this.ShellController.Shell,
                        this.CommandRegistry);
                    await addedProject.UndoChangesAsync();
                    this.ShellController.Shell.MediaApplicationState.AllExistingProjects[e.Model.Document.Tenant.Id]
                        .Add(addedProject);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while adding project", exception);
            }

            this.RaiseProjectAdded(EventArgs.Empty);
        }

        private async void OnUpdateGroupRemovedAsync(object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            var exportScreen = this.ShellController.MainMenuPrompt.ExportScreen;
            var updateGroup = exportScreen.UpdateGroups.FirstOrDefault(u => u.Item.Id == e.Model.Id);
            if (updateGroup != null)
            {
                var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
                dispatcher.Dispatch(
                    () => exportScreen.UpdateGroups.Remove(updateGroup));
                return;
            }

            try
            {
                await this.GetExistingUpdateGroupsAsync();
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while trying to get update groups.", exception);
            }
        }

        private async void OnUpdateGroupAdded(object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
        {
            var exportScreen = this.ShellController.MainMenuPrompt.ExportScreen;
            var updateGroup = e.Model;
            await updateGroup.LoadNavigationPropertiesAsync();
            if (updateGroup.Tenant.Id != this.ConnectedApplicationState.CurrentTenant.Id
                || exportScreen.UpdateGroups.Any(g => g.Item.Equals(updateGroup)))
            {
                return;
            }

            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(
                () => exportScreen.UpdateGroups.Add(
                    new UpdateGroupItemViewModel(updateGroup)));
        }

        private void CloseMainWindow()
        {
            this.ShellController.WindowClosing -= this.ShellControllerOnWindowClosing;
            this.ShellController.WindowClosed -= this.ShellControllerOnWindowClosed;
            this.ShellController.Close();
            this.SaveState();
        }

        private async void ShellControllerOnWindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            this.ShutdownStart();

            // this is neccessary to stop the close timer, so the dialog appears in async. Do not remove.
            cancelEventArgs.Cancel = true;

            var success = await this.ShellController.ProjectController.EnsureCanExitAsync();
            if (!success)
            {
                cancelEventArgs.Cancel = true;
                this.ShutdownCancel();
                return;
            }

            if (this.ShellController.Shell.MediaApplicationState.CurrentProjectState
                == ProjectStates.ProjectStates.Saved)
            {
                MessageBox.Show(
                    MediaStrings.Project_SavedLocallyMessage,
                    MediaStrings.Project_SavedLocallyTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
            }

            this.SaveState();
            if (Application.Current == null)
            {
                return;
            }

            this.ShutdownFinalize();
            Application.Current.Shutdown();
        }

        private void ShellControllerOnWindowClosed(object sender, EventArgs eventArgs)
        {
            this.Shutdown();
        }

        private void InitializeApplicationState()
        {
            var state = this.ShellController.Shell.MediaApplicationState;
            state.Initialize(this.ShellController.Shell);
            this.InitializeApplicationStateOptions(state);
            this.stateSaved = false;
        }

        private void CleanupResources()
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            resourceManager.CleanupResources(
                !resourceManager.CheckAvailableDiskSpace() || !resourceManager.CheckUsedDiskSpace(0));
        }

        private void ShutdownStart()
        {
            if (this.ShellController != null)
            {
                this.ShellController.WindowClosed -= this.ShellControllerOnWindowClosed;
                this.ShellController.WindowClosing -= this.ShellControllerOnWindowClosing;
            }
        }

        private void ShutdownCancel()
        {
            if (this.ShellController != null)
            {
                this.ShellController.WindowClosed += this.ShellControllerOnWindowClosed;
                this.ShellController.WindowClosing += this.ShellControllerOnWindowClosing;
            }
        }

        private void ShutdownFinalize()
        {
            base.Shutdown();
        }
    }
}