// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System.Windows;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the base class for status bar items.
    /// </summary>
    public abstract class StatusBarItemBase : ViewModelBase
    {
        private object content;

        /// <summary>
        /// Gets or sets the name displayed as item description.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content of the item.
        /// </summary>
        public object Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.SetProperty(ref this.content, value, () => this.Content);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}
