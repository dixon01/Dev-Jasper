// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceTransfer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceTransfer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Object describing a resource transfer from a <see cref="Source"/> to a <see cref="Destination"/>.
    /// </summary>
    public class ResourceTransfer : IEquatable<ResourceTransfer>
    {
        /// <summary>
        /// Gets or sets the source address.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        public MediAddress Destination { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transfer is temporary
        /// (this is not the same as the resource being temporary).
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// This is only used for temporary file transfers.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ResourceTransfer other)
        {
            return other != null
                   && object.Equals(other.Source, this.Source)
                   && other.IsTemporary == this.IsTemporary
                   && string.Equals(other.OriginalName, this.OriginalName, StringComparison.InvariantCultureIgnoreCase)
                   && object.Equals(other.Destination, this.Destination);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ResourceTransfer);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^ (this.Destination.GetHashCode() * 317);
        }
    }
}