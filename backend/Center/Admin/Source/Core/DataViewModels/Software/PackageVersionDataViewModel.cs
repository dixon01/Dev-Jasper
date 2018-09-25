// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageVersionDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Software
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    /// <summary>
    /// Extension of the package version data view model to support manual adding and removing of files.
    /// </summary>
    public partial class PackageVersionDataViewModel
    {
        private readonly ObservableCollection<FolderItem> rootFolders = new ObservableCollection<FolderItem>();

        private FileSystemItemBase selectedItem;

        /// <summary>
        /// Gets or sets the file system item selected in the tree.
        /// </summary>
        public FileSystemItemBase SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                this.SetProperty(ref this.selectedItem, value, () => this.SelectedItem);
            }
        }

        /// <summary>
        /// Gets the root folders shown in the tree.
        /// </summary>
        public ObservableCollection<FolderItem> RootFolders
        {
            get
            {
                return this.rootFolders;
            }
        }

        /// <summary>
        /// Gets the add folder command.
        /// </summary>
        public ICommand AddFolderCommand
        {
            get
            {
                return
                    this.Factory.CommandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.Editor.PackageVersion.AddFolder);
            }
        }

        /// <summary>
        /// Gets the add file command.
        /// </summary>
        public ICommand AddFileCommand
        {
            get
            {
                return
                    this.Factory.CommandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.Editor.PackageVersion.AddFile);
            }
        }

        /// <summary>
        /// Gets the delete file/folder command.
        /// </summary>
        public ICommand DeleteItemCommand
        {
            get
            {
                return
                    this.Factory.CommandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.Editor.PackageVersion.DeleteItem);
            }
        }
    }
}
