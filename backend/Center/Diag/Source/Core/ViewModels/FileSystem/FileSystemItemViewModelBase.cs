// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemItemViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemItemViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.FileSystem
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// View model base class for all file system view models.
    /// </summary>
    public abstract class FileSystemItemViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemItemViewModelBase"/> class.
        /// </summary>
        /// <param name="item">the file system item</param>
        /// <param name="unit">the unit on which this item resides</param>
        protected FileSystemItemViewModelBase(IFileSystemInfo item, UnitViewModelBase unit)
        {
            if (item == null)
            {
                return;
            }

            this.Path = item.FullName;
            this.Name = item.Name;
            this.LastWriteTime = item.LastWriteTime.ToString("dd.MM.yyyy HH:mm:ss");
            this.Unit = unit;
        }

        /// <summary>
        /// Gets the file name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the path to the folder
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the LastWriteTime of the folder
        /// </summary>
        public string LastWriteTime { get; private set; }

        /// <summary>
        /// Gets the unit on which this item resides.
        /// </summary>
        public UnitViewModelBase Unit { get; private set; }
    }
}