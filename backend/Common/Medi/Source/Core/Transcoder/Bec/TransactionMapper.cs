// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionMapper.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionMapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.Collections.Generic;

    /// <summary>
    /// Mapper implementation that can handle transactions by 
    /// tracking changes during a transaction and then committing
    /// all changes to a parent mapper.
    /// This class is not thread-safe!
    /// </summary>
    /// <typeparam name="T">
    /// The type to be mapped.
    /// </typeparam>
    internal class TransactionMapper<T> : IMapper<T> where T : class
    {
        private readonly Mapper<T> parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionMapper{T}"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent mapper to which changes will be committed.
        /// </param>
        public TransactionMapper(Mapper<T> parent)
        {
            this.parent = parent;
            this.Changes = new Dictionary<T, int>();
        }

        /// <summary>
        /// Gets a value indicating whether this mapper has been changed since the last
        /// call to <see cref="Commit"/>.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return this.Changes.Count > 0;
            }
        }

        /// <summary>
        /// Gets a list of changes done in this transaction.
        /// This will be cleared when <see cref="Commit"/> is called.
        /// </summary>
        public Dictionary<T, int> Changes { get; private set; }

        /// <summary>
        /// Gets the value for a given key.
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
                return this.parent[key];
            }
        }

        /// <summary>
        /// Gets the key for a given value.
        /// This method first checks the parent and if it can't find
        /// the value, it will create a new key.
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
                if (!this.parent.TryGetKey(value, out key))
                {
                    if (!this.Changes.TryGetValue(value, out key))
                    {
                        key = this.parent.GetNextId();
                        this.Changes.Add(value, key);
                    }
                }

                return key;
            }
        }

        /// <summary>
        /// Commits all changes to the parent and clears the
        /// <see cref="Changes"/> dictionary.
        /// </summary>
        public void Commit()
        {
            foreach (var change in this.Changes)
            {
                this.parent.Add(change.Value, change.Key);
            }

            this.Changes.Clear();
        }
    }
}