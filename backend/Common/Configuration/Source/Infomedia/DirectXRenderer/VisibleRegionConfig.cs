// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisibleRegionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisibleRegionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the visible region of the screen
    /// </summary>
    [Serializable]
    public class VisibleRegionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibleRegionConfig"/> class.
        /// </summary>
        public VisibleRegionConfig()
        {
            this.X = 0;
            this.Y = 0;
            this.Width = -1;
            this.Height = -1;
        }

        /// <summary>
        /// Gets or sets the x position of the visible region of the screen.
        /// The default value is 0.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position of the visible region of the screen.
        /// The default value is 0.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the visible region of the screen.
        /// The default value is -1, meaning the adapter's currently set
        /// screen width will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the visible region of the screen.
        /// The default value is -1, meaning the adapter's currently set
        /// screen height will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Height { get; set; }
    }
}