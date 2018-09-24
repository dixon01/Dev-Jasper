// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFolderStructure.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFolderStructure type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// The folder structure of an update.
    /// </summary>
    [Serializable]
    public class UpdateFolderStructure : IEquatable<UpdateFolderStructure>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFolderStructure"/> class.
        /// </summary>
        public UpdateFolderStructure()
        {
            this.Folders = new List<FolderUpdate>();
        }

        /// <summary>
        /// Gets or sets the root folders of this update.
        /// </summary>
        [XmlElement("Folder")]
        public List<FolderUpdate> Folders { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(UpdateFolderStructure other)
        {
            if (other == null || this.Folders.Count != other.Folders.Count)
            {
                return false;
            }

            foreach (var folder in this.Folders)
            {
                if (!other.Folders.Contains(folder))
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
            var other = obj as UpdateFolderStructure;
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
            return this.Folders.Count * 371;
        }
    }
}
