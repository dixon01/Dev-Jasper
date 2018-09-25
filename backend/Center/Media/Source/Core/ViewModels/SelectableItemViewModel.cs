// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectableItemViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectableItemViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model for items that can be selected (e.g. in a list).
    /// </summary>
    /// <typeparam name="T">
    /// The type of the item.
    /// </typeparam>
    public class SelectableItemViewModel<T> : ViewModelBase
    {
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItemViewModel{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public SelectableItemViewModel(T item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
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