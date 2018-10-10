// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RendererConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The renderer config.
    /// </summary>
    [XmlRoot("SharpDXRenderer")]
    [Serializable]
    public class RendererConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererConfig"/> class.
        /// </summary>
        public RendererConfig()
        {
            this.Screens = new List<ScreenConfig>(2);
            this.Device = new DeviceConfig();
            this.Text = new TextConfig();
            this.Video = new VideoConfig();
        }

        /// <summary>
        /// Gets or sets the screens to use.
        /// If this list is empty, all currently available adapters will be taken.
        /// </summary>
        [XmlArrayItem("Screen")]
        public List<ScreenConfig> Screens { get; set; }

        /// <summary>
        /// Gets or sets the device configuration.
        /// </summary>
        public DeviceConfig Device { get; set; }

        /// <summary>
        /// Gets or sets the text configuration.
        /// </summary>
        public TextConfig Text { get; set; }

        /// <summary>
        /// Gets or sets the video configuration.
        /// </summary>
        public VideoConfig Video { get; set; }
    }
}
