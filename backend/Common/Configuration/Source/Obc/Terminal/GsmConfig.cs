// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GsmConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GsmConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    using Gorba.Common.Configuration.Obc.Common;

    /// <summary>
    /// The GSM configuration.
    /// </summary>
    public class GsmConfig
    {
        ////private ConfigItem<int> maxSpeechTime = new ConfigItem<int>();
        ////private ConfigItem<int> countDownWindow = new ConfigItem<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GsmConfig"/> class.
        /// </summary>
        public GsmConfig()
        {
            this.MicVolume = new ConfigItem<int>();
            this.SpeakerVolume = new ConfigItem<int>();
            this.SpeakerEnableOutput = new ConfigItem<int>();
            this.SpeakerIoInverted = new ConfigItem<bool>(false, "Set to true if the output is inverted");

            this.SpeakerEnableOutput.Description =
                "Turns off or on the speaker. The IO from the Buco. Negative value: disabled";
            this.SpeakerEnableOutput.Value = -1;

            ////maxSpeechTime.Description = "The maximum speechtime for a GSM call. (seconds)";
            ////maxSpeechTime.Value = 240;

            this.SpeakerVolume.Description = "Volume from the speaker. Between 0 and 100%";
            this.SpeakerVolume.Value = 50;

            this.MicVolume.Description = "Microphone volume. Between 0 and 100%";
            this.MicVolume.Value = 50;
        }

        /// <summary>
        /// Gets or sets the speaker I/O inverted setting.
        /// </summary>
        public ConfigItem<bool> SpeakerIoInverted { get; set; }

        /// <summary>
        /// Gets or sets the speaker output setting.
        /// </summary>
        public ConfigItem<int> SpeakerEnableOutput { get; set; }

        /* public ConfigItem<int> MaxSpeechTime
        {
            get { return maxSpeechTime; }
            set { maxSpeechTime = value; }
        }*/

        /// <summary>
        /// Gets or sets the speaker volume setting.
        /// </summary>
        public ConfigItem<int> SpeakerVolume { get; set; }

        /// <summary>
        /// Gets or sets the microphone volume setting.
        /// </summary>
        public ConfigItem<int> MicVolume { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "\tSpeakerIO_inverted: " + this.SpeakerIoInverted + "\n\tSpeakerEnableOutput: "
                   + this.SpeakerEnableOutput + "\n\tSpeakerVolume: " + this.SpeakerVolume + "\n\tMicVolume: "
                   + this.MicVolume;
        }
    }
}