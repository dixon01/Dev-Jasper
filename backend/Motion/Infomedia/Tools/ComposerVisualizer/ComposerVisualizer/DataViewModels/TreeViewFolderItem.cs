// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeViewFolderItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The tree view folder item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels
{
    using System.Collections;

    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore;

    /// <summary>
    /// The tree view folder item.
    /// </summary>
    public class TreeViewFolderItem : ViewModelBase
    {
        private IEnumerable items;

        private string name;

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
    }
}
