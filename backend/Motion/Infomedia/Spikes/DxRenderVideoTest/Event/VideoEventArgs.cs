// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest.Event
{
    using System;

    /// <summary>
    /// The event about the video stopping.
    /// </summary>
    public class VideoEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoEventArgs"/> class.
        /// </summary>
        /// <param name="videoState">
        /// The video state.
        /// </param>
        public VideoEventArgs(VideoEvent videoState)
        {
            this.VideoState = videoState;
        }

        /// <summary>
        /// Gets the video state.
        /// </summary>
        public VideoEvent VideoState { get; private set; }
    }
}
