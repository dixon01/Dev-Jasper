// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfigurationDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The media configuration data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    using NLog;

    /// <summary>
    /// The media configuration data view model.
    /// </summary>
    public class MediaConfigurationDataViewModel : DataViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMediaShell mediaShell;

        private readonly ICommandRegistry commandRegistry;

        private MediaProjectDataViewModel projectDataViewModel;

        private int loadingProgress;

        private bool isReadableModelDirty;

        private DocumentVersionReadableModel currentVersion;

        private bool isVersionSelectionOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaConfigurationDataViewModel"/> class.
        /// </summary>
        /// <param name="readableModel">
        /// The model.
        /// </param>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public MediaConfigurationDataViewModel(
            MediaConfigurationReadableModel readableModel,
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry)
        {
            this.mediaShell = mediaShell;
            this.commandRegistry = commandRegistry;
            this.ReadableModel = readableModel;

            if (this.ReadableModel == null)
            {
                return;
            }

            this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
            this.SetUpDocument();
            if (this.ReadableModel.Document != null)
            {
                this.CurrentVersion = this.ReadableModel.Document.Versions.LastOrDefault();
            }
        }

        /// <summary>
        /// Event to inform that the project has been loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Gets the readable model.
        /// </summary>
        public MediaConfigurationReadableModel ReadableModel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is readable model dirty.
        /// </summary>
        public bool IsReadableModelDirty
        {
            get
            {
                return this.isReadableModelDirty;
            }

            set
            {
                this.SetProperty(ref this.isReadableModelDirty, value, () => this.IsReadableModelDirty);
            }
        }

        /// <summary>
        /// Gets the document (ReadOnly).
        /// </summary>
        public DocumentReadOnlyDataViewModel Document { get; private set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.ReadableModel.Document.Name;
            }

            set
            {
                if (this.ReadableModel.Document.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                if (!this.mediaShell.PermissionController.PermissionTrap(
                    Permission.Write,
                    DataScope.MediaConfiguration))
                {
                    return;
                }

                this.IsReadableModelDirty = true;
                var writableDocument = new DocumentDataViewModel(this.ReadableModel.Document.ToChangeTrackingModel())
                                       {
                                           Name = value
                                       };

                var publishParameters = new PublishDocumentWritableModelParameters
                {
                    Model = writableDocument.Model,
                    OnFinished = this.OnPublishingNameFinished,
                    ErrorCallbackAction = this.OnPublishError,
                    OldValues = this.ReadableModel.Document.Name.Clone(),
                    NewValues = writableDocument.Model.Name.Clone()
                };

                var publishCommand = this.commandRegistry.GetCommand(CommandCompositionKeys.Project.PublishDocument);
                publishCommand.Execute(publishParameters);
            }
        }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        public DateTime CreatedOn
        {
            get
            {
                return this.ReadableModel.CreatedOn;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the version selection is open.
        /// </summary>
        public bool IsVersionSelectionOpen
        {
            get
            {
                return this.isVersionSelectionOpen;
            }

            set
            {
                this.SetProperty(ref this.isVersionSelectionOpen, value, () => this.IsVersionSelectionOpen);
            }
        }

        /// <summary>
        /// Gets the last modified on.
        /// </summary>
        /// <remarks>
        /// If the last modified is null, created on is returned.
        /// </remarks>
        public DateTime LastModifiedOn
        {
            get
            {
                DocumentVersionReadableModel version = null;
                if (this.ReadableModel.Document != null && this.ReadableModel.Document.Versions != null)
                {
                    version = this.ReadableModel.Document.Versions.LastOrDefault();
                }

                if (version != null)
                {
                    if (version.LastModifiedOn.HasValue)
                    {
                        return version.LastModifiedOn.Value;
                    }

                    return version.CreatedOn;
                }

                return this.Document.ReadableModel.CreatedOn;
            }
        }

        /// <summary>
        /// Gets the latest version.
        /// </summary>
        public string LatestVersion
        {
            get
            {
                var latest = this.ReadableModel.Document.Versions.LastOrDefault();
                if (latest == null)
                {
                    return "0.0";
                }

                return string.Format("{0}.{1}", latest.Major, latest.Minor);
            }
        }

        /// <summary>
        /// Gets the document versions.
        /// </summary>
        public IObservableReadOnlyCollection<DocumentVersionReadableModel> Versions
        {
            get
            {
                return this.ReadableModel.Document.Versions;
            }
        }

        /// <summary>
        /// Gets or sets the current document version.
        /// </summary>
        public DocumentVersionReadableModel CurrentVersion
        {
            get
            {
                return this.currentVersion;
            }

            set
            {
                if (this.SetProperty(ref this.currentVersion, value, () => this.CurrentVersion))
                {
                    this.projectDataViewModel = null;
                    this.RaisePropertyChanged(() => this.IsLatestVersionSelected);
                }
            }
        }

        /// <summary>
        /// Gets the physical screens count.
        /// </summary>
        public int? PhysicalScreensCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.InfomediaConfig.PhysicalScreens.Count;
            }
        }

        /// <summary>
        /// Gets the virtual displays count.
        /// </summary>
        public int? VirtualDisplaysCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.InfomediaConfig.VirtualDisplays.Count;
            }
        }

        /// <summary>
        /// Gets the layouts count.
        /// </summary>
        public int? LayoutsCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.InfomediaConfig.Layouts.Count;
            }
        }

        /// <summary>
        /// Gets the cycles count.
        /// </summary>
        public int? CyclesCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.InfomediaConfig.Cycles.StandardCycles.Count
                    + this.projectDataViewModel.InfomediaConfig.Cycles.EventCycles.Count;
            }
        }

        /// <summary>
        /// Gets the cycle packages count.
        /// </summary>
        public int? CyclePackagesCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.InfomediaConfig.CyclePackages.Count;
            }
        }

        /// <summary>
        /// Gets or sets the user details.
        /// </summary>
        public string Description
        {
            get
            {
                return this.ReadableModel.Document.Description;
            }

            set
            {
                if (this.ReadableModel.Document.Description != null
                    && this.ReadableModel.Document.Description.Equals(
                        value,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                if (!this.mediaShell.PermissionController.PermissionTrap(
                    Permission.Write,
                    DataScope.MediaConfiguration))
                {
                    return;
                }

                this.IsReadableModelDirty = true;
                var writableDocument = new DocumentDataViewModel(this.ReadableModel.Document.ToChangeTrackingModel())
                                       {
                                           Description = value
                                       };

                var publishParameters = new PublishDocumentWritableModelParameters
                                 {
                                     Model = writableDocument.Model,
                                     ErrorCallbackAction = this.OnPublishError
                                 };

                var publishCommand = this.commandRegistry.GetCommand(CommandCompositionKeys.Project.PublishDocument);
                publishCommand.Execute(publishParameters);
            }
        }

        /// <summary>
        /// Gets the resources count.
        /// </summary>
        public int? ResourcesCount
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.Resources.Count;
            }
        }

        /// <summary>
        /// Gets the project size.
        /// </summary>
        public long? ProjectSize
        {
            get
            {
                if (this.projectDataViewModel == null)
                {
                    return null;
                }

                return this.projectDataViewModel.ProjectSize;
            }
        }

        /// <summary>
        /// Gets or sets the loading progress (the current resource this is being loaded).
        /// </summary>
        public int LoadingProgress
        {
            get
            {
                return this.loadingProgress;
            }

            set
            {
                if (this.SetProperty(ref this.loadingProgress, value, () => this.LoadingProgress))
                {
                    this.RaisePropertyChanged(() => this.LoadingProgressValue);
                }
            }
        }

        /// <summary>
        /// Gets the loading progress value between 0 and 100 (for busy indicator).
        /// </summary>
        public double LoadingProgressValue
        {
            get
            {
                var max = this.projectDataViewModel == null ? 0 : this.projectDataViewModel.Resources.Count;
                return 100.0 * this.LoadingProgress / (max + 1);
            }
        }

        /// <summary>
        /// Gets the data ViewModel of the project.
        /// </summary>
        public MediaProjectDataViewModel MediaProjectDataViewModel
        {
            get
            {
                return this.projectDataViewModel;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the latest document version is selected.
        /// </summary>
        public bool IsLatestVersionSelected
        {
            get
            {
                if (this.currentVersion != null)
                {
                    return this.LatestVersion
                      == string.Format("{0}.{1}", this.CurrentVersion.Major, this.CurrentVersion.Minor);
                }

                return string.IsNullOrEmpty(this.LatestVersion);
            }
        }

        /// <summary>
        /// Reverts all changes done in the projects to the server version.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UndoChangesAsync()
        {
            try
            {
                await this.SetProjectDataViewModelAsync();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Exception while trying to undo changes.", e);
            }
        }

        /// <summary>
        /// Raises the Loaded event.
        /// </summary>
        public void RaiseLoadedEvent()
        {
            var handler = this.Loaded;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Asynchronously loads the media project data view model.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadMediaProjectDataViewModelAsync()
        {
            if (this.projectDataViewModel == null)
            {
                try
                {
                    await this.SetProjectDataViewModelAsync();
                    this.RaiseProjectDetailsChanged();
                }
                catch (Exception e)
                {
                    Logger.ErrorException("Error while loading the MediaProjectDataViewModel.", e);
                }
            }
        }

        private void ResetVersionToLatestAsync()
        {
            this.CurrentVersion = this.ReadableModel.Document.Versions.LastOrDefault();
        }

        private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Document")
            {
                this.Document.PropertyChanged -= this.DocumentReadableModelOnPropertyChanged;
                this.Document.ReadableModel.Versions.CollectionChanged -= this.VersionsCollectionChanged;
                this.SetUpDocument();
                this.IsReadableModelDirty = false;
            }
        }

        private void RaiseProjectDetailsChanged()
        {
            this.RaisePropertyChanged(() => this.ResourcesCount);
            this.RaisePropertyChanged(() => this.LastModifiedOn);
            this.RaisePropertyChanged(() => this.LayoutsCount);
            this.RaisePropertyChanged(() => this.ProjectSize);
            this.RaisePropertyChanged(() => this.CyclePackagesCount);
            this.RaisePropertyChanged(() => this.CyclesCount);
            this.RaisePropertyChanged(() => this.Description);
            this.RaisePropertyChanged(() => this.VirtualDisplaysCount);
            this.RaisePropertyChanged(() => this.PhysicalScreensCount);
            this.RaisePropertyChanged(() => this.LatestVersion);
        }

        private void SetUpDocument()
        {
            try
            {
                this.Document = new DocumentReadOnlyDataViewModel(this.ReadableModel.Document);
                this.Document.PropertyChanged += this.DocumentReadableModelOnPropertyChanged;
                this.Document.ReadableModel.Versions.CollectionChanged += this.VersionsCollectionChanged;
                this.RaiseLoadedEvent();
            }
            catch (Exception e)
            {
                Logger.ErrorException("Error while trying to get the latest project version.", e);
            }
        }

        private void VersionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.ResetVersionToLatestAsync();
            }

            this.RaisePropertyChanged(() => this.Name);
            this.RaiseProjectDetailsChanged();
        }

        private void OnPublishingNameFinished(
            bool success,
            object oldValues,
            object newValues)
        {
            var oldname = oldValues as string;
            var newname = newValues as string;

            if (!success && oldname != null && newname != null)
            {
                return;
            }

            var renameRecentlyUsedCommand =
                this.commandRegistry.GetCommand(CommandCompositionKeys.Project.RenameRecentlyUsed);
            var commandParameters = new RenameProjectParameters
            {
                OldName = oldname,
                NewName = newname
            };

            renameRecentlyUsedCommand.Execute(commandParameters);
        }

        private void OnPublishError(Exception e)
        {
            this.IsReadableModelDirty = false;
        }

        private async Task SetProjectDataViewModelAsync()
        {
            DocumentVersionReadableModel documentVersion;
            if (this.currentVersion == null)
            {
                documentVersion = this.ReadableModel.Document.Versions.LastOrDefault();
            }
            else
            {
                documentVersion = this.currentVersion;
            }

            if (documentVersion != null)
            {
                await documentVersion.LoadXmlPropertiesAsync();
                var dataModel = (MediaProjectDataModel)documentVersion.Content.Deserialize();
                this.projectDataViewModel = new MediaProjectDataViewModel(
                    this.mediaShell,
                    this.commandRegistry,
                    dataModel);
            }
            else
            {
                Logger.Warn("Document had no latest version.");
            }
        }

        private void DocumentReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsReadableModelDirty)
            {
                this.IsReadableModelDirty = false;
            }

            if (e.PropertyName == "Name")
            {
                this.RaisePropertyChanged(() => this.DisplayText);
            }

            this.RaisePropertyChanged(e.PropertyName);
        }
    }
}
