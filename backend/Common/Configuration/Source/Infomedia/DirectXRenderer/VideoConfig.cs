// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
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
        /// Default value is <see cref="DirectXRenderer.VideoMode.DirectShow"/>.
        /// </summary>
        public VideoMode VideoMode { get; set; }
    }
}