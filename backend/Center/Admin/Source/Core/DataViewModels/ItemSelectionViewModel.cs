// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSelectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemSelectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System.Collections;

    /// <summary>
    /// Simple implementation of <see cref="ItemSelectionViewModelBase"/> that has an array of typed objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items stored in this view model.
    /// </typeparam>
    public class ItemSelectionViewModel<T> : ItemSelectionViewModelBase
    {
        private T selectedObject;

        private T[] objects;

        /// <summary>
        /// Gets or sets the items from which the reference can be selected.
        /// </summary>
        public T[] Objects
        {
            get
            {
                return this.objects;
            }

            set
            {
                if (this.SetProperty(ref this.objects, value, () => this.Objects))
                {
                    this.RaisePropertyChanged(() => this.Items);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/>.
        /// </summary>
        public T SelectedObject
        {
            get
            {
                return this.selectedObject;
            }

            set
            {
                if (this.SetProperty(ref this.selectedObject, value, () => this.SelectedObject))
                {
                    this.RaisePropertyChanged(() => this.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Gets the entities available for selection.
        /// </summary>
        public override ICollection Items
        {
            get
            {
                return this.Objects;
            }
        }

        /// <summary>
        /// Gets or sets the selected item from the <see cref="Items"/> collection.
        /// </summary>
        public override object SelectedItem
        {
            get
            {
                return this.SelectedObject;
            }

            set
            {
                this.SelectedObject = (T)value;
            }
        }
    }
}