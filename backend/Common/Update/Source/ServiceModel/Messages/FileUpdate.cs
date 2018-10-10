// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Update of a single file.
    /// </summary>
    [Serializable]
    public class FileUpdate : FileSystemUpdate, IEquatable<FileUpdate>
    {
        /// <summary>
        /// Gets or sets the MD5 hash of the resource that has
        /// to be copied to the given file name.
        /// </summary>
        [XmlAttribute]
        public string Hash { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(FileUpdate other)
        {
            return other != null && this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase)
                   && this.Hash.Equals(other.Hash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the
        /// current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current
        /// <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current
        /// <see cref="T:System.Object"/>.</param>
        public override bool Equals(object obj)
        {
            var other = obj as FileUpdate;
            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Hash.GetHashCode();
        }
    }
}