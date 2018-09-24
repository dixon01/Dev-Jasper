// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeViewFolderItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System.Collections;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The tree view folder item used to show multiple collections on the same TreeView level.
    /// </summary>
    public class TreeViewFolderItem : DataViewModelBase
    {
        private string name;

        private IEnumerable items;

        private bool isExpanded;

        private bool isChildItemSelected;

        /// <summary>
        /// Gets or sets a value indicating whether is expanded.
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
        /// Gets or sets a value indicating whether one of the child items is selected.
        /// </summary>
        public bool IsChildItemSelected
        {
            get
            {
                return this.isChildItemSelected;
            }

            set
            {
                this.SetProperty(ref this.isChildItemSelected, value, () => this.IsChildItemSelected);
            }
        }

        /// <summary>
        /// Gets or sets the name of the folder.
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
        /// Gets or sets the child items.
        /// </summary>
        public IEnumerable Items
        {
            get
            {
                return this.items;
            }

            set
            {
                this.SetProperty(ref this.items, value, () => this.Items);
            }
        }

        /// <summary>
        /// This method is invoked by WPF to render the object if
        /// no data template is available.
        /// </summary>
        /// <returns>Returns the value of the <see cref="Name"/>
        /// property.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name, this.Name);
        }
    }
}
