// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoftwarePackageDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SoftwarePackageDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.SoftwareDescription
{
    using System.Xml.Serialization;

    /// <summary>
    /// The description of a software package.
    /// </summary>
    [XmlRoot("SoftwarePackage")]
    public class SoftwarePackageDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwarePackageDescriptor"/> class.
        /// </summary>
        public SoftwarePackageDescriptor()
        {
            this.Version = new SoftwareVersion();
        }

        /// <summary>
        /// Gets or sets the unique package id.
        /// This is usually the root namespace of the package (e.g. Gorba.Motion.Protran for Protran).
        /// </summary>
        [XmlAttribute("ID")]
        public string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the human readable name of the package (e.g. "Protran").
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the human readable description of the package.
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the information about the specific version in this package.
        /// </summary>
        public SoftwareVersion Version { get; set; }
    }
}
