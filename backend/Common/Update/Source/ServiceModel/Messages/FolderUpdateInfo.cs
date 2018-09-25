// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderUpdateInfo.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FolderUpdateInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Update state information of a folder.
    /// </summary>
    [Serializable]
    public class FolderUpdateInfo : FileSystemUpdateInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderUpdateInfo"/> class.
        /// </summary>
        public FolderUpdateInfo()
        {
            this.Items = new List<FileSystemUpdateInfo>();
        }

        /// <summary>
        /// Gets or sets the items that are available in this folder on the unit.
        /// </summary>
        [XmlElement("File", typeof(FileUpdateInfo))]
        [XmlElement("Folder", typeof(FolderUpdateInfo))]
        public List<FileSystemUpdateInfo> Items { get; set; }
    }
}