// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaybackItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlaybackItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    /// <summary>
    /// Base class for items in the <see cref="AudioPlayer"/> that have a volume.
    /// </summary>
    internal abstract class PlaybackItemBase : AudioItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackItemBase"/> class.
        /// </summary>
        /// <param name="volume">
        /// The audio volume at which to play this item (0..100).
        /// </param>
        protected PlaybackItemBase(int volume)
        {
            this.Volume = volume;
        }

        /// <summary>
        /// Gets the volume at which to play this item (0..100).
        /// </summary>
        public int Volume { get; private set; }
    }
}