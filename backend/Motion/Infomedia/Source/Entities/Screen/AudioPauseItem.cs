// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPauseItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioPauseItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System.Xml;

    /// <summary>
    /// Playback item representing a pause during playback.
    /// </summary>
    public partial class AudioPauseItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("AudioPause: {0:0.00}s", this.Duration.TotalSeconds);
        }
    }
}