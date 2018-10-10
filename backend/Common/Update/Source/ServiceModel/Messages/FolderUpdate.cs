// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FolderUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Update of a folder.
    /// </summary>
    [Serializable]
    public class FolderUpdate : FileSystemUpdate, IEquatable<FolderUpdate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderUpdate"/> class.
        /// </summary>
        public FolderUpdate()
        {
            this.Items = new List<FileSystemUpdate>();
        }

        /// <summary>
        /// Gets or sets the items that have to be updated in this folder.
        /// </summary>
        [XmlElement("File", typeof(FileUpdate))]
        [XmlElement("Folder", typeof(FolderUpdate))]
        public List<FileSystemUpdate> Items { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            var clone = (FolderUpdate)base.Clone();
            clone.Items = this.Items.ConvertAll(i => (FileSystemUpdate)i.Clone());
            return clone;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(FolderUpdate other)
        {
            if (other == null || this.Items.Count != other.Items.Count
                || !this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            foreach (var item in this.Items)
            {
                if (!other.Items.Contains(item))
                {
                    return false;
                }
            }

            return true;
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
            var other = obj as FolderUpdate;
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
            return this.Name.GetHashCode();
        }
    }
}