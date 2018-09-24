// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PointInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    using System;

    using Gorba.Motion.Obc.Bus.Core.Data;

    /// <summary>
    /// Detailed information about a point.
    /// </summary>
    [Serializable]
    public class PointInfo : PointBase
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public PointType Type { get; set; }

        /// <summary>
        /// Gets or sets the sub type.
        /// This can be either <see cref="PointSubTypeStop"/>,
        /// <see cref="PointSubTypeTrafficLight"/> or the speed limit in km/h,
        /// depending on the <see cref="Type"/>.
        /// </summary>
        public int SubType { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public PointStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the theoretical time when the bus passes this point, counted from the start of the trip.
        /// </summary>
        public TimeSpan TheoreticalPassageTime { get; set; }

        /// <summary>
        /// Gets or sets the name 1.
        /// </summary>
        public string Name1 { get; set; }

        /// <summary>
        /// Gets or sets the TTS name 1.
        /// </summary>
        public string Name1Tts { get; set; }

        /// <summary>
        /// Gets or sets the name 2.
        /// </summary>
        public string Name2 { get; set; }

        /// <summary>
        /// Gets or sets the TTS name 2.
        /// </summary>
        public string Name2Tts { get; set; }

        /// <summary>
        /// Gets or sets the sign code used for the exterior sign.
        /// </summary>
        public int SignCode { get; set; }

        /// <summary>
        /// Gets or sets the interior sound mode.
        /// </summary>
        public InteriorSoundMode InteriorSoundMode { get; set; }

        /// <summary>
        /// Gets or sets the interior announcement MP3 file number.
        /// </summary>
        public int InteriorAnnouncementMp3 { get; set; }

        /// <summary>
        /// Gets or sets the exterior announcement string.
        /// </summary>
        public string ExteriorAnnouncement { get; set; }

        /// <summary>
        /// Gets or sets the exterior announcement MP3 file number.
        /// </summary>
        public int ExteriorAnnouncementMp3 { get; set; }

        /// <summary>
        /// Gets or sets the exterior announcement TTS.
        /// </summary>
        public string ExteriorAnnouncementTts { get; set; }

        /// <summary>
        /// Gets or sets the line name.
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// Gets or sets the data associated with this point.
        /// </summary>
        public int Data { get; set; }

        /// <summary>
        /// Gets or sets the direction code.
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// Gets or sets the speech media type associated to this bus stop ID.
        /// </summary>
        /// <remarks>Attention: the values admitted are
        /// Radio = 0;
        /// GSM = 1;
        /// </remarks>
        public int VoiceType { get; set; }

        /// <summary>
        /// Gets or sets the Didok number (Atron/Biel)
        /// </summary>
        public int Didok { get; set; }

        /// <summary>
        /// Gets or sets the Didok number (Atron/Biel)
        /// </summary>
        public string DruckName { get; set; }
    }
}