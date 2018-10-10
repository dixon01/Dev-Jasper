// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwareVersion.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwareVersion type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.SoftwareDescription
{
    using System.Xml.Serialization;

    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// The description of a specific version in a software package.
    /// </summary>
    public class SoftwareVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareVersion"/> class.
        /// </summary>
        public SoftwareVersion()
        {
            this.Structure = new UpdateFolderStructure();
        }

        /// <summary>
        /// Gets or sets the version number (e.g. 2.4.1444.12345).
        /// </summary>
        [XmlAttribute("Name")]
        public string VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the human readable description of this version.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the root folder structure of this software version.
        /// </summary>
        [XmlElement("Folders")]
        public UpdateFolderStructure Structure { get; set; }
    }
}