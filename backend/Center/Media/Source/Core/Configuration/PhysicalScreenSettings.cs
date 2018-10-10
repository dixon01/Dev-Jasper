// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalScreenSettings.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The physical screen settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The physical screen settings.
    /// </summary>
    [XmlRoot("PhysicalScreenSettings")]
    public class PhysicalScreenSettings
    {
        /// <summary>
        /// Gets or sets the physical screen types.
        /// </summary>
        [XmlElement("ScreenType")]
        public List<PhysicalScreenTypeConfig> PhysicalScreenTypes { get; set; }
    }
}
