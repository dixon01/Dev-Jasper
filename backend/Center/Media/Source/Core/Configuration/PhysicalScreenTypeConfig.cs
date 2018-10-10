// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenTypeConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The physical screen type config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The physical screen type config.
    /// </summary>
    [XmlRoot("ScreenType")]
    public class PhysicalScreenTypeConfig
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the available resolutions.
        /// </summary>
        [XmlArray("AvailableResolutions"), XmlArrayItem("Resolution")]
        public List<ResolutionConfiguration> AvailableResolutions { get; set; }
    }
}
