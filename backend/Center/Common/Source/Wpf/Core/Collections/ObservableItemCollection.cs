// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableItemCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObservableItemCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> that also allows to directly track if an item's property changed.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the item in the collection.
    /// </typeparam>
    public class ObservableItemCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Event that is risen when a property of an item in this collection has changed.
        /// </summary>
        public event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.Unsubscribe(e.OldItems);
            this.Subscribe(e.NewItems);
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            this.Unsubscribe(this);
            base.ClearItems();
        }

        /// <summary>
        /// Raises the <see cref="ItemPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseItemPropertyChanged(ItemPropertyChangedEventArgs<T> e)
        {
            var handler = this.ItemPropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Subscribes to changes in the items of the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        protected virtual void Subscribe(IEnumerable list)
        {
            if (list == null)
            {
                return;
            }

            foreach (T element in list)
            {
                element.PropertyChanged += this.ContainedElementChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from changes in the items of the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        protected virtual void Unsubscribe(IEnumerable list)
        {
            if (list == null)
            {
                return;
            }

            foreach (T element in list)
            {
                element.PropertyChanged -= this.ContainedElementChanged;
            }
        }

        private void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaiseItemPropertyChanged(new ItemPropertyChangedEventArgs<T>((T)sender, e.PropertyName));
        }
    }
}
