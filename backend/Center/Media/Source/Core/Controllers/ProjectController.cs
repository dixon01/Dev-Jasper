// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Remoting;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Media.Core.Controllers.ProjectStates;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Exceptions;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implementation for the <see cref="IProjectController"/>.
    /// </summary>
    public class ProjectController : ControllerBase, IProjectControllerContext, IShellController
    {
        private const int MaxDegreeOfParallelism = 16;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan MasterSectionDuration = TimeSpan.FromSeconds(86400);

        private static readonly Lazy<IWritableFileSystem> LazyFileSystem = new Lazy<IWritableFileSystem>(GetFileSystem);

        private readonly Lazy<IMediaApplicationState> lazyApplicationState =
            new Lazy<IMediaApplicationState>(GetApplicationState);

        private readonly Lazy<ConsistencyChecker> consistencyChecker =
            new Lazy<ConsistencyChecker>(GetConsistencyChecker);

        private readonly Lazy<CompatibilityChecker> compatibilityChecker =
            new Lazy<CompatibilityChecker>(GetCompatibilityChecker);

        private readonly Lazy<IResourceManager> lazyResourceManager =
            new Lazy<IResourceManager>(() => ServiceLocator.Current.GetInstance<IResourceManager>());

        private SaveAsParameters saveAsParameter;

        private IState currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="parentController">
        /// The parent Controller.
        /// </param>
        /// <param name="mainMenuPrompt">
        /// The main menu prompt.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ProjectController(
            IMediaShell mediaShell,
            IMediaShellController parentController,
            MainMenuPrompt mainMenuPrompt,
            ICommandRegistry commandRegistry)
        {
            this.MediaShell = mediaShell;
            this.MainMenuPrompt = mainMenuPrompt;
            this.CommandRegistry = commandRegistry;
            this.ParentController = parentController;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.New,
                new RelayCommand<CreateProjectParameters>(this.CreateProjectAsync, this.HasCreatePermission));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.Open,
                new RelayCommand<MediaConfigurationDataViewModel>(this.OpenProjectAsync));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.Save,
                new RelayCommand(this.SaveProjectLocal, this.CanSaveProject));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CheckIn,
                new RelayCommand(this.CheckInProject, this.CanCheckIn));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.SaveAs,
                new RelayCommand<SaveAsParameters>(this.CheckInAsProject, this.HasCreatePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.RenameRecentlyUsed,
                new RelayCommand<RenameProjectParameters>(this.RenameProjectInRecentlyUsed));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.PublishDocument,
                new RelayCommand<PublishDocumentWritableModelParameters>(this.PublishDocumentWritableModelAsync));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.CreateMediaPool,
                new RelayCommand(this.CreateMediaPool, this.HasWritePermission));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeleteMediaPool,
                new RelayCommand<PoolConfigDataViewModel>(this.DeleteMediaPool, this.HasWritePermission));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.UpdateResourceListElement,
                new RelayCommand<UpdateEntityParameters>(this.UpdateResourceListElement, this.HasWritePermission));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeleteProject,
                new RelayCommand<MediaConfigurationDataViewModel>(this.DeleteProject, this.HasDeletePermission));

            this.ParentController.ChangeHistoryController.ResetChangeHistory();
        }

        /// <summary>
        /// Gets or sets the media shell.
        /// </summary>
        /// <value>
        /// The media shell.
        /// </value>
        public IMediaShell MediaShell { get; set; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Gets or sets the main menu prompt.
        /// </summary>
        /// <value>
        /// The main menu prompt.
        /// </value>
        public MainMenuPrompt MainMenuPrompt { get; set; }

        /// <summary>
        /// Gets the Consistency Checker
        /// </summary>
        public ConsistencyChecker ConsistencyChecker
        {
            get
            {
                return this.consistencyChecker.Value;
            }
        }

        /// <summary>
        /// Gets the compatibility checker.
        /// </summary>
        public CompatibilityChecker CompatibilityChecker
        {
            get
            {
                return this.compatibilityChecker.Value;
            }
        }

        IShellViewModel IShellController.Shell
        {
            get
            {
                return this.MediaShell;
            }
        }

        /// <summary>
        /// Gets the options controller.
        /// </summary>
        public OptionsController OptionsController { get; private set; }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        public IResourceManager ResourceManager
        {
            get
            {
                return this.lazyResourceManager.Value;
            }
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public IState CurrentState
        {
            get
            {
                return this.currentState;
            }

            private set
            {
                if (this.currentState == value)
                {
                    return;
                }

                this.currentState = value;
                this.MediaShell.MediaApplicationState.CurrentProjectState = this.CurrentState.StateInfo;
            }
        }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        protected static IWritableFileSystem FileSystem
        {
            get
            {
                return LazyFileSystem.Value;
            }
        }

        /// <summary>
        /// Gets the state of the application.
        /// </summary>
        /// <value>
        /// The state of the application.
        /// </value>
        protected IMediaApplicationState ApplicationState
        {
            get
            {
                return this.lazyApplicationState.Value;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            this.CurrentState = new NoProjectState(this.MediaShell, this.CommandRegistry, this);
        }

        /// <summary>
        /// Creates a new media media pool.
        /// </summary>
        public void CreateMediaPool()
        {
            var name = this.GenerateMediaPoolName();
            var baseDirectory = Path.Combine(Settings.Default.PoolExportPath, name);
            var mediaPool = new PoolConfigDataViewModel(this.MediaShell)
            {
                Name = new DataValue<string>(this.GenerateMediaPoolName()),
                BaseDirectory = new DataValue<string>(baseDirectory),
                IsInEditMode = true,
            };

            var historyEntry = new AddListElementHistoryEntry<PoolConfigDataViewModel>(
                this.MediaShell,
                mediaPool,
                this.ApplicationState.CurrentProject.InfomediaConfig.Pools,
                MediaStrings.ProjectController_AddMediaPool);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Deletes the media pool.
        /// </summary>
        /// <param name="mediaPool">
        /// The parameter of the command. It must be a valid <see cref="PoolConfigDataViewModel"/>.
        /// </param>
        public void DeleteMediaPool(PoolConfigDataViewModel mediaPool)
        {
            if (mediaPool.IsUsed)
            {
                MessageBox.Show(MediaStrings.ResourceManager_canNotRemovePool);
                return;
            }

            Action onAfterDelete = () =>
            {
                foreach (var reference in mediaPool.ResourceReferences)
                {
                    this.ParentController.ResourceController.DecrementResourceReferenceCount(reference.Hash);
                }
            };

            Action onAfterUndoDelete = () =>
            {
                foreach (var reference in mediaPool.ResourceReferences)
                {
                    this.ParentController.ResourceController.IncrementResourceReferenceCount(reference.Hash);
                }
            };

            var historyEntry = new DeleteListElementEntry<PoolConfigDataViewModel>(
                this.MediaShell,
                mediaPool,
                this.ApplicationState.CurrentProject.InfomediaConfig.Pools,
                onAfterDelete,
                onAfterUndoDelete,
                MediaStrings.ProjectController_DeleteMediaPool);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        /// <summary>
        /// Updates the media pool.
        /// </summary>
        /// <param name="parameters">
        /// The parameters of the command.
        /// </param>
        public void UpdateResourceListElement(UpdateEntityParameters parameters)
        {
            var oldElements = parameters.OldElements.ToList();
            var newElements = parameters.NewElements.ToList();
            var elementsContainer = parameters.ElementsContainerReference;

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                elementsContainer,
                MediaStrings.ProjectController_UpdateMediaPoolHistoryEntryLabel,
                this.CommandRegistry);

            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
        }

        /// <summary>
        /// Tries to achieve a checked in state.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. On success true.
        /// </returns>
        public async Task<bool> EnsureCheckInAsync()
        {
            this.CurrentState = await this.CurrentState.CheckInAsync();

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved
                && this.CurrentState.CheckinDecision == CheckinUserDecision.Cancel)
            {
                return false;
            }

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.CheckedIn)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to achieve a checked in state. Skip is allowed and returns true.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>. On success true.
        /// </returns>
        public async Task<bool> EnsureCheckInSkippableAsync()
        {
            this.CurrentState = await this.CurrentState.CheckInAsync(CheckinTrapBehaviour.Skippable);

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved
                && this.CurrentState.CheckinDecision == CheckinUserDecision.Cancel)
            {
                return false;
            }

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.CheckedIn
                || this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The ensure project controller has nothing left to do before exit.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> EnsureCanExitAsync()
        {
            var saveUserDecision = this.SaveTrap();

            switch (saveUserDecision)
            {
                case SaveUserDecision.NoSaveRequired:
                    break;
                case SaveUserDecision.Save:
                    this.CurrentState = await this.CurrentState.SaveAsync();
                    break;
                case SaveUserDecision.Discard:
                    this.CurrentState = await this.CurrentState.ReloadAsync();
                    break;
                case SaveUserDecision.Cancel:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.CurrentState = await this.CurrentState.CheckInAsync(CheckinTrapBehaviour.Skippable);

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved
                && this.CurrentState.CheckinDecision == CheckinUserDecision.Cancel)
            {
                return false;
            }

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Modified)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The ensure check in.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> EnsureCheckInOrResetAsync()
        {
            var stateInfo = this.CurrentState.StateInfo;

            if (stateInfo == ProjectStates.ProjectStates.CheckedIn
                || stateInfo == ProjectStates.ProjectStates.NoProject)
            {
                return true;
            }

            this.CurrentState = await this.CurrentState.CheckInAsync(CheckinTrapBehaviour.Skippable);

            if (this.CurrentState.CheckinDecision == CheckinUserDecision.Cancel)
            {
                return false;
            }

            if (this.CurrentState.CheckinDecision == CheckinUserDecision.Skip)
            {
                var result = MessageBox.Show(
                    MediaStrings.Project_CheckIn_SkipWillReset,
                    MediaStrings.Project_CheckIn_SkipWillResetTitle,
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.OK)
                {
                    return false;
                }

                var mediaConfigurationDataViewModel = this.GetCurrentMediaConfiguration();
                if (mediaConfigurationDataViewModel != null)
                {
                    this.DeletePendingProjectFile(mediaConfigurationDataViewModel);
                    this.CurrentState = new CheckedInState(this.MediaShell, this.CommandRegistry, this);
                }
            }

            return true;
        }

        /// <summary>
        /// Opens a recent project that was not checked in.
        /// </summary>
        /// <param name="projectName">
        /// The project Name.
        /// </param>
        /// <returns>
        /// True if open was successful.
        /// </returns>
        public async Task<bool> OpenLocalProjectAsync(string projectName)
        {
            this.CurrentState = await this.CurrentState.OpenLocalAsync(projectName);

            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks in the project with a different name and/or on a different tenant on the server.
        /// </summary>
        /// <param name="saveAsParameters">
        /// The parameters needed for checking in a project.
        /// </param>
        public async void CheckInAsProject(SaveAsParameters saveAsParameters)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(
                Permission.Create,
                DataScope.MediaConfiguration)
                || saveAsParameters == null)
            {
                return;
            }

            Logger.Info("Saving project as new one for tenant {0}.", saveAsParameters.Tenant.Name);

            this.saveAsParameter = saveAsParameters;
            await this.ExecuteCheckInAsProject(saveAsParameters);
        }

        /// <summary>
        /// The import project.
        /// </summary>
        /// <param name="projectToImport">
        /// The project to import.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ImportProject(MediaProjectDataViewModel projectToImport)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(Permission.Create, DataScope.MediaConfiguration))
            {
                return;
            }

            Logger.Info("Creating new project from import.");
            var createParameters = new CreateProjectParameters { ImportingProject = projectToImport };
            await this.CreateNewProjectAsync(createParameters);
        }

        /// <summary>
        /// Handles situations when the project should be closed and, therefore, saved or not.
        /// </summary>
        /// <returns>
        /// True if was not dirty or user decided to save or restore previous state, false on cancel
        /// </returns>
        public SaveUserDecision SaveTrap()
        {
            if (this.ApplicationState.CurrentProject == null || !this.ApplicationState.CurrentProject.IsDirty)
            {
                return SaveUserDecision.NoSaveRequired;
            }

            MessageBoxResult result;
            if (!this.MediaShell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration))
            {
                result = MessageBoxResult.No;
            }
            else
            {
                var message = string.Format(MediaStrings.Project_UnsavedBox_Content, Environment.NewLine);
                result = MessageBox.Show(
                    message,
                    MediaStrings.Project_UnsavedBox_Title,
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
            }

            switch (result)
            {
                case MessageBoxResult.Cancel:
                    return SaveUserDecision.Cancel;

                case MessageBoxResult.Yes:
                    return SaveUserDecision.Save;

                case MessageBoxResult.No:
                    return SaveUserDecision.Discard;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// The check in trap.
        /// </summary>
        /// <param name="skippable">
        /// Indicates if skip is available
        /// </param>
        /// <exception cref="Exception">
        /// Error if dialog is not can not be skipped but permissions missing.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<CheckinTrapResult> CheckinTrapAsync(
            CheckinTrapBehaviour skippable = CheckinTrapBehaviour.NotSkippable)
        {
            if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.CheckedIn)
            {
                return new CheckinTrapResult { Decision = CheckinUserDecision.NoCheckinRequired };
            }

            if (!this.MediaShell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration))
            {
                if (skippable == CheckinTrapBehaviour.NotSkippable)
                {
                    throw new Exception("This checkin is not skippable but user has no rights to check in.");
                }

                return new CheckinTrapResult { Decision = CheckinUserDecision.Skip };
            }

            var taskSource = new TaskCompletionSource<CheckinTrapResult>();
            this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowCheckinDialog).Execute(
                new CheckinDialogArguments
                {
                    Skippable = skippable == CheckinTrapBehaviour.Skippable,
                    OnCheckinCompleted = result => taskSource.SetResult(result)
                });

            return await taskSource.Task;
        }

        /// <summary>
        /// Resets the <see cref="IMediaApplicationState.CurrentProject"/> and the <see cref="ProjectManager"/>.
        /// </summary>
        /// <param name="mediaProjectDataViewModel">
        /// The media project data view model.
        /// </param>
        /// <param name="resetCurrentProjectObject">
        /// If set to <c>false</c>, ApplicationState.CurrentProject is not replaced (Needed when creating a new project)
        /// </param>
        public void ResetProject(
            MediaProjectDataViewModel mediaProjectDataViewModel,
            bool resetCurrentProjectObject = true)
        {
            if (resetCurrentProjectObject)
            {
                this.ApplicationState.CurrentProject = mediaProjectDataViewModel;
            }

            this.RefreshLayoutUsageReferences(mediaProjectDataViewModel.InfomediaConfig);
            this.RefreshCycleUsageReferences(mediaProjectDataViewModel.InfomediaConfig);
            this.ApplicationState.CurrentPhysicalScreen =
                mediaProjectDataViewModel.InfomediaConfig.PhysicalScreens.FirstOrDefault();
            if (this.ApplicationState.CurrentPhysicalScreen != null)
            {
                ((MediaShell)this.MediaShell).SetCurrentEditor(this.ApplicationState.CurrentPhysicalScreen.Type.Value);
            }

            this.ApplicationState.CurrentVirtualDisplay =
                mediaProjectDataViewModel.InfomediaConfig.VirtualDisplays.FirstOrDefault();
            if (this.ApplicationState.CurrentVirtualDisplay == null)
            {
                return;
            }

            this.ApplicationState.CurrentCyclePackage = this.ApplicationState.CurrentVirtualDisplay.CyclePackage;
            if (this.ApplicationState.CurrentCyclePackage == null
                || this.ApplicationState.CurrentCyclePackage.StandardCycles == null
                || this.ApplicationState.CurrentCyclePackage.StandardCycles.Count == 0)
            {
                return;
            }

            this.ChangeToInitialLayout();

            if (this.ApplicationState.CurrentCycle == null)
            {
                return;
            }

            this.ApplicationState.CurrentSection = this.ApplicationState.CurrentCycle.Sections.FirstOrDefault();
            if (this.ApplicationState.CurrentSection == null)
            {
                return;
            }

            this.ApplicationState.CurrentLayout = this.ApplicationState.CurrentSection.Layout;
            if (this.ApplicationState.CurrentLayout == null || this.ApplicationState.CurrentPhysicalScreen == null)
            {
                return;
            }

            var resolution = ((LayoutConfigDataViewModel)this.ApplicationState.CurrentLayout)
                .IndexedResolutions[this.ApplicationState.CurrentVirtualDisplay.Width.Value,
                    this.ApplicationState.CurrentVirtualDisplay.Height.Value];
            if (resolution != null)
            {
                this.LoadResolutionElements(resolution);
            }
            else
            {
                Logger.Error("Did not find a resolution in layout.");
            }

            this.ParentController.ChangeHistoryController.ResetChangeHistory();
            foreach (
                var resource in this.ApplicationState.CurrentProject.Resources.Where(r => r.Type == ResourceType.Video))
            {
                IResource thumbnail;
                this.ParentController.ResourceController.EnsurePreview(resource, out thumbnail);
            }

            this.EnsureReferencesLoaded(mediaProjectDataViewModel.InfomediaConfig);
            this.EnsurePredefinedFormulaReferences(mediaProjectDataViewModel);
            this.LegacyHandling(mediaProjectDataViewModel);
            this.ParentController.MainMenuPrompt.SaveAsScreen.Name = string.Empty;
            this.MediaShell.CycleNavigator.SelectedCycleNavigationTreeViewDataViewModel =
                this.MediaShell.CycleNavigator.TreeViewFirstLevelElements.FirstOrDefault();

            this.ApplicationState.CurrentProject.ClearDirty();
            this.ApplicationState.ClearDirty();
        }

        /// <summary>
        /// The update recent projects.
        /// </summary>
        /// <param name="isCheckedIn">
        /// The is checked in.
        /// </param>
        /// <param name="saveThumbnail">
        /// The save thumbnail.
        /// </param>
        public void UpdateRecentProjects(bool isCheckedIn = true, bool saveThumbnail = true)
        {
            Logger.Trace("Updating the recent projects.");
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(
                () =>
                {
                    var recentProjects = this.ApplicationState.RecentProjects;
                    if (recentProjects == null)
                    {
                        Logger.Debug("No recent project to update");
                        return;
                    }

                    var project =
                        recentProjects.FirstOrDefault(
                            p =>
                                p.ProjectName == this.ApplicationState.CurrentProject.Name
                                && p.TenantId == this.ApplicationState.CurrentTenant.Id
                                && p.ServerName == this.ApplicationState.LastServer)
                        ?? this.AddRecentProject(saveThumbnail, recentProjects);

                    project.ProjectName = this.ApplicationState.CurrentProject.Name;
                    project.TenantId = this.ApplicationState.CurrentTenant.Id;
                    project.ServerName = this.ApplicationState.LastServer;
                    project.LastUsed = TimeProvider.Current.Now;
                    project.ProjectId = this.ApplicationState.CurrentProject.ProjectId;
                    project.PreviewThumbnailPath = this.SavePreviewThumbnail(saveThumbnail);

                    project.IsCheckedIn = isCheckedIn;
                    var projects = recentProjects.OrderByDescending(p => p.LastUsed).ToList();
                    this.ApplicationState.RecentProjects.Clear();
                    projects.ForEach(this.ApplicationState.RecentProjects.Add);
                });
            Logger.Trace("Recent projects updated.");
        }

        /// <summary>
        /// Wraps notify so it is available by the interface.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public void NotifyWrapper(Notification notification)
        {
            this.Notify(notification);
        }

        /// <summary>
        /// create a new default resolution data view model
        /// </summary>
        /// <param name="resolutionWidth">
        /// The resolution Width.
        /// </param>
        /// <param name="resolutionHeight">
        /// The resolution Height.
        /// </param>
        /// <returns>
        /// the new ResolutionConfigDataViewModel
        /// </returns>
        public ResolutionConfigDataViewModel CreateDefaultResolutionConfigDataViewModel(
            int resolutionWidth, int resolutionHeight)
        {
            var resolutionConfig = new ResolutionConfigDataViewModel(this.MediaShell)
                                       {
                                           Height = new DataValue<int>(resolutionHeight),
                                           Width = new DataValue<int>(resolutionWidth),
                                       };
            return resolutionConfig;
        }

        /// <summary>
        /// Refreshes the layout usage references.
        /// </summary>
        /// <param name="currentInfomediaConfig">
        /// The current infomedia config.
        /// </param>
        public void RefreshLayoutUsageReferences(InfomediaConfigDataViewModel currentInfomediaConfig)
        {
            foreach (var layout in currentInfomediaConfig.Layouts)
            {
                var layoutName = layout.Name.Value;
                layout.CycleSectionReferences.Clear();
                foreach (var cycle in currentInfomediaConfig.Cycles.StandardCycles)
                {
                    foreach (var section in cycle.Sections.Where(section => section.LayoutName == layoutName))
                    {
                        layout.CycleSectionReferences.Add(new LayoutCycleSectionRefDataViewModel(cycle, section));
                    }
                }

                foreach (var cycle in currentInfomediaConfig.Cycles.EventCycles)
                {
                    foreach (var section in cycle.Sections.Where(section => section.LayoutName == layoutName))
                    {
                        layout.CycleSectionReferences.Add(new LayoutCycleSectionRefDataViewModel(cycle, section));
                    }
                }
            }
        }

        /// <summary>
        /// The on project got dirty.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task OnProjectGotDirty()
        {
            this.CurrentState = await this.CurrentState.MakeDirtyAsync();
        }

         /// <summary>
        /// Sets the state of the project back to saved.
        /// </summary>
        public void OnUndoToSaveMark()
        {
            this.CurrentState = new SavedState(this.MediaShell, this.CommandRegistry, this);
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that can be awaited.
        /// </returns>
        public async Task CreateNewProjectAsync(CreateProjectParameters parameter)
        {
            if (parameter.ImportingProject == null || this.MainMenuPrompt.IsOpenAfterImport)
            {
                this.ClearCurrentProjectSettings();
            }

            this.ApplicationState.ProjectManager.IsFileSelected = false;
            var isImportingProject = parameter.ImportingProject != null;
            MediaProjectDataViewModel mediaProjectDataViewModel;
            if (isImportingProject)
            {
                mediaProjectDataViewModel = parameter.ImportingProject;
                this.ApplicationState.CurrentProject = mediaProjectDataViewModel;
                this.CurrentState = new ModifiedState(this.MediaShell, this.CommandRegistry, this);
            }
            else
            {
                mediaProjectDataViewModel = new MediaProjectDataViewModel
                                                {
                                                    InfomediaConfig = this.CreateDefaultInfomediaConfig(),
                                                    DateCreated = TimeProvider.Current.UtcNow,
                                                    ProjectId = Guid.NewGuid(),
                                                    Name = parameter.Name,
                                                    Description = parameter.Description,
                                                    ProjectSize = Settings.Default.ProjectSizeXmlPart / 1024
                                                };
                this.ApplicationState.CurrentProject = mediaProjectDataViewModel;
                this.CurrentState = new ModifiedState(this.MediaShell, this.CommandRegistry, this);
                this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.PhysicalScreen.CreateNew)
                    .Execute(parameter);
            }

            try
            {
                // we just created a new DVM
                if (this.CurrentState.StateInfo == ProjectStates.ProjectStates.Modified)
                {
                   // this.MediaShell.IsBusy = true;
                    this.CurrentState = await this.CurrentState.CreateAsync(mediaProjectDataViewModel);

                    // clear all selected update groups in export
                    foreach (var selectable in this.ParentController.MainMenuPrompt.ExportScreen.UpdateGroups)
                    {
                        selectable.IsSelected = false;
                    }
                }
            }
            catch (Exception e)
            {
                this.HandleCreateDocumentException(e);
                if (!isImportingProject)
                {
                    this.ApplicationState.CurrentProject = null;
                    this.CurrentState = new NoProjectState(this.MediaShell, this.CommandRegistry, this);
                }

                return;
            }

            if (!isImportingProject)
            {
                this.ApplicationState.CurrentProject.InfomediaConfig.PhysicalScreens.First().IsInEditMode = false;
                this.ResetProject(this.ApplicationState.CurrentProject, false);
                var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                controller.InitializeLayoutEditorControllers();
                this.ConsistencyChecker.Check();
                this.MediaShell.SetProjectTitle(mediaProjectDataViewModel.Name);
                this.ApplicationState.CurrentProject.IsCheckedIn = true;
                this.ApplicationState.CurrentProject.ClearDirty();
            }

            this.MediaShell.IsBusy = false;
            this.Notify((Notification)new StatusNotification
                {
                    Title = string.Format(MediaStrings.ProjectController_ProjectCreated, mediaProjectDataViewModel.Name)
                });
        }

        /// <summary>
        /// The execute open project.
        /// </summary>
        /// <param name="mediaConfiguration">
        /// The media Configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> ExecuteOpenProject(MediaConfigurationDataViewModel mediaConfiguration)
        {
            var success = false;

            try
            {
                Logger.Info("Open project '{0}'", mediaConfiguration.Name);
                if (!this.CleanupResources(mediaConfiguration.MediaProjectDataViewModel))
                {
                    return false;
                }

                this.MediaShell.IsBusy = true;
                this.MediaShell.BusyContentTextFormat = MediaStrings.ProjectList_LoadingMessage;
                mediaConfiguration.LoadingProgress = 0;
                success = await this.LoadProjectFromServerAsync(mediaConfiguration);
                this.Notify((Notification)new StatusNotification { Title = mediaConfiguration.Name, });
            }
            catch (Exception exception)
            {
                var message = string.Format(
                    "Exception trying to load project {0} from server.", mediaConfiguration.Name);
                Logger.ErrorException(message, exception);
                MessageBox.Show(
                    MediaStrings.Project_OpenRecent_ErrorMessage,
                    MediaStrings.Project_OpenRecent_ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                this.RemoveRecentProject(mediaConfiguration);
            }

            this.MediaShell.ClearBusy();
            mediaConfiguration.RaiseLoadedEvent();
            return success;
        }

        /// <summary>
        /// The check in project async.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <exception cref="Exception">
        /// Exception if no media configuration is found.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CheckInProjectAsync(CreateDocumentVersionParameters parameters)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(
                Permission.Write,
                DataScope.MediaConfiguration))
            {
                return false;
            }

            try
            {
                this.MediaShell.IsBusyIndeterminate = false;
                this.MediaShell.IsBusy = true;
                this.MediaShell.BusyContentTextFormat = MediaStrings.CheckInDialog_CollectingInformation;
                this.ApplicationState.IsCheckingIn = true;
                var mediaConfigurationDataViewModel = this.GetCurrentMediaConfiguration();
                var resultSize = await this.UploadResourcesAsync();

                this.ApplicationState.CurrentProject.ProjectSize = resultSize / 1024;

                if (mediaConfigurationDataViewModel == null
                    || !await this.CreateDocumentVersionAsync(
                        parameters.Minor,
                        parameters.Major,
                        parameters.Comment,
                        this.ParentController.ParentController.ConnectionController,
                        mediaConfigurationDataViewModel.Document.ReadableModel,
                        this.ApplicationState.CurrentProject.ToDataModel()))
                {
                    throw new Exception("Could not find the media configuration for the current project.");
                }

                this.ApplicationState.CurrentProject.IsCheckedIn = true;
                this.ApplicationState.CurrentProject.ClearDirty();
                this.ParentController.ChangeHistoryController.AddSaveMarker();
                this.UpdateRecentProjects();
                this.ApplicationState.ClearDirty();
                this.MediaShell.ClearBusy();
                this.DeletePendingProjectFile(mediaConfigurationDataViewModel);
                Logger.Info("Project checked in.");

                this.ApplicationState.IsCheckingIn = false;
            }
            catch (Exception e)
            {
                this.HandleCheckinException(e);
                this.ExecuteSaveProjectLocal();
                this.ApplicationState.IsCheckingIn = false;
                this.MediaShell.ClearBusy();
                return false;
            }

            return true;
        }

        /// <summary>
        /// The reload project. If project is not found locally the project is loaded from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public async Task<ReloadProjectResult> ReloadProjectAsync()
        {
            Logger.Info("Reload project structure locally.");

            var result = ReloadProjectResult.Fail;

            if (this.ApplicationState.CurrentProject == null)
            {
                return result;
            }

            var projectName = this.ApplicationState.CurrentProject.Name;
            this.ClearCurrentProjectSettings();
            if (!(await this.OpenLocalProjectAsync(projectName)))
            {
                await this.ResetProjectToServerStateAsync(projectName);
                result = ReloadProjectResult.FromServer;
            }
            else
            {
                result = ReloadProjectResult.FromLocal;
            }

            if (this.ApplicationState.CurrentProject != null)
            {
                this.MediaShell.SetProjectTitle(this.ApplicationState.CurrentProject.Name);
                this.ApplicationState.CurrentProject.ClearDirty();
                this.ApplicationState.ClearDirty();
                this.ParentController.ChangeHistoryController.ResetChangeHistory();
                var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                controller.InitializeLayoutEditorControllers();
                this.ConsistencyChecker.Check();
            }

            return result;
        }

        /// <summary>
        /// Saves the project locally.
        /// </summary>
        /// <returns>a value indicating whether the project was saved</returns>
        public bool ExecuteSaveProjectLocal()
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(Permission.Write, DataScope.MediaConfiguration))
            {
                return false;
            }

            Logger.Info("Saving project structure locally.");

            var existingConfiguration = this.GetCurrentMediaConfiguration();
            if (existingConfiguration == null)
            {
                return false;
            }

            try
            {
                var filename = this.ApplicationState.LastServer.GetValidFileName()
                               + existingConfiguration.Document.GetIdString() + ".rx";
                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                var rootPath = resourceManager.GetLocalProjectsPath();
                IDirectoryInfo projectDirectory;
                if (!FileSystemManager.Local.TryGetDirectory(rootPath, out projectDirectory))
                {
                    Directory.CreateDirectory(rootPath);
                }

                var path = Path.Combine(rootPath, filename);
                var file = ((IWritableFileSystem)FileSystemManager.Local).CreateFile(path);
                this.ApplicationState.CurrentProject.FilePath = path;
                if (this.ApplicationState.CurrentProject.ProjectId == new Guid())
                {
                    this.ApplicationState.CurrentProject.ProjectId = Guid.NewGuid();
                }

                this.ApplicationState.CurrentProject.DateLastModified = TimeProvider.Current.UtcNow;
                var mediaProject = this.ApplicationState.CurrentProject.ToDataModel();
                using (var writableFile = file.OpenWrite())
                {
                    var serializer = new XmlSerializer(typeof(MediaProjectDataModel));
                    serializer.Serialize(writableFile, mediaProject);
                }

                this.ApplicationState.CurrentProject.IsCheckedIn = false;
                this.ApplicationState.CurrentProject.ClearDirty();
                this.ApplicationState.ClearDirty();
                this.ParentController.ChangeHistoryController.AddSaveMarker();
                this.UpdateRecentProjects(false, false);
                this.ParentController.ParentController.SaveState(true);
                Logger.Info("Project saved");
                this.Notify(
                    (Notification)
                    new StatusNotification
                    {
                        Title =
                            string.Format(
                                MediaStrings.ProjectController_SaveLocal,
                                this.ApplicationState.CurrentProject.Name)
                    });

                return true;
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while trying to save the project locally", exception);
                var prompt = new ConnectionExceptionPrompt(
                    exception,
                    MediaStrings.ProjectController_SaveLocalFailedMessage,
                    MediaStrings.ProjectController_SaveLocalFailedTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }

            return false;
        }

        /// <summary>
        /// The create document async.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="tenant">
        /// The tenant.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CreateDocumentAsync(MediaProjectDataModel model, TenantReadableModel tenant = null)
        {
            var connectionController = this.ParentController.ParentController.ConnectionController;
            var document = connectionController.DocumentChangeTrackingManager.Create();
            document.Name = model.Name;
            document.Description = model.Description;
            if (tenant == null)
            {
                await this.ApplicationState.CurrentTenant.LoadReferencePropertiesAsync();
                document.Tenant = this.ApplicationState.CurrentTenant;
            }
            else
            {
                await tenant.LoadReferencePropertiesAsync();
                document.Tenant = tenant;
            }

            DocumentReadableModel createdDocument;
            try
            {
                createdDocument =
                    await connectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(document);
            }
            catch (Exception e)
            {
                var errorString = string.Format("Exception during document creation. ('{0}')", model.Name);
                Logger.ErrorException(errorString, e);
                this.HandleCreateDocumentException(e);
                return false;
            }

            if (createdDocument == null)
            {
                return false;
            }

            var comment = MediaStrings.ProjectController_CreateProjectInitialVersion;
            if (!await this.CreateDocumentVersionAsync(1, 0, comment, connectionController, createdDocument, model))
            {
                return false;
            }

            await createdDocument.LoadNavigationPropertiesAsync();
            var mediaConfiguration = connectionController.MediaConfigurationChangeTrackingManager.Create();
            mediaConfiguration.Document = createdDocument;

            try
            {
                var project = await
                    connectionController.MediaConfigurationChangeTrackingManager.CommitAndVerifyAsync(
                        mediaConfiguration);
                var mediaConfig = new MediaConfigurationDataViewModel(project, this.MediaShell, this.CommandRegistry);
                this.ApplicationState.CurrentMediaConfiguration = mediaConfig;
                await mediaConfig.UndoChangesAsync();
            }
            catch (Exception e)
            {
                var errorString = string.Format("Exception during media configuration update. ('{0}')", model.Name);
                Logger.ErrorException(errorString, e);

                var errorPrompt = new ConnectionExceptionPrompt(
                    e,
                    MediaStrings.BackgroundSystem_CouldNotUpdatemediaConfiguration);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(errorPrompt);
                return false;
            }

            var prompt = ((MediaShellController)this.ParentController).MainMenuPrompt;
            prompt.NewProjectName = string.Empty;
            prompt.Description = string.Empty;
            prompt.SaveAsScreen.Name = string.Empty;
            if (this.ApplicationState.CurrentProject != null)
            {
                this.ApplicationState.CurrentProject.ClearDirty();
            }

            Logger.Info("Project '{0}' created on server.", model.Name);
            return true;
        }

        /// <summary>
        /// Clears all project related settings.
        /// </summary>
        public void ClearCurrentProjectSettings()
        {
            this.ApplicationState.CurrentCycle = null;
            this.ApplicationState.CurrentSection = null;
            this.ApplicationState.CurrentCyclePackage = null;
            this.ApplicationState.CurrentLayout = null;
            this.ApplicationState.CurrentPhysicalScreen = null;
            this.ApplicationState.CurrentProject = null;
            this.ApplicationState.CurrentVirtualDisplay = null;
            this.ApplicationState.ConsistencyMessages = null;
            this.ApplicationState.CompatibilityMessages = null;

            if (this.ParentController != null && this.ParentController.MainMenuPrompt != null)
            {
                this.ParentController.MainMenuPrompt.ExportScreen.UpdateGroups.ToList().ForEach(g =>
                    {
                        g.HasCompatibilityIssue = false;
                        g.IsSelected = false;
                    });
            }

            this.ApplicationState.CurrentMediaConfiguration = null;
            if (this.MediaShell != null && this.MediaShell.Editor != null)
            {
                var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
                dispatcher.Dispatch(
                    () =>
                    {
                        this.MediaShell.Editor.Elements.Clear();
                        this.MediaShell.Editor.SelectedElements.Clear();
                    });
            }

            this.CurrentState = new NoProjectState(this.MediaShell, this.CommandRegistry, this);
        }

        private static async Task CopyProjectPreviewThumbnailAsync(
            string originalThumbnailName, string newThumbnailName)
        {
            var originalName = originalThumbnailName + ".png";
            var filePath = string.Empty;
            try
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Settings.Default.ProjectPreviewsRelativePath);
                IWritableDirectoryInfo writableDirectory;
                if (!FileSystem.TryGetDirectory(directory, out writableDirectory))
                {
                    // ReSharper disable once RedundantAssignment
                    writableDirectory = FileSystem.CreateDirectory(directory);
                }

                filePath = Path.Combine(directory, originalName);
                IWritableFileInfo originalThumbnail;
                if (!FileSystem.TryGetFile(filePath, out originalThumbnail))
                {
                    Logger.Debug("Could not find original project thumbnail '{0}'", filePath);
                    return;
                }

                var newName = newThumbnailName + ".png";
                var destination = Path.Combine(directory, newName);
                IWritableFileInfo copiedThumbnail;
                if (!FileSystem.TryGetFile(destination, out copiedThumbnail))
                {
                    copiedThumbnail = FileSystem.CreateFile(destination);
                }

                using (var originalStream = originalThumbnail.OpenRead())
                {
                    using (var destinationStream = copiedThumbnail.OpenWrite())
                    {
                        await originalStream.CopyToAsync(destinationStream);
                    }
                }
            }
            catch (Exception e)
            {
                var message = string.Format("Exception occurred while copy the preview thumbnail '{0}'.", filePath);
                Logger.DebugException(message, e);
            }
        }

        private static IWritableFileSystem GetFileSystem()
        {
            return (IWritableFileSystem)FileSystemManager.Local;
        }

        private static IMediaApplicationState GetApplicationState()
        {
            return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
        }

        private static ConsistencyChecker GetConsistencyChecker()
        {
            return new ConsistencyChecker
            {
                MediaApplicationState = GetApplicationState(),
            };
        }

        private static CompatibilityChecker GetCompatibilityChecker()
        {
            return new CompatibilityChecker();
        }

        /// <summary>
        /// The reset project to server state.
        /// </summary>
        /// <param name="projectName">
        /// The project Name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ResetProjectToServerStateAsync(string projectName)
        {
            var project = this.ApplicationState.ExistingProjects.FirstOrDefault(
                        p => p.Name.Equals(projectName));

            if (project == null)
            {
                throw new Exception("Project not found in existing projects.");
            }

            await project.UndoChangesAsync();
            this.ApplicationState.CurrentProject = project.MediaProjectDataViewModel;
            this.ResetProject(project.MediaProjectDataViewModel);
            this.ApplicationState.CurrentProject.IsCheckedIn = true;
        }

        private async Task ExecuteCheckInAsProject(SaveAsParameters saveAsParameters)
        {
            if (this.ApplicationState.CurrentProject == null)
            {
                return;
            }

            this.MainMenuPrompt.Shell.BusyContentTextFormat = MediaStrings.ExportAsBusyText;
            this.MainMenuPrompt.Shell.IsBusyIndeterminate = true;
            this.MainMenuPrompt.Shell.IsBusy = true;

            try
            {
                var mediaProjectDataViewModel = (MediaProjectDataViewModel)this.ApplicationState.CurrentProject.Clone();
                mediaProjectDataViewModel.Name = saveAsParameters.Name;
                mediaProjectDataViewModel.Description =
                    string.Format(
                        MediaStrings.ProjectController_SaveAsDocumentDescription,
                        saveAsParameters.Name,
                        saveAsParameters.Tenant.Name);
                mediaProjectDataViewModel.ProjectId = Guid.NewGuid();
                await
                    CopyProjectPreviewThumbnailAsync(
                        this.ApplicationState.CurrentProject.ProjectId.ToString("N"),
                        mediaProjectDataViewModel.ProjectId.ToString("N"));
                this.ParentController.ParentController.ProjectAdded += this.OnProjectAdded;

                // Upload pending resources
                var resultSize = await this.UploadResourcesAsync();
                mediaProjectDataViewModel.ProjectSize = resultSize / 1024;

                var success =
                    await this.CreateDocumentAsync(mediaProjectDataViewModel.ToDataModel(), saveAsParameters.Tenant);

                if (!success)
                {
                    this.MainMenuPrompt.Shell.ClearBusy();
                    this.ParentController.ParentController.ProjectAdded -= this.OnProjectAdded;
                    this.saveAsParameter = null;
                }
            }
            catch (Exception e)
            {
                this.MainMenuPrompt.Shell.IsBusy = false;
                Logger.ErrorException("Error while creating a new project on BackgroundSystem", e);
                var prompt = new ConnectionExceptionPrompt(
                    e,
                    MediaStrings.ProjectController_CreateServerFailedMessage,
                    MediaStrings.ProjectController_CreateServerFailedTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private async void CheckInProject()
        {
            this.CurrentState = await this.CurrentState.CheckInAsync();
        }

        private async void CreateProjectAsync(CreateProjectParameters parameter)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(Permission.Create, DataScope.MediaConfiguration))
            {
                return;
            }

            var success = await this.EnsureCheckInOrResetAsync();
            if (!success)
            {
                return;
            }

            this.MediaShell.IsBusy = true;
            this.MediaShell.BusyContentTextFormat = MediaStrings.BusyCreateProject;
            this.MediaShell.IsBusyIndeterminate = true;
            Logger.Info("Creating new project");
            await this.CreateNewProjectAsync(parameter);
        }

        private async void SaveProjectLocal()
        {
            this.CurrentState = await this.CurrentState.SaveAsync();
        }

        private async void OpenProjectAsync(MediaConfigurationDataViewModel mediaConfiguration)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(Permission.Read, DataScope.MediaConfiguration))
            {
                return;
            }

            if (mediaConfiguration == null)
            {
                throw new Exception("Called open project with no parameter.");
            }

            Logger.Info("Request to open project");
            var success = await this.EnsureCheckInOrResetAsync();
            if (!success)
            {
                return;
            }

            this.CurrentState = await this.CurrentState.OpenFromServerAsync(mediaConfiguration);

            this.MediaShell.ClearBusy();
        }

        private MediaConfigurationDataViewModel GetCurrentMediaConfiguration()
        {
            if (this.ApplicationState.CurrentProject == null)
            {
                return null;
            }

            var mediaConfigurationDataViewModel =
                this.ApplicationState.ExistingProjects.FirstOrDefault(
                    p => p.Name.Equals(
                            this.ApplicationState.CurrentProject.Name,
                            StringComparison.InvariantCultureIgnoreCase));

            if (mediaConfigurationDataViewModel == null)
            {
                Logger.Error("No media configuration for current project found");
            }

            return mediaConfigurationDataViewModel;
        }

        private void LoadResolutionElements(ResolutionConfigDataViewModel resolution)
        {
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(() =>
                {
                    if (this.ApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.Audio)
                    {
                        this.MediaShell.Editor.Elements.Clear();
                        var element = resolution.Elements.FirstOrDefault();
                        if (element != null)
                        {
                            this.MediaShell.Editor.CurrentAudioOutputElement = (AudioOutputElementDataViewModel)element;
                        }
                    }
                    else
                    {
                        this.MediaShell.Editor.CurrentAudioOutputElement = null;

                        this.MediaShell.Editor.Elements.Clear();
                        foreach (var element in resolution.Elements.Where(e => e is GraphicalElementDataViewModelBase))
                        {
                            this.MediaShell.Editor.Elements.Add((GraphicalElementDataViewModelBase)element);
                        }

                        this.MediaShell.Editor.SortByZOrder();
                    }
                });
        }

        private void RefreshCycleUsageReferences(InfomediaConfigDataViewModel infomediaConfig)
        {
            foreach (var cycle in infomediaConfig.Cycles.StandardCycles)
            {
                foreach (var cyclePackage in infomediaConfig.CyclePackages)
                {
                    if (cyclePackage.StandardCycles.Any(c => c.Reference == cycle))
                    {
                        if (!cycle.CyclePackageReferences.Contains(cyclePackage))
                        {
                            cycle.CyclePackageReferences.Add(cyclePackage);
                        }
                    }
                }
            }

            foreach (var cycle in infomediaConfig.Cycles.EventCycles)
            {
                foreach (var cyclePackage in infomediaConfig.CyclePackages.Where(
                            package => package.EventCycles.Any(c => c.Reference == cycle)))
                {
                    if (!cycle.CyclePackageReferences.Contains(cyclePackage))
                    {
                        cycle.CyclePackageReferences.Add(cyclePackage);
                    }
                }
            }
        }

        private MasterCycleConfigDataViewModel CreateMasterCycleConfigDataViewModel(
            MasterSectionConfigDataViewModel masterSection)
        {
            var masterCycleConfigDataViewModel = new MasterCycleConfigDataViewModel(this.MediaShell)
            {
                Name = new DataValue<string>(MediaStrings.ProjectController_DefaultMasterCycleName),
                Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase> { masterSection }
            };
            return masterCycleConfigDataViewModel;
        }

        private MasterSectionConfigDataViewModel CreateMasterSectionConfigDataViewModel()
        {
            var masterSection = new MasterSectionConfigDataViewModel(this.MediaShell)
            {
                Duration = new DataValue<TimeSpan>(MasterSectionDuration)
            };
            return masterSection;
        }

        private void EnsurePredefinedFormulaReferences(MediaProjectDataViewModel mediaProjectDataViewModel)
        {
            foreach (var element in from layout in mediaProjectDataViewModel.InfomediaConfig.Layouts
                                    let result = new List<EvaluationConfigDataViewModel>()
                                    from element in layout.Resolutions.SelectMany(resolution => resolution.Elements)
                                    select element)
            {
                element.ResetContainedPredefinedFormulaReferences(
                    this.ApplicationState.CurrentProject.InfomediaConfig.Evaluations);
            }

            foreach (var cycle in mediaProjectDataViewModel.InfomediaConfig.Cycles.StandardCycles)
            {
                cycle.ResetContainedPredefinedFormulaReferences(
                    this.ApplicationState.CurrentProject.InfomediaConfig.Evaluations);
                foreach (var section in cycle.Sections)
                {
                    section.ResetContainedPredefinedFormulaReferences(
                        this.ApplicationState.CurrentProject.InfomediaConfig.Evaluations);
                }
            }

            foreach (var cycle in mediaProjectDataViewModel.InfomediaConfig.Cycles.EventCycles)
            {
                cycle.ResetContainedPredefinedFormulaReferences(
                    this.ApplicationState.CurrentProject.InfomediaConfig.Evaluations);
                foreach (var section in cycle.Sections)
                {
                    section.ResetContainedPredefinedFormulaReferences(
                        this.ApplicationState.CurrentProject.InfomediaConfig.Evaluations);
                }
            }
        }

        private async Task<bool> LoadProjectFromServerAsync(
            MediaConfigurationDataViewModel mediaConfiguration,
            bool isCheckedIn = true)
        {
            try
            {
                this.ClearCurrentProjectSettings();
                var controller = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
                if (mediaConfiguration != null)
                {
                    await mediaConfiguration.UndoChangesAsync();
                    if (mediaConfiguration.MediaProjectDataViewModel != null)
                    {
                        var totalResources = mediaConfiguration.MediaProjectDataViewModel.Resources.Count;
                        this.MediaShell.IsBusy = true;
                        this.MediaShell.IsBusyIndeterminate = false;
                        this.MediaShell.TotalBusyProgress = totalResources;
                        this.MediaShell.BusyContentTextFormat = MediaStrings.ProjectList_LoadingMessage;

                        for (var i = 0; i < totalResources; i++)
                        {
                            this.MediaShell.CurrentBusyProgress = i + 1;
                            this.MediaShell.CurrentBusyProgressText =
                                Path.GetFileName(mediaConfiguration.MediaProjectDataViewModel.Resources[i].Filename);
                            mediaConfiguration.LoadingProgress = i + 1;

                            // ReSharper disable once UnusedVariable
                            try
                            {
                                var resource =
                                    await
                                    this.ApplicationState.ProjectManager.GetResourceAsync(
                                        mediaConfiguration.MediaProjectDataViewModel.Resources[i].Hash);
                            }
                            catch (Exception e)
                            {
                                this.HandleResourceNotFoundException(mediaConfiguration, i, e);
                            }
                        }

                        mediaConfiguration.MediaProjectDataViewModel.IsCheckedIn = true;
                    }

                    this.ResetProject(mediaConfiguration.MediaProjectDataViewModel);
                    this.ResourceManager.SetReferencesForProject(mediaConfiguration.MediaProjectDataViewModel);
                    this.MediaShell.MediaApplicationState.ClearDirty();
                    this.MediaShell.MediaApplicationState.CurrentProject.IsCheckedIn = true;

                    this.MediaShell.SetProjectTitle(mediaConfiguration.Name);

                    // setting resource references will make project dirty
                    this.MediaShell.ClearBusy();
                    this.ApplicationState.CurrentMediaConfiguration = mediaConfiguration;

                    // update the selected update groups
                    await mediaConfiguration.ReadableModel.LoadNavigationPropertiesAsync();
                    foreach (var selectable in this.ParentController.MainMenuPrompt.ExportScreen.UpdateGroups)
                    {
                        selectable.IsSelected = mediaConfiguration.ReadableModel.UpdateGroups.Contains(selectable.Item);
                    }
                }

                if (this.ApplicationState.CurrentProject != null)
                {
                    controller.InitializeLayoutEditorControllers();
                    this.ConsistencyChecker.Check();
                }

                this.UpdateRecentProjects(isCheckedIn, false);
            }
            catch (Exception e)
            {
                this.HandleExceptionFailedToOpenProject(mediaConfiguration, e);
                return false;
            }

            return true;
        }

        private void HandleExceptionFailedToOpenProject(MediaConfigurationDataViewModel mediaConfiguration, Exception e)
        {
            string logMessage;
            string userMessage;
            var name = mediaConfiguration == null ? "<unknown>" : mediaConfiguration.Name;
            if (e is ReasonException)
            {
                logMessage = string.Format(
                    "Exception trying to load project {0} from server. Reason: {1}",
                    name,
                    e.Message);
                userMessage = string.Format(MediaStrings.Project_Open_ServerErrorMessageWithReason, name, e.Message);
            }
            else
            {
                logMessage = string.Format(
                    "Exception trying to load project {0} from server.",
                    mediaConfiguration == null ? "<unknown>" : mediaConfiguration.Name);
                 userMessage = string.Format(MediaStrings.Project_Open_ServerErrorMessage, name);
            }

            Logger.ErrorException(logMessage, e);
            if (mediaConfiguration != null)
            {
                this.MediaShell.ClearBusy();
            }

            this.ClearCurrentProjectSettings();

            MessageBox.Show(
                userMessage,
                MediaStrings.Project_Open_ServerErrorTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private void HandleResourceNotFoundException(
            MediaConfigurationDataViewModel mediaConfiguration,
            int i,
            Exception e)
        {
            var name = string.Empty;
            var resource = mediaConfiguration.MediaProjectDataViewModel.Resources[i];
            if (resource.Filename != null)
            {
                name = Path.GetFileName(resource.Filename);
            }

            throw new ReasonException(
                string.Format(MediaStrings.Project_Open_ServerResourceNotFoundErrorMessage, name), e);
        }

        private bool CleanupResources(MediaProjectDataViewModel mediaProject, string filename = null)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            resourceManager.CleanupResources();
            long resourcesSize = 0;
            string localPath = null;

            if (mediaProject == null)
            {
                return true;
            }

            foreach (var resourceInfo in mediaProject.Resources)
            {
                localPath = resourceManager.GetResourcePath(resourceInfo.Hash);
                IWritableFileInfo fileInfo;
                if (FileSystem.TryGetFile(localPath, out fileInfo))
                {
                    continue;
                }

                resourcesSize += resourceInfo.Length;
            }

            if (!resourceManager.CheckAvailableDiskSpace(resourcesSize)
                || !resourceManager.CheckUsedDiskSpace(resourcesSize))
            {
                resourceManager.CleanupResources(true);
                if (!resourceManager.CheckAvailableDiskSpace(resourcesSize)
                    || !resourceManager.CheckUsedDiskSpace(resourcesSize))
                {
                    var drive = Path.GetPathRoot(localPath ?? resourceManager.GetResourcePath(string.Empty));
                    var message = string.Format(
                        MediaStrings.Project_Open_NotEnoughSpace_Message, drive, filename ?? mediaProject.Name);
                    MessageBox.Show(
                        message,
                        MediaStrings.Project_Open_NotEnoughSpace_Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void RemoveRecentProject(MediaConfigurationDataViewModel project)
        {
            var recentProjectEntry =
                this.ApplicationState.RecentProjects.FirstOrDefault(e => e.ProjectName == project.Name);
            if (recentProjectEntry == null)
            {
                return;
            }

            this.RemoveRecentProject(recentProjectEntry);
        }

        private void RemoveRecentProject(RecentProjectDataViewModel project)
        {
            var recentProjects = this.ApplicationState.RecentProjects;
            Logger.Debug("Removing recent project {0} from the list.", project.ProjectName);
            if (string.IsNullOrWhiteSpace(project.PreviewThumbnailPath))
            {
                Logger.Debug("Preview path not specified for project '{0}'", project.ProjectName);
            }
            else
            {
                IWritableFileInfo previewThumbnail;
                if (FileSystem.TryGetFile(project.PreviewThumbnailPath, out previewThumbnail))
                {
                    try
                    {
                        previewThumbnail.Delete();
                        Logger.Debug(
                            "Deleted preview thumbnail with hash '{0}' for project", project.PreviewThumbnailPath);
                    }
                    catch (UpdateException e)
                    {
                        var message = string.Format(
                            "Could not delete preview thumbnail with hash '{0}'", project.PreviewThumbnailPath);
                        Logger.WarnException(message, e);
                    }
                }
                else
                {
                    Logger.Debug("Preview thumbnail path '{0}' not found", project.PreviewThumbnailPath);
                }
            }

            recentProjects.Remove(project);
            Logger.Debug("Recent project successfully removed.");
        }

        private RecentProjectDataViewModel AddRecentProject(
            bool saveThumbnail,
            ExtendedObservableCollection<RecentProjectDataViewModel> recentProjects)
        {
            var project = new RecentProjectDataViewModel
                              {
                                  FilePath = this.ApplicationState.ProjectManager.FullFileName,
                                  ProjectId = this.ApplicationState.CurrentProject.ProjectId,
                              };

            if (
                recentProjects.Count(
                    p =>
                    p.TenantId == this.ApplicationState.CurrentTenant.Id
                    && (p.ServerName == null
                        || p.ServerName.Equals(
                            this.ApplicationState.LastServer,
                            StringComparison.InvariantCultureIgnoreCase))) >= Settings.Default.MaxRecentProjects)
            {
                var recentToRemove = recentProjects.LastOrDefault();
                if (recentToRemove != null)
                {
                    this.RemoveRecentProject(recentToRemove);
                }
            }

            if (!saveThumbnail)
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Settings.Default.ProjectPreviewsRelativePath);
                IWritableDirectoryInfo writableDirectory;
                if (!FileSystem.TryGetDirectory(directory, out writableDirectory))
                {
                    // ReSharper disable once RedundantAssignment
                    writableDirectory = FileSystem.CreateDirectory(directory);
                }

                var thumbName = project.ProjectId.ToString("N") + ".png";

                var filePath = Path.Combine(directory, thumbName);
                IWritableFileInfo thumbnail;
                if (FileSystem.TryGetFile(filePath, out thumbnail))
                {
                    project.PreviewThumbnailPath = filePath;
                }
            }

            recentProjects.Insert(0, project);
            Logger.Debug("Added project {0} to the recent list.", project.ProjectName);
            return project;
        }

        private string SavePreviewThumbnail(bool replaceExisting)
        {
            var filePath = string.Empty;
            try
            {
                var directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Settings.Default.ProjectPreviewsRelativePath);
                IWritableDirectoryInfo writableDirectory;
                if (!FileSystem.TryGetDirectory(directory, out writableDirectory))
                {
                    // ReSharper disable once RedundantAssignment
                    writableDirectory = FileSystem.CreateDirectory(directory);
                }

                var thumbName = this.ApplicationState.CurrentProject.ProjectId.ToString("N") + ".png";

                filePath = Path.Combine(directory, thumbName);
                IWritableFileInfo thumbnail;
                var created = false;
                if (!FileSystem.TryGetFile(filePath, out thumbnail))
                {
                    thumbnail = FileSystem.CreateFile(filePath);
                    created = true;
                }

                if (replaceExisting || created)
                {
                    var bitmapEncoder = new PngBitmapEncoder();
                    this.MediaShell.Editor.TakeScreenshot(bitmapEncoder);

                    using (var s = thumbnail.OpenWrite())
                    {
                        // Needed when writing over an existing file.
                        s.SetLength(0);
                        bitmapEncoder.Save(s);
                    }
                }
            }
            catch (Exception e)
            {
                var message = string.Format("Exception occurred while creating the preview thumbnail '{0}'.", filePath);
                Logger.DebugException(message, e);
            }

            return filePath;
        }

        /// <summary>
        /// Creates the Infomedia config.
        /// </summary>
        /// <returns>
        /// A data model with the default structure for an Infomedia config.
        /// </returns>
        private InfomediaConfigDataViewModel CreateDefaultInfomediaConfig()
        {
            var config = new InfomediaConfigDataViewModel(this.MediaShell)
            {
                CreationDate = new DataValue<DateTime>(TimeProvider.Current.Now),
                Version = new DataValue<Version>(new Version(2, 0)),
            };

            var masterSection = this.CreateMasterSectionConfigDataViewModel();
            var masterCycleConfigDataViewModel = this.CreateMasterCycleConfigDataViewModel(masterSection);
            var masterLayoutConfigDataViewModel = new MasterLayoutConfigDataViewModel(this.MediaShell)
            {
                Name =
                    new DataValue<string>(MediaStrings.ProjectController_DefaultMasterLayoutName)
            };
            masterSection.Layout = masterLayoutConfigDataViewModel;
            var masterPresentation = new MasterPresentationConfigDataViewModel(this.MediaShell)
            {
                MasterCycles =
                    new ExtendedObservableCollection<MasterCycleConfigDataViewModel>
                            {
                                masterCycleConfigDataViewModel
                            },
                MasterLayouts =
                    new ExtendedObservableCollection<MasterLayoutConfigDataViewModel>
                            {
                                masterLayoutConfigDataViewModel
                            }
            };
            config.MasterPresentation = masterPresentation;

            this.AddPredefinesFormulas(config);
            return config;
        }

        private void AddPredefinesFormulas(InfomediaConfigDataViewModel config)
        {
#if !__UseLuminatorTftDisplay
            var missingIbis = this.CreateMissingIbisFormula();
            var specialLinePicture = this.CreateSpecialLinePictureFormula();
            var specialLineText = this.CreateSpecialLineTextFormula();
            var lineZero = this.CreateLineZeroFormula();
            var stopIndex = this.CreateStopIndexFormula();
            var doorClose = this.CreateDoorCloseFormula();
#else
            var stopRequest = this.CreateStopRequestFormula();
            var lineBlank = this.CreateLineBlankFormula();
            var lineSet = this.CreateLineSetFormula();
            var doorOpen = this.CreateDoorOpenFormula();
            var doorsClosed = this.CreateDoorsClosedFormula();
            var offRoute = this.CreateOffRouteFormula();
            var onRoute = this.CreateOnRouteFormula();
            var displayVersion = this.CreateDisplayVersionFormula();
            var fifteenMinuteInterval = this.CreateFifteenMinuteIntervalFormula();
            var sixMinuteInterval = this.CreateSixMinuteIntervalFormula();
            var stop = this.CreateStopFormula();
            var stops = this.CreateStopsFormula();
            var approachingStop = this.CreateApproachingStopFormula();
            var departing = this.CreateDepartingFormula();
            var destinationAvailable = this.CreateDestinationAvailableFormula();
            var door1Open = this.CreateDoor1OpenFormula();
            var door2Open = this.CreateDoor2OpenFormula();
            var endStrip = this.CreateEndStripFormula();
            var fullStrip = this.CreateFullStripFormula();
            var lastStrip = this.CreateLastStripFormula();
            var interiorAudioZone = this.CreateInteriorAudioZoneFormula();
            var exteriorAudioZone = this.CreateExteriorAudioZoneFormula();
#endif
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ApproachingStop },
                Evaluation = approachingStop
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_Departing },
                Evaluation = departing
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_DestinationAvailable },
                Evaluation = destinationAvailable
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_DisplayVersion },
                Evaluation = displayVersion
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_DoorOpen },
                Evaluation = doorOpen
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_Door1Open },
                Evaluation = door1Open
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_Door2Open },
                Evaluation = door2Open
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_DoorsClosed },
                Evaluation = doorsClosed
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_EndStrip },
                Evaluation = endStrip
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ExteriorAudioZone },
                Evaluation = exteriorAudioZone
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_FifteenMinuteInterval },
                Evaluation = fifteenMinuteInterval
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_FullStrip },
                Evaluation = fullStrip
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_InteriorAudioZone },
                Evaluation = interiorAudioZone
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_LastStrip },
                Evaluation = lastStrip
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_LineBlank },
                Evaluation = lineBlank
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_LineSet },
                Evaluation = lineSet
            });
#if !__UseLuminatorTftDisplay
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_DoorClose },
                Evaluation = doorClose
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_LineZero },
                Evaluation = lineZero
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_MissingIbis },
                Evaluation = missingIbis
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_SpecialLinePicture },
                Evaluation = specialLinePicture
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = new DataValue<string>(MediaStrings.PredefinedFormula_LineSpecialSign),
                Evaluation = specialLineText,
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_StopIndex },
                Evaluation = stopIndex
            });
#endif
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_OffRoute },
                Evaluation = offRoute
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_OnRoute },
                Evaluation = onRoute
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_SixMinuteInterval },
                Evaluation = sixMinuteInterval
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_Stop },
                Evaluation = stop
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_StopRequest },
                Evaluation = stopRequest
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_Stops },
                Evaluation = stops
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ServiceAlert },
                Evaluation = this.CreateServiceAlertFormula()
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ServiceAlert1 },
                Evaluation = this.CreateServiceAlert1Formula()
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ServiceAlert2 },
                Evaluation = this.CreateServiceAlert2Formula()
            });
            config.Evaluations.Add(new EvaluationConfigDataViewModel(this.MediaShell)
            {
                Name = { Value = MediaStrings.PredefinedFormula_ServiceAlert3 },
                Evaluation = this.CreateServiceAlert3Formula()
            });
        }

#if __UseLuminatorTftDisplay
        private OrEvalDataViewModel CreateDoorOpenFormula()
        {
            var leftPart = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = {Value = 105},
                        Column = {Value = 0}
                    },
                Value = {Value = "1"},
                IgnoreCase = {Value = false}
            };
            var rightPart = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = {Value = 105},
                        Column = {Value = 1}
                    },
                Value = {Value = "1"},
                IgnoreCase = {Value = false}
            };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(leftPart);
            orEval.Conditions.Add(rightPart);
            return orEval;
        }
#else
        private StringCompareEvalDataViewModel CreateDoorCloseFormula()
        {
            var doorClose = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                    this.MediaShell)
                    {
                        Table = { Value = 0 },
                        Column = { Value = 4 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            return doorClose;
        }

        private StringCompareEvalDataViewModel CreateDoorOpenFormula()
        {
            var doorOpen = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                    this.MediaShell)
                    {
                        Table = { Value = 0 },
                        Column = { Value = 4 }
                    },
                Value = { Value = "1" },
                IgnoreCase = { Value = false }
            };
            return doorOpen;
        }
#endif
        private StringCompareEvalDataViewModel CreateLineBlankFormula()
        {
            var lineBlank = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                    this.MediaShell)
                    {
                        Table = new DataValue<int>(10),
                        Column = new DataValue<int>(0)
                    },
                IgnoreCase = { Value = false },
                Value = { Value = string.Empty }
            };
            return lineBlank;
        }

#if !__UseLuminatorTftDisplay
        private IntegerCompareEvalDataViewModel CreateStopIndexFormula()
        {
            var stopIndex = new IntegerCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                    this.MediaShell)
                    {
                        Table = { Value = 10 },
                        Column = { Value = 4 }
                    },
                Begin = { Value = 1 },
                End = { Value = 89 }
            };
            return stopIndex;
        }

        private IntegerCompareEvalDataViewModel CreateLineZeroFormula()
        {
            var lineZero = new IntegerCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                    this.MediaShell)
                    {
                        Table = { Value = 10 },
                        Column = { Value = 0 }
                    },
                Begin = { Value = 0 },
                End = { Value = 0 }
            };
            return lineZero;
        }

        private CodeConversionEvalDataViewModel CreateSpecialLineTextFormula()
        {
            var specialLineText = new CodeConversionEvalDataViewModel(this.MediaShell)
            {
                FileName = new DataValue<string>("codeconversion.csv"),
                UseImage = new DataValue<bool>(false)
            };
            return specialLineText;
        }

        private CodeConversionEvalDataViewModel CreateSpecialLinePictureFormula()
        {
            var specialLinePicture = new CodeConversionEvalDataViewModel(this.MediaShell)
            {
                FileName = new DataValue<string>("codeconversion.csv"),
                UseImage = new DataValue<bool>(true)
            };
            return specialLinePicture;
        }
        
        private StringCompareEvalDataViewModel CreateMissingIbisFormula()
        {
            var missingIbis = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation = new GenericEvalDataViewModel(this.MediaShell)
                {
                    Table = new DataValue<int>(0),
                    Column = new DataValue<int>(2)
                },
                IgnoreCase = { Value = false },
                Value = { Value = "0" }
            };
            return missingIbis;
        }
#endif

        private StringCompareEvalDataViewModel CreateStopRequestFormula()
        {
            var stopRequest = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation = new GenericEvalDataViewModel(this.MediaShell)
                {
                    Table = new DataValue<int>(0),
                    Column = new DataValue<int>(3)
                },
                IgnoreCase = { Value = false },
                Value = { Value = "1" }
            };
            return stopRequest;
        }

        private StringCompareEvalDataViewModel CreateOffRouteFormula()
        {
            var offRoute = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 10 },
                        Column = { Value = 12 }
                    },
                Value = { Value = "1" },
                IgnoreCase = { Value = false }
            };
            return offRoute;
        }

        private StringCompareEvalDataViewModel CreateOnRouteFormula()
        {
            var onRoute = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 10 },
                        Column = { Value = 12 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            return onRoute;
        }

        private NotEvalDataViewModel CreateDisplayVersionFormula()
        {
            var noVersionInfo = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 20 },
                        Column = { Value = 2 },
                        Row = { Value = 90 }
                    },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell)
            {
                Evaluation = noVersionInfo
            };
            return notEval;
        }

        private AndEvalDataViewModel CreateDoorsClosedFormula()
        {
            var leftPart = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 0 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            var rightPart = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 1 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            var andEval = new AndEvalDataViewModel(this.MediaShell);
            andEval.Conditions.Add(leftPart);
            andEval.Conditions.Add(rightPart);
            return andEval;
        }

        private OrEvalDataViewModel CreateFifteenMinuteIntervalFormula()
        {
            var zeroMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 27 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            var fifteenMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 27 }
                    },
                Value = { Value = "15" },
                IgnoreCase = { Value = false }
            };
            var thirtyMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 27 }
                    },
                Value = { Value = "30" },
                IgnoreCase = { Value = false }
            };
            var fortyFiveMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 27 }
                    },
                Value = { Value = "45" },
                IgnoreCase = { Value = false }
            };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(zeroMinutes);
            orEval.Conditions.Add(fifteenMinutes);
            orEval.Conditions.Add(thirtyMinutes);
            orEval.Conditions.Add(fortyFiveMinutes);
            return orEval;
        }

        private OrEvalDataViewModel CreateSixMinuteIntervalFormula()
        {
            var zeroMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 105 },
                                Column = { Value = 27 }
                            },
                Value = { Value = "0" },
                IgnoreCase = { Value = false }
            };
            var sixMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "6" },
                                     IgnoreCase = { Value = false }
                                 };
            var twelveMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "12" },
                                     IgnoreCase = { Value = false }
                                 };
            var eighteenMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "18" },
                                     IgnoreCase = { Value = false }
                                 };
            var twentyFourMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "24" },
                                     IgnoreCase = { Value = false }
                                 };
            var thirtyMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "30" },
                                     IgnoreCase = { Value = false }
                                 };
            var thirtySixMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "36" },
                                     IgnoreCase = { Value = false }
                                 };
            var fortyTwoMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "42" },
                                     IgnoreCase = { Value = false }
                                 };
            var fortyEightMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "48" },
                                     IgnoreCase = { Value = false }
                                 };
            var fiftyFourMinutes = new StringCompareEvalDataViewModel(this.MediaShell)
                                 {
                                     Evaluation =
                                         new GenericEvalDataViewModel(
                                                 this.MediaShell)
                                                 {
                                                     Table = { Value = 105 },
                                                     Column = { Value = 27 }
                                                 },
                                     Value = { Value = "54" },
                                     IgnoreCase = { Value = false }
                                 };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(zeroMinutes);
            orEval.Conditions.Add(sixMinutes);
            orEval.Conditions.Add(twelveMinutes);
            orEval.Conditions.Add(eighteenMinutes);
            orEval.Conditions.Add(twentyFourMinutes);
            orEval.Conditions.Add(thirtyMinutes);
            orEval.Conditions.Add(thirtySixMinutes);
            orEval.Conditions.Add(fortyTwoMinutes);
            orEval.Conditions.Add(fortyEightMinutes);
            orEval.Conditions.Add(fiftyFourMinutes);
            return orEval;
        }

        private NotEvalDataViewModel CreateStopFormula()
        {
            var noStopName = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 12 },
                        Column = { Value = 0 },
                        Row = { Value = 0 }
                    },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell)
            {
                Evaluation = noStopName
            };
            return notEval;
        }

        private AndEvalDataViewModel CreateStopsFormula()
        {
            var noStop0Name = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 12 },
                        Column = { Value = 0 },
                        Row = { Value = 0 }
                    },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell)
            {
                Evaluation = noStop0Name
            };
            var noStop1Name = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 12 },
                        Column = { Value = 0 },
                        Row = { Value = 1 }
                    },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var not1Eval = new NotEvalDataViewModel(this.MediaShell)
            {
                Evaluation = noStop1Name
            };
            var andEval = new AndEvalDataViewModel(this.MediaShell);
            andEval.Conditions.Add(notEval);
            andEval.Conditions.Add(not1Eval);
            return andEval;
        }

        private OrEvalDataViewModel CreateApproachingStopFormula()
        {
            var leftPart = new GenericEvalDataViewModel(
                this.MediaShell)
            {
                Table = {Value = 105},
                Column = {Value = 18}
            };
            var rightPart = new GenericEvalDataViewModel(this.MediaShell)
            {
                Table = { Value = 105 },
                Column = { Value = 19 }
            };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(leftPart);
            orEval.Conditions.Add(rightPart);
            return orEval;
        }

        private GenericEvalDataViewModel CreateDepartingFormula()
        {
            var departing = new GenericEvalDataViewModel(
                this.MediaShell)
            {
                Table = { Value = 105 },
                Column = { Value = 20 }
            };
            return departing;
        }

        private StringCompareEvalDataViewModel CreateDoor1OpenFormula()
        {
            var door1Open = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 0 }
                    },
                Value = { Value = "1" },
                IgnoreCase = { Value = false }
            };
            return door1Open;
        }

        private StringCompareEvalDataViewModel CreateDoor2OpenFormula()
        {
            var door2Open = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                        this.MediaShell)
                    {
                        Table = { Value = 105 },
                        Column = { Value = 1 }
                    },
                Value = { Value = "1" },
                IgnoreCase = { Value = false }
            };
            return door2Open;
        }

        private NotEvalDataViewModel CreateServiceAlertFormula()
        {
            var noServiceAlertMsg = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 20 },
                                Column = { Value = 2 },
                                Row = { Value = 0 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noServiceAlertMsg };
            return notEval;
        }

        private NotEvalDataViewModel CreateServiceAlert1Formula()
        {
            var noServiceAlertMsg = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 20 },
                                Column = { Value = 2 },
                                Row = { Value = 1 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noServiceAlertMsg };
            return notEval;
        }


        private NotEvalDataViewModel CreateServiceAlert2Formula()
        {
            var noServiceAlertMsg = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 20 },
                                Column = { Value = 2 },
                                Row = { Value = 2 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noServiceAlertMsg };
            return notEval;
        }

        private NotEvalDataViewModel CreateServiceAlert3Formula()
        {
            var noServiceAlertMsg = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 20 },
                                Column = { Value = 2 },
                                Row = { Value = 3 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noServiceAlertMsg };
            return notEval;
        }

        private AndEvalDataViewModel CreateEndStripFormula()
        {
            var noThirdStop = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 12 },
                                Column = { Value = 0 },
                                Row = { Value = 2 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notNoThirdStop = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noThirdStop };
            var noFourthStop = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 12 },
                                Column = { Value = 0 },
                                Row = { Value = 3 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var andEval = new AndEvalDataViewModel(this.MediaShell);
            andEval.Conditions.Add(notNoThirdStop);
            andEval.Conditions.Add(noFourthStop);
            return andEval;
        }

        private NotEvalDataViewModel CreateFullStripFormula()
        {
            var noThirdStop = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 12 },
                                Column = { Value = 0 },
                                Row = { Value = 3 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noThirdStop };
            return notEval;
        }

        private AndEvalDataViewModel CreateLastStripFormula()
        {
            var noSecondStop = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 12 },
                                Column = { Value = 0 },
                                Row = { Value = 1 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notNoSecondStop = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noSecondStop };
            var noThirdStop = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 12 },
                                Column = { Value = 0 },
                                Row = { Value = 2 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var andEval = new AndEvalDataViewModel(this.MediaShell);
            andEval.Conditions.Add(notNoSecondStop);
            andEval.Conditions.Add(noThirdStop);
            return andEval;
        }

        private NotEvalDataViewModel CreateLineSetFormula()
        {
            var noLineSet = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 10 },
                                Column = { Value = 0 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noLineSet };
            return notEval;
        }

        private NotEvalDataViewModel CreateDestinationAvailableFormula()
        {
            var noDestination = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                            {
                                Table = { Value = 11 },
                                Column = { Value = 0 }
                            },
                Value = { Value = "" },
                IgnoreCase = { Value = false }
            };
            var notEval = new NotEvalDataViewModel(this.MediaShell) { Evaluation = noDestination };
            return notEval;
        }

        private OrEvalDataViewModel CreateInteriorAudioZoneFormula()
        {
            var zeroValue = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 101 },
                        Column = { Value = 1 }
                    },
                Value = { Value = "0" },
                IgnoreCase = { Value = true }
            };
            var interiorValue = new StringCompareEvalDataViewModel(this.MediaShell)
            {
                Evaluation =
                    new GenericEvalDataViewModel(
                            this.MediaShell)
                    {
                        Table = { Value = 101 },
                        Column = { Value = 1 }
                    },
                Value = { Value = "interior" },
                IgnoreCase = { Value = true }
            };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(zeroValue);
            orEval.Conditions.Add(interiorValue);
            return orEval;
        }

        private OrEvalDataViewModel CreateExteriorAudioZoneFormula()
        {
            var zeroValue = new StringCompareEvalDataViewModel(this.MediaShell)
                                {
                                    Evaluation =
                                        new GenericEvalDataViewModel(
                                                this.MediaShell)
                                                {
                                                    Table = { Value = 101 },
                                                    Column = { Value = 1 }
                                                },
                                    Value = { Value = "1" },
                                    IgnoreCase = { Value = true }
                                };
            var interiorValue = new StringCompareEvalDataViewModel(this.MediaShell)
                                    {
                                        Evaluation =
                                            new GenericEvalDataViewModel(
                                                    this.MediaShell)
                                                    {
                                                        Table = { Value = 101 },
                                                        Column = { Value = 1 }
                                                    },
                                        Value = { Value = "exterior" },
                                        IgnoreCase = { Value = true }
                                    };
            var orEval = new OrEvalDataViewModel(this.MediaShell);
            orEval.Conditions.Add(zeroValue);
            orEval.Conditions.Add(interiorValue);
            return orEval;
        }

        private bool CanSaveProject(object obj)
        {
            if (this.ApplicationState.CurrentProject == null || this.ApplicationState.ProjectManager == null)
            {
                return false;
            }

            if (!this.MediaShell.PermissionController.HasPermission(Permission.Write, DataScope.MediaConfiguration))
            {
                return false;
            }

            if (this.CurrentState.StateInfo != ProjectStates.ProjectStates.Modified)
            {
                return false;
            }

            // We can save either if the project is dirty or if the file for the project was never selected (save as
            // behavior).
            return this.ApplicationState.CurrentProject.IsDirty || !this.ApplicationState.ProjectManager.IsFileSelected;
        }

        private string GenerateMediaPoolName()
        {
            var index = 1;
            var newPoolName = "Media pool";
            var isUnique = this.IsMediaPoolNameUnique(newPoolName);

            while (!isUnique)
            {
                index++;
                newPoolName = "Media pool" + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
                isUnique = this.IsMediaPoolNameUnique(newPoolName);
            }

            return newPoolName;
        }

        private bool IsMediaPoolNameUnique(string newPoolName)
        {
            return this.ApplicationState.CurrentProject.InfomediaConfig.Pools.All(p => p.Name.Value != newPoolName);
        }

        /// <summary>
        /// Ensures that all reference objects are loaded.
        /// </summary>
        /// <param name="infomediaConfig">
        /// The current infomedia configuration.
        /// </param>
        private void EnsureReferencesLoaded(InfomediaConfigDataViewModel infomediaConfig)
        {
            // ReSharper disable UnusedVariable
            foreach (var masterLayout in from masterLayout in infomediaConfig.MasterPresentation.MasterLayouts
                                         from physicalScreen in from physicalScreen in masterLayout.PhysicalScreens
                                                                let physicalScreenReference = physicalScreen.Reference
                                                                from virtualDisplayReference in
                                                                    physicalScreen.VirtualDisplays.Select(
                                                                        virtualDisplay => virtualDisplay.Reference)
                                                                select physicalScreen
                                         select masterLayout)
            {
            }

            foreach (var baseLayout in infomediaConfig.Layouts.Select(layout => layout.BaseLayoutName))
            {
            }

            foreach (var package in from virtualDisplay in infomediaConfig.VirtualDisplays
                                    select virtualDisplay.CyclePackage
                                    into package
                                    from reference in from cycle in package.StandardCycles
                                                      select cycle.Reference
                                                      into reference
                                                      from layout in
                                                          reference.Sections.Select(section => section.Layout)
                                                      select reference
                                    select package)
            {
            }

            // ReSharper restore UnusedVariable
        }

        private async Task<bool> CreateDocumentVersionAsync(
            int minor,
            int major,
            string comment,
            IConnectionController connectionController,
            DocumentReadableModel document,
            MediaProjectDataModel content)
        {
            var documentVersion = connectionController.DocumentVersionChangeTrackingManager.Create();
            documentVersion.Document = document;
            documentVersion.Content = new XmlData(content);
            var creatingUser = connectionController.UserChangeTrackingManager.Wrap(this.ApplicationState.CurrentUser);
            await creatingUser.LoadReferencePropertiesAsync();
            documentVersion.CreatingUser = creatingUser;
            documentVersion.Minor = minor;
            documentVersion.Major = major;
            documentVersion.Description = string.IsNullOrEmpty(comment) ? string.Empty : comment;

            try
            {
                await connectionController.DocumentVersionChangeTrackingManager.AddAsync(documentVersion);
            }
            catch (Exception e)
            {
                var errorString = string.Format("Exception during document version creation. ('{0}')", content.Name);
                Logger.ErrorException(errorString, e);
                var prompt = new ConnectionExceptionPrompt(
                    e,
                    MediaStrings.BackgroundSystem_CouldNotCommmitNewDocumentVersion);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                return false;
            }

            return true;
        }

        private void RenameProjectInRecentlyUsed(RenameProjectParameters renameParameters)
        {
            var recentProjectsToRename = from p in this.MediaShell.MediaApplicationState.RecentProjects
                                         where p.ProjectName == renameParameters.OldName
                                         where p.ServerName == this.ApplicationState.LastServer
                                         where p.TenantId == this.ApplicationState.CurrentTenant.Id
                                         select p;

            foreach (var project in recentProjectsToRename)
            {
                project.ProjectName = renameParameters.NewName;
            }
        }

        private async void PublishDocumentWritableModelAsync(PublishDocumentWritableModelParameters publishParameters)
        {
            if (!this.MediaShell.PermissionController.PermissionTrap(
                Permission.Write,
                DataScope.MediaConfiguration))
            {
                return;
            }

            var connectionController = this.ParentController.ParentController.ConnectionController;

            var success = false;
            try
            {
                await
                    connectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(
                        publishParameters.Model);
                success = true;
            }
            catch (Exception e)
            {
                string errorString;
                if (e is EndpointNotFoundException)
                {
                    errorString = MediaStrings.ProjectController_NoConnectionToBackend;
                }
                else
                {
                    errorString = string.Format(
                        "Exception during project update. ('{0}')",
                        publishParameters.Model.Name);
                }

                Logger.ErrorException(errorString, e);
                var prompt = new ConnectionExceptionPrompt(
                e,
                MediaStrings.BackgroundSystem_CouldNotUpdateProject);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                publishParameters.ErrorCallbackAction(new Exception("Could not update project."));
            }

            if (publishParameters.OnFinished != null)
            {
                publishParameters.OnFinished(success, publishParameters.OldValues, publishParameters.NewValues);
            }
        }

        private async void DeleteProject(MediaConfigurationDataViewModel project)
        {
            var confirmation = MessageBox.Show(
                string.Format(MediaStrings.ProjectController_DeleteConfirmMessage, project.Name),
                MediaStrings.ProjectController_DeleteConfirmTitle,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);
            if (confirmation == MessageBoxResult.Cancel)
            {
                return;
            }

            Logger.Info("Deleting project", project.Name);
            this.MainMenuPrompt.ProjectListScreen.IsBusy = true;
            this.MainMenuPrompt.ProjectListScreen.IsBusyIndeterminate = true;
            this.MainMenuPrompt.ProjectListScreen.BusyContentTextFormat =
                string.Format(MediaStrings.ProjectList_DeleteProjectMessage, project.Name);

            try
            {
                var connectionController = this.ParentController.ParentController.ConnectionController;
                await project.ReadableModel.LoadNavigationPropertiesAsync();
                await project.Document.ReadableModel.LoadNavigationPropertiesAsync();
                foreach (var updateGroup in project.ReadableModel.UpdateGroups.ToList())
                {
                    var writableUpdateGroup = updateGroup.ToChangeTrackingModel();
                    writableUpdateGroup.MediaConfiguration = null;
                    writableUpdateGroup.Commit();
                }

                var versions = project.Document.ReadableModel.Versions.ToList();
                foreach (var documentVersionReadOnlyDataViewModel in versions)
                {
                    await documentVersionReadOnlyDataViewModel.LoadReferencePropertiesAsync();
                    await documentVersionReadOnlyDataViewModel.CreatingUser.LoadReferencePropertiesAsync();
                    await
                        connectionController.DocumentVersionChangeTrackingManager.DeleteAsync(
                            documentVersionReadOnlyDataViewModel);
                }

                await connectionController.MediaConfigurationChangeTrackingManager.DeleteAsync(project.ReadableModel);
                await connectionController.DocumentChangeTrackingManager.DeleteAsync(project.Document.ReadableModel);
                Logger.Info("Project deleted.");
            }
            catch (Exception e)
            {
                var message = string.Format(
                    MediaStrings.ProjectController_DeleteProjectServerFailedMessage,
                    project.Name);
                Logger.ErrorException(message, e);
                var prompt = new ConnectionExceptionPrompt(
                        e,
                        message,
                        MediaStrings.ProjectController_DeleteProjectServerFailedTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }

            this.RemoveDeletedProjectFromRecentlyUsed(project);

            this.MediaShell.ClearBusy();
            this.MainMenuPrompt.ProjectListScreen.ClearBusy();
        }

        private void RemoveDeletedProjectFromRecentlyUsed(MediaConfigurationDataViewModel project)
        {
            if (this.MediaShell.MediaApplicationState == null || project == null)
            {
                return;
            }

            var recentDeleted = this.MediaShell.MediaApplicationState.RecentProjects.FirstOrDefault(
                rp => rp.ProjectName == project.Name
                      && (rp.ServerName == null
                         || rp.ServerName.Equals(
                            this.MediaShell.MediaApplicationState.LastServer,
                            StringComparison.InvariantCultureIgnoreCase))
                      && rp.TenantId == this.MediaShell.MediaApplicationState.CurrentTenant.Id);
            Logger.Debug(
                recentDeleted == null
                    ? "Recent project entry for '{0}' not found"
                    : "Removing project '{0}' from recent list",
                project.Name);

            this.MediaShell.MediaApplicationState.RecentProjects.Remove(recentDeleted);
        }

        private void HandleCreateDocumentException(Exception e)
        {
            Logger.ErrorException("Error while creating a new project on BackgroundSystem", e);
            var prompt = new ConnectionExceptionPrompt(
                e,
                MediaStrings.ProjectController_CreateServerFailedMessage,
                MediaStrings.ProjectController_CreateServerFailedTitle);
            InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            this.MediaShell.IsBusy = false;
        }

        private void HandleCheckinException(Exception e)
        {
            string message;

            var aggregateException = e as AggregateException;
            if (aggregateException != null)
            {
                var flatAggregateException = aggregateException.Flatten();

                if (flatAggregateException.InnerException is ServerException)
                {
                    message = MediaStrings.ProjectController_NoConnectionToBackend;
                }
                else
                {
                    // FaultException from WCF service
                    message = MediaStrings.ProjectController_ErrorBackend;
                }
            }
            else
            {
                message = string.Format(
                    MediaStrings.ProjectController_CheckInFailed,
                    this.ApplicationState.CurrentProject.Name);
            }

            Logger.ErrorException(message, e);
            var prompt = new ConnectionExceptionPrompt(e, message, MediaStrings.ProjectController_CheckInFailedTitle);
            InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
        }

        private void DeletePendingProjectFile(MediaConfigurationDataViewModel mediaConfigurationDataViewModel)
        {
            var filename = this.ApplicationState.LastServer + mediaConfigurationDataViewModel.Document.GetIdString()
                           + ".rx";
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            var rootPath = resourceManager.GetLocalProjectsPath();

            IDirectoryInfo directory;
            if (!FileSystemManager.Local.TryGetDirectory(rootPath, out directory))
            {
                return;
            }

            var path = Path.Combine(rootPath, filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private async Task<long> UploadResourcesAsync()
        {
            Logger.Debug("Uploading new resources");
            var projectSize = Settings.Default.ProjectSizeXmlPart;

            var errors = new List<Exception>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var localResources =
                this.ApplicationState.CurrentProject.Resources.Distinct(new ResourceInfoDataViewModelQualityComparer())
                    .ToList();
            var serverResources =
                (await
                 this.ParentController.ParentController.ConnectionController.ResourceChangeTrackingManager.QueryAsync(
                     ResourceQuery.Create())).ToList();
            var localResourceHashes = localResources.Select(r => r.Hash);
            var serverResourceHashes = serverResources.Select(r => r.Hash);
            var pendingResourceHashes = localResourceHashes.Except(serverResourceHashes).ToList();

            var pendingResources =
                pendingResourceHashes.Select(hash => localResources.Single(r => r.Hash == hash)).ToList();
            this.MediaShell.BusyContentTextFormat = MediaStrings.CheckInDialog_BusyIndicatorText;
            this.MediaShell.TotalBusyProgress = pendingResources.Count;
            this.MediaShell.CurrentBusyProgress = 0;
            var count = System.Math.Ceiling(pendingResourceHashes.Count * 1.0 / MaxDegreeOfParallelism);

            for (var i = 0; i < count; i++)
            {
                var uploadTasks =
                    (from resourceInfo in pendingResources.Skip(MaxDegreeOfParallelism * i).Take(MaxDegreeOfParallelism)
                     let uploadTask = this.UploadResourceAsync(resourceInfo.ToResource())
                     select new Tuple<string, Task<Resource>>(resourceInfo.Filename, uploadTask)).ToList();

                this.MediaShell.CurrentBusyProgress += uploadTasks.Count;
                this.MediaShell.CurrentBusyProgressText = Path.GetFileName(uploadTasks.Last().Item1);
                var aggregateTask = Task.WhenAll(uploadTasks.Select(tuple => tuple.Item2));
                await aggregateTask.ConfigureAwait(false);

                foreach (var uploadTask in uploadTasks)
                {
                    if (uploadTask.Item2.Exception == null)
                    {
                        var resource = await uploadTask.Item2;
                        projectSize += resource.Length;
                        continue;
                    }

                    var message = string.Format("Error trying to upload resource '{0}'.", uploadTask.Item1);
                    Logger.ErrorException(message, uploadTask.Item2.Exception);
                    errors.Add(uploadTask.Item2.Exception);
                }
            }

            var existingResources = localResources.Except(pendingResources);
            projectSize += existingResources.Sum(
                resource => serverResources.Single(r => r.Hash == resource.Hash).Length);
            stopWatch.Stop();

            if (errors.Any())
            {
                throw new AggregateException(errors);
            }

            Logger.Info("{0} resource(s) uploaded in {1}s", pendingResources.Count, stopWatch.Elapsed.TotalSeconds);
            return projectSize;
        }

        private async Task<Resource> UploadResourceAsync(Resource resource)
        {
            var resourceController = this.ParentController.ResourceController;
            var length = (await resourceController.UploadResourceAsync(resource)).Length;

            resource.Length = length;
            return resource;
        }

        private void OnProjectAdded(object sender, EventArgs e)
        {
            this.ParentController.ParentController.ProjectAdded -= this.OnProjectAdded;
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    try
                    {
                        this.ClearCurrentProjectSettings();
                        this.ApplicationState.IsCheckinAs = true;
                        if (this.ApplicationState.CurrentTenant.Id != this.saveAsParameter.Tenant.Id)
                        {
                            this.ApplicationState.IsCheckingIn = true;
                            var tenant =
                                this.ApplicationState.AuthorizedTenants.FirstOrDefault(
                                    t => t.Id == this.saveAsParameter.Tenant.Id);
                            if (tenant != null)
                            {
                                this.ApplicationState.CurrentTenant = tenant;
                            }

                            this.ApplicationState.IsCheckingIn = false;
                        }

                        var project =
                            this.ParentController.Shell.MediaApplicationState.ExistingProjects.LastOrDefault();
                        this.saveAsParameter = null;
                        if (project != null)
                        {
                            try
                            {
                                this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.Open).Execute(project);
                                var message = string.Format(
                                    MediaStrings.ProjectController_CheckedInAsMessage,
                                    project.Name,
                                    this.ApplicationState.CurrentTenant.Name);
                                MessageBox.Show(message, MediaStrings.ProjectController_CheckedInAsTitle);
                            }
                            catch (Exception exception)
                            {
                                Logger.ErrorException(
                                    "Error while trying to load a project after checkin as.",
                                    exception);
                            }
                        }
                    }
                    finally
                    {
                        this.ApplicationState.IsCheckinAs = false;
                        this.MainMenuPrompt.Shell.ClearBusy();
                    }
                });
        }

        private bool HasDeletePermission(MediaConfigurationDataViewModel obj)
        {
            return this.MediaShell.PermissionController.HasPermission(
                Permission.Delete,
                DataScope.MediaConfiguration);
        }

        private bool HasWritePermission(object obj)
        {
            return this.MediaShell.PermissionController.HasPermission(
                Permission.Write,
                DataScope.MediaConfiguration);
        }

        private bool HasReadPermission(object obj)
        {
            return this.MediaShell.PermissionController.HasPermission(
                Permission.Read,
                DataScope.MediaConfiguration);
        }

        private bool HasCreatePermission(object obj)
        {
            return this.MediaShell.PermissionController.HasPermission(
                Permission.Create,
                DataScope.MediaConfiguration);
        }

        private bool CanCheckIn(object obj)
        {
            var hasPermission = this.HasCreatePermission(null);
            var needsCheckin = this.CurrentState.StateInfo == ProjectStates.ProjectStates.Modified
                               || this.CurrentState.StateInfo == ProjectStates.ProjectStates.Saved;

            return needsCheckin && hasPermission;
        }

        private void LegacyHandling(MediaProjectDataViewModel mediaProjectDataViewModel)
        {
            // checks should be moved to migration manager in the future
            this.UpdateCsvMapping(mediaProjectDataViewModel);
            var resourceController = this.ParentController.ResourceController;
            resourceController.UpdateResourcesLedFontType();
        }

        private void ChangeToInitialLayout()
        {
            if (this.ApplicationState.CurrentPhysicalScreen != null
                  && this.ApplicationState.CurrentPhysicalScreen.Type.Value == PhysicalScreenType.Audio)
            {
                CycleRefConfigDataViewModelBase cycle =
                    this.ApplicationState.CurrentCyclePackage.EventCycles.FirstOrDefault();

                if (cycle == null)
                {
                    cycle = this.ApplicationState.CurrentCyclePackage.StandardCycles.First();
                }

                this.ApplicationState.CurrentCycle = cycle.Reference;

                var layouts = this.ApplicationState.CurrentProject.InfomediaConfig.Layouts;
                ServiceLocator.Current.GetInstance<IMediaShell>()
                    .CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo)
                    .Execute(layouts.FirstOrDefault());
            }
            else
            {
                this.ApplicationState.CurrentCycle =
                  this.ApplicationState.CurrentCyclePackage.StandardCycles.First().Reference;
            }
        }

        private class ResourceInfoDataViewModelQualityComparer : IEqualityComparer<ResourceInfoDataViewModel>
        {
            public bool Equals(ResourceInfoDataViewModel x, ResourceInfoDataViewModel y)
            {
                if (x == null)
                {
                    return y == null;
                }

                return y != null && this.GetHashCode(x).Equals(this.GetHashCode(y));
            }

            public int GetHashCode(ResourceInfoDataViewModel obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }

                if (string.IsNullOrEmpty(obj.Hash))
                {
                    return 0;
                }

                return obj.Hash.GetHashCode();
            }
        }
    }
}
