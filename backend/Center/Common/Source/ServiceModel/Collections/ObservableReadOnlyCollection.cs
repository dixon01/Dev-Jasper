// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableReadOnlyCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObservableReadOnlyCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Class implementing <see cref="IObservableReadOnlyCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in this list.
    /// </typeparam>
    public class ObservableReadOnlyCollection<T> : IObservableReadOnlyCollection<T>
    {
        private readonly List<T> list = new List<T>();

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used
        /// to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the end of this list.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(T item)
        {
            this.list.Add(item);
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                item,
                this.list.Count - 1);
            this.RaiseCollectionChanged(args);
        }

        /// <summary>
        /// Removes an item from this list.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// True if the item was found, otherwise false.
        /// </returns>
        public bool Remove(T item)
        {
            var index = this.list.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            this.list.RemoveAt(index);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            this.RaiseCollectionChanged(args);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }
    }
}
