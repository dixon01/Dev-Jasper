// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for file system updates.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(FileUpdate))]
    [XmlInclude(typeof(FolderUpdate))]
    [XmlInclude(typeof(RunApplication))]
    [XmlInclude(typeof(ExecutableFile))]
    public abstract class FileSystemUpdate : ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the file or folder.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}