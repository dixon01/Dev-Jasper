// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioChannelConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio channel config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for an audio channel with speaker ports.
    /// </summary>
    [Serializable]
    public class AudioChannelConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioChannelConfig"/> class.
        /// </summary>
        public AudioChannelConfig()
        {
            this.SpeakerPorts = new List<IOPortConfig>();
        }

        /// <summary>
        /// Gets or sets the screen id used for this audio channel.
        /// </summary>
        [XmlAttribute("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the port to be used to enable and disable the speaker.
        /// The port has to be a flag value port (true/false).
        /// </summary>
        [XmlElement("SpeakerPort")]
        public List<IOPortConfig> SpeakerPorts { get; set; }
    }
}