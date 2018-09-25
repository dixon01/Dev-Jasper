// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportExecutionPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The export execution part controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.MainUnit;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Client.Views;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The export execution part controller.
    /// </summary>
    public class ExportExecutionPartController : PartControllerBase<ExportExecutionPartViewModel>
    {
        private static readonly DateTime DefaultStartTime = new DateTime(2000, 1, 1);

        private static readonly DateTime DefaultEndTime = new DateTime(2100, 12, 31);

        private readonly bool isEpaperUnit;

        private readonly IAdminApplicationState applicationState;

        private ExportPreparationPartController exportPreparation;

        private LocalDownloadPartController localDownload;

        private PreInstallationActionPartController preInstallationAction;

        private PostInstallationActionPartController postInstallationAction;

        private MainUnitConfigPartController mainUnitPart;

        private List<DisplayUnitPartController> displayUnitParts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportExecutionPartController"/> class.
        /// </summary>
        /// <param name="parent">
        ///     The parent controller.
        /// </param>
        /// <param name="isEpaperUnit">
        /// Indicating if the unit to be configured is a e-paper unit
        /// </param>
        public ExportExecutionPartController(CategoryControllerBase parent, bool isEpaperUnit)
            : base(UnitConfigKeys.Conclusion.ExportExecution, parent)
        {
            this.isEpaperUnit = isEpaperUnit;
            this.applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
        }

        /// <summary>
        /// Asynchronously prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override async Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.exportPreparation = this.GetPart<ExportPreparationPartController>();
            this.exportPreparation.ViewModelUpdated += this.ExportPreparationOnViewModelUpdated;

            if (this.isEpaperUnit)
            {
                this.mainUnitPart = this.GetPart<MainUnitConfigPartController>();
                this.mainUnitPart.ViewModelUpdated += this.OnMainUnitConfigPartUpdated;

                this.displayUnitParts = new List<DisplayUnitPartController>();

                var displayUnitsCount = HardwareDescriptors.PowerUnit.GetDisplayUnitCount(descriptor.Name);
                for (var i = 0; i < displayUnitsCount; i++)
                {
                    var displayUnitPart =
                        this.GetPart<DisplayUnitPartController>(UnitConfigKeys.MainUnit.DisplayUnit + (i + 1));
                    displayUnitPart.ViewModelUpdated += this.OnMainUnitConfigPartUpdated;
                    this.displayUnitParts.Add(displayUnitPart);
                }
            }
            else
            {
                this.preInstallationAction = this.GetPart<PreInstallationActionPartController>();
                this.preInstallationAction.ViewModelUpdated += this.ExportPreparationOnViewModelUpdated;

                this.postInstallationAction = this.GetPart<PostInstallationActionPartController>();
                this.postInstallationAction.ViewModelUpdated += this.ExportPreparationOnViewModelUpdated;

                this.localDownload = this.GetPart<LocalDownloadPartController>();
            }

            this.Parent.Parent.ViewModel.PropertyChanged += this.UnitConfiguratorViewModelOnPropertyChanged;

            var unitConfiguration = this.Parent.Parent.UnitConfiguration.ReadableModel;
            var updateGroups = this.Parent.Parent.DataController.UpdateGroup;
            await Task.WhenAll(
                updateGroups.AwaitAllDataAsync(),
                unitConfiguration.LoadNavigationPropertiesAsync());

            foreach (var updateGroup in updateGroups.All.Select(u => u.ReadableModel))
            {
                this.ViewModel.Editor.UpdateGroups.Options.Add(
                    new CheckableOptionViewModel(
                        updateGroup.Name,
                        updateGroup,
                        unitConfiguration.UpdateGroups.Contains(updateGroup)));
            }

            this.UpdateVisibility();
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            // nothing to load
            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            // nothing to save
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="ExportExecutionPartViewModel"/>.
        /// </returns>
        protected override ExportExecutionPartViewModel CreateViewModel()
        {
            var viewModel = new ExportExecutionPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Conclusion_ExportExecution;
            viewModel.Description = AdminStrings.UnitConfig_Conclusion_ExportExecution_Description;
            viewModel.Editor.ExportCommand = new RelayCommand(this.Export, this.CanExport);
            viewModel.Editor.PropertyChanged += this.EditorOnPropertyChanged;
            return viewModel;
        }

        private static void AddStaticContent(
           List<Tuple<DisplayUnitEditorViewModel, decimal>> staticDisplayUnitEditors,
           DynamicContentInfo dynamicContentInfo)
        {
            foreach (var displayUnitEditor in staticDisplayUnitEditors)
            {
                var staticFileSourceHash = displayUnitEditor.Item1.StaticContentHash;
                var contentPart = new EPaperDynamicContentPart
                {
                    IsPersistentFile = true,
                    StaticFileSourceHash = staticFileSourceHash,
                    DisplayUnitIndex = (int)displayUnitEditor.Item2
                };
                dynamicContentInfo.Parts.Add(contentPart);
            }
        }

        private static void AddDynamicContent(
            List<Tuple<DisplayUnitEditorViewModel, decimal>> orderedDynamicDisplayUnitEditors,
            DynamicContentInfo dynamicContentInfo)
        {
            foreach (var displayUnitEditor in orderedDynamicDisplayUnitEditors)
            {
                var contentPart = new EPaperDynamicContentPart { Url = displayUnitEditor.Item1.DynamicContentUrl };
                dynamicContentInfo.Parts.Add(contentPart);
            }
        }

        private static void ShowExportErrorMessage(string errorMessages)
        {
            MessageBox.Show(
                errorMessages,
                AdminStrings.UnitConfiguration_Export_ErrorTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private async Task<bool> EnsureIsCheckedIn()
        {
            if (!this.Parent.Parent.ViewModel.IsDirty)
            {
                return true;
            }

            var checkinCompleted = new TaskCompletionSource<CheckinTrapResult>();
            var checkinArguments = new CheckinDialogArguments
            {
                OnCheckinCompleted = async checkinResult =>
                {
                    await this.Parent.Parent.CheckInAsync(checkinResult);
                    checkinCompleted.TrySetResult(checkinResult);
                }
            };

            this.Parent.Parent.CommandRegistry.GetCommand(CommandCompositionKeys.UnitConfig.Commit)
                .Execute(checkinArguments);
            var result = await checkinCompleted.Task;
            if (result.Decision == CheckinUserDecision.Cancel)
            {
                this.ViewModel.Editor.IsExporting = false;
                return false;
            }

            return true;
        }

        private async void Export()
        {
            var connectionController = this.Parent.Parent.DataController.ConnectionController;
            this.ViewModel.Editor.ExportProgress = 0;
            this.ViewModel.Editor.IsExporting = true;

            try
            {
                if (!await this.EnsureIsCheckedIn())
                {
                    return;
                }

                var resourceHashes = await this.UploadResourcesAsync();
                this.ViewModel.Editor.ExportItemName =
                    AdminStrings.UnitConfig_Conclusion_ExportExecution_FolderStructure;
                var folderStructure = this.CreateFolderStructure(resourceHashes);

                var updateGroups =
                    this.ViewModel.Editor.UpdateGroups.GetCheckedValues().Cast<UpdateGroupReadableModel>().ToList();
                var hasExportError = false;
                if (!this.isEpaperUnit)
                {
                    var installInstructions = this.CreateInstallationInstructions(resourceHashes);
                    await this.CreateUpdatePartsAsync(updateGroups, folderStructure, installInstructions);
                }
                else
                {
                    // content of display units is passed to the BS as dynamic update parts
                    var dynamicContent = this.CreateDynamicContent();
                    var description = this.CreateDescription();
                    var errorMessages = string.Empty;
                    foreach (var updateGroupReadableModel in updateGroups)
                    {
                        try
                        {
                            await this.CreateEPaperUpdatePartAsync(
                                                    updateGroupReadableModel,
                                                    connectionController,
                                                    description,
                                                    dynamicContent,
                                                    folderStructure);
                        }
                        catch (Exception ex)
                        {
                            hasExportError = true;
                            errorMessages += "- " + ex.Message + System.Environment.NewLine;
                        }
                    }

                    if (hasExportError)
                    {
                        ShowExportErrorMessage(errorMessages);
                    }
                }

                this.exportPreparation.ViewModel.Editor.ClearHasChanges();
                this.ViewModel.Editor.WasExported = !hasExportError;

                if (!this.isEpaperUnit)
                {
                    this.localDownload.SetFolderStructure(folderStructure);
                }

                foreach (var updateGroup in updateGroups)
                {
                    var editableGroup = updateGroup.ToChangeTrackingModel();
                    editableGroup.UnitConfiguration = this.Parent.Parent.UnitConfiguration.ReadableModel;
                    await connectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(editableGroup);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.ViewModel.Editor.IsExporting = false;
            }
        }

        private DynamicContentInfo CreateDynamicContent()
        {
            var displayDynamicContentUnitEditors = new List<Tuple<DisplayUnitEditorViewModel, decimal>>();
            var displayStaticContentUnitEditors = new List<Tuple<DisplayUnitEditorViewModel, decimal>>();
            foreach (var displayUnitPartController in this.displayUnitParts)
            {
                var editor = displayUnitPartController.ViewModel.Editor;
                if (displayUnitPartController.ViewModel.Editor.IsDynamicContentSelected)
                {
                    displayDynamicContentUnitEditors.Add(
                        new Tuple<DisplayUnitEditorViewModel, decimal>(
                            editor,
                            displayUnitPartController.ViewModel.UnitIndex));
                }

                if (displayUnitPartController.ViewModel.Editor.IsStaticContentSelected)
                {
                    displayStaticContentUnitEditors.Add(
                        new Tuple<DisplayUnitEditorViewModel, decimal>(
                            editor,
                            displayUnitPartController.ViewModel.UnitIndex));
                }
            }

            if (displayDynamicContentUnitEditors.Count == 0 && displayStaticContentUnitEditors.Count == 0)
            {
                return null;
            }

            var dynamicContentInfo = new DynamicContentInfo();
            AddStaticContent(displayStaticContentUnitEditors, dynamicContentInfo);
            AddDynamicContent(displayDynamicContentUnitEditors, dynamicContentInfo);

            return dynamicContentInfo;
        }

        private async Task<UpdatePartReadableModel> CreateEPaperUpdatePartAsync(
            UpdateGroupReadableModel updateGroupReadable,
            IConnectionController connectionController,
            string description,
            DynamicContentInfo dynamicContent,
            UpdateFolderStructure folderStructure)
        {
            await updateGroupReadable.LoadNavigationPropertiesAsync();
            var updatePart = connectionController.UpdatePartChangeTrackingManager.Create();
            await updateGroupReadable.LoadNavigationPropertiesAsync();
            if (updateGroupReadable.UnitConfiguration != null)
            {
                await updateGroupReadable.UnitConfiguration.LoadNavigationPropertiesAsync();
                if (updateGroupReadable.UnitConfiguration.Document != null)
                {
                    await updateGroupReadable.UnitConfiguration.Document.LoadReferencePropertiesAsync();
                }
            }

            updatePart.UpdateGroup = updateGroupReadable;

            if (!updateGroupReadable.Units.Any())
            {
                throw new Exception(
                    string.Format(
                        AdminStrings.UnitConfiguration_Export_Error_NoUnitsInUpdateGroup,
                        updatePart.UpdateGroup.Name));
            }

            if (updateGroupReadable.Units.Count > 1)
            {
                throw new Exception(
                    string.Format(
                    AdminStrings.UnitConfiguration_Export_Error_TooManyUnitsInUpdateGroup,
                        updatePart.UpdateGroup.Name));
            }

            updatePart.Type = UpdatePartType.Presentation;

            var startTime = this.ViewModel.Editor.StartTime.DateTime;
            updatePart.Start = this.ViewModel.Editor.StartTime.HasValue && startTime.HasValue
                                ? startTime.Value : DefaultStartTime;

            var endTime = this.ViewModel.Editor.EndTime.DateTime;
            updatePart.End = this.ViewModel.Editor.EndTime.HasValue && endTime.HasValue
                                ? endTime.Value : DefaultEndTime;

            Action<Tuple<int, EPaperDynamicContentPart>> updateWithUnit = t =>
                {
                    t.Item2.MainUnitId = updatePart.UpdateGroup.Units[0].Id;
                    t.Item2.DisplayUnitIndex = t.Item1 + 1;
                };
            dynamicContent.Parts.OfType<EPaperDynamicContentPart>()
                .Select((p, i) => new Tuple<int, EPaperDynamicContentPart>(i, p))
                .ToList()
                .ForEach(updateWithUnit);
            updatePart.DynamicContent = new XmlData(dynamicContent);

            updatePart.Description = string.IsNullOrEmpty(description) ? string.Empty : description;
            updatePart.Structure = new XmlData(folderStructure);

            var updateReadable = await connectionController.UpdatePartChangeTrackingManager
                .CommitAndVerifyAsync(updatePart);
            return updateReadable;
        }

        private async Task<IDictionary<IExportableFile, string>> UploadResourcesAsync()
        {
            var exportableFiles = new List<IExportableFile>();
            foreach (var folder in this.exportPreparation.ViewModel.Editor.ExportFolders)
            {
                this.AddConfigFiles(exportableFiles, folder);
            }

            if (this.preInstallationAction != null)
            {
                this.AddInstallationActionFiles(exportableFiles, this.preInstallationAction.ViewModel);
            }

            if (this.postInstallationAction != null)
            {
                this.AddInstallationActionFiles(exportableFiles, this.postInstallationAction.ViewModel);
            }

            var resourceHashes = new Dictionary<IExportableFile, string>(exportableFiles.Count);
            var connectionController = this.Parent.Parent.DataController.ConnectionController;
            using (var resourceService = connectionController.CreateChannelScope<IResourceService>())
            {
                for (int i = 0; i < exportableFiles.Count; i++)
                {
                    var configFile = exportableFiles[i];
                    this.ViewModel.Editor.ExportItemName = configFile.DisplayName;
                    var hash = await this.UploadFileAsync(resourceService, configFile);
                    resourceHashes[configFile] = hash;
                    this.ViewModel.Editor.ExportProgress = 80 * (i + 1) / exportableFiles.Count;
                }
            }

            return resourceHashes;
        }

        private UpdateFolderStructure CreateFolderStructure(IDictionary<IExportableFile, string> resourceHashes)
        {
            var folderStructure = new UpdateFolderStructure();
            foreach (var exportFolder in this.exportPreparation.ViewModel.Editor.ExportFolders)
            {
                folderStructure.Folders.Add(this.CreateFolderUpdate(exportFolder, resourceHashes));
            }

            return folderStructure;
        }

        private async Task CreateUpdatePartsAsync(
            IList<UpdateGroupReadableModel> updateGroups,
            UpdateFolderStructure folderStructure,
            InstallationInstructions installationInstructions)
        {
            var connectionController = this.Parent.Parent.DataController.ConnectionController;
            using (var updateService = connectionController.CreateChannelScope<IUpdateService>())
            {
                for (int i = 0; i < updateGroups.Count; i++)
                {
                    var updateGroup = updateGroups[i];
                    this.ViewModel.Editor.ExportItemName = updateGroup.Name;

                    var updatePart = connectionController.UpdatePartChangeTrackingManager.Create();
                    updatePart.Description = this.CreateDescription();
                    updatePart.UpdateGroup = updateGroup;
                    updatePart.Type = UpdatePartType.Setup;
                    updatePart.Structure = new XmlData(folderStructure);
                    updatePart.InstallInstructions = new XmlData(installationInstructions);

                    var startTime = this.ViewModel.Editor.StartTime.DateTime;
                    updatePart.Start = this.ViewModel.Editor.StartTime.HasValue && startTime.HasValue
                                           ? startTime.Value
                                           : DefaultStartTime;

                    var endTime = this.ViewModel.Editor.EndTime.DateTime;
                    updatePart.End = this.ViewModel.Editor.EndTime.HasValue && endTime.HasValue
                                         ? endTime.Value
                                         : DefaultEndTime;

                    await connectionController.UpdatePartChangeTrackingManager.CommitAndVerifyAsync(updatePart);

                    this.ViewModel.Editor.ExportProgress = 80 + (((20 * i) + 10) / updateGroups.Count);

                    await updateService.Channel.CreateUpdateCommandsForUpdateGroupAsync(updateGroup.Id);

                    this.ViewModel.Editor.ExportProgress = 80 + (((20 * i) + 20) / updateGroups.Count);
                }
            }
        }

        private InstallationInstructions CreateInstallationInstructions(
            IDictionary<IExportableFile, string> resourceHashes)
        {
            var installInstructions = new InstallationInstructions
                                          {
                                              PreInstallation = new RunCommands(),
                                              PostInstallation = new RunCommands()
                                          };

            if (this.preInstallationAction != null)
            {
                installInstructions.PreInstallation = this.CreateRunCommands(
                    this.preInstallationAction.ViewModel,
                    resourceHashes);
            }

            if (this.postInstallationAction != null)
            {
                installInstructions.PostInstallation = this.CreateRunCommands(
                    this.postInstallationAction.ViewModel,
                    resourceHashes);
            }

            return installInstructions;
        }

        private RunCommands CreateRunCommands(
            InstallationActionPartViewModel viewModel, IDictionary<IExportableFile, string> resourceHashes)
        {
            var runCommands = new RunCommands();

            // add files first
            foreach (var file in viewModel.Files)
            {
                runCommands.Items.Add(
                    new FileUpdate { Name = Path.GetFileName(file.FileName), Hash = resourceHashes[file] });
            }

            // add actions
            foreach (var action in viewModel.Actions)
            {
                var localExecutable = action as LocalExecutableInstallationActionViewModel;
                if (localExecutable != null)
                {
                    runCommands.Items.Add(
                        new ExecutableFile
                            {
                                Name = Path.GetFileName(localExecutable.ExecutablePathBase),
                                Hash = resourceHashes[localExecutable],
                                Arguments = localExecutable.Arguments
                            });

                    continue;
                }

                var unitExecutable = action as UnitExecutableInstallationActionViewModel;
                if (unitExecutable != null)
                {
                    var runApplication = new RunApplication
                                             {
                                                 Name = unitExecutable.ExecutablePathBase,
                                                 Arguments = unitExecutable.Arguments
                                             };
                    runCommands.Items.Add(runApplication);

                    continue;
                }

                throw new ArgumentException("Action is not local or on unit");
            }

            return runCommands;
        }

        private string CreateDescription()
        {
            return string.Format(
                "Created for unit configuration {0}, version {1}",
                this.Parent.Parent.UnitConfiguration.Document.Name,
                this.Parent.Parent.VersionNumber);
        }

        private void AddConfigFiles(ICollection<IExportableFile> configFiles, ExportFolder folder)
        {
            foreach (var child in folder.ChildItems)
            {
                var subFolder = child as ExportFolder;
                if (subFolder != null)
                {
                    this.AddConfigFiles(configFiles, subFolder);
                    continue;
                }

                var configFile = child as ExportConfigFileBase;
                if (configFile != null)
                {
                    configFiles.Add(configFile);
                }
            }
        }

        private void AddInstallationActionFiles(
            ICollection<IExportableFile> exportableFiles, InstallationActionPartViewModel viewModel)
        {
            foreach (var file in viewModel.Actions.OfType<IExportableFile>())
            {
                exportableFiles.Add(file);
            }

            foreach (var file in viewModel.Files)
            {
                exportableFiles.Add(file);
            }
        }

        private FolderUpdate CreateFolderUpdate(
            ExportFolder exportFolder,
            IDictionary<IExportableFile, string> resourceHashes)
        {
            var folderUpdate = new FolderUpdate { Name = exportFolder.Name };
            foreach (var exportItem in exportFolder.Children)
            {
                var resourceFile = exportItem as ExportResourceFile;
                if (resourceFile != null)
                {
                    var fileUpdate = new FileUpdate { Hash = resourceFile.Resource.Hash, Name = resourceFile.Name };
                    folderUpdate.Items.Add(fileUpdate);
                    continue;
                }

                var configFile = exportItem as ExportConfigFileBase;
                if (configFile != null)
                {
                    var fileUpdate = new FileUpdate { Hash = resourceHashes[configFile], Name = configFile.Name };
                    folderUpdate.Items.Add(fileUpdate);
                    continue;
                }

                var subFolder = exportItem as ExportFolder;
                if (subFolder != null)
                {
                    folderUpdate.Items.Add(this.CreateFolderUpdate(subFolder, resourceHashes));
                }
            }

            return folderUpdate;
        }

        private async Task<string> UploadFileAsync(
            ChannelScope<IResourceService> resourceService, IExportableFile file)
        {
            Stream content;
            string hash;

            var configFile = file as ExportConfigFileBase;
            var localExecutable = file as LocalExecutableInstallationActionViewModel;
            var additionalFile = file as InstallationActionAdditionalFileViewModel;
            if (configFile != null)
            {
                var data = Encoding.UTF8.GetBytes(configFile.Document);
                hash = ResourceHash.Create(data, 0, data.Length);
                var found = await resourceService.Channel.GetAsync(hash);
                if (found != null)
                {
                    return hash;
                }

                content = new MemoryStream(data, false);
            }
            else if (localExecutable != null || additionalFile != null)
            {
                var fileName = localExecutable != null ? localExecutable.ExecutablePathBase : additionalFile.FileName;
                hash = ResourceHash.Create(fileName);
                var found = await resourceService.Channel.GetAsync(hash);
                if (found != null)
                {
                    return hash;
                }

                content = File.OpenRead(fileName);
            }
            else
            {
                throw new NotSupportedException("Don't know how to upload " + file.GetType().FullName);
            }

            var resource = new Resource
                               {
                                   Hash = hash,
                                   Length = content.Length,
                                   UploadingUser = this.applicationState.CurrentUser,
                                   OriginalFilename = file.DisplayName,
                                   MimeType = file.ContentType,
                                   Description = this.CreateDescription()
                               };
            await resourceService.Channel.UploadAsync(
                    new ResourceUploadRequest { Content = content, Resource = resource });
            return hash;
        }

        private bool CanExport(object obj)
        {
            return this.ViewModel.IsVisible && this.ViewModel.Editor.UpdateGroups.GetCheckedOptions().Any();
        }

        private void UpdateVisibility()
        {
            var exportPreparationCheck = this.exportPreparation != null
                                         && !this.exportPreparation.ViewModel.Editor.HasErrors
                                         && !this.exportPreparation.ViewModel.Editor.IsLoading
                                         && !this.exportPreparation.ViewModel.Editor.ShouldReload
                                         && this.exportPreparation.ViewModel.Editor.ExportFolders.Any();

            var epaperUnitCheck = this.isEpaperUnit
                                  && this.mainUnitPart != null
                                  && !this.mainUnitPart.ViewModel.Editor.HasErrors
                                  && this.displayUnitParts.All(d => d.ViewModel.Editor.HasErrors == false);

            var standardtCheck = this.preInstallationAction != null
                                 && !this.preInstallationAction.ViewModel.HasErrors()
                                 && this.postInstallationAction != null
                                 && !this.postInstallationAction.ViewModel.HasErrors();

            this.ViewModel.IsVisible = exportPreparationCheck && (epaperUnitCheck || standardtCheck);
        }

        private void OnMainUnitConfigPartUpdated(object sender, EventArgs eventArgs)
        {
            this.UpdateVisibility();
        }

        private void ExportPreparationOnViewModelUpdated(object sender, EventArgs eventArgs)
        {
            this.UpdateVisibility();
            if (this.exportPreparation.ViewModel.Editor.HasChanges)
            {
                // reset the "was exported" flag as soon as we have manual changes
                this.ViewModel.Editor.WasExported = false;

                if (this.localDownload != null)
                {
                    this.localDownload.SetFolderStructure(null);
                }
            }
        }

        private void UnitConfiguratorViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty" && this.Parent.Parent.ViewModel.IsDirty)
            {
                // reset the "was exported" flag as soon as we have changes
                this.ViewModel.Editor.WasExported = false;

                if (this.localDownload != null)
                {
                    this.localDownload.SetFolderStructure(null);
                }
            }
        }

        private void EditorOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasSelectedUpdateGroups")
            {
                this.UpdateErrors();
            }
        }

        private void UpdateErrors()
        {
            this.ViewModel.Editor.SetError(
                "UpdateGroups",
                this.ViewModel.Editor.HasSelectedUpdateGroups ? ErrorState.Ok : ErrorState.Warning,
                AdminStrings.UnitConfig_Conclusion_ExportExecution_NoUpdateGroupSelected);
        }

        private class DynamicDisplayUnitComparer : IComparer<DisplayUnitEditorViewModel>
        {
            public int Compare(DisplayUnitEditorViewModel x, DisplayUnitEditorViewModel y)
            {
                var result = string.Compare(
                    x.DynamicContentUrl,
                    y.DynamicContentUrl,
                    StringComparison.InvariantCulture);
                return result;
            }
        }
    }
}