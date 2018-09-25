// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolutionConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The resolution configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The resolution configuration.
    /// </summary>
    [XmlRoot("Resolution")]
    public class ResolutionConfiguration
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [XmlAttribute("Width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [XmlAttribute("Height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the master layouts.
        /// </summary>
        [XmlArray("MasterLayouts"), XmlArrayItem("MasterLayout")]
        public List<MasterLayout> MasterLayouts { get; set; }

        /// <summary>
        /// Gets the resolution text.
        /// </summary>
        public string Text
        {
            get
            {
                return string.Format("{0}x{1}", this.Width, this.Height);
            }
        }
    }
}
