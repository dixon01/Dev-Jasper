// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemUpdateInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemUpdateInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for file system update state information.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(FileUpdateInfo))]
    [XmlInclude(typeof(FolderUpdateInfo))]
    public abstract class FileSystemUpdateInfo
    {
        /// <summary>
        /// Gets or sets the name of the file or folder.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current update state of this item.
        /// </summary>
        [XmlAttribute]
        public ItemUpdateState State { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}