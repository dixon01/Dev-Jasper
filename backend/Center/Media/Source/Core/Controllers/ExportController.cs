// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.ServiceModel.Dynamic;
    using Gorba.Center.Common.ServiceModel.Filters.Resources;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using MessageBox = System.Windows.MessageBox;

    /// <summary>
    /// Implements the <see cref="IExportController"/>.
    /// </summary>
    public class ExportController : IExportController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string exportFilePath;

        private string transferFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportController"/> class.
        /// </summary>
        /// <param name="parentController">
        /// The parent Controller.
        /// </param>
        /// <param name="mainMenuPrompt">
        /// The main menu prompt.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ExportController(
            IMediaShellController parentController,
            MainMenuPrompt mainMenuPrompt,
            ICommandRegistry commandRegistry)
        {
            this.MainMenuPrompt = mainMenuPrompt;
            this.ParentController = parentController;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Presentation.ExportServer,
                new RelayCommand<ExportParameters>(this.ExportToServerAsync));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Presentation.ExportLocal,
                new RelayCommand(this.ExportToLocalAsync));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.Presentation.SelectExportFile,
                new RelayCommand<ExportFileSelectionFilter>(this.SelectExportFile));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.Transfer,
                new RelayCommand(this.TransferProjectAsync));
        }

        /// <summary>
        /// The exported event.
        /// </summary>
        public event EventHandler Exported;

        private enum ExportType
        {
            Local,

            Server,

            Transfer
        }

        /// <summary>
        /// Gets the media application state.
        /// </summary>
        public IMediaApplicationState MediaApplicationState
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            }
        }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; private set; }

        /// <summary>
        /// Gets or sets the main menu prompt.
        /// </summary>
        /// <value>
        /// The main menu prompt.
        /// </value>
        public MainMenuPrompt MainMenuPrompt { get; set; }

        private static bool CleanupExistingContent(string exportDirectory)
        {
            var imagesPath = Path.Combine(exportDirectory, Settings.Default.ImageExportPath);
            var videosPath = Path.Combine(exportDirectory, Settings.Default.VideoExportPath);
            var poolsPath = Path.Combine(exportDirectory, Settings.Default.PoolExportPath);
            var symbolsPath = Path.Combine(exportDirectory, Settings.Default.SymbolExportPath);
            var audioPath = Path.Combine(exportDirectory, Settings.Default.AudioExportPath);
            var fontsPath = Path.Combine(exportDirectory, Settings.Default.RelativeFontsFolderPath);

            var imagesPathExists = Directory.Exists(imagesPath);
            var videosPathExists = Directory.Exists(videosPath);
            var poolsPathExists = Directory.Exists(poolsPath);
            var symbolsPathExists = Directory.Exists(symbolsPath);
            var audioPathExists = Directory.Exists(audioPath);
            var fontPathExists = Directory.Exists(fontsPath);

            var csvFiles = Directory.GetFiles(exportDirectory, "*.csv", SearchOption.TopDirectoryOnly);
            var csvFileInfos = csvFiles.Select(mapping => new FileInfo(mapping)).ToList();
            var csvFilesExist = csvFileInfos.Any(fi => fi.Exists);

            if (imagesPathExists || videosPathExists || poolsPathExists || symbolsPathExists || audioPathExists
                || fontPathExists || csvFilesExist)
            {
                var messageBox = MessageBox.Show(
                    MediaStrings.ExportController_DestinationNotEmpty,
                    MediaStrings.ExportController_DestinationNotEmptyTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (messageBox != MessageBoxResult.Yes)
                {
                    return false;
                }

                if (imagesPathExists)
                {
                    CleanupDirectory(imagesPath);
                }

                if (videosPathExists)
                {
                    CleanupDirectory(videosPath);
                }

                if (poolsPathExists)
                {
                    CleanupDirectory(poolsPath);
                }

                if (symbolsPathExists)
                {
                    CleanupDirectory(symbolsPath);
                }

                if (audioPathExists)
                {
                    CleanupDirectory(audioPath);
                }

                if (fontPathExists)
                {
                    CleanupDirectory(fontsPath);
                }

                if (csvFilesExist)
                {
                    csvFileInfos.ForEach(fi => fi.Delete());
                }
            }

            return true;
        }

        private static void CleanupDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            foreach (var directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
        }

        private static void CopyResource(
            IMediaApplicationState state,
            string hash,
            string resourceFilename,
            FileInfo fileInfo,
            string folder,
            int? poolFileIndex = null)
        {
            try
            {
                var resource = state.ProjectManager.GetResource(hash);
                using (var stream = resource.OpenRead())
                {
                    if (fileInfo.Directory == null)
                    {
                        throw new Exception("Directory null.");
                    }

                    if (resourceFilename == null)
                    {
                        throw new Exception("Resource filename null.");
                    }

                    var fileName = Path.GetFileName(resourceFilename);

                    if (poolFileIndex.HasValue)
                    {
                        fileName = poolFileIndex.Value.ToString("D3") + "_" + fileName;
                    }

                    var path = Path.Combine(fileInfo.Directory.FullName, folder, fileName);

                    var outputPathInfo = new FileInfo(path);

                    if (outputPathInfo.Directory == null)
                    {
                        throw new Exception("Directory null.");
                    }

                    if (outputPathInfo.Exists)
                    {
                        outputPathInfo.Delete();
                    }
                    else if (!outputPathInfo.Directory.Exists)
                    {
                        outputPathInfo.Directory.Create();
                    }

                    using (var output = File.OpenWrite(path))
                    {
                        stream.CopyTo(output);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while copying the resource", exception);
            }
        }

        /// <summary>
        /// Asynchronously exports a project to a local destination including all resource directories.
        /// </summary>
        private async void ExportToLocalAsync()
        {
            try
            {
                var isCheckedIn = await this.ParentController.ProjectController.EnsureCheckInAsync();
                if (isCheckedIn)
                {
                    await this.ExportToLocalAfterCheckinAsync();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception during local export.", e);
            }
        }

        /// <summary>
        /// Asynchronously exports a project to a local destination.
        /// </summary>
        /// <returns>
        /// The Task.
        /// </returns>
        private async Task ExportToLocalAfterCheckinAsync()
        {
            if (!this.ParentController.ParentController.PermissionController.PermissionTrap(
                Permission.Write, DataScope.MediaConfiguration) || !this.CheckConsistency())
            {
                return;
            }

            try
            {
                this.SetBusyIndicator(ExportType.Local);
                this.SelectExportFile(ExportFileSelectionFilter.Im2);
                if (string.IsNullOrEmpty(this.exportFilePath))
                {
                    Logger.Debug("Export was cancelled by user");
                    this.UnsetBusyIndicator();
                    return;
                }

                Logger.Info("Export project locally to '{0}'", this.exportFilePath);
                var fileInfo = new FileInfo(this.exportFilePath);
                if (fileInfo.Directory == null)
                {
                    throw new DirectoryNotFoundException("Directory is null.");
                }

                var exportDirectory = fileInfo.Directory.FullName;
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                if (!this.CleanDestinationFolder(exportDirectory))
                {
                    this.UnsetBusyIndicator();
                    Logger.Info("Project export cancelled.");
                    return;
                }

                this.ExportResourcesLocal(fileInfo);
                await this.CreateTextReplacementCsvAsync(true, exportDirectory);
                await this.CreateCsvMappingsAsync(true, exportDirectory);
                var infomediaConfig = this.MediaApplicationState.CurrentProject.InfomediaConfig.Export();
                var serializer = new XmlSerializer(typeof(InfomediaConfig));
                using (var fileStream = new FileStream(
                        this.exportFilePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                {
                    serializer.Serialize(fileStream, infomediaConfig);
                }

                this.ParentController.Notify(
                    (Notification)
                    new StatusNotification
                        {
                            Title = string.Format(
                                    MediaStrings.ExportController_PresentationSuccessfullyExported,
                                    this.MediaApplicationState.CurrentProject.Name)
                        });
                Logger.Info("Project exported.");
                this.UnsetBusyIndicator();
            }
            catch (Exception exception)
            {
                this.UnsetBusyIndicator();
                Logger.ErrorException("Error while exporting", exception);
                var prompt = new ConnectionExceptionPrompt(
                    exception,
                    MediaStrings.ExportController_PresentationExportError,
                    MediaStrings.ExportController_PresentationExportedErrorTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
            finally
            {
                this.exportFilePath = string.Empty;
                this.RaiseExported();
            }
        }

        private void ExportResourcesLocal(FileInfo fileInfo)
        {
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Image && r.ShouldExport)
                .ToList()
                .ForEach(
                    r =>
                    CopyResource(
                        this.MediaApplicationState,
                        r.Hash,
                        r.Filename,
                        fileInfo,
                        Settings.Default.ImageExportPath));
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Video && r.ShouldExport)
                .ToList()
                .ForEach(
                    r =>
                    CopyResource(
                        this.MediaApplicationState,
                        r.Hash,
                        r.Filename,
                        fileInfo,
                        Settings.Default.VideoExportPath));
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Symbol)
                .ToList()
                .ForEach(
                    r =>
                    CopyResource(
                        this.MediaApplicationState,
                        r.Hash,
                        r.Filename,
                        fileInfo,
                        Settings.Default.SymbolExportPath));
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Audio)
                .ToList()
                .ForEach(
                    r =>
                    CopyResource(
                        this.MediaApplicationState,
                        r.Hash,
                        r.Filename,
                        fileInfo,
                        Settings.Default.AudioExportPath));
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Font && r.ShouldExport)
                .ToList()
                .ForEach(
                    r =>
                    CopyResource(
                        this.MediaApplicationState,
                        r.Hash,
                        r.Filename,
                        fileInfo,
                        Settings.Default.RelativeFontsFolderPath));

            this.ExportPools(fileInfo);
        }

        private async void TransferProjectAsync()
        {
            try
            {
                var isCheckedIn = await this.ParentController.ProjectController.EnsureCheckInAsync();

                if (isCheckedIn)
                {
                    await this.TransferProjectAfterCheckinAsync();
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error during project transfer.", e);
            }
        }

        private async Task TransferProjectAfterCheckinAsync()
        {
            if (
                !this.ParentController.ParentController.PermissionController.PermissionTrap(
                    Permission.Write,
                    DataScope.MediaConfiguration) || !this.CheckConsistency())
            {
                return;
            }

            this.SetBusyIndicator(ExportType.Transfer);
            this.SelectExportFile(ExportFileSelectionFilter.Icm);
            if (string.IsNullOrEmpty(this.transferFilePath))
            {
                Logger.Debug("Transfer was cancelled by user");
                this.UnsetBusyIndicator();
                return;
            }

            Logger.Info("Export project to file '{0}'", this.transferFilePath);

            // WARNING: it's important to create the project. The ProjectManager.CanSave flag should be set
            // here to avoid infinite recursion
            this.MediaApplicationState.ProjectManager.CreateProject(this.transferFilePath);
            this.MediaApplicationState.CurrentProject.FilePath = this.MediaApplicationState.ProjectManager.FullFileName;
            this.MediaApplicationState.CurrentProject.ProjectId = Guid.NewGuid();

            if (!this.MediaApplicationState.ProjectManager.IsFileSelected)
            {
                return;
            }

            this.MediaApplicationState.CurrentProject.DateLastModified = TimeProvider.Current.UtcNow;

            var mediaProject = this.MediaApplicationState.CurrentProject.ToDataModel();
            try
            {
                await this.MediaApplicationState.ProjectManager.SaveAsync(mediaProject);
                Logger.Info("Project exported");
                this.ParentController.Notify(
                    (Notification)
                    new StatusNotification
                        {
                            Title =
                                string.Format(
                                    MediaStrings.ExportController_ProjectSuccessfullyTransferred,
                                    this.MediaApplicationState.CurrentProject.Name)
                        });
                this.UnsetBusyIndicator();
            }
            catch (Exception exception)
            {
                this.UnsetBusyIndicator();
                Logger.ErrorException("Error while transferring project.", exception);
                var prompt = new ConnectionExceptionPrompt(
                    exception,
                    MediaStrings.ExportController_TranserErrorMessage,
                    MediaStrings.ExportController_TransferErrorTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
            finally
            {
                this.transferFilePath = string.Empty;
                this.RaiseExported();
            }
        }

        private async void ExportToServerAsync(ExportParameters parameters)
        {
            try
            {
                var isCheckedIn = await this.ParentController.ProjectController.EnsureCheckInAsync();

                if (isCheckedIn)
                {
                    await this.ExportToServerAfterCheckinAsync(parameters);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error during export to server.", e);
                this.UnsetBusyIndicator();
                var errorMessage = string.Format(
                    "Error while exporting project {0} to server.",
                    this.MediaApplicationState.CurrentProject.Name);
                Logger.ErrorException(errorMessage, e);
                var prompt = new ConnectionExceptionPrompt(
                    e,
                    MediaStrings.ExportController_PresentationExportError,
                    MediaStrings.ExportController_PresentationExportedErrorTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
        }

        /// <summary>
        /// Exports the current project to server asynchronously.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ExportToServerAfterCheckinAsync(ExportParameters parameters)
        {
            this.SetBusyIndicator(ExportType.Server);

            var updateGroups = parameters.UpdateGroups.ToList();
            if (
                !this.ParentController.ParentController.PermissionController.PermissionTrap(
                    Permission.Write,
                    DataScope.MediaConfiguration)
                    || !this.CheckConsistency()
                    || !await this.CheckCompatibilityAsync(updateGroups))
            {
                this.UnsetBusyIndicator();
                return;
            }

            Logger.Info("Export project to server.");
            try
            {
                this.MainMenuPrompt.Shell.TotalBusyProgress = 1
                                                              + (this.MediaApplicationState.CurrentProject.Replacements
                                                                     .Count > 0 ? 1 : 0);

                var dynamicContent = this.CreateDynamicContent();
                var additionalResources = await this.UploadAdditionalResourcesAsync(updateGroups);

                foreach (var updateGroupReadableModel in updateGroups)
                {
                    // ReSharper disable once UnusedVariable
                    var updatePart =
                        await
                        this.CreateUpdatePartAsync(
                            updateGroupReadableModel,
                            parameters.Start,
                            parameters.End,
                            parameters.Description,
                            additionalResources,
                            dynamicContent);
                }

                // Create update commands for all update groups (needed for FTP)
                await this.CreateUpdateCommands(updateGroups);

                foreach (var updateGroup in updateGroups)
                {
                    var writeableUpdateGroup = updateGroup.Item.ToChangeTrackingModel();
                    writeableUpdateGroup.MediaConfiguration =
                        this.MediaApplicationState.CurrentMediaConfiguration.ReadableModel;
                    writeableUpdateGroup.Commit();
                }

                Logger.Info("Project exported.");
                this.ParentController.Notify(
                    (Notification)
                    new StatusNotification
                        {
                            Title =
                                string.Format(
                                    MediaStrings.ExportController_PresentationSuccessfullyExported,
                                    this.MediaApplicationState.CurrentProject.Name)
                        });
                this.UnsetBusyIndicator();
            }
            catch (Exception e)
            {
                this.UnsetBusyIndicator();
                var errorMessage = string.Format(
                    "Error while exporting project {0} to server.",
                    this.MediaApplicationState.CurrentProject.Name);
                Logger.ErrorException(errorMessage, e);
                var prompt = new ConnectionExceptionPrompt(
                    e,
                    MediaStrings.ExportController_PresentationExportError,
                    MediaStrings.ExportController_PresentationExportedErrorTitle);
                InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt);
            }
            finally
            {
                this.RaiseExported();
            }
        }

        private async Task CreateUpdateCommands(List<UpdateGroupItemViewModel> updateGroups)
        {
            using (
                var updateService =
                    this.ParentController.ParentController.ConnectionController.CreateChannelScope<IUpdateService>())
            {
                foreach (var updateGroupReadableModel in updateGroups)
                {
                    await
                        updateService.Channel.CreateUpdateCommandsForUpdateGroupAsync(updateGroupReadableModel.Item.Id);
                }
            }
        }

        private IEnumerable<RssTickerElementDataViewModel> CollectRssFeedInformation()
        {
            this.MainMenuPrompt.Shell.CurrentBusyProgressText = MediaStrings.ExportController_BusyText_CollectLiveFeed;

            var rssElements =
                this.MediaApplicationState.CurrentProject.GetElementsOfType<RssTickerElementDataViewModel>();

            return rssElements;
        }

        // also updates used table row in layout elements
        private DynamicContentInfo CreateDynamicContent()
        {
            var rssElements = this.CollectRssFeedInformation().ToList();
            if (rssElements.Count == 0)
            {
                return null;
            }

            // get unique feed and set row number, equal feeds get the same row number
            var comparer = new RssElementComparer();
            rssElements.Sort(comparer);
            var dynamicContentInfo = new DynamicContentInfo();
            var exportRowIndex = -1;
            RssTickerElementDataViewModel previousElement = null;
            foreach (var rssElement in rssElements)
            {
                if (previousElement != null && comparer.Compare(previousElement, rssElement) == 0)
                {
                    rssElement.ExportingRow = exportRowIndex;
                    continue;
                }

                exportRowIndex++;
                rssElement.ExportingRow = exportRowIndex;
                previousElement = rssElement;

                var rssFeedDynamicContentPart = new RssFeedDynamicContentPart
                                                    {
                                                        Url = rssElement.RssUrl.Value,
                                                        RefreshInterval =
                                                            rssElement.UpdateInterval.Value,
                                                        TableRow = exportRowIndex,
                                                        Delimiter = rssElement.Delimiter.Value,
                                                        Validity = rssElement.Validity.Value
                                                    };

                dynamicContentInfo.Parts.Add(rssFeedDynamicContentPart);
            }

            return dynamicContentInfo;
        }

        private async Task<List<Resource>> UploadAdditionalResourcesAsync(
            IEnumerable<UpdateGroupItemViewModel> updateGroups)
        {
            var groupsSoftwareConfigsList = updateGroups.Select(updateGroup => updateGroup.ComponentVersions);
            var lowestComponentVersions = FeatureComponentRequirements.GetLowestVersions(groupsSoftwareConfigsList);
            var exportParams = this.CreateExportCompatibilityParameters(lowestComponentVersions);
            var infomediaConfig = this.MediaApplicationState.CurrentProject.InfomediaConfig.Export(exportParams);

            var codeConversionResource = await this.CreateTextReplacementCsvAsync(false);
            var csvMappingResources = await this.CreateCsvMappingsAsync(false);

            this.MainMenuPrompt.Shell.CurrentBusyProgress++;
            this.MainMenuPrompt.Shell.CurrentBusyProgressText = "main.im2";
            Resource infomediaResource;
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(InfomediaConfig));
                serializer.Serialize(memoryStream, infomediaConfig);
                infomediaResource = await this.UploadResourceStreamAsync(memoryStream, "main.im2", "text/xml");
            }

            var additionalResources = new List<Resource>();
            if (infomediaResource != null)
            {
                additionalResources.Add(infomediaResource);
            }

            if (codeConversionResource != null)
            {
                additionalResources.Add(codeConversionResource);
            }

            additionalResources.AddRange(csvMappingResources);

            return additionalResources;
        }

        private ExportCompatibilityParameters CreateExportCompatibilityParameters(
            List<FeatureComponentRequirements.SoftwareConfig> componentVersions)
        {
            var exportParams = new ExportCompatibilityParameters();
            exportParams.CurrentSoftwareConfigs = componentVersions;

            var csvMappingRequirements = new List<FeatureComponentRequirements.SoftwareConfig>
                                             {
                                                 new FeatureComponentRequirements.SoftwareConfig(
                                                     FeatureComponentRequirements.SoftwareComponent.Composer,
                                                     new SoftwareComponentVersion("2.5"))
                                             };
            exportParams.CsvMappingCompatibilityRequired = !FeatureComponentRequirements.RequirementsOk(
                componentVersions,
                csvMappingRequirements);

            return exportParams;
        }

        private bool CheckConsistency()
        {
            var showedErrorBox = false;
            if (this.MediaApplicationState.ConsistencyMessages.Any(model => model.Severity == Severity.Error))
            {
                var messageBox = MessageBox.Show(
                    MediaStrings.ExportController_TryingToExportWithErrors,
                    MediaStrings.ExportController_TryingToExportWithErrorsTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (messageBox != MessageBoxResult.Yes)
                {
                    return false;
                }

                showedErrorBox = true;
            }

            if (!showedErrorBox
                && this.MediaApplicationState.ConsistencyMessages.Any(model => model.Severity == Severity.Warning))
            {
                var messageBox = MessageBox.Show(
                    MediaStrings.ExportController_TryingToExportWithWarnings,
                    MediaStrings.ExportController_TryingToExportWithWarningsTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (messageBox != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> RefreshUpdateGroupVersionsAsync(
            UpdateGroupItemViewModel updateGroup,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            var group = updateGroup.Item;
            updateGroup.ComponentVersions = null;

            if (group.UnitConfiguration == null)
            {
                this.AddNoUnitDecriptionError(updateGroup.Item.Name, messages);
                return true;
            }

            await group.UnitConfiguration.LoadReferencePropertiesAsync();
            await group.UnitConfiguration.Document.LoadNavigationPropertiesAsync();
            var version = this.GetHighestVersion(group.UnitConfiguration.Document.Versions);
            await version.LoadXmlPropertiesAsync();

            var unitConfigurationXml = version.Content.Xml;
            try
            {
                updateGroup.ComponentVersions = UnitConfigurationReader.GetComponentVersions(
                    unitConfigurationXml,
                    Logger);
            }
            catch (Exception)
            {
                this.AddBadUnitDecriptionError(updateGroup.Item.Name, messages);
                Logger.Error("Error while parsing UnitConfiguration for software versions.");
                return true;
            }

            return false;
        }

        private async Task<bool> CheckCompatibilityAsync(List<UpdateGroupItemViewModel> updateGroups)
        {
            var checkOk = true;
            var messages = new ExtendedObservableCollection<ConsistencyMessageDataViewModel>();

            foreach (var updateGroup in updateGroups)
            {
                var groupHasError = await this.RefreshUpdateGroupVersionsAsync(updateGroup, messages);

                if (!groupHasError)
                {
                    var parameters = new CompatibilityCheckParameters()
                    {
                        MediaApplicationState = this.MediaApplicationState,
                        SoftwareConfigs = updateGroup.ComponentVersions,
                        UpdateGroupName = updateGroup.Item.Name
                    };

                    groupHasError = this.ParentController.ProjectController.CompatibilityChecker.Check(
                        parameters,
                        messages);
                }

                updateGroup.HasCompatibilityIssue = groupHasError;
                if (groupHasError)
                {
                    checkOk = false;
                }
            }

            if (!checkOk)
            {
                MessageBox.Show(
                    MediaStrings.ExportController_TryingToExportCompatibilityIssues,
                    MediaStrings.ExportController_TryingToExportCompatibilityIssuesTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            this.MediaApplicationState.CompatibilityMessages = messages;
            return checkOk;
        }

        private DocumentVersionReadableModel GetHighestVersion(
            IObservableReadOnlyCollection<DocumentVersionReadableModel> versions)
        {
            var maxMajor = versions.Max(v => v.Major);
            var maxMinor = versions.Where(v => v.Major == maxMajor).Max(v => v.Minor);

            return versions.First(v => v.Major == maxMajor && v.Minor == maxMinor);
        }

        private void AddBadUnitDecriptionError(
            string groupName,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text =
                                string.Format(
                                    MediaStrings.CompatibilityChecker_BadUnitConfiguration,
                                    groupName),
                            Description = MediaStrings.CompatibilityChecker_PleaseContactSupport,
                            Severity = Severity.CompatibilityIssue,
                            Source = null
                        });
        }

        private void AddNoUnitDecriptionError(
            string groupName,
            ExtendedObservableCollection<ConsistencyMessageDataViewModel> messages)
        {
            messages.Add(
                        new ConsistencyMessageDataViewModel
                        {
                            Text = string.Format(
                                    MediaStrings.ExportController_UpdateGroupHasNoUnitConfiguration,
                                    groupName),
                            Severity = Severity.CompatibilityIssue,
                            Source = null
                        });
        }

        private async Task<Resource> UploadResourceStreamAsync(Stream stream, string originalName, string mimeType)
        {
            var connectionController = this.ParentController.ParentController.ConnectionController;
            stream.Seek(0, SeekOrigin.Begin);
            var hash = ResourceHash.Create(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var existingResource =
                (await
                 connectionController.ResourceChangeTrackingManager.QueryAsync(ResourceQuery.Create().WithHash(hash)))
                    .FirstOrDefault();

            if (existingResource != null)
            {
                return existingResource.ToDto();
            }

            var resource = new Resource { OriginalFilename = originalName, Hash = hash, MimeType = mimeType };
            using (var channelScope = connectionController.CreateChannelScope<IResourceService>())
            {
                resource.Length = stream.Length;
                var streamedResource = new ResourceUploadRequest { Content = stream, Resource = resource };
                await channelScope.Channel.UploadAsync(streamedResource);
            }

            return resource;
        }

        /// <summary>
        /// Selects the file.
        /// </summary>
        /// <param name="exportFileSelectionFilter">
        /// The export File Selection Filter.
        /// </param>
        private void SelectExportFile(ExportFileSelectionFilter exportFileSelectionFilter)
        {
            string filter;
            string defaultExt;
            string title;
            DialogDirectoryType directoryType = null;
            switch (exportFileSelectionFilter)
            {
                case ExportFileSelectionFilter.Im2:
                    filter = MediaStrings.ExportController_InfomediaFilter;
                    defaultExt = "im2";
                    title = MediaStrings.ExportController_SaveAs_LocalTitle;
                    break;
                case ExportFileSelectionFilter.Icm:
                    filter = MediaStrings.ExportController_ProjectFilter;
                    defaultExt = "icm";
                    title = MediaStrings.ExportController_SaveAs_ProjectTitle;
                    directoryType = DialogDirectoryTypes.Project;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("exportFileSelectionFilter");
            }

            Action<SaveFileDialogInteraction> onSave = interaction =>
                {
                    if (!interaction.Confirmed)
                    {
                        return;
                    }

                    switch (exportFileSelectionFilter)
                    {
                        case ExportFileSelectionFilter.Im2:
                            this.exportFilePath = interaction.FileName;
                            break;
                        case ExportFileSelectionFilter.Icm:
                            this.transferFilePath = interaction.FileName;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("exportFileSelectionFilter");
                    }
                };

            var saveDialogInteraction = new SaveFileDialogInteraction
                                            {
                                                AddExtension = true,
                                                DefaultExtension = defaultExt,
                                                Filter = filter,
                                                OverwritePrompt = true,
                                                RestoreDirectory = true,
                                                Title = title,
                                                DirectoryType = directoryType
                                            };

            InteractionManager<SaveFileDialogInteraction>.Current.Raise(saveDialogInteraction, onSave);
        }

        private void ExportPools(FileInfo fileInfo)
        {
            var pools = this.MediaApplicationState.CurrentProject.InfomediaConfig.Pools.Where(p => p.IsUsed).ToList();
            foreach (var pool in pools)
            {
                var poolPath = Path.Combine(Settings.Default.PoolExportPath, pool.Name.Value);
                var poolFileIndex = 1;
                foreach (var resourceReference in pool.ResourceReferences)
                {
                    CopyResource(
                        this.MediaApplicationState,
                        resourceReference.Hash,
                        resourceReference.ResourceInfo.Filename,
                        fileInfo,
                        poolPath,
                        poolFileIndex);
                    poolFileIndex++;
                }
            }
        }

        private bool CleanDestinationFolder(string exportDirectory)
        {
            if (!CleanupExistingContent(exportDirectory))
            {
                return false;
            }

            return true;
        }

        private async Task<Resource> CreateTextReplacementCsvAsync(bool createLocal, string exportDirectory = null)
        {
            if (this.MediaApplicationState.CurrentProject.Replacements.Count <= 0)
            {
                return null;
            }

            Logger.Debug("Creating the text replacement csv file.");
            this.MainMenuPrompt.CurrentBusyProgress++;
            this.MainMenuPrompt.CurrentBusyProgressText = Settings.Default.CodeConversion_Filename;
            var replacementList = this.MediaApplicationState.CurrentProject.Replacements.ToList();
            replacementList.Sort();

            using (var memoryStream = new MemoryStream())
            {
                this.AddText(memoryStream, "#Special Line;#Line Number;#Graphic;#Text\n");
                foreach (TextualReplacementDataViewModel replacement in replacementList)
                {
                    this.AddText(memoryStream, replacement.Number.Value.ToString(CultureInfo.InvariantCulture) + ";");
                    this.AddText(memoryStream, "*" + ";");

                    if (replacement.IsImageReplacement)
                    {
                        if (replacement.Image != null)
                        {
                            var symbolfilename = Path.GetFileName(replacement.Image.Filename);
                            symbolfilename = Settings.Default.SymbolExportPath + "\\" + symbolfilename;
                            this.AddText(memoryStream, symbolfilename);
                        }
                    }

                    this.AddText(memoryStream, ";");

                    if (!replacement.IsImageReplacement)
                    {
                        var exportcode = string.Empty;
                        if (replacement.Code != null)
                        {
                            exportcode = replacement.Code.Value;
                        }

                        this.AddText(memoryStream, exportcode);
                    }

                    this.AddText(memoryStream, ";\n");
                }

                if (createLocal)
                {
                    if (string.IsNullOrEmpty(exportDirectory))
                    {
                        return null;
                    }

                    var csvFilename = Path.Combine(exportDirectory, Settings.Default.CodeConversion_Filename);
                    using (
                        var fileStream = new FileStream(
                            csvFilename,
                            FileMode.CreateNew,
                            FileAccess.Write,
                            FileShare.None))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(fileStream);
                    }

                    return null;
                }

                Logger.Debug("Uploading the csv file.");
                return
                    await
                    this.UploadResourceStreamAsync(memoryStream, Settings.Default.CodeConversion_Filename, "text/csv");
            }
        }

        private async Task<List<Resource>> CreateCsvMappingsAsync(bool createLocal, string exportDirectory = null)
        {
            var resultList = new List<Resource>();

            if (this.MediaApplicationState.CurrentProject.CsvMappings.Count <= 0)
            {
                return resultList;
            }

            Logger.Debug("Creating csv mapping files.");
            var mappingsList = this.MediaApplicationState.CurrentProject.CsvMappings.ToList();

            foreach (var mapping in mappingsList)
            {
                using (var memoryStream = new MemoryStream())
                {
                    this.AddText(memoryStream, mapping.RawContent.Value);

                    var filenameWithExtension = mapping.Filename.Value + Settings.Default.CsvMapping_DefaultExtension;
                    this.MainMenuPrompt.CurrentBusyProgressText = filenameWithExtension;
                    if (createLocal)
                    {
                        if (string.IsNullOrEmpty(exportDirectory))
                        {
                            return resultList;
                        }

                        var csvFilename = Path.Combine(exportDirectory, filenameWithExtension);
                        using (
                            var fileStream = new FileStream(
                                csvFilename,
                                FileMode.CreateNew,
                                FileAccess.Write,
                                FileShare.None))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await memoryStream.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        Logger.Debug("Uploading the csv mapping file: {0}.", mapping.Filename.Value);
                        resultList.Add(
                            await this.UploadResourceStreamAsync(memoryStream, filenameWithExtension, "text/csv"));
                    }
                }
            }

            return resultList;
        }

        private void AddText(Stream fileStream, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fileStream.Write(info, 0, info.Length);
        }

        private async Task<UpdatePartReadableModel> CreateUpdatePartAsync(
            UpdateGroupItemViewModel updateGroupItemViewModel,
            DateTime start,
            DateTime end,
            string description,
            List<Resource> additionalResources,
            DynamicContentInfo dynamicContent)
        {
            Logger.Debug("Creating UpdatePart");
            var updateGroupReadable = updateGroupItemViewModel.Item;
            try
            {
                var updatePart =
                    this.ParentController.ParentController
                        .ConnectionController.UpdatePartChangeTrackingManager.Create();
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
                updatePart.Type = UpdatePartType.Presentation;

                updatePart.Start = start;
                updatePart.End = end;
                updatePart.DynamicContent = new XmlData(dynamicContent);

                if (string.IsNullOrEmpty(description))
                {
                    var version = this.MediaApplicationState.CurrentMediaConfiguration == null
                                      ? "0.0"
                                      : this.MediaApplicationState.CurrentMediaConfiguration.LatestVersion;
                    updatePart.Description = string.Format(
                        "Created for media project '{0}', version {1}",
                        this.MediaApplicationState.CurrentProject.Name,
                        version);
                }
                else
                {
                    updatePart.Description = description;
                }

                var updateFolderStructure = this.CreateUpdateFolderStructure(additionalResources);

                updatePart.Structure = new XmlData(updateFolderStructure);

                var updateReadable =
                    await
                    this.ParentController.ParentController.ConnectionController.UpdatePartChangeTrackingManager
                        .CommitAndVerifyAsync(updatePart);
                return updateReadable;
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error creating the update part.", e);
                return null;
            }
        }

        private UpdateFolderStructure CreateUpdateFolderStructure(List<Resource> additionalResources)
        {
            Logger.Trace("Creating update folder structure.");
            var updateFolderStructure = new UpdateFolderStructure();
            var presentationFolder = new FolderUpdate { Name = "Presentation" };
            var imagesFolder = this.CreateImagesFolderUpdate();
            var videosFolder = this.CreateVideosFolderUpdate();
            var fontsFolder = this.CreateFontsFolderUpdate();
            var symbolsFolder = this.CreateSymbolsFolderUpdate();
            var audioFolder = this.CreateAudioFolderUpdate();
            var poolsFolder = this.CreatePoolsFolderUpdate();

            if (imagesFolder != null)
            {
                presentationFolder.Items.Add(imagesFolder);
            }

            if (videosFolder != null)
            {
                presentationFolder.Items.Add(videosFolder);
            }

            if (fontsFolder != null)
            {
                presentationFolder.Items.Add(fontsFolder);
            }

            if (symbolsFolder != null)
            {
                presentationFolder.Items.Add(symbolsFolder);
            }

            if (audioFolder != null)
            {
                presentationFolder.Items.Add(audioFolder);
            }

            if (poolsFolder != null)
            {
                presentationFolder.Items.Add(poolsFolder);
            }

            additionalResources.ForEach(
                r => presentationFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.OriginalFilename)));

            updateFolderStructure.Folders.Add(presentationFolder);
            return updateFolderStructure;
        }

        private FolderUpdate CreateImagesFolderUpdate()
        {
            var imagesFolder = new FolderUpdate { Name = Settings.Default.ImageExportPath };
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Image && r.ShouldExport)
                .ToList()
                .ForEach(r => imagesFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.Filename)));

            return !imagesFolder.Items.Any() ? null : imagesFolder;
        }

        private FolderUpdate CreateVideosFolderUpdate()
        {
            var videosFolder = new FolderUpdate { Name = Settings.Default.VideoExportPath };
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Video && r.ShouldExport)
                .ToList()
                .ForEach(r => videosFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.Filename)));

            return !videosFolder.Items.Any() ? null : videosFolder;
        }

        private FolderUpdate CreateFontsFolderUpdate()
        {
            var fontsFolder = new FolderUpdate { Name = Settings.Default.RelativeFontsFolderPath };
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Font && r.ShouldExport)
                .ToList()
                .ForEach(r => fontsFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.Filename)));

            return !fontsFolder.Items.Any() ? null : fontsFolder;
        }

        private FolderUpdate CreateSymbolsFolderUpdate()
        {
            var symbolsFolder = new FolderUpdate { Name = Settings.Default.SymbolExportPath };
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Symbol)
                .ToList()
                .ForEach(r => symbolsFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.Filename)));

            return !symbolsFolder.Items.Any() ? null : symbolsFolder;
        }

        private FolderUpdate CreateAudioFolderUpdate()
        {
            var audioFolder = new FolderUpdate { Name = Settings.Default.AudioExportPath };
            this.MediaApplicationState.CurrentProject.Resources.Where(
                r => r.Type == ResourceType.Audio)
                .ToList()
                .ForEach(r => audioFolder.Items.Add(this.CreateFileUpdate(r.Hash, r.Filename)));

            return !audioFolder.Items.Any() ? null : audioFolder;
        }

        private FolderUpdate CreatePoolsFolderUpdate()
        {
            var pools = this.MediaApplicationState.CurrentProject.InfomediaConfig.Pools.Where(p => p.IsUsed).ToList();
            if (!pools.Any())
            {
                return null;
            }

            var poolsFolder = new FolderUpdate { Name = Settings.Default.PoolExportPath };
            foreach (var pool in pools)
            {
                var poolFolder = new FolderUpdate { Name = pool.Name.Value };
                var poolFileIndex = 1;
                foreach (var resourceReference in pool.ResourceReferences)
                {
                    poolFolder.Items.Add(
                        this.CreateFileUpdate(
                            resourceReference.Hash,
                            resourceReference.ResourceInfo.Filename,
                            poolFileIndex));
                    poolFileIndex++;
                }

                if (poolFolder.Items.Any())
                {
                    poolsFolder.Items.Add(poolFolder);
                }
            }

            return !poolsFolder.Items.Any() ? null : poolsFolder;
        }

        private FileUpdate CreateFileUpdate(string hash, string filename, int? poolFileIndex = null)
        {
            var name = Path.GetFileName(filename);
            if (poolFileIndex.HasValue)
            {
                name = poolFileIndex.Value.ToString("D3") + "_" + name;
            }

            return new FileUpdate { Hash = hash, Name = name };
        }

        private void RaiseExported()
        {
            if (this.Exported != null)
            {
                this.Exported(this, null);
            }
        }

        private void SetBusyIndicator(ExportType type)
        {
            string text;
            switch (type)
            {
                case ExportType.Local:
                    text = string.Format(
                        MediaStrings.ExportController_LocalBusyContentText,
                        this.exportFilePath);
                    this.MainMenuPrompt.Shell.IsBusyIndeterminate = true;
                    break;
                case ExportType.Server:
                    text = MediaStrings.ExportController_ServerBusyContentText;
                    this.MainMenuPrompt.Shell.IsBusyIndeterminate = false;
                    break;
                case ExportType.Transfer:
                    text = string.Format(
                        MediaStrings.ExportController_TransferBusyContentText,
                        this.transferFilePath);
                    this.MainMenuPrompt.Shell.IsBusyIndeterminate = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            this.MainMenuPrompt.Shell.BusyContentTextFormat = text;
            this.MainMenuPrompt.Shell.IsBusy = true;
        }

        private void UnsetBusyIndicator()
        {
            this.MainMenuPrompt.Shell.ClearBusy();
        }

        private class RssElementComparer : IComparer<RssTickerElementDataViewModel>
        {
            public int Compare(RssTickerElementDataViewModel x, RssTickerElementDataViewModel y)
            {
                var result = string.Compare(x.RssUrl.Value, y.RssUrl.Value, StringComparison.InvariantCulture);
                if (result != 0)
                {
                    return result;
                }

                result = string.Compare(x.Delimiter.Value, y.Delimiter.Value, StringComparison.InvariantCulture);
                if (result != 0)
                {
                    return result;
                }

                return string.Compare(
                    x.UpdateInterval.Value.ToString(),
                    y.UpdateInterval.Value.ToString(),
                    StringComparison.InvariantCulture);
            }
        }
    }
}