// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implements the <see cref="IImportController"/>.
    /// </summary>
    public class ImportController : ControllerBase, IImportController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportController"/> class.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ImportController(
            IMediaShellController parentController,
            ICommandRegistry commandRegistry)
        {
            this.ParentController = parentController;
            this.commandRegistry = commandRegistry;
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.Import,
                new RelayCommand(this.ImportAsync, this.CanImport));
        }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; set; }

        private async void ImportAsync()
        {
            if (!this.ParentController.ParentController.PermissionController.PermissionTrap(
                Permission.Create,
                DataScope.MediaConfiguration))
            {
                return;
            }

            var success = await this.ParentController.ProjectController.EnsureCheckInOrResetAsync();
            if (success)
            {
                await this.ExecuteImportAsync();
            }
        }

        private async Task ExecuteImportAsync()
        {
            if (!File.Exists(this.ParentController.MainMenuPrompt.ImportFilePath))
            {
                var message = string.Format(
                    MediaStrings.ImportMenu_ImportFileNotFound,
                    this.ParentController.MainMenuPrompt.ImportFilePath);
                var prompt = new ConnectionExceptionPrompt(
                    null,
                    message,
                    MediaStrings.ImportMenu_ImportFileNotFoundTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                return;
            }

            var mainMenuPrompt = this.ParentController.MainMenuPrompt;
            mainMenuPrompt.BusyContentTextFormat = MediaStrings.ImportMenu_BusyIndicatorText;
            mainMenuPrompt.IsBusy = true;
            Logger.Info("Importing project from '{0}'", mainMenuPrompt.ImportFilePath);
            try
            {
                await this.LoadProjectFile(mainMenuPrompt.ImportFilePath);
            }
            catch (Exception e)
            {
                mainMenuPrompt.ImportFilePath = string.Empty;
                var aggregateException = new AggregateException(e);
                var prompt = new ConnectionExceptionPrompt(
                    aggregateException.Flatten(),
                    MediaStrings.ImportMenu_ResourceUploadFailedMessage,
                    MediaStrings.ImportMenu_ResourceUploadeFailedTitle);

                var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();

                dispatcher.Dispatch(
                    () =>
                    {
                        mainMenuPrompt.IsBusy = false;
                        InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                    });

                var message = string.Format("Failed to import project '{0}'", mainMenuPrompt.NewProjectName);
                Logger.ErrorException(message, aggregateException.Flatten());
                mainMenuPrompt.NewProjectName = string.Empty;
                mainMenuPrompt.CurrentBusyProgress = 0;
                mainMenuPrompt.TotalBusyProgress = 0;
                mainMenuPrompt.BusyContentTextFormat = string.Empty;
                return;
            }

            mainMenuPrompt.ImportFilePath = string.Empty;
            mainMenuPrompt.NewProjectName = string.Empty;

            if (mainMenuPrompt.IsOpenAfterImport)
            {
                Logger.Debug("User requested opening project after import.");
                this.ParentController.ParentController.ProjectAdded += this.OnProjectAdded;
            }
            else
            {
                mainMenuPrompt.IsBusy = false;
                mainMenuPrompt.BusyContentTextFormat = string.Empty;
                mainMenuPrompt.CurrentBusyProgress = 0;
                mainMenuPrompt.TotalBusyProgress = 0;
            }

            Logger.Info("Project successfully imported.");
        }

        private void OnProjectAdded(object sender, EventArgs args)
        {
            this.ParentController.ParentController.ProjectAdded -= this.OnProjectAdded;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    var parameters = new MenuNavigationParameters
                                     {
                                         Root = MenuNavigationParameters.MainMenuEntries.FileOpen
                                     };
                    this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMainMenu)
                        .Execute(parameters);
                    this.ParentController.MainMenuPrompt.IsBusy = false;
                    this.ParentController.MainMenuPrompt.BusyContentTextFormat = string.Empty;
                    var project = this.ParentController.Shell.MediaApplicationState.ExistingProjects.LastOrDefault();
                    if (project != null)
                    {
                        if (project.MediaProjectDataViewModel == null)
                        {
                            project.Loaded += this.OnMediaConfigurationDataViewModelLoaded;
                        }
                        else
                        {
                            this.commandRegistry.GetCommand(CommandCompositionKeys.Project.Open).Execute(project);
                        }
                    }

                    this.ParentController.MainMenuPrompt.CurrentBusyProgress = 0;
                    this.ParentController.MainMenuPrompt.TotalBusyProgress = 0;
                });
        }

        private void OnMediaConfigurationDataViewModelLoaded(object sender, EventArgs eventArgs)
        {
            Application.Current.Dispatcher.Invoke(
                () =>
                    {
                        var project = sender as MediaConfigurationDataViewModel;
                        if (project == null)
                        {
                            return;
                        }

                        project.Loaded -= this.OnMediaConfigurationDataViewModelLoaded;
                        this.commandRegistry.GetCommand(CommandCompositionKeys.Project.Open).Execute(project);
                        this.ParentController.MainMenuPrompt.CurrentBusyProgress = 0;
                        this.ParentController.MainMenuPrompt.TotalBusyProgress = 0;
                    });
        }

        private async Task<MediaProjectDataViewModel> LoadProjectFile(string filename)
        {
            var errors = new List<Exception>();
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            var mediaProject = applicationState.ProjectManager.LoadProject(filename);
            var mediaProjectDataViewModel = new MediaProjectDataViewModel(
                this.ParentController.Shell, this.commandRegistry, mediaProject);
            var existingResourcesQuery =
                await
                this.ParentController.ParentController.ConnectionController.ResourceChangeTrackingManager.QueryAsync();
            var existingResources = existingResourcesQuery.ToList();
            if (this.EnsureCompatibility(mediaProjectDataViewModel))
            {
                // Initial project size (default value for the xml part).
                var projectSize = Settings.Default.ProjectSizeXmlPart;
                this.ParentController.MainMenuPrompt.TotalBusyProgress = mediaProjectDataViewModel.Resources.Count;
                this.ParentController.MainMenuPrompt.CurrentBusyProgress = 0;
                foreach (var resourceInfo in mediaProjectDataViewModel.Resources)
                {
                    this.ParentController.MainMenuPrompt.CurrentBusyProgress++;
                    this.ParentController.MainMenuPrompt.CurrentBusyProgressText =
                        Path.GetFileName(resourceInfo.Filename);
                    var existing = existingResources.FirstOrDefault(r => r.Hash == resourceInfo.Hash);
                    if (existing != null)
                    {
                        resourceInfo.Length = existing.Length;
                        projectSize += existing.Length;
                        continue;
                    }

                    try
                    {
                        var resource =
                            await
                            this.ParentController.ResourceController.UploadResourceAsync(resourceInfo.ToResource());
                        resourceInfo.Length = resource.Length;
                        projectSize += resource.Length;
                    }
                    catch (Exception exception)
                    {
                        var message = string.Format(
                            "Error trying to upload resource from project file '{0}'.",
                            filename);
                        Logger.ErrorException(message, exception);
                        errors.Add(exception);
                    }
                }

                mediaProjectDataViewModel.ProjectSize = projectSize / 1024;
                if (this.ParentController.Shell.MediaApplicationState.CurrentProject != null)
                {
                    this.ParentController.Shell.MediaApplicationState.CurrentProject.IsCheckedIn = true;
                    this.ParentController.Shell.MediaApplicationState.CurrentProject.ClearDirty();
                    this.ParentController.Shell.MediaApplicationState.ClearDirty();
                }

                await this.ParentController.ProjectController.ImportProject(mediaProjectDataViewModel)
                        .ConfigureAwait(false);
            }

            if (errors.Any())
            {
                throw new AggregateException(errors);
            }

            return mediaProjectDataViewModel;
        }

        private bool EnsureCompatibility(MediaProjectDataViewModel project)
        {
            foreach (
                var screen in
                    project.InfomediaConfig.PhysicalScreens.Where(screen => screen.SelectedMasterLayout == null))
            {
                Logger.Trace(
                    "Master layout for physical screen '{0}' not set. Setting the default master layout.",
                    screen.Name);
                screen.SelectedMasterLayout =
                    (MasterLayout)this.ParentController.Shell.MediaApplicationState.DefaultMasterLayout.Clone();
            }

            this.UpdateCsvMapping(project);

            project.Name = this.ParentController.MainMenuPrompt.NewProjectName;
            project.Description = this.ParentController.MainMenuPrompt.Description;
            return true;
        }

        private bool CanImport(object obj)
        {
            return !string.IsNullOrEmpty(this.ParentController.MainMenuPrompt.ImportFilePath)
                   && string.IsNullOrEmpty(this.ParentController.MainMenuPrompt.Error);
        }
    }
}
