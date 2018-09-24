// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAudioIOHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAudioIOHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    /// <summary>
    /// Access to the speaker and speaker volume I/O.
    /// </summary>
    public interface IAudioIOHandler
    {
        /// <summary>
        /// Gets or sets a value indicating whether the speaker should be enabled.
        /// </summary>
        bool SpeakerEnabled { get; set; }

        /// <summary>
        /// Gets or sets the current system speaker volume (0..100).
        /// </summary>
        int SpeakerVolume { get; set; }

        /// <summary>
        /// Gets or sets the system volume.
        /// </summary>
        int SystemVolume { get; set; }
    }
}