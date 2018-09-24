// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
{
    using System;

    /// <summary>
    /// The video configuration.
    /// </summary>
    [Serializable]
    public class VideoConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoConfig"/> class.
        /// </summary>
        public VideoConfig()
        {
            this.VideoMode = VideoMode.DirectShow;
        }

        /// <summary>
        /// Gets or sets the video mode.
        /// Default value is <see cref="Config.VideoMode.DirectShow"/>.
        /// </summary>
        public VideoMode VideoMode { get; set; }
    }
}