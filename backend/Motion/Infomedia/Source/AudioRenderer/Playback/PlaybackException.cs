// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaybackException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlaybackException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Playback
{
    using System;

    /// <summary>
    /// Exception that is thrown in Audio Renderer when something goes wrong with the playback of audio data.
    /// </summary>
    [Serializable]
    public class PlaybackException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackException"/> class.
        /// </summary>
        public PlaybackException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public PlaybackException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public PlaybackException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
