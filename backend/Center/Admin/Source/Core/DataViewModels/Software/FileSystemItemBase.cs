// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Software
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The base class for file system items for software package version handling.
    /// </summary>
    public abstract class FileSystemItemBase : ViewModelBase
    {
        private string name;

        private bool isEditing;

        private bool isExpanded;

        private bool isSelected;

        /// <summary>
        /// Gets or sets the name of the file or folder.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets the items inside this item.
        /// This will always return an empty list for files and the list of items for directories.
        /// </summary>
        public abstract IEnumerable<FileSystemItemBase> Items { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is being edited.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return this.isEditing;
            }

            set
            {
                this.SetProperty(ref this.isEditing, value, () => this.IsEditing);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is expanded in the tree.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected in the tree.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.SetProperty(ref this.isSelected, value, () => this.IsSelected);
            }
        }
    }
}