// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenVisualizationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SplashScreenVisualizationConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for the splash screen window.
    /// </summary>
    [Serializable]
    public class SplashScreenVisualizationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenVisualizationConfig"/> class.
        /// </summary>
        public SplashScreenVisualizationConfig()
        {
            this.Enabled = true;
            this.X = 0;
            this.Y = 0;
            this.Width = -1;
            this.Height = -1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the splash screen should be shown.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

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
    }
}