// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioPlaybackEvent.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;
    using System.Linq;

    /// <summary>
    ///     Event sent from a renderer to the composer to let it know a certain
    ///     audio has finished playing.
    /// </summary>
    [Serializable]
    public class AudioPlaybackEvent
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="AudioPlaybackEvent" /> class.</summary>
        public AudioPlaybackEvent()
        {
            this.AudioZone = AudioZoneTypes.None;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the audio zone.</summary>
        public AudioZoneTypes AudioZone { get; set; }

        /// <summary>Gets or sets the <see cref="ItemBase.Id" /> of the audio.</summary>
        public int ItemId { get; set; }

        /// <summary>Gets or sets a value indicating whether playback paused.</summary>
        public bool PlaybackPaused { get; set; }

        /// <summary>Gets or sets the PortName</summary>
        public string PortName { get; set; }

        /// <summary>Gets or sets the speaker volume.</summary>
        public int SpeakerVolume { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create audio playback event.</summary>
        /// <param name="name">The name.</param>
        /// <param name="speakerEnabled">The speaker enabled.</param>
        /// <param name="interiorSpeakerName">The interior Speaker1 Name default port name.</param>
        /// <param name="exteriorSpeakerName">The exterior Speaker2 Name default port name.</param>
        /// <param name="playbackPaused">The playback Paused.</param>
        /// <returns>The <see cref="AudioPlaybackEvent"/>.</returns>
        public static AudioPlaybackEvent CreateAudioPlaybackEvent(
            string name, 
            bool speakerEnabled = true, 
            string interiorSpeakerName = "Interior", 
            string exteriorSpeakerName = "Exterior", 
            bool playbackPaused = false)
        {
            // Infotransite
            AudioZoneTypes zoneType = AudioZoneTypes.None;
            if (speakerEnabled)
            {
                if (Enum.TryParse(name, out zoneType) == false)
                {
                    if (name.Equals(interiorSpeakerName, StringComparison.OrdinalIgnoreCase) && speakerEnabled)
                    {
                        zoneType |= AudioZoneTypes.Interior;
                    }

                    if (name.Equals(exteriorSpeakerName, StringComparison.OrdinalIgnoreCase) && speakerEnabled)
                    {
                        zoneType |= AudioZoneTypes.Exterior;
                    }
                }
            }

            var audioPlaybackEvent = new AudioPlaybackEvent { PortName = name, AudioZone = zoneType, PlaybackPaused = playbackPaused };
            return audioPlaybackEvent;
        }

        /// <summary>The to string.</summary>
        /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</returns>
        public override string ToString()
        {
            return typeof(AudioPlaybackEvent).GetProperties()
                .Aggregate(string.Empty, (current, p) => current + string.Format("  {0}={1}\r\n", p.Name, p.GetValue(this)));
        }

        #endregion
    }
}