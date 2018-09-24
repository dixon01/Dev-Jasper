// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelReferenceKey.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelReferenceKey type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The key/identifier of a data view model reference.
    /// </summary>
    public class DataViewModelReferenceKey : IEquatable<DataViewModelReferenceKey>
    {
        private static readonly IEqualityComparer<DataViewModelReferenceKey> KeyComparerInstance =
            new KeyEqualityComparer();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelReferenceKey"/> class.
        /// </summary>
        /// <param name="dataViewModel">The data view model.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="dataViewModel"/> is null.
        /// </exception>
        public DataViewModelReferenceKey(DataViewModelBase dataViewModel)
        {
            if (dataViewModel == null)
            {
                throw new ArgumentNullException("dataViewModel");
            }

            this.Key = dataViewModel.ClonedFrom > 0 ? dataViewModel.ClonedFrom : dataViewModel.GetHashCode();
        }

        /// <summary>
        /// Gets the key comparer.
        /// </summary>
        public static IEqualityComparer<DataViewModelReferenceKey> KeyComparer
        {
            get
            {
                return KeyComparerInstance;
            }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public int Key { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DataViewModelReferenceKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Key == other.Key;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((DataViewModelReferenceKey)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Key;
        }

        private sealed class KeyEqualityComparer : IEqualityComparer<DataViewModelReferenceKey>
        {
            public bool Equals(DataViewModelReferenceKey x, DataViewModelReferenceKey y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Key == y.Key;
            }

            public int GetHashCode(DataViewModelReferenceKey obj)
            {
                return obj.Key;
            }
        }
    }
}