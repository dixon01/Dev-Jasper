// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminShellController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdminShellController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Gorba.Center.Admin.Core.Controllers.Entities;
    using Gorba.Center.Admin.Core.Controllers.Entities.Meta;
    using Gorba.Center.Admin.Core.Controllers.RemovableMedia;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig;
    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Admin.Core.ViewModels.Navigator;
    using Gorba.Center.Admin.Core.ViewModels.Stages;
    using Gorba.Center.Admin.Core.ViewModels.Widgets;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;

    using NLog;

    /// <summary>
    /// The shell controller for icenter.admin.
    /// </summary>
    [Export(typeof(IAdminShellController)), PartCreationPolicy(CreationPolicy.Shared)]
    public partial class AdminShellController : WindowControllerBase, IAdminShellController, IWeakEventListener
    {
        private const int MaxLastEditedEntries = 20;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICommandRegistry commandRegistry;

        private readonly DataController dataController;

        private readonly List<EntityStageControllerBase> stageControllers = new List<EntityStageControllerBase>();

        private readonly Dictionary<int, UnitConfiguratorController> unitConfigurators =
            new Dictionary<int, UnitConfiguratorController>();

        private AboutScreenPrompt aboutScreenPrompt;

        private OptionsController optionsController;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminShellController"/> class.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="commandRegistry">the command registry</param>
        [ImportingConstructor]
        public AdminShellController(IAdminShell shell, ICommandRegistry commandRegistry)
            : base(shell)
        {
            this.commandRegistry = commandRegistry;
            this.RegisterCommands();

            this.dataController = new DataController(new DataViewModelFactory(this.commandRegistry));

            this.Shell.Closing += this.ShellOnClosing;
            this.Shell.Closed += this.ShellOnClosed;
            this.Shell.Created += this.ShellOnCreated;

            this.LoadControllers();

            var index = this.stageControllers.FindIndex(c => c is SystemConfigStageController);
            this.stageControllers.RemoveAt(index);
            this.stageControllers.Insert(
                index,
                new BackgroundSystemSettingsStageController(this.dataController.SystemConfig));
            index++;
            this.stageControllers.Insert(index, new FtpServerStageController(this.dataController.SystemConfig));

            PropertyChangedEventManager.AddListener(this.Shell, this, "CurrentStage");
        }

        IShellViewModel IShellController.Shell
        {
            get
            {
                return this.Shell;
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public IAdminShell Shell
        {
            get
            {
                return this.Window as IAdminShell;
            }
        }

        /// <summary>
        /// Gets the options controller.
        /// </summary>
        public OptionsController OptionsController
        {
            get
            {
                return this.optionsController;
            }
        }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        [Import]
        public IAdminApplicationController ApplicationController { get; private set; }

        /// <summary>
        /// Gets the removable media controller.
        /// </summary>
        public RemovableMediaListController RemovableMedia { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.DisposeControllers();
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">
        /// The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>
        /// true if the listener handled the event.
        /// It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF
        /// to register a listener for an event that the listener does not handle.
        /// Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var args = (PropertyChangedEventArgs)e;
            return this.OnPropertyChanged(args);
        }

        private static string GetDisplayText(ReadOnlyDataViewModelBase readOnly)
        {
            return string.Format("{0} (id={1})", readOnly.DisplayText, readOnly.GetIdString());
        }

        private void InitializeControllers()
        {
            this.optionsController = new OptionsController(this.commandRegistry, "Admin", "AdminApplication");
            this.RemovableMedia = new RemovableMediaListController(this.Shell, this.commandRegistry);
            this.RemovableMedia.Initialize(this.ApplicationController.ConnectionController);

            this.dataController.Initialize(this.ApplicationController.ConnectionController);
            foreach (var stageController in this.stageControllers)
            {
                stageController.Initialize();

                this.Shell.EntityStages.Add(stageController.StageViewModel);

                var partition =
                    this.Shell.Navigator.Partitions.FirstOrDefault(p => p.Name == stageController.PartitionName);
                if (partition == null)
                {
                    partition = new PartitionViewModel(stageController.PartitionName);
                    this.Shell.Navigator.Partitions.Add(partition);
                }

                var navigatorEntity = new NavigatorEntityViewModel(stageController.Name)
                                          {
                                              DisplayName =
                                                  stageController.StageViewModel
                                                  .PluralDisplayName,
                                              IsAllowed = stageController.StageViewModel.IsAllowed
                                          };
                partition.Entities.Add(navigatorEntity);
                stageController.StageViewModel.PropertyChanged += (sender, e) =>
                    {
                        var stage = sender as EntityStageViewModelBase;
                        if (stage == null || e.PropertyName != "IsAllowed")
                        {
                            return;
                        }

                        navigatorEntity.IsAllowed = stage.IsAllowed;
                        partition.FilteredEntities.Refresh();
                        this.Shell.Navigator.FilteredPartitions.Refresh();
                        this.Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                    {
                                        if (!navigatorEntity.IsAllowed && this.Shell.CurrentStage == stage)
                                        {
                                            this.GoHome();
                                        }
                                    }),
                            DispatcherPriority.Background);
                    };
            }

            this.Shell.Navigator.FilteredPartitions.Refresh();
            this.GoHome();
        }

        private void DisposeControllers()
        {
            this.CancelEditingEntity();

            this.Shell.EntityStages.Clear();
            this.Shell.Navigator.Partitions.Clear();

            this.Shell.HomeStage.RecentlyEditedWidget.RecentlyEditedEntities.Clear();

            if (this.RemovableMedia != null)
            {
                this.RemovableMedia.Dispose();
                this.RemovableMedia = null;
            }
        }

        private void RegisterCommands()
        {
            this.RegisterShellCommands();

            this.RegisterEntityCommands();

            this.RegisterEditorCommands();
        }

        private void RegisterShellCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowFileMenu,
                new RelayCommand(this.ShowFileMenu));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Navigator.GoHome,
                new RelayCommand(this.GoHome, this.CanGoHome));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Widgets.NavigateToRecentEntity,
                new RelayCommand<RecentlyEditedEntityViewModel>(
                    this.NavigateToRecentEntity,
                    this.CanNavigateToRecentEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowAboutScreen,
                new RelayCommand(this.ShowAboutScreen));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.UI.ShowOptionsDialog,
                new RelayCommand(this.ShowOptionsDialog));
        }

        private void RegisterEntityCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.Add,
                new RelayCommand(this.AddEntity, this.CanAddEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.Edit,
                new RelayCommand<ReadOnlyDataViewModelBase>(this.EditEntity, this.CanEditEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.Copy,
                new RelayCommand<ReadOnlyDataViewModelBase>(this.CopyEntity, this.CanCopyEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.Delete,
                new RelayCommand<ReadOnlyDataViewModelBase>(this.DeleteEntity, this.CanDeleteEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.FilterColumn,
                new RelayCommand<ColumnVisibilityParameters>(this.FilterColumn));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.UpdateColumnVisibility,
                new RelayCommand<ColumnVisibilityParameters>(this.UpdateColumnVisibility));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.NavigateTo,
                new RelayCommand<ReadOnlyDataViewModelBase>(this.NavigateToEntity, this.CanNavigateToEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.LoadDetails,
                new RelayCommand<ReadOnlyDataViewModelBase>(this.LoadEntityDetails, this.CanLoadEntityDetails));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Entities.UnitConfiguration.Edit,
                new RelayCommand<UnitConfigurationReadOnlyDataViewModel>(
                    this.EditUnitConfiguration,
                    this.CanEditUnitConfiguration));
        }

        private void RegisterEditorCommands()
        {
            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.Save,
                new RelayCommand(this.SaveEditingEntityAsync, this.CanSaveEditingEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.Create,
                new RelayCommand(this.SaveEditingEntityAsync, this.CanSaveEditingEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.CancelEdit,
                new RelayCommand(this.CancelEditingEntity, this.CanCancelEditingEntity));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.UpdatePropertyDisplay,
                new RelayCommand<PropertyDisplayParameters>(this.UpdatePropertyDisplay));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.PackageVersion.AddFolder,
                new RelayCommand<PackageVersionDataViewModel>(
                    this.AddPackageVersionFolder,
                    this.CanAddPackageVersionFolder));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.PackageVersion.AddFile,
                new RelayCommand<PackageVersionDataViewModel>(
                    this.AddPackageVersionFile,
                    this.CanAddPackageVersionFile));

            this.commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Editor.PackageVersion.DeleteItem,
                new RelayCommand<PackageVersionDataViewModel>(
                    this.DeletePackageVersionItem,
                    this.CanDeletePackageVersionItem));
        }

        private EntityStageControllerBase GetCurrentStageController()
        {
            return this.stageControllers.FirstOrDefault(c => c.StageViewModel == this.Shell.CurrentStage);
        }

        private EntityStageControllerBase GetStageController(RecentlyEditedEntityReference reference)
        {
            var controller =
                this.stageControllers.FirstOrDefault(
                    c => c.PartitionName == reference.Partition && c.Name == reference.Entity);
            return controller;
        }

        private EntityStageControllerBase GetStageController(ReadOnlyDataViewModelBase dataViewModel)
        {
            return this.stageControllers.FirstOrDefault(c => c.SupportsEntity(dataViewModel));
        }

        private EntityStageControllerBase GetStageController(DataViewModelBase dataViewModel)
        {
            return this.stageControllers.FirstOrDefault(c => c.SupportsEntity(dataViewModel));
        }

        private void ShowFileMenu()
        {
            Logger.Debug("Request to show the main menu.");
            InteractionManager<FileMenuPrompt>.Current.Raise(new FileMenuPrompt(this.Shell, this.commandRegistry));
        }

        private bool CanGoHome(object obj)
        {
            return true;
        }

        private void GoHome()
        {
            this.Shell.CurrentStage = this.Shell.HomeStage;
        }

        private bool CanNavigateToRecentEntity(RecentlyEditedEntityViewModel entity)
        {
            return entity != null;
        }

        private async void NavigateToRecentEntity(RecentlyEditedEntityViewModel entity)
        {
            var controller = this.GetStageController(entity.Reference);
            if (controller == null || !controller.StageViewModel.IsAllowed)
            {
                return;
            }

            this.Shell.CurrentStage = controller.StageViewModel;
            try
            {
                await controller.NavigateToAsync(entity.Reference.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    "Couldn't navigate to " + entity.Reference.Entity + " id=" + entity.Reference.Id);
                var message = string.Format(
                    AdminStrings.ServerError_NavigateTo_MessageFormat,
                    controller.StageViewModel.SingularDisplayName,
                    entity.DisplayName);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_NavigateTo_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private bool CanAddEntity(object obj)
        {
            var stage = this.Shell.CurrentStage as EntityStageViewModelBase;
            return stage != null && !stage.IsLoading && stage.CanCreate && this.Shell.Editor.EditingEntity == null;
        }

        private async void AddEntity()
        {
            var controller = this.ShowEditor();
            if (controller == null)
            {
                return;
            }

            try
            {
                this.Shell.Editor.EditingEntity = await controller.DataController.CreateEntityAsync();
                this.Shell.Editor.IsNewEntity = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, controller.Name + "Couldn't add entity ");
                var message = string.Format(
                    AdminStrings.ServerError_AddEntity_MessageFormat,
                    controller.StageViewModel.SingularDisplayName);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_NavigateTo_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private bool CanEditEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                return false;
            }

            var controller = this.GetCurrentStageController();
            if (controller == null)
            {
                return false;
            }

            var stage = controller.StageViewModel;
            return stage != null && !stage.IsLoading && stage.CanWrite
                   && (this.Shell.Editor.EditingEntity == null
                       || !dataViewModel.Equals(this.Shell.Editor.EditingEntity.ReadOnlyDataViewModel))
                       && controller.CanEditEntity(dataViewModel);
        }

        private async void EditEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                return;
            }

            var controller = this.ShowEditor();
            if (controller == null)
            {
                return;
            }

            try
            {
                this.Shell.Editor.IsNewEntity = false;
                this.Shell.Editor.EditingEntity = await controller.DataController.EditEntityAsync(dataViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, controller.Name + "Couldn't edit entity ");
                var message = string.Format(
                    AdminStrings.ServerError_EditEntity_MessageFormat,
                    controller.StageViewModel.SingularDisplayName);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_NavigateTo_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private bool CanCopyEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                return false;
            }

            if (dataViewModel is UnitConfigurationReadOnlyDataViewModel)
            {
                // TODO Is a temporary fix that a unitconfiguration cannot be copied
                return false;
            }

            var controller = this.GetCurrentStageController();
            if (controller == null)
            {
                return false;
            }

            var stage = controller.StageViewModel;
            return stage != null && !stage.IsLoading && stage.CanCreate
                   && this.Shell.Editor.EditingEntity == null
                   && controller.CanCopyEntity(dataViewModel);
        }

        private async void CopyEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                return;
            }

            var controller = this.ShowEditor();
            if (controller == null)
            {
                return;
            }

            try
            {
                this.Shell.Editor.IsNewEntity = true;
                this.Shell.Editor.EditingEntity = await controller.DataController.CopyEntityAsync(dataViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, controller.Name + "Couldn't copy entity ");
                var message = string.Format(
                    AdminStrings.ServerError_CopyEntity_MessageFormat,
                    controller.StageViewModel.SingularDisplayName);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_NavigateTo_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private EntityStageControllerBase ShowEditor()
        {
            var controller = this.GetCurrentStageController();
            if (controller != null)
            {
                this.Shell.Editor.EntityTypeName = controller.StageViewModel.EntityName;
                this.Shell.Editor.EntityTypeDisplayName = controller.StageViewModel.SingularDisplayName;
            }

            return controller;
        }

        private bool CanDeleteEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                return false;
            }

            var controller = this.GetCurrentStageController();
            if (controller == null)
            {
                return false;
            }

            var stage = controller.StageViewModel;
            return stage != null && !stage.IsLoading && stage.CanDelete
                   && (this.Shell.Editor.EditingEntity == null
                       || !dataViewModel.Equals(this.Shell.Editor.EditingEntity.ReadOnlyDataViewModel))
                       && controller.CanDeleteEntity(dataViewModel);
        }

        private async void DeleteEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            var controller = this.GetCurrentStageController();
            if (controller == null)
            {
                return;
            }

            var message = string.Format(
                AdminStrings.Requests_Delete_MessageFormat,
                controller.StageViewModel.SingularDisplayName,
                dataViewModel.DisplayText,
                dataViewModel.GetIdString());
            var messageBox = MessageBox.Show(
                message,
                AdminStrings.Requests_Delete_Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            if (messageBox != MessageBoxResult.Yes)
            {
                return;
            }

            controller.StageViewModel.IsLoading = true;
            try
            {
                await controller.DataController.DeleteEntityAsync(dataViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't delete entity");
                message = string.Format(
                    AdminStrings.ServerError_DeleteEntity_MessageFormat,
                    controller.StageViewModel.SingularDisplayName,
                    dataViewModel.DisplayText);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_DeleteEntity_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }

            controller.StageViewModel.IsLoading = false;
        }

        private bool CanSaveEditingEntity(object obj)
        {
            return this.Shell.Editor.EditingEntity != null && !this.Shell.Editor.IsSaving
                   && !this.Shell.Editor.EditingEntity.HasErrors;
        }

        private async void SaveEditingEntityAsync()
        {
            var entity = this.Shell.Editor.EditingEntity;
            if (entity == null || entity.HasErrors)
            {
                return;
            }

            var controller = this.GetStageController(entity);

            this.Shell.Editor.IsSaving = true;
            ReadOnlyDataViewModelBase readOnly;
            try
            {
                readOnly = await controller.DataController.SaveEntityAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't save entity");
                var message = string.Format(
                    AdminStrings.ServerError_SaveEntity_MessageFormat,
                    controller.StageViewModel.SingularDisplayName,
                    entity.DisplayText);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_SaveEntity_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
                return;
            }
            finally
            {
                this.Shell.Editor.IsSaving = false;
            }

            this.Shell.Editor.EditingEntity = null;

            var widget = this.Shell.HomeStage.RecentlyEditedWidget;
            var reference = new RecentlyEditedEntityReference(
                controller.PartitionName,
                controller.Name,
                readOnly.GetIdString());
            var previous = widget.RecentlyEditedEntities.FirstOrDefault(e => e.Reference.Equals(reference));
            if (previous != null)
            {
                widget.RecentlyEditedEntities.Remove(previous);
            }

            widget.RecentlyEditedEntities.Insert(
                0,
                new RecentlyEditedEntityViewModel(reference) { DisplayName = GetDisplayText(readOnly) });

            while (widget.RecentlyEditedEntities.Count > MaxLastEditedEntries)
            {
                widget.RecentlyEditedEntities.RemoveAt(MaxLastEditedEntries);
            }

            entity.Dispose();
        }

        private bool CanCancelEditingEntity(object obj)
        {
            return this.Shell.Editor.EditingEntity != null && !this.Shell.Editor.IsSaving;
        }

        private void CancelEditingEntity()
        {
            if (this.Shell.Editor.EditingEntity == null)
            {
                return;
            }

            this.Shell.Editor.EditingEntity.Dispose();
            this.Shell.Editor.EditingEntity = null;
        }

        private void UpdatePropertyDisplay(PropertyDisplayParameters parameters)
        {
            var controller = this.GetStageController(parameters.EditingEntity);
            if (controller != null)
            {
                controller.UpdatePropertyDisplay(parameters);
            }
        }

        private bool CanAddPackageVersionFolder(PackageVersionDataViewModel packageVersion)
        {
            return packageVersion != null
                   && (packageVersion.SelectedItem == null || !packageVersion.SelectedItem.IsEditing);
        }

        private void AddPackageVersionFolder(PackageVersionDataViewModel packageVersion)
        {
            var parent = packageVersion.SelectedItem as FolderItem;
            var folder = new FolderItem { Name = "New Folder" };
            if (parent == null)
            {
                packageVersion.RootFolders.Add(folder);
            }
            else
            {
                parent.IsExpanded = true;
                parent.Children.Add(folder);
            }

            folder.IsSelected = true;
            folder.IsEditing = true;
        }

        private bool CanAddPackageVersionFile(PackageVersionDataViewModel packageVersion)
        {
            return packageVersion != null && packageVersion.SelectedItem is FolderItem
                   && !packageVersion.SelectedItem.IsEditing;
        }

        private void AddPackageVersionFile(PackageVersionDataViewModel packageVersion)
        {
            var folder = packageVersion.SelectedItem as FolderItem;
            if (folder == null)
            {
                return;
            }

            var interaction = new OpenFileDialogInteraction
                                  {
                                      Filter = AdminStrings.FileDialog_AllFilesFilter,
                                      MultiSelect = true,
                                      Title = AdminStrings.FileDialog_AddFiles
                                  };
            InteractionManager<OpenFileDialogInteraction>.Current.Raise(
                interaction,
                i => this.PackageVersionFileAdded(i, folder));
        }

        private void PackageVersionFileAdded(OpenFileDialogInteraction interaction, FolderItem folder)
        {
            if (!interaction.Confirmed)
            {
                return;
            }

            foreach (var fileName in interaction.FileNames)
            {
                folder.Children.Add(new FileItem { OriginalFileName = fileName });
            }

            folder.IsExpanded = true;
        }

        private bool CanDeletePackageVersionItem(PackageVersionDataViewModel packageVersion)
        {
            return packageVersion != null && packageVersion.SelectedItem != null
                   && !packageVersion.SelectedItem.IsEditing;
        }

        private void DeletePackageVersionItem(PackageVersionDataViewModel packageVersion)
        {
            var parent = this.FindParent(packageVersion.SelectedItem, null, packageVersion.RootFolders);
            if (parent != null)
            {
                parent.Children.Remove(packageVersion.SelectedItem);
                return;
            }

            var folder = packageVersion.SelectedItem as FolderItem;
            if (folder != null)
            {
                packageVersion.RootFolders.Remove(folder);
            }
        }

        private FolderItem FindParent(FileSystemItemBase item, FolderItem parent, IEnumerable<FileSystemItemBase> items)
        {
            if (item == null)
            {
                return null;
            }

            foreach (var subItem in items)
            {
                if (subItem == item)
                {
                    return parent;
                }

                var folder = subItem as FolderItem;
                if (folder == null)
                {
                    continue;
                }

                var found = this.FindParent(item, folder, folder.Children);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private bool CanEditUnitConfiguration(UnitConfigurationReadOnlyDataViewModel unitConfiguration)
        {
            return this.CanEditEntity(unitConfiguration);
        }

        private void EditUnitConfiguration(UnitConfigurationReadOnlyDataViewModel unitConfiguration)
        {
            UnitConfiguratorController configuratorController;
            if (this.unitConfigurators.TryGetValue(unitConfiguration.Id, out configuratorController))
            {
                configuratorController.BringToFront();
                return;
            }

            configuratorController = new UnitConfiguratorController(unitConfiguration, this.dataController);
            configuratorController.WindowClosed += (s, e) => this.unitConfigurators.Remove(unitConfiguration.Id);
            this.unitConfigurators.Add(unitConfiguration.Id, configuratorController);
            configuratorController.Show();
        }

        private void FilterColumn(ColumnVisibilityParameters parameters)
        {
            var controller = this.stageControllers.FirstOrDefault(c => c.Name == parameters.EntityName);
            if (controller == null)
            {
                return;
            }

            parameters.Visibility = controller.FilterColumn(parameters.ColumnName);
        }

        private void UpdateColumnVisibility(ColumnVisibilityParameters parameters)
        {
            var controller = this.stageControllers.FirstOrDefault(c => c.Name == parameters.EntityName);
            if (controller == null)
            {
                return;
            }

            controller.SetColumnVisibility(parameters.ColumnName, parameters.Visibility);
        }

        private bool CanLoadEntityDetails(ReadOnlyDataViewModelBase dataViewModel)
        {
            return dataViewModel != null;
        }

        private async void LoadEntityDetails(ReadOnlyDataViewModelBase dataViewModel)
        {
            var controller = this.GetStageController(dataViewModel);
            if (controller == null)
            {
                return;
            }

            try
            {
                await controller.DataController.LoadEntityDetailsAsync(dataViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't load entity details of " + dataViewModel);
                var message = string.Format(
                    AdminStrings.ServerError_LoadEntityDetails_MessageFormat,
                    controller.StageViewModel.SingularDisplayName,
                    dataViewModel.DisplayText);
                var prompt = new ConnectionExceptionPrompt(
                    ex, message, AdminStrings.ServerError_LoadEntityDetails_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private bool CanNavigateToEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return dataViewModel != null;
        }

        private async void NavigateToEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            var controller = this.GetStageController(dataViewModel);
            if (controller == null || !controller.StageViewModel.IsAllowed)
            {
                return;
            }

            try
            {
                var task = controller.NavigateToAsync(dataViewModel.GetIdString());
                this.Shell.CurrentStage = controller.StageViewModel;
                await task;
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Couldn't navigate to " + dataViewModel);
                var message = string.Format(
                    AdminStrings.ServerError_NavigateTo_MessageFormat,
                    controller.StageViewModel.SingularDisplayName,
                    dataViewModel.DisplayText);
                var prompt = new ConnectionExceptionPrompt(ex, message, AdminStrings.ServerError_NavigateTo_Title);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        private void LoadRecentlyEditedEntities()
        {
            var allEntities = this.Shell.AdminApplicationState.RecentlyEditedEntities;
            if (allEntities == null)
            {
                allEntities =
                    this.Shell.AdminApplicationState.RecentlyEditedEntities =
                    new Dictionary<string, IList<RecentlyEditedEntityReference>>();
            }

            IList<RecentlyEditedEntityReference> entities;
            if (!allEntities.TryGetValue(this.Shell.AdminApplicationState.LastServer, out entities))
            {
                entities = new RecentlyEditedEntityReference[0];
            }

            for (int i = 0; i < entities.Count && i < MaxLastEditedEntries; i++)
            {
                var reference = entities[i];

                this.Shell.HomeStage.RecentlyEditedWidget.RecentlyEditedEntities.Add(
                    new RecentlyEditedEntityViewModel(reference) { DisplayName = reference.Id });
            }

            this.LoadRecentlyEditedEntityDisplayNamesAsync().ContinueWith(
                t =>
                    {
                        if (t.IsFaulted && t.Exception != null)
                        {
                            Logger.Error(t.Exception.Flatten(),"Couldn't load recently edited entitites");
                        }
                    });
        }

        private async Task LoadRecentlyEditedEntityDisplayNamesAsync()
        {
            var widget = this.Shell.HomeStage.RecentlyEditedWidget;
            var references = widget.RecentlyEditedEntities.ToDictionary(e => e, e => e.Reference);
            foreach (var reference in references)
            {
                var viewModel = reference.Key;
                var readOnly = await this.GetReadOnlyViewModelAsync(reference.Value);
                if (readOnly == null)
                {
                    widget.RecentlyEditedEntities.Remove(viewModel);
                }
                else
                {
                    viewModel.DisplayName = GetDisplayText(readOnly);
                }
            }
        }

        private async Task<ReadOnlyDataViewModelBase> GetReadOnlyViewModelAsync(RecentlyEditedEntityReference reference)
        {
            var controller = this.GetStageController(reference);
            if (controller == null)
            {
                return null;
            }

            return await controller.DataController.GetEntityAsync(reference.Id);
        }

        private void SaveRecentlyEditedEntities()
        {
            this.Shell.AdminApplicationState.RecentlyEditedEntities[this.Shell.AdminApplicationState.LastServer] =
                this.Shell.HomeStage.RecentlyEditedWidget.RecentlyEditedEntities.Select(e => e.Reference).ToList();
        }

        private bool OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "CurrentStage")
            {
                return true;
            }

            var controller = this.stageControllers.FirstOrDefault(c => c.Name == this.Shell.CurrentStage.Name);
            if (controller != null)
            {
                controller.LoadData();
            }

            return true;
        }

        private void ShellOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            this.SaveRecentlyEditedEntities();
            this.RaiseWindowClosing(cancelEventArgs);
        }

        private void ShellOnClosed(object sender, EventArgs eventArgs)
        {
            this.RaiseWindowClosed();
            this.DisposeControllers();
        }

        private void ShellOnCreated(object sender, EventArgs eventArgs)
        {
            this.Shell.Title = AdminShell.DefaultWindowTitle + " - " + this.Shell.AdminApplicationState.LastServer;
            this.InitializeControllers();
            this.LoadRecentlyEditedEntities();

            this.Shell.AdminApplicationState.PropertyChanged += this.AdminApplicationStateOnPropertyChanged;

            var stages = this.Shell.AdminApplicationState.Stages;
            if (stages == null)
            {
                stages = this.Shell.AdminApplicationState.Stages = new List<StageModel>();
            }

            foreach (var controller in this.stageControllers)
            {
                var stage = stages.FirstOrDefault(s => s.Name == controller.Name);
                if (stage == null)
                {
                    stage = new StageModel(controller.Name);
                    stages.Add(stage);
                }

                controller.Model = stage;
            }
        }

        private void AdminApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CancelEditingEntity();
        }

        private void ShowAboutScreen()
        {
            Logger.Debug("Showing about screen");
            if (this.aboutScreenPrompt == null)
            {
                var applicationIcon = new BitmapImage();
                applicationIcon.BeginInit();
                applicationIcon.UriSource =
                    new Uri("pack://application:,,,/Gorba.Center.Admin.Core;component/Resources/admin_196x196.png");
                applicationIcon.EndInit();
                this.aboutScreenPrompt = new AboutScreenPrompt { ApplicationIconSource = applicationIcon };
            }

            InteractionManager<AboutScreenPrompt>.Current.Raise(this.aboutScreenPrompt);
        }

        private void ShowOptionsDialog()
        {
            Logger.Debug("Showing options dialog");

            var optionsPrompt = new OptionsPrompt(this.commandRegistry, this.Shell.AdminApplicationState.Options);
            var category = optionsPrompt.Categories.First();
            category.Title = FrameworkStrings.OptionsDialog_GeneralTitle;
            category.TitleTooltip = FrameworkStrings.OptionsDialog_GeneralTooltip;
            var generalIcon = new BitmapImage();
            generalIcon.BeginInit();
            generalIcon.UriSource =
                new Uri(
                    "pack://application:,,,/Gorba.Center.Common.Wpf.Views;component/Icons/application-gear_32x32.png");
            generalIcon.EndInit();
            category.CategoryIconSource = generalIcon;
            var group = category.Groups.First();
            group.Label = FrameworkStrings.OptionsDialog_LanguageLabel;
            group.GroupLabelTooltip = FrameworkStrings.OptionsDialog_LanguageLabelTooltip;
            ((LanguageOptionGroupViewModel)group).RestartInformation =
               FrameworkStrings.OptionsDialog_LanguageRestartInformation;
            optionsPrompt.SelectedCategory = optionsPrompt.Categories.First();
            InteractionManager<OptionsPrompt>.Current.Raise(optionsPrompt);
        }
    }
}