// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioFileRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// Render engine for <see cref="AudioFileItem"/>.
    /// </summary>
    public class AudioFileRenderEngine :
        RenderEngineBase<AudioFileItem,
            IAudioFileRenderEngine<IAudioRenderContext>,
            AudioFileRenderManager<IAudioRenderContext>>,
        IAudioFileRenderEngine<IAudioRenderContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The render manager.
        /// </param>
        public AudioFileRenderEngine(AudioFileRenderManager<IAudioRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(IAudioRenderContext context)
        {
            context.GetPlaylist(this.Manager.Priority).AddFile(this.Manager.Filename, this.Manager.Volume);
        }
    }
}