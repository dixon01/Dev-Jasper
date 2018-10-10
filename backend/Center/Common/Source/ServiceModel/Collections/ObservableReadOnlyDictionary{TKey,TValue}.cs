// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableReadOnlyDictionary{TKey,TValue}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ObservableReadOnlyDictionary type.
//   Original code:
//      Copyright (c) 2007, Dr. WPF
//      All rights reserved.
//      Redistribution and use in source and binary forms, with or without
//      modification, are permitted provided that the following conditions are met:
//      Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//      Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//      The name Dr. WPF may not be used to endorse or promote products
//      derived from this software without specific prior written permission.
//
//      THIS SOFTWARE IS PROVIDED BY Dr. WPF ``AS IS'' AND ANY
//      EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//      WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//      DISCLAIMED. IN NO EVENT SHALL Dr. WPF BE LIABLE FOR ANY
//      DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//      (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//      LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//      ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//      (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//      SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Default implementation for a read-only dictionary that supports collection change notifications.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the items in this dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of the values in this dictionary.
    /// </typeparam>
    /// <remarks>
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> events are fired when an item is added or removed.
    /// There will be one event for each of the following property names:
    /// - Count
    /// - Item[]
    /// - Keys
    /// - Values
    /// The <see cref="INotifyCollectionChanged.CollectionChanged"/> event is fired if a key is added or removed.
    /// </remarks>
    [Serializable]
    public class ObservableReadOnlyDictionary<TKey, TValue> :
        IObservableReadOnlyDictionary<TKey, TValue>,
        ISerializable,
        IDeserializationCallback
    {
        private readonly KeyedDictionaryEntryCollection keyedEntryCollection;

        private readonly Dictionary<TKey, TValue> dictionaryCache = new Dictionary<TKey, TValue>();

        [NonSerialized]
        private readonly SerializationInfo serializationInfo;

        private int countCache;

        private int dictionaryCacheVersion;

        private int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        public ObservableReadOnlyDictionary()
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public ObservableReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection();

            foreach (var entry in dictionary)
            {
                this.DoAddEntry(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public ObservableReadOnlyDictionary(IEqualityComparer<TKey> comparer)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public ObservableReadOnlyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection(comparer);

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
            {
                this.DoAddEntry(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected ObservableReadOnlyDictionary(SerializationInfo info, StreamingContext context)
        {
            this.serializationInfo = info;
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        public IEqualityComparer<TKey> Comparer
        {
            get { return this.keyedEntryCollection.Comparer; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return this.keyedEntryCollection.Count; }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get { return this.TrueDictionary.Keys; }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get { return this.TrueDictionary.Values; }
        }

        private Dictionary<TKey, TValue> TrueDictionary
        {
            get
            {
                if (this.dictionaryCacheVersion == this.version)
                {
                    return this.dictionaryCache;
                }

                this.dictionaryCache.Clear();
                foreach (var entry in this.keyedEntryCollection)
                {
                    this.dictionaryCache.Add((TKey)entry.Key, (TValue)entry.Value);
                }

                this.dictionaryCacheVersion = this.version;

                return this.dictionaryCache;
            }
        }

        /// <summary>
        /// The indexer.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TValue"/>.
        /// </returns>
        public TValue this[TKey key]
        {
            get { return (TValue)this.keyedEntryCollection[key].Value; }
            set { this.DoSetEntry(key, value); }
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Add(TKey key, TValue value)
        {
            this.DoAddEntry(key, value);
        }

        /// <summary>
        /// Checks if the key exists in the dictionary.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A flag indicating whether the key exists or not.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return this.keyedEntryCollection.Contains(key);
        }

        /// <summary>
        /// Checks if the value exists in the dictionary.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A flag indicating whether the value exists or not.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            return this.TrueDictionary.ContainsValue(value);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this, false);
        }

        /// <summary>
        /// Removes the item with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A flag indicating whether the key was present and the corresponding item removed.
        /// </returns>
        public bool Remove(TKey key)
        {
            return this.DoRemoveEntry(key);
        }

        /// <summary>
        /// Removes all items in this dictionary.
        /// </summary>
        public void Clear()
        {
            this.DoClearEntries();
        }

        /// <summary>
        /// Tries to get the value with the given key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A flag indicating whether the key was found or not.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = this.keyedEntryCollection.Contains(key);
            value = result ? (TValue)this.keyedEntryCollection[key].Value : default(TValue);
            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the
        /// collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, false);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to
        /// serialize the target object.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.
        /// </param>
        /// <param name="context">
        /// The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            var entries = new Collection<DictionaryEntry>();
            foreach (var entry in this.keyedEntryCollection)
            {
                entries.Add(entry);
            }

            info.AddValue("entries", entries);
        }

        /// <summary>
        /// Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">
        /// The object that initiated the callback. The functionality for this parameter is not currently implemented.
        /// </param>
        public virtual void OnDeserialization(object sender)
        {
            if (this.serializationInfo == null)
            {
                return;
            }

            var entries = (Collection<DictionaryEntry>)
                          this.serializationInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
            foreach (var entry in entries)
            {
                this.AddEntry((TKey)entry.Key, (TValue)entry.Value);
            }
        }

        private void FirePropertyChangedNotifications()
        {
            if (this.Count == this.countCache)
            {
                return;
            }

            this.countCache = this.Count;
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnPropertyChanged("Keys");
            this.OnPropertyChanged("Values");
        }

        private void FireResetNotifications()
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private bool AddEntry(TKey key, TValue value)
        {
            this.keyedEntryCollection.Add(new DictionaryEntry(key, value));
            return true;
        }

        private bool ClearEntries()
        {
            // check whether there are entries to clear
            var result = this.Count > 0;
            if (result)
            {
                // if so, clear the dictionary
                this.keyedEntryCollection.Clear();
            }

            return result;
        }

        private int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            var index = -1;
            if (!this.keyedEntryCollection.Contains(key))
            {
                return index;
            }

            entry = this.keyedEntryCollection[key];
            index = this.keyedEntryCollection.IndexOf(entry);

            return index;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged == null)
            {
                return;
            }

            this.CollectionChanged(this, args);
        }

        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private bool RemoveEntry(TKey key)
        {
            // remove the entry
            return this.keyedEntryCollection.Remove(key);
        }

        private bool SetEntry(TKey key, TValue value)
        {
            var keyExists = this.keyedEntryCollection.Contains(key);

            // if identical key/value pair already exists, nothing to do
            if (keyExists && value.Equals((TValue)this.keyedEntryCollection[key].Value))
            {
                return false;
            }

            // otherwise, remove the existing entry
            if (keyExists)
            {
                this.keyedEntryCollection.Remove(key);
            }

            // add the new entry
            this.keyedEntryCollection.Add(new DictionaryEntry(key, value));

            return true;
        }

        private void DoAddEntry(TKey key, TValue value)
        {
            if (!this.AddEntry(key, value))
            {
                return;
            }

            this.version++;

            DictionaryEntry entry;
            var index = this.GetIndexAndEntryForKey(key, out entry);
            this.FireEntryAddedNotifications(entry, index);
        }

        private void DoClearEntries()
        {
            if (this.ClearEntries())
            {
                this.version++;
                this.FireResetNotifications();
            }
        }

        private bool DoRemoveEntry(TKey key)
        {
            DictionaryEntry entry;
            var index = this.GetIndexAndEntryForKey(key, out entry);

            if (!this.RemoveEntry(key))
            {
                return false;
            }

            this.version++;
            if (index > -1)
            {
                this.FireEntryRemovedNotifications(entry, index);
            }

            return true;
        }

        private void DoSetEntry(TKey key, TValue value)
        {
            DictionaryEntry entry;
            var index = this.GetIndexAndEntryForKey(key, out entry);

            if (!this.SetEntry(key, value))
            {
                return;
            }

            this.version++;

            // if prior entry existed for this key, fire the removed notifications
            if (index > -1)
            {
                this.FireEntryRemovedNotifications(entry, index);

                // force the property change notifications to fire for the modified entry
                this.countCache--;
            }

            // then fire the added notifications
            index = this.GetIndexAndEntryForKey(key, out entry);
            this.FireEntryAddedNotifications(entry, index);
        }

        private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
            {
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value),
                        index));
                return;
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
            {
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value),
                        index));
                return;
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// The enumerator.
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly ObservableReadOnlyDictionary<TKey, TValue> dictionary;

            private readonly int version;

            private readonly bool isDictionaryEntryEnumerator;

            private int index;

            private KeyValuePair<TKey, TValue> current;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="dictionary">
            /// The dictionary.
            /// </param>
            /// <param name="isDictionaryEntryEnumerator">
            /// The is dictionary entry enumerator.
            /// </param>
            internal Enumerator(ObservableReadOnlyDictionary<TKey, TValue> dictionary, bool isDictionaryEntryEnumerator)
            {
                this.dictionary = dictionary;
                this.version = dictionary.version;
                this.index = -1;
                this.isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                this.current = new KeyValuePair<TKey, TValue>();
            }

            /// <summary>
            /// Gets the value of the current dictionary entry.
            /// </summary>
            /// <returns>
            /// The value of the current element of the enumeration.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of
            /// the dictionary or after the last entry.
            /// </exception>
            /// <filterpriority>2</filterpriority>
            object IDictionaryEnumerator.Value
            {
                get
                {
                    this.ValidateCurrent();
                    return this.current.Value;
                }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    this.ValidateCurrent();
                    return this.current;
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <returns>
            /// The current element in the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            object IEnumerator.Current
            {
                get
                {
                    this.ValidateCurrent();
                    if (this.isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(this.current.Key, this.current.Value);
                    }

                    return new KeyValuePair<TKey, TValue>(this.current.Key, this.current.Value);
                }
            }

            /// <summary>
            /// Gets both the key and the value of the current dictionary entry.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.DictionaryEntry"/> containing both the key and the value of the
            /// current dictionary entry.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of
            /// the dictionary or after the last entry.
            /// </exception>
            /// <filterpriority>2</filterpriority>
            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    this.ValidateCurrent();
                    return new DictionaryEntry(this.current.Key, this.current.Value);
                }
            }

            /// <summary>
            /// Gets the key of the current dictionary entry.
            /// </summary>
            /// <returns>
            /// The key of the current element of the enumeration.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The <see cref="T:System.Collections.IDictionaryEnumerator"/> is positioned before the first entry of
            /// the dictionary or after the last entry.
            /// </exception>
            /// <filterpriority>2</filterpriority>
            object IDictionaryEnumerator.Key
            {
                get
                {
                    this.ValidateCurrent();
                    return this.current.Key;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has
            /// passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            /// <filterpriority>2</filterpriority>
            public bool MoveNext()
            {
                this.ValidateVersion();
                this.index++;
                if (this.index < this.dictionary.keyedEntryCollection.Count)
                {
                    this.current =
                        new KeyValuePair<TKey, TValue>(
                            (TKey)this.dictionary.keyedEntryCollection[this.index].Key,
                            (TValue)this.dictionary.keyedEntryCollection[this.index].Value);
                    return true;
                }

                this.index = -2;
                this.current = new KeyValuePair<TKey, TValue>();
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            /// <filterpriority>2</filterpriority>
            void IEnumerator.Reset()
            {
                this.ValidateVersion();
                this.index = -1;
                this.current = new KeyValuePair<TKey, TValue>();
            }

            private void ValidateVersion()
            {
                if (this.version != this.dictionary.version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

            private void ValidateCurrent()
            {
                if (this.index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }

                if (this.index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }
        }

        private class KeyedDictionaryEntryCollection : KeyedCollection<TKey, DictionaryEntry>
        {
            public KeyedDictionaryEntryCollection()
            {
            }

            public KeyedDictionaryEntryCollection(IEqualityComparer<TKey> comparer)
                : base(comparer)
            {
            }

            protected override TKey GetKeyForItem(DictionaryEntry entry)
            {
                return (TKey)entry.Key;
            }
        }
    }
}