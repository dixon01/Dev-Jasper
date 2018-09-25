// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FileViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.FileSystem
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// The FileViewModel.
    /// </summary>
    public class FileViewModel : FileSystemItemViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileViewModel"/> class.
        /// </summary>
        /// <param name="file">the file</param>
        /// <param name="unit">the unit</param>
        /// <param name="commandRegistry">command registry</param>
        public FileViewModel(
            IFileInfo file, UnitViewModelBase unit, ICommandRegistry commandRegistry)
            : base(file, unit)
        {
            this.commandRegistry = commandRegistry;
            this.File = file;
            this.Size = (this.File.Size / 1024.0).ToString("n") + "KB";
        }

        /// <summary>
        /// Gets the underlying file information.
        /// </summary>
        public IFileInfo File { get; private set; }

        /// <summary>
        /// Gets the file size of the remote file.
        /// </summary>
        public string Size { get; private set; }

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
        /// Gets the download remote file command.
        /// </summary>
        public ICommand DownloadRemoteFileCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Unit.DownloadRemoteFile);
            }
        }
    }
}