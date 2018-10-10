// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoProjectState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers.ProjectStates
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Utility.Files;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// No project is open. Also the initial state after application start.
    /// OpenLocalAsync() is called only if on startup a local project exists (which is not checked in)
    /// </summary>
    internal class NoProjectState : StateBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoProjectState"/> class.
        /// </summary>
        /// <param name="mediaShell">
        ///     The media shell
        /// </param>
        /// <param name="commandRegistry">
        ///     The command registry
        /// </param>
        /// <param name="projectController">
        /// The project controller
        /// </param>
        public NoProjectState(
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry,
            IProjectControllerContext projectController)
            : base(mediaShell, commandRegistry, projectController)
        {
            this.StateInfo = ProjectStates.NoProject;
        }

        /// <summary>
        /// The open local async.
        /// </summary>
        /// <param name="projectName">
        ///     The project Name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Development in progress.
        /// </exception>
        public async override Task<IState> OpenLocalAsync(string projectName)
        {
            Logger.Info("Open pending project '{0}'.", projectName);
            var existingConfiguration =
                this.MediaApplicationState.ExistingProjects.FirstOrDefault(p => p.Name.Equals(projectName));
            if (existingConfiguration == null)
            {
                return await this.NoTransitionAsync();
            }

            var hasError = false;
            IFileInfo file = null;
            try
            {
                var filename = this.MediaApplicationState.LastServer.GetValidFileName()
                               + existingConfiguration.Document.GetIdString() + ".rx";
                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                var rootPath = resourceManager.GetLocalProjectsPath();
                try
                {
                    FileSystemManager.Local.GetDirectory(rootPath);
                }
                catch (Exception e)
                {
                    Logger.DebugException("Local projects directory doesn't exist. Loading project from server.", e);
                    hasError = true;
                }

                if (hasError)
                {
                    return await this.NoTransitionAsync();
                }

                var filePath = Path.Combine(rootPath, filename);
                if (!FileSystemManager.Local.TryGetFile(filePath, out file))
                {
                    return await this.NoTransitionAsync();
                }

                var projectDataViewModel = this.DeserializeMediaProjectDataViewModel(file);
                await this.EnsureResourcesLocalAvailableAsync(projectDataViewModel).ContinueWith(
                    t =>
                    {
                        if (!t.Result)
                        {
                            hasError = true;
                            return;
                        }

                        this.ProjectController.ResetProject(projectDataViewModel);
                        this.MediaApplicationState.CurrentProject.IsCheckedIn = false;
                        this.MediaApplicationState.CurrentProject.FilePath = filePath;
                        this.MediaApplicationState.CurrentMediaConfiguration = existingConfiguration;
                        this.MediaShell.SetProjectTitle(projectDataViewModel.Name);
                        this.ProjectController.ParentController.ParentController
                            .InitializeLayoutEditorControllers();
                        this.ProjectController.UpdateRecentProjects(false, false);
                        this.ResourceManager.SetReferencesForProject(projectDataViewModel);
                        this.MediaApplicationState.CurrentProject.ClearDirty();
                        this.MediaApplicationState.ClearDirty();
                        this.ProjectController.ConsistencyChecker.Check();
                        Logger.Info("Pending project opened.");
                        this.ProjectController.NotifyWrapper(
                            new StatusNotification { Title = this.MediaApplicationState.CurrentProject.Name });
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (Exception exception)
            {
                HandleOpenLocalProjectFailed(projectName, exception, file);
                hasError = true;
            }

            if (hasError)
            {
                return await this.NoTransitionAsync();
            }

            // we opened a local file which is not checked in
            return await this.TransitionToAsync(ProjectStates.Saved);
        }

        /// <summary>
        /// Transition to open a project from the server.
        /// </summary>
        /// <param name="mediaConfiguration">
        ///     The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async override Task<IState> OpenFromServerAsync(MediaConfigurationDataViewModel mediaConfiguration)
        {
            var success = await this.ProjectController.ExecuteOpenProject(mediaConfiguration);

            if (success)
            {
                return await this.TransitionToAsync(ProjectStates.CheckedIn);
            }

            return await this.NoTransitionAsync();
        }

        private static void HandleOpenLocalProjectFailed(string projectName, Exception exception, IFileInfo file)
        {
            Logger.ErrorException("Error while trying to open pending project.", exception);
            var failedMessage = string.Format(MediaStrings.ProjectController_OpenPendingFailedMessage, projectName);
            var prompt = new ConnectionExceptionPrompt(
                exception,
                failedMessage,
                MediaStrings.ProjectController_OpenPendingFailedTitle);

            // Delete the broken file after the user closes the prompt, so that the user has the chance to create a
            // copy of the file for analysis before it is deleted.
            Action<ConnectionExceptionPrompt> callback = c =>
                {
                    if (file != null)
                    {
                        Logger.Trace("Deleting local project file '{0}'", file.FullName);
                        File.Delete(file.FullName);
                    }
                };
            InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt, callback);
        }

        private MediaProjectDataViewModel DeserializeMediaProjectDataViewModel(IFileInfo file)
        {
            MediaProjectDataViewModel projectDataViewModel;
            using (var filestream = file.OpenRead())
            {
                var serializer = new XmlSerializer(typeof(MediaProjectDataModel));
                var project = (MediaProjectDataModel)serializer.Deserialize(filestream);
                projectDataViewModel = new MediaProjectDataViewModel(this.MediaShell, this.CommandRegistry, project);
            }

            return projectDataViewModel;
        }

        private async Task<bool> EnsureResourcesLocalAvailableAsync(MediaProjectDataViewModel projectDataViewModel)
        {
            foreach (var resourceInfoDataViewModel in projectDataViewModel.Resources)
            {
                try
                {
                    await this.MediaApplicationState.ProjectManager.GetResourceAsync(resourceInfoDataViewModel.Hash);
                }
                catch (UpdateException updateException)
                {
                    var filename = string.Empty;
                    if (resourceInfoDataViewModel.Filename != null)
                    {
                        filename = Path.GetFileName(resourceInfoDataViewModel.Filename);
                    }

                    var prompt = new ConnectionExceptionPrompt(
                        updateException,
                        string.Format(MediaStrings.Project_Open_ServerResourceNotFoundErrorMessage, filename),
                        MediaStrings.Project_Open_ServerErrorTitle);
                    InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                    return false;
                }
            }

            return true;
        }
    }
}