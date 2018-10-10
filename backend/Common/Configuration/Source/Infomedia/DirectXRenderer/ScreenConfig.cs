// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
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
            this.Width = -1;
            this.Height = -1;
            this.FallbackImage = string.Empty;
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
        /// Gets or sets the id of the screen used to identify it in Composer.
        /// If the <see cref="Id"/> is not set (null), the <see cref="Adapter"/> is used
        /// as its id.
        /// </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the width of the screen.
        /// The default value is -1, meaning the adapter's currently set
        /// screen width will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the screen.
        /// The default value is -1, meaning the adapter's currently set
        /// screen height will be used.
        /// </summary>
        [DefaultValue(-1)]
        [XmlAttribute]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the visible region of the screen.
        /// </summary>
        public VisibleRegionConfig VisibleRegion { get; set; }

        /// <summary>
        /// Gets or sets the absolute path of the fallback image.
        /// </summary>
        public string FallbackImage { get; set; }
    }
}