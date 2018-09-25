// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConversionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectConversionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.Config
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Per project specific configuration.
    /// The file is searched in the project directory with the name <c>ProjectConversion.{suffix}.xml</c>.
    /// </summary>
    [XmlRoot("ProjectConversion")]
    public class ProjectConversionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectConversionConfig"/> class.
        /// </summary>
        public ProjectConversionConfig()
        {
            this.IgnoreProject = false;
            this.AdditionalAssemblyReferences = new List<string>();
            this.AdditionalProjectReferences = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the project (not add it to the solution).
        /// </summary>
        [XmlElement]
        public bool IgnoreProject { get; set; }

        /// <summary>
        /// Gets or sets the list of additional assembly references, relative to the project file.
        /// The assembly reference should point to the new platform version (e.g. within <c>x.y.z-cf35</c>).
        /// </summary>
        [XmlElement("AdditionalAssemblyReference")]
        public List<string> AdditionalAssemblyReferences { get; set; }

        /// <summary>
        /// Gets or sets the list of additional project references, relative to the project file.
        /// The project reference should point to the original file, not the converted one (i.e. w/o <c>.CFxx</c>).
        /// </summary>
        [XmlElement("AdditionalProjectReference")]
        public List<string> AdditionalProjectReferences { get; set; }
    }
}
