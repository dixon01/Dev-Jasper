// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortedMap.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SortedMap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A sorted map (or dictionary) implementation that is also supported by .NET Compact Framework.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of key by which the items are ordered and can be retrieved.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The value that is stored in this map.
    /// </typeparam>
    public class SortedMap<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
        where TKey : IComparable
    {
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        private readonly SortedView view;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedMap{TKey,TValue}"/> class.
        /// </summary>
        public SortedMap()
        {
            this.view = new SortedView(this);
            this.Keys = new KeysView(this);
            this.Values = new ValuesView(this);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the
        /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys { get; private set; }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object
        /// that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values { get; private set; }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)this.dictionary).SyncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return ((ICollection)this.dictionary).IsSynchronized;
            }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get
            {
                return this.Count;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return (ICollection)this.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return (ICollection)this.Values;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return ((IDictionary)this.dictionary).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return ((IDictionary)this.dictionary).IsFixedSize;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return ((IDictionary)this.dictionary).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param>
        public TValue this[TKey key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }

            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains
        /// an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element
        /// with the key; otherwise, false.
        /// </returns>
        /// <param name="key">
        /// The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </param>
        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(TKey key, TValue value)
        {
            this.dictionary.Add(key, value);
        }

        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.
        /// This method also returns false if <paramref name="key"/> was not found in the original
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param>
        public bool Remove(TKey key)
        {
            return this.dictionary.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>
        /// contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the <paramref name="value"/> parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the
        /// collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.view.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            this.dictionary.Remove((TKey)key);
        }

        void IDictionary.Clear()
        {
            this.Clear();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator(this.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)this.dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            ((IDictionary)this.dictionary).Add(key, value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.view.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.view.CopyTo(array, index);
        }

        private abstract class ViewBase<T> : ICollection<T>, ICollection
        {
            protected readonly SortedMap<TKey, TValue> Map;

            protected ViewBase(SortedMap<TKey, TValue> map)
            {
                this.Map = map;
            }

            public int Count
            {
                get
                {
                    return this.Map.Count;
                }
            }

            public object SyncRoot
            {
                get
                {
                    return ((ICollection)this.Map).SyncRoot;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    return ((ICollection)this.Map).IsSynchronized;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public abstract bool Contains(T item);

            public abstract IEnumerator<T> GetEnumerator();

            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            public void CopyTo(Array array, int index)
            {
                foreach (var key in this)
                {
                    array.SetValue(key, index++);
                }
            }

            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                foreach (var item in this)
                {
                    array[arrayIndex] = item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        private class SortedView : ViewBase<KeyValuePair<TKey, TValue>>
        {
            public SortedView(SortedMap<TKey, TValue> map)
                : base(map)
            {
            }

            public override bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)this.Map).Contains(item);
            }

            public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                var items = new List<KeyValuePair<TKey, TValue>>(this.Map.dictionary);
                items.Sort((a, b) => a.Key.CompareTo(b.Key));
                return items.GetEnumerator();
            }
        }

        private class ValuesView : ViewBase<TValue>
        {
            public ValuesView(SortedMap<TKey, TValue> map)
                : base(map)
            {
            }

            public override bool Contains(TValue item)
            {
                foreach (var value in this)
                {
                    if (object.Equals(value, item))
                    {
                        return true;
                    }
                }

                return false;
            }

            public override IEnumerator<TValue> GetEnumerator()
            {
                foreach (var pair in this.Map)
                {
                    yield return pair.Value;
                }
            }
        }

        private class KeysView : ViewBase<TKey>
        {
            public KeysView(SortedMap<TKey, TValue> map)
                : base(map)
            {
            }

            public override IEnumerator<TKey> GetEnumerator()
            {
                foreach (var pair in this.Map)
                {
                    yield return pair.Key;
                }
            }

            public override bool Contains(TKey item)
            {
                return this.Map.ContainsKey(item);
            }
        }

        private class DictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerable;

            public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerable)
            {
                this.enumerable = enumerable;
            }

            public object Current
            {
                get
                {
                    return this.Entry;
                }
            }

            public object Key
            {
                get
                {
                    return this.enumerable.Current.Key;
                }
            }

            public object Value
            {
                get
                {
                    return this.enumerable.Current.Value;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(this.Key, this.Value);
                }
            }

            public bool MoveNext()
            {
                return this.enumerable.MoveNext();
            }

            public void Reset()
            {
                this.enumerable.Reset();
            }
        }
    }
}
