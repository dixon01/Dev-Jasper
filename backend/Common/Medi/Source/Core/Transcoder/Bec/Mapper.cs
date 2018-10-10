// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mapper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Mapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// Base implementation for mappers.
    /// </summary>
    /// <typeparam name="T">
    /// The type to be mapped.
    /// </typeparam>
    internal abstract class Mapper<T> : IMapper<T>, IManageableTable where T : class
    {
        private readonly Dictionary<int, T> mapping = new Dictionary<int, T>();
        private readonly Dictionary<T, int> reverseMapping = new Dictionary<T, int>();

        private int nextId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mapper{T}"/> class.
        /// Key 0 will always map to null.
        /// </summary>
        protected Mapper()
        {
            this.mapping[0] = null;
        }

        /// <summary>
        /// Gets or sets the peer. A peer is a mapper used in the
        /// opposite direction when in a bidirectional communication.
        /// This allows for keys to be reused in both direction
        /// (instead of creating new keys for both directions).
        /// </summary>
        public Mapper<T> Peer { get; set; }

        /// <summary>
        /// Gets the value for a given key.
        /// This method checks the <see cref="Peer"/> if the given key is negative.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The value for the given key or null if
        /// the key does not exist.
        /// </returns>
        public T this[int key]
        {
            get
            {
                if (key < 0 && this.Peer != null)
                {
                    return this.Peer[key * -1];
                }

                return this.mapping[key];
            }
        }

        /// <summary>
        /// Gets the key for a given value.
        /// This method might create a new key if it
        /// does not yet exist.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The key for the given value.
        /// </returns>
        public int this[T value]
        {
            get
            {
                int key;
                if (!this.TryGetKey(value, out key))
                {
                    key = this.GetNextId();
                    this.Add(key, value);
                }

                return key;
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<List<ManagementProperty>> IManageableTable.GetRows()
        {
            foreach (var pair in this.mapping)
            {
                yield return new List<ManagementProperty>
                                 {
                                     new ManagementProperty<int>("Id", pair.Key, true),
                                     new ManagementProperty<string>("Value", Convert.ToString(pair.Value), true),
                                 };
            }
        }

        /// <summary>
        /// Tries to get a key for a given value.
        /// This method checks the <see cref="Peer"/> if the given value already has
        /// a key and returns it inverted (negative) if found.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// True if the key was found.
        /// </returns>
        internal bool TryGetKey(T value, out int key)
        {
            if (value == null)
            {
                key = 0;
                return true;
            }

            if (this.reverseMapping.TryGetValue(value, out key))
            {
                return true;
            }

            if (this.Peer != null && this.Peer.reverseMapping.TryGetValue(value, out key))
            {
                key *= -1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a key with its associated value to this mapper.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal void Add(int key, T value)
        {
            this.mapping.Add(key, value);
            this.reverseMapping.Add(value, key);
        }

        /// <summary>
        /// Gets the next available ID and increments the internal
        /// ID counter by one.
        /// </summary>
        /// <returns>
        /// The next available ID.
        /// </returns>
        internal int GetNextId()
        {
            return this.nextId++;
        }
    }
}
