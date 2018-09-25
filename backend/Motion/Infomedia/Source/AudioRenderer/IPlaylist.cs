// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlaylist.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPlaylist type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using System;
    using System.Speech.Synthesis;

    /// <summary>
    /// A playlist of items (files, TTS, pause) to play.
    /// </summary>
    public interface IPlaylist
    {
        /// <summary>
        /// Add an audio file to play.
        /// </summary>
        /// <param name="fileName">
        /// The absolute file name.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to play to file (0..100).
        /// </param>
        void AddFile(string fileName, int volume);

        /// <summary>
        /// Add a text to be spoken.
        /// </summary>
        /// <param name="voice">
        /// The TTS voice to use.
        /// </param>
        /// <param name="text">
        /// The text to speak.
        /// </param>
        /// <param name="volume">
        /// The audio volume at which to speak the prompt (0..100).
        /// </param>
        void AddSpeech(string voice, string text, int volume);

        /// <summary>
        /// Add a pause during playback.
        /// </summary>
        /// <param name="duration">
        /// The duration of the pause.
        /// </param>
        void AddPause(TimeSpan duration);
    }
}