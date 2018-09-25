// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    /// <summary>
    /// Screen item representing a video.
    /// </summary>
    public partial class VideoItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Video: \"{0}\" @ [{1},{2}]", this.VideoUri, this.X, this.Y);
        }
    }
}
