// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAudioRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAudioRenderContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using Gorba.Motion.Infomedia.AudioRenderer.Playback;
    using Gorba.Motion.Infomedia.RendererBase;

    /// <summary>
    /// The audio render context.
    /// </summary>
    public interface IAudioRenderContext : IRenderContext
    {
        /// <summary>
        /// Gets the playlist for the given priority.
        /// This playlist can be used to add files, TTS and pauses.
        /// </summary>
        /// <param name="priority">
        /// The priority with which the items should be played.
        /// The lower the number, the lower the priority.
        /// </param>
        /// <returns>
        /// A playlist object to be filled.
        /// </returns>
        IPlaylist GetPlaylist(int priority);
    }
}