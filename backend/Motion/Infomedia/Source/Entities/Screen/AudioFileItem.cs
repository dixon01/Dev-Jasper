// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioFileItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    /// <summary>
    /// Playback item representing an audio file being played back.
    /// </summary>
    public partial class AudioFileItem
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("AudioFile: \"{0}\"", this.Filename);
        }
    }
}