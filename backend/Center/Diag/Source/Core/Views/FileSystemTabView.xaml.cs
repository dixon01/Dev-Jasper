// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemTabView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FileSystemTabView.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System.Windows.Input;

    using Gorba.Center.Diag.Core.Extensions;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    using Telerik.Windows;
    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for FileSystemTabView.xaml
    /// </summary>
    public partial class FileSystemTabView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemTabView" /> class.
        /// </summary>
        public FileSystemTabView()
        {
            InitializeComponent();
        }

        private void TreeViewLoadOnDemand(object sender, RadRoutedEventArgs e)
        {
            var itemContainer = e.OriginalSource as RadTreeViewItem;
            if (itemContainer == null)
            {
                return;
            }

            var item = itemContainer.Item;
            var folder = item as FolderViewModel;
            var context = (FileSystemInfoSectionViewModel)this.DataContext;

            context.LoadFileSystemFolderCommand.Execute(folder);
        }

        private void TreeViewItemPrepared(object sender, RadTreeViewItemPreparedEventArgs e)
        {
        }

        private void OnFolderSelected(object sender, RadRoutedEventArgs e)
        {
            var itemContainer = e.OriginalSource as RadTreeViewItem;
            if (itemContainer == null)
            {
                return;
            }

            var item = itemContainer.Item;
            var folder = item as FolderViewModel;
            var context = (FileSystemInfoSectionViewModel)this.DataContext;

            context.LoadFileSystemFolderCommand.Execute(folder);
        }

        private void OnFolderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var newFolder = this.FolderViewBox.SelectedItem;
                if (newFolder is FolderViewModel)
                {
                    var container = this.FolderTreeView.ContainerFromItem(this.FolderTreeView.SelectedItem);
                    if (container != null)
                    {
                        container.IsExpanded = true;

                        this.FolderTreeView.SelectedItem = newFolder;
                        var context = (FileSystemInfoSectionViewModel)this.DataContext;

                        if (context.LoadFileSystemFolderCommand.CanExecute(newFolder))
                        {
                            context.LoadFileSystemFolderCommand.Execute(newFolder);
                        }
                    }
                }
            }
        }

        private void OnFileMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var newFile = this.FolderViewBox.SelectedItem;
                if (newFile is FileViewModel)
                {
                    var context = (FileSystemInfoSectionViewModel)this.DataContext;
                    if (context.OpenRemoteFileCommand.CanExecute(newFile))
                    {
                        context.OpenRemoteFileCommand.Execute(newFile);
                    }
                }
            }
        }
    }
}
