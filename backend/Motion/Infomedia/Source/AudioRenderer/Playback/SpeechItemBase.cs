// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpeechItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpeechItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;

    using Gorba.Common.Configuration.Infomedia.AudioRenderer;

    /// <summary>
    /// The base class for all items that do text-to-speech using different engines.
    /// To create an object implementing this class, call <see cref="Create(TextToSpeechApi,string,string,int)"/>.
    /// </summary>
    internal abstract class SpeechItemBase : PlaybackItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechItemBase"/> class.
        /// </summary>
        /// <param name="volume">
        /// The audio volume at which to speak the text (0..100).
        /// </param>
        protected SpeechItemBase(int volume)
            : base(volume)
        {
        }

        /// <summary>
        /// Creates an object implementing <see cref="SpeechItemBase"/> depending on the
        /// <see cref="api"/> that is provided.
        /// </summary>
        /// <param name="api">
        /// The API to use when speaking the text.
        /// </param>
        /// <param name="voice">
        /// The voice to be used.
        /// </param>
        /// <param name="text">
        /// The text to be spoken.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to speak the text (0..100).
        /// </param>
        /// <returns>
        /// An object implementing <see cref="SpeechItemBase"/>.
        /// </returns>
        public static SpeechItemBase Create(TextToSpeechApi api, string voice, string text, int volume)
        {
            var item = Create(api, volume);
            item.Configure(voice, text);
            return item;
        }

        /// <summary>
        /// Configure this item.
        /// </summary>
        /// <param name="voice">
        /// The voice to be used.
        /// </param>
        /// <param name="text">
        /// The text to be spoken.
        /// </param>
        protected abstract void Configure(string voice, string text);

        private static SpeechItemBase Create(TextToSpeechApi api, int volume)
        {
            switch (api)
            {
                case TextToSpeechApi.Microsoft:
                    return new MicrosoftSpeechItem(volume);
                case TextToSpeechApi.Acapela:
                    return new AcapelaSpeechItem(volume);
                default:
                    throw new ArgumentOutOfRangeException("api");
            }
        }
    }
}