// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPauseRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioPauseRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// Render engine for <see cref="AudioPauseItem"/>.
    /// </summary>
    public class AudioPauseRenderEngine :
        RenderEngineBase<AudioPauseItem,
            IAudioPauseRenderEngine<IAudioRenderContext>,
            AudioPauseRenderManager<IAudioRenderContext>>,
        IAudioPauseRenderEngine<IAudioRenderContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPauseRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The render manager.
        /// </param>
        public AudioPauseRenderEngine(AudioPauseRenderManager<IAudioRenderContext> manager)
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
            context.GetPlaylist(this.Manager.Priority).AddPause(this.Manager.Duration);
        }
    }
}