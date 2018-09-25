// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FolderItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Software
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A folder inside a software package version.
    /// </summary>
    public class FolderItem : FileSystemItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderItem"/> class.
        /// </summary>
        public FolderItem()
        {
            this.Children = new ObservableCollection<FileSystemItemBase>();
        }

        /// <summary>
        /// Gets the list of children (files or folders).
        /// </summary>
        public ObservableCollection<FileSystemItemBase> Children { get; private set; }

        /// <summary>
        /// Gets the list of children (files or folders).
        /// </summary>
        public override IEnumerable<FileSystemItemBase> Items
        {
            get
            {
                return this.Children;
            }
        }
    }
}