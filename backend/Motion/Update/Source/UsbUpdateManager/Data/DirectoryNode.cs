// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryNode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectoryNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// A directory in the tree of the current file system structure for a unit group.
    /// </summary>
    public class DirectoryNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryNode"/> class.
        /// </summary>
        public DirectoryNode()
        {
            this.Modifiable = true;
            this.Files = new List<FileNode>();
            this.Directories = new List<DirectoryNode>();
        }

        /// <summary>
        /// Gets or sets the name of the directory.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this directory modifiable (can be deleted).
        /// </summary>
        [XmlAttribute]
        public bool Modifiable { get; set; }

        /// <summary>
        /// Gets or sets the list of files in this directory.
        /// </summary>
        public List<FileNode> Files { get; set; }

        /// <summary>
        /// Gets or sets the list of sub-directories in this directory.
        /// </summary>
        public List<DirectoryNode> Directories { get; set; }
    }
}