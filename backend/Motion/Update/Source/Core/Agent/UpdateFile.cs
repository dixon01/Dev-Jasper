// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// The file to be updated.
    /// </summary>
    public class UpdateFile : UpdateSubNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFile"/> class.
        /// Don't use this constructor except for XML serialization!
        /// </summary>
        public UpdateFile()
            : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateFile"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent folder.
        /// </param>
        public UpdateFile(UpdateFolder parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets the resource id for the file.
        /// This value is not set when <see cref="UpdateSubNode.Action"/>
        /// is <see cref="ActionType.Delete"/>.
        /// </summary>
        [XmlIgnore]
        public ResourceId ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the resource id in XML serializable format.
        /// </summary>
        [XmlAttribute("ResourceId")]
        public string ResourceIdXml
        {
            get
            {
                return this.ResourceId != null ? this.ResourceId.Hash : null;
            }

            set
            {
                this.ResourceId = value != null ? new ResourceId(value) : null;
            }
        }
    }
}
