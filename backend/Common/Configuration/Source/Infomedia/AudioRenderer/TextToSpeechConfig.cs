// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextToSpeechConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The text-to-speech configuration.
    /// </summary>
    [Serializable]
    public class TextToSpeechConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextToSpeechConfig"/> class.
        /// </summary>
        public TextToSpeechConfig()
        {
            this.Api = TextToSpeechApi.Acapela;
        }

        /// <summary>
        /// Gets or sets the API to use when rendering text-to-speech.
        /// Default value is <see cref="TextToSpeechApi.Acapela"/>.
        /// </summary>
        [XmlElement("API")]
        public TextToSpeechApi Api { get; set; }

        /// <summary>
        /// Gets or sets the hint path where text-to-speech engine related files are located.
        /// By default each engine searches in the most usual places for its file;
        /// this setting allows you to search first in the given path.
        /// </summary>
        /// <remarks>
        /// For Acapela, this path is used to find the <code>AcaTTS.dll</code>.
        /// </remarks>
        public string HintPath { get; set; }
    }
}