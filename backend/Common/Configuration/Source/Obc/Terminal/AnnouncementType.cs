// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnouncementType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    /// <summary>
    /// The announcement type.
    /// </summary>
    public enum AnnouncementType
    {
        /// <summary>
        /// No announcement is played.
        /// </summary>
        None = 0,

        /// <summary>
        /// Announcements are played with TTS.
        /// </summary>
        Tts = 1,

        /// <summary>
        /// Announcements are played with MP3.
        /// </summary>
        Mp3 = 2,
    }
}