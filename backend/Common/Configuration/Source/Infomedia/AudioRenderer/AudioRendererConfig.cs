// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioRendererConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioRendererConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the audio renderer.
    /// </summary>
    [XmlRoot("AudioRenderer")]
    [Serializable]
    public class AudioRendererConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioRendererConfig"/> class.
        /// </summary>
        public AudioRendererConfig()
        {
            this.IO = new IOConfig();
            this.TextToSpeech = new TextToSpeechConfig();
            this.AudioChannels = new List<AudioChannelConfig>();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input =
                        typeof(AudioRendererConfig).Assembly.GetManifestResourceStream(
                            typeof(AudioRendererConfig), "AudioRenderer.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find AudioRenderer.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the I/O configuration.
        /// </summary>
        public IOConfig IO { get; set; }

        /// <summary>
        /// Gets or sets the configured audio channels.
        /// </summary>
        [XmlArrayItem("AudioChannel")]
        public List<AudioChannelConfig> AudioChannels { get; set; }

        /// <summary>
        /// Gets or sets the text-to-speech configuration.
        /// </summary>
        public TextToSpeechConfig TextToSpeech { get; set; }
    }
}
