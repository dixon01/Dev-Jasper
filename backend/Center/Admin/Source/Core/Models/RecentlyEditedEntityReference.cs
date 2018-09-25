// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentlyEditedEntityReference.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RecentlyEditedEntityReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A reference to a recently edited entity.
    /// </summary>
    [DataContract]
    public class RecentlyEditedEntityReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyEditedEntityReference"/> class.
        /// </summary>
        /// <param name="partition">
        /// The partition of the entity type.
        /// </param>
        /// <param name="entity">
        /// The entity type.
        /// </param>
        /// <param name="id">
        /// The entity instance id (usually an integer).
        /// </param>
        public RecentlyEditedEntityReference(string partition, string entity, string id)
        {
            this.Partition = partition;
            this.Entity = entity;
            this.Id = id;
        }

        /// <summary>
        /// Gets the partition of the entity type.
        /// </summary>
        [DataMember]
        public string Partition { get; private set; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        [DataMember]
        public string Entity { get; private set; }

        /// <summary>
        /// Gets the entity instance id (usually an integer).
        /// </summary>
        [DataMember]
        public string Id { get; private set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            var other = obj as RecentlyEditedEntityReference;
            return other != null && this.Partition == other.Partition && this.Entity.Equals(other.Entity)
                   && this.Id.Equals(other.Id);
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
            return this.Entity.GetHashCode() ^ this.Id.GetHashCode();
        }
    }
}