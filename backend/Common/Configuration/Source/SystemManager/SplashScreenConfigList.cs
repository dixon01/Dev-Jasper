// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenConfigList.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenConfigList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.SplashScreen;

    /// <summary>
    /// The list of splash screen configurations.
    /// </summary>
    [Serializable]
    public class SplashScreenConfigList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenConfigList"/> class.
        /// </summary>
        public SplashScreenConfigList()
        {
            this.X = 0;
            this.Y = 0;
            this.Width = -1;
            this.Height = -1;

            this.Items = new List<SplashScreenConfig>();
        }

        /// <summary>
        /// Gets or sets the X position of the content on the splash screen.
        /// Default value: 0.
        /// </summary>
        [XmlAttribute]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y position of the content on the splash screen.
        /// Default value: 0.
        /// </summary>
        [XmlAttribute]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the content on the splash screen.
        /// Default value: -1 (the screen width is used).
        /// </summary>
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the content on the splash screen.
        /// Default value: -1 (the screen height is used).
        /// </summary>
        [XmlAttribute]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the list of configurations.
        /// </summary>
        [XmlElement("SplashScreen")]
        public List<SplashScreenConfig> Items { get; set; }
    }
}