// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDownloadPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalDownloadPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;

    using Ookii.Dialogs.Wpf;

    /// <summary>
    /// Controller to download locally the just exported configuration.
    /// </summary>
    public class LocalDownloadPartController : PartControllerBase<LocalDownloadPartViewModel>
    {
        private UpdateFolderStructure folderStructure;

        private int totalFileCount;
        private int downloadedFileCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDownloadPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public LocalDownloadPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Conclusion.LocalDownload, parent)
        {
        }

        /// <summary>
        /// Sets the folder structure to be downloaded by this controller.
        /// </summary>
        /// <param name="structure">
        /// The structure. If this is null, the corresponding view is hidden.
        /// </param>
        public void SetFolderStructure(UpdateFolderStructure structure)
        {
            this.ViewModel.ExportFolders.Clear();
            this.totalFileCount = 0;

            this.folderStructure = structure;
            this.ViewModel.IsVisible = structure != null;
            if (structure == null)
            {
                return;
            }

            foreach (var folderUpdate in structure.Folders)
            {
                this.ViewModel.ExportFolders.Add(this.CreateExportFolder(folderUpdate));
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="LocalDownloadPartViewModel"/>.
        /// </returns>
        protected override LocalDownloadPartViewModel CreateViewModel()
        {
            var viewModel = new LocalDownloadPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Conclusion_LocalDownload;
            viewModel.Description = AdminStrings.UnitConfig_Conclusion_LocalDownload_Description;
            viewModel.IsVisible = false;
            viewModel.DownloadCommand = new RelayCommand(this.Download);

            return viewModel;
        }

        private ExportFolder CreateExportFolder(FolderUpdate folderUpdate)
        {
            var exportFolder = new ExportFolder(folderUpdate.Name);
            foreach (var item in folderUpdate.Items)
            {
                var file = item as FileUpdate;
                if (file != null)
                {
                    this.totalFileCount++;
                    exportFolder.Children.Add(new ExportResourceFile(file.Name, null));
                    continue;
                }

                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    exportFolder.Children.Add(this.CreateExportFolder(subFolder));
                }
            }

            return exportFolder;
        }

        private async void Download()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.Description = AdminStrings.UnitConfig_Conclusion_LocalDownload_FolderSelection;
            dialog.UseDescriptionForTitle = true;
            var result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return;
            }

            try
            {
                if (Directory.GetFileSystemEntries(dialog.SelectedPath).Any())
                {
                    var message = string.Format(
                        AdminStrings.Warnings_FolderNotEmpty_MessageFormat, dialog.SelectedPath);
                    if (MessageBox.Show(
                        message,
                        AdminStrings.Warnings_FolderNotEmpty_Title,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning,
                        MessageBoxResult.No) != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    Directory.Delete(dialog.SelectedPath, true);
                }

                this.ViewModel.IsDownloading = true;
                this.downloadedFileCount = 0;
                this.ViewModel.DownloadProgress = 0;

                await this.DownloadToAsync(dialog.SelectedPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.ViewModel.IsDownloading = false;
        }

        private async Task DownloadToAsync(string directoryName)
        {
            using (
                var resourceService =
                    this.Parent.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                foreach (var folderUpdate in this.folderStructure.Folders)
                {
                    await this.DownloadFolderAsync(folderUpdate, directoryName, directoryName, resourceService);
                }
            }
        }

        private async Task DownloadFolderAsync(
            FolderUpdate folderUpdate,
            string parentDirectory,
            string rootFolder,
            ChannelScope<IResourceService> resourceService)
        {
            var directoryPath = Path.Combine(parentDirectory, folderUpdate.Name);
            Directory.CreateDirectory(directoryPath);

            foreach (var item in folderUpdate.Items)
            {
                var file = item as FileUpdate;
                if (file != null)
                {
                    var path = Path.Combine(directoryPath, item.Name);
                    this.ViewModel.DownloadingFile = path.Substring(rootFolder.Length + 1);
                    this.downloadedFileCount++;
                    this.ViewModel.DownloadProgress = 100 * this.downloadedFileCount / this.totalFileCount;

                    await this.DownloadFileAsync(file, path, resourceService);
                    continue;
                }

                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    await this.DownloadFolderAsync(subFolder, directoryPath, rootFolder, resourceService);
                }
            }
        }

        private async Task DownloadFileAsync(
            FileUpdate file, string filePath, ChannelScope<IResourceService> resourceService)
        {
            var request = new ResourceDownloadRequest { Hash = file.Hash };
            var result = await resourceService.Channel.DownloadAsync(request);
            using (var input = result.Content)
            {
                using (var output = File.Create(filePath))
                {
                    await input.CopyToAsync(output);
                }
            }
        }
    }
}