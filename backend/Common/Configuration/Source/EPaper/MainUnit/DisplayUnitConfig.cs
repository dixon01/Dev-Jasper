// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayUnitConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.EPaper.MainUnit
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The display unit config.
    /// </summary>
    [Serializable]
    [XmlRoot("DisplayUnit")]
    public class DisplayUnitConfig
    {
        /// <summary>
        /// Gets or sets the hash of the firmware (using xxHash64 algorithm).
        /// </summary>
        [XmlAttribute("FirmwareHash")]
        public string FirmwareHash { get; set; }

        /// <summary>
        /// Gets or sets the hash of the content displayed by the unit (using xxHash64 algorithm).
        /// </summary>
        [XmlAttribute("ContentHash")]
        public string ContentHash { get; set; }
    }
}