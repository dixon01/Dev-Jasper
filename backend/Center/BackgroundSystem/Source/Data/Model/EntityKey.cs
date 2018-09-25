// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityKey.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityKey type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Model
{
    using System;

    /// <summary>
    /// Key to identify an entity by its type and id.
    /// </summary>
    internal class EntityKey : IEquatable<EntityKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityKey"/> class.
        /// </summary>
        /// <param name="id">
        /// The id (usually an integer).
        /// </param>
        /// <param name="type">
        /// The type of reference.
        /// </param>
        public EntityKey(object id, ReferenceTypes type)
        {
            this.Id = id;
            this.Type = type;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public object Id { get; private set; }

        /// <summary>
        /// Gets the type of reference.
        /// </summary>
        public ReferenceTypes Type { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EntityKey other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id) && this.Type == other.Type;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as EntityKey);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^ (int)this.Type;
            }
        }
    }
}