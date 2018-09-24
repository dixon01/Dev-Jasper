// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackedResourceReference.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TrackedResourceReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;

    /// <summary>
    /// The tracked resource reference.
    /// </summary>
    public class TrackedResourceReference : IEquatable<TrackedResourceReference>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedResourceReference"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public TrackedResourceReference(DataViewModelBase viewModel, string propertyName)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            this.Key = new DataViewModelReferenceKey(viewModel);
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public DataViewModelReferenceKey Key { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(TrackedResourceReference other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Key.Equals(other.Key) && string.Equals(this.PropertyName, other.PropertyName);
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

            return this.Equals((TrackedResourceReference)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Key.GetHashCode() * 397)
                       ^ (this.PropertyName != null ? this.PropertyName.GetHashCode() : 0);
            }
        }
    }
}