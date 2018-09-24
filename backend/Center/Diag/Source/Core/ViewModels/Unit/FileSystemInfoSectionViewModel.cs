// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemInfoSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemInfoSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels.FileSystem;

    /// <summary>
    /// The view model for the section that shows the file system of a unit.
    /// </summary>
    public class FileSystemInfoSectionViewModel : InfoSectionViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private FolderViewModel selectedFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemInfoSectionViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        /// <param name="commandRegistry">the command registry</param>
        public FileSystemInfoSectionViewModel(UnitViewModelBase unit, ICommandRegistry commandRegistry)
            : base(unit)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets the load file system folder command
        /// </summary>
        public ICommand LoadFileSystemFolderCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.LoadFileSystemFolder);
            }
        }

        /// <summary>
        /// Gets or sets the selected folder
        /// </summary>
        public FolderViewModel SelectedFolder
        {
            get
            {
                return this.selectedFolder;
            }

            set
            {
                this.SetProperty(ref this.selectedFolder, value, () => this.SelectedFolder);
            }
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        public ICommand OpenRemoteFileCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.OpenRemoteFile);
            }
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        public ICommand CancelRemoteFileDownloadCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.CancelRemoteFileDownload);
            }
        }
    }
}