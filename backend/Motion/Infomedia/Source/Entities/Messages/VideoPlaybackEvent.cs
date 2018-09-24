// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlaybackEvent.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoPlaybackEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Event sent from a renderer to the composer to let it know a certain
    /// video has finished playing.
    /// </summary>
    public class VideoPlaybackEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemBase.Id"/> of the video.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the URI of the video.
        /// </summary>
        public string VideoUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the video started (true) or stopped (false).
        /// </summary>
        public bool Playing { get; set; }
        
        /// <summary>
        /// Gets or sets the unit name where this video is playing
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("VideoPlaybackEvent[{0},{1} => {2}]", this.ItemId, this.VideoUri, this.Playing);
        }
    }
}
