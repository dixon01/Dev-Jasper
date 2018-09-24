// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// The config of a single screen to be used for rendering.
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
            this.X = -1;
            this.Y = 0;
            this.Width = -1;
            this.Height = -1;
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
        /// Gets or sets the x position of the viewport of the screen.
        /// The default value is -1, meaning the viewport is calculated
        /// by putting this screen on the right of the pervious screen.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the x position of the viewport of the screen.
        /// The default value is 0.
        /// </summary>
        [DefaultValue(0)]
        [XmlAttribute]
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// The default value is -1, meaning the adapter's currently set 
        /// screen width will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// The default value is -1, meaning the adapter's currently set 
        /// screen height will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Height { get; set; }
    }
}