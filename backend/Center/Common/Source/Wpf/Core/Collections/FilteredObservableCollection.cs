// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteredObservableCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilteredObservableCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// A read-only observable collection that filters item depending on a filter function.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in the collection.
    /// </typeparam>
    public class FilteredObservableCollection<T> : IList<T>, INotifyCollectionChanged
        where T : INotifyPropertyChanged
    {
        private readonly ObservableItemCollection<T> baseCollection;

        private readonly Func<T, bool> filter;

        private readonly List<IndexItem> filtered;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="baseCollection">
        /// The base collection which is to be filtered.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public FilteredObservableCollection(ObservableItemCollection<T> baseCollection, Func<T, bool> filter)
        {
            this.baseCollection = baseCollection;
            this.filter = filter;

            this.filtered =
                new List<IndexItem>(
                    this.baseCollection.Where(this.filter).Select(item => new IndexItem(item, this.baseCollection)));

            this.baseCollection.ItemPropertyChanged += this.BaseCollectionOnItemPropertyChanged;
            this.baseCollection.CollectionChanged += this.BaseCollectionOnCollectionChanged;
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return this.filtered.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">
        /// The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The property is set because this list is read-only.
        /// </exception>
        public T this[int index]
        {
            get
            {
                return this.filtered[index].Item;
            }

            set
            {
                throw new NotSupportedException(
                    "Can't add to the filtered collection directly, use the base collection instead");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return this.filtered.Select(item => item.Item).GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// Always because this list is read-only.
        /// </exception>
        public void Add(T item)
        {
            throw new NotSupportedException(
                "Can't add to the filtered collection directly, use the base collection instead");
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// Always because this list is read-only.
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException(
                "Can't clear the filtered collection directly, use the base collection instead");
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the list; otherwise, false.
        /// </returns>
        /// <param name="item">
        /// The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        public bool Contains(T item)
        {
            return this.filtered.Any(i => object.Equals(i.Item, item));
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an
        /// <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied
        /// from <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// The <see cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.filtered.Select(i => i.Item).ToList().CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// This method also returns false if <paramref name="item"/> is not found in the
        /// original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <exception cref="T:System.NotSupportedException">
        /// Always because this list is read-only.
        /// </exception>
        public bool Remove(T item)
        {
            throw new NotSupportedException(
                "Can't remove from the filtered collection directly, use the base collection instead");
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// Always because this list is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException(
                "Can't remove from the filtered collection directly, use the base collection instead");
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(T item)
        {
            return this.filtered.FindIndex(i => object.Equals(i.Item, item));
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// Always because this list is read-only.
        /// </exception>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException(
                "Can't insert into the filtered collection directly, use the base collection instead");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BaseCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                    {
                        throw new NotSupportedException("Expected Add with a single object");
                    }

                    this.filtered.ForEach(i => i.UpdateIndex());

                    var newItem = (T)e.NewItems[0];
                    if (!this.filter(newItem))
                    {
                        // nothing to do since the item is not visible
                        return;
                    }

                    this.AddIndexItem(newItem);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("Expected Remove with a single object");
                    }

                    this.filtered.ForEach(i => i.UpdateIndex());

                    var oldItem = (T)e.OldItems[0];
                    if (!this.filter(oldItem))
                    {
                        // nothing to do since the item is not visible
                        return;
                    }

                    this.RemoveIndexItem(oldItem);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("Expected Replace with a single object");
                    }
                    
                    oldItem = (T)e.OldItems[0];
                    newItem = (T)e.NewItems[0];
                    this.ReplaceIndexItem(oldItem, newItem);
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                    {
                        throw new NotSupportedException("Expected Move with a single object");
                    }

                    this.filtered.ForEach(i => i.UpdateIndex());

                    newItem = (T)e.NewItems[0];
                    this.MoveIndexItem(newItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.filtered.Clear();
                    this.RaiseCollectionChanged(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveIndexItem(T item)
        {
            var oldIndex = this.filtered.FindIndex(i => object.Equals(i.Item, item));
            this.filtered.Sort();
            var newIndex = this.filtered.FindIndex(i => object.Equals(i.Item, item));
            if (oldIndex == newIndex)
            {
                return;
            }

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, oldIndex, newIndex));
        }

        private void ReplaceIndexItem(T oldItem, T newItem)
        {
            var hasOld = this.filter(oldItem);
            var hasNew = this.filter(newItem);
            if (hasOld && !hasNew)
            {
                this.RemoveIndexItem(oldItem);
            }
            else if (!hasOld && hasNew)
            {
                this.AddIndexItem(newItem);
            }
            else if (!hasNew)
            {
                // neither old nor new, nothing to do
                return;
            }

            var index = this.filtered.FindIndex(i => object.Equals(i.Item, oldItem));
            this.filtered.RemoveAt(index);
            this.filtered.Add(new IndexItem(newItem, this.baseCollection));
            this.filtered.ForEach(i => i.UpdateIndex());
            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, newItem, index));
        }

        private void RemoveIndexItem(T item)
        {
            var oldIndex = this.filtered.FindIndex(i => object.Equals(i.Item, item));
            this.filtered.RemoveAt(oldIndex);
            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, oldIndex));
        }

        private void BaseCollectionOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<T> e)
        {
            var oldIndex = this.filtered.FindIndex(i => object.Equals(i.Item, e.Item));
            if ((oldIndex >= 0) == this.filter(e.Item))
            {
                // filter didn't change
                return;
            }

            if (oldIndex >= 0)
            {
                // the item has gone
                this.filtered.RemoveAll(i => object.Equals(i.Item, e.Item));
                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.Item, oldIndex));
                return;
            }

            // the item was added
            this.AddIndexItem(e.Item);
        }

        private void AddIndexItem(T item)
        {
            var indexItem = new IndexItem(item, this.baseCollection);
            this.filtered.Add(indexItem);
            this.filtered.Sort();

            var filterIndex = this.filtered.IndexOf(indexItem);
            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, filterIndex));
        }

        private class IndexItem : IComparable<IndexItem>
        {
            private readonly ObservableItemCollection<T> collection;

            public IndexItem(T item, ObservableItemCollection<T> collection)
            {
                this.collection = collection;
                this.Item = item;

                this.UpdateIndex();
            }

            public int Index { get; private set;  }

            public T Item { get; private set; }

            public void UpdateIndex()
            {
                this.Index = this.collection.IndexOf(this.Item);
            }

            int IComparable<IndexItem>.CompareTo(IndexItem other)
            {
                return this.Index.CompareTo((int)other.Index);
            }
        }
    }
}
