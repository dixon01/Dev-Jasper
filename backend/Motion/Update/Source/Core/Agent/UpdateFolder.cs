// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFolder.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The root folder to be updated.
    /// </summary>
    public class UpdateFolder : UpdateSubNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFolder"/> class.
        /// Don't use this constructor except for XML serialization!
        /// </summary>
        public UpdateFolder()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFolder"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent folder.
        /// </param>
        public UpdateFolder(UpdateFolder parent)
            : base(parent)
        {
            this.Items = new List<UpdateSubNode>();
        }

        /// <summary>
        /// Gets or sets the items(files or folders) of the root node to be updated.
        /// </summary>
        [XmlElement("File", typeof(UpdateFile))]
        [XmlElement("Folder", typeof(UpdateFolder))]
        public List<UpdateSubNode> Items { get; set; }
    }
}
