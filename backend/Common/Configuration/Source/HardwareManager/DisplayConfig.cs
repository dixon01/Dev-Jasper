// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The configuration for the different screens on hardware.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for the different screens on hardware.
    /// </summary>
    [Serializable]
    public class DisplayConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayConfig"/> class.
        /// </summary>
        public DisplayConfig()
        {
            this.Mode = DisplayMode.Clone;
            this.Screens = new List<ScreenConfig>(2);
        }

        /// <summary>
        /// Gets or sets the mode in which the two screens are used.
        /// </summary>
        [XmlAttribute]
        public DisplayMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the screens to be used.
        /// If this list is empty, all currently available adapters will be taken.
        /// </summary>
        [XmlArrayItem("Screen")]
        public List<ScreenConfig> Screens { get; set; }
    }
}