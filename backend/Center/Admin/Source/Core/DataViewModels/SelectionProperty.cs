// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectionProperty.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SelectionProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Type to select the value of a property from a list instead of entering it in the given (text) editor.
    /// </summary>
    /// <typeparam name="TDataViewModel">
    /// The type of the data view model that owns this property.
    /// </typeparam>
    /// <typeparam name="TProperty">
    /// The type of the property represented by this object.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of the item to select from the list (this can be different from the property type).
    /// </typeparam>
    public sealed class SelectionProperty<TDataViewModel, TProperty, TItem> : ItemSelectionViewModelBase
        where TDataViewModel : DataViewModelBase
    {
        private readonly TDataViewModel dataViewModel;

        private readonly Func<TDataViewModel, TProperty> getter;

        private readonly Action<TDataViewModel, TProperty> setter;

        private readonly Func<TItem, TProperty> converter;

        private readonly List<ItemWrapper> items;

        private object selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionProperty{TDataViewModel,TProperty,TItem}"/> class.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model that owns this property.
        /// </param>
        /// <param name="getter">
        /// The getter of the property.
        /// </param>
        /// <param name="setter">
        /// The setter of the property.
        /// </param>
        /// <param name="items">
        /// The items selectable from the list.
        /// </param>
        /// <param name="converter">
        /// The converter to convert from the selectable items to the property value.
        /// </param>
        /// <param name="toDisplayString">
        /// The converter to convert from the selectable items to a string (for display purposes only).
        /// </param>
        public SelectionProperty(
            TDataViewModel dataViewModel,
            Func<TDataViewModel, TProperty> getter,
            Action<TDataViewModel, TProperty> setter,
            IEnumerable<TItem> items,
            Func<TItem, TProperty> converter,
            Func<TItem, string> toDisplayString)
        {
            this.dataViewModel = dataViewModel;
            this.getter = getter;
            this.setter = setter;
            this.converter = converter;

            this.items = items.Select(i => new ItemWrapper(i, toDisplayString)).ToList();

            this.dataViewModel.PropertyChanged += (s, e) => this.UpdateSelectedItem();
            this.UpdateSelectedItem();
        }

        /// <summary>
        /// Gets the items available for selection.
        /// The items in this list are actually wrappers around the <see cref="TItem"/> type,
        /// so don't try to cast them.
        /// </summary>
        public override ICollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/> collection.
        /// The object in this property is actually a wrapper around the <see cref="TItem"/> type,
        /// so don't try to cast them.
        /// </summary>
        public override object SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                var wrapper = value as ItemWrapper;
                if (!this.SetProperty(ref this.selectedItem, wrapper, () => this.SelectedItem))
                {
                    return;
                }

                this.setter(
                    this.dataViewModel,
                    wrapper == null ? default(TProperty) : this.converter(wrapper.Item));
            }
        }

        private void UpdateSelectedItem()
        {
            var originalValue = this.getter(this.dataViewModel);
            this.SelectedItem = this.items.FirstOrDefault(i => this.converter(i.Item).Equals(originalValue));
        }

        private class ItemWrapper
        {
            private readonly Func<TItem, string> toString;

            public ItemWrapper(TItem item, Func<TItem, string> toString)
            {
                this.toString = toString;
                this.Item = item;
            }

            public TItem Item { get; private set; }

            public override string ToString()
            {
                return this.toString(this.Item);
            }
        }
    }
}