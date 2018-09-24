// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FolderViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.FileSystem
{
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// The FolderViewModel.
    /// </summary>
    public class FolderViewModel : FileSystemItemViewModelBase
    {
        /// <summary>
        /// The dummy item that is not representing a real directory.
        /// </summary>
        public static readonly FolderViewModel Dummy = new FolderViewModel();

        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderViewModel"/> class.
        /// </summary>
        /// <param name="directory">the directory</param>
        /// <param name="unit">the unit</param>
        public FolderViewModel(IDirectoryInfo directory, UnitViewModelBase unit)
            : base(directory, unit)
        {
            this.Directory = directory;
            this.Children = new ObservableItemCollection<FileSystemItemViewModelBase>();
            this.Folders = new FilteredObservableCollection<FileSystemItemViewModelBase>(
                this.Children,
                i => i is FolderViewModel);
        }

        private FolderViewModel()
            : base(null, null)
        {
        }

        /// <summary>
        /// Gets the directory information.
        /// </summary>
        public IDirectoryInfo Directory { get; private set; }

        /// <summary>
        /// Gets the children of this folder
        /// </summary>
        public ObservableItemCollection<FileSystemItemViewModelBase> Children { get; private set; }

        /// <summary>
        /// Gets all folders from the <see cref="Children"/> collection.
        /// </summary>
        public FilteredObservableCollection<FileSystemItemViewModelBase> Folders { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this folder is currently loading
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.SetProperty(ref this.isLoading, value, () => this.IsLoading);
            }
        }
    }
}