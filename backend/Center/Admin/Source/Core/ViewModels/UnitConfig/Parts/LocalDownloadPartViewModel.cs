// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDownloadPartViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalDownloadPartViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;

    /// <summary>
    /// The view model for the local download part.
    /// </summary>
    public class LocalDownloadPartViewModel : PartViewModelBase
    {
        private bool isDownloading;

        private int downloadProgress;

        private string downloadingFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDownloadPartViewModel"/> class.
        /// </summary>
        public LocalDownloadPartViewModel()
        {
            this.ExportFolders = new ObservableCollection<ExportFolder>();
        }

        /// <summary>
        /// Gets all errors of this part.
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return new ErrorItem[0];
            }
        }

        /// <summary>
        /// Gets the export folder structure.
        /// </summary>
        public ObservableCollection<ExportFolder> ExportFolders { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the controller is currently downloading.
        /// </summary>
        public bool IsDownloading
        {
            get
            {
                return this.isDownloading;
            }

            set
            {
                this.SetProperty(ref this.isDownloading, value, () => this.IsDownloading);
            }
        }

        /// <summary>
        /// Gets or sets the download progress of the controller (0..100).
        /// </summary>
        public int DownloadProgress
        {
            get
            {
                return this.downloadProgress;
            }

            set
            {
                this.SetProperty(ref this.downloadProgress, value, () => this.DownloadProgress);
            }
        }

        /// <summary>
        /// Gets or sets the file currently being downloaded by the controller.
        /// </summary>
        public string DownloadingFile
        {
            get
            {
                return this.downloadingFile;
            }

            set
            {
                this.SetProperty(ref this.downloadingFile, value, () => this.DownloadingFile);
            }
        }

        /// <summary>
        /// Gets or sets the download command.
        /// This property is set by the controller.
        /// </summary>
        public ICommand DownloadCommand { get; set; }
    }
}
