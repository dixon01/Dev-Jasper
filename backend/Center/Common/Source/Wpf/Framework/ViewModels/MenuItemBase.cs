// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Windows.Markup;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the base class for all menu items.
    /// </summary>
    [ContentProperty("Children")]
    public abstract class MenuItemBase : ViewModelBase
    {
        private readonly ObservableCollection<MenuItemBase> children;

        private int index;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItemBase"/> class.
        /// </summary>
        protected MenuItemBase()
        {
            this.children = new ObservableCollection<MenuItemBase>();
        }

        /// <summary>
        /// Gets or sets the display name of the menu item.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the command for the menu item.
        /// </summary>
        public virtual ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the image source for an option picture.
        /// </summary>
        public Uri ImageSource { get; set; }

        /// <summary>
        /// Gets or sets the index in the list of top menu items.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                this.SetProperty(ref this.index, value, () => this.Index);
            }
        }

        /// <summary>
        /// Gets the child menu items.
        /// </summary>
        public ObservableCollection<MenuItemBase> Children
        {
            get
            {
                return this.children;
            }
        }
    }
}
