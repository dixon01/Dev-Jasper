// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileNode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Data
{
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// A file in the tree of the current file system structure for a unit group.
    /// </summary>
    public class FileNode
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource ID (hash) of the file.
        /// </summary>
        public ResourceId ResourceId { get; set; }
    }
}