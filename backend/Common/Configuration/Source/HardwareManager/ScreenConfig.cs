// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The configuration of a single screen in hardware.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration of a single screen in hardware.
    /// </summary>
    [Serializable]
    public class ScreenConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenConfig"/> class.
        /// </summary>
        public ScreenConfig()
        {
            this.Adapter = -1;
            this.Width = 0;
            this.Height = 0;
        }

        /// <summary>
        /// Gets or sets the adapter ordinal.
        /// The default value is -1, meaning the adapter ordinal will be
        /// taken from the index in the config file.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Adapter { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// </summary>
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// </summary>
        [XmlAttribute]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the display screen.
        /// </summary>
        [XmlAttribute]
        public OrientationMode Orientation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is the screen is the main display or not.
        /// </summary>
        [XmlAttribute]
        public bool IsMainDisplay { get; set; }
    }
}