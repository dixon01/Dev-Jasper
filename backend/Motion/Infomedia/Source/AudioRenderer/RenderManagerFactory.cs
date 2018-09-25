// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer
{
    using Gorba.Motion.Infomedia.AudioRenderer.Engine;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The render manager factory for audio render managers.
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IAudioRenderContext>
    {
        /// <summary>
        /// Creates an <see cref="IAudioFileRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IAudioFileRenderEngine<IAudioRenderContext> CreateEngine(
            AudioFileRenderManager<IAudioRenderContext> manager)
        {
            return new AudioFileRenderEngine(manager);
        }

        /// <summary>
        /// Creates an <see cref="IAudioPauseRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IAudioPauseRenderEngine<IAudioRenderContext> CreateEngine(
            AudioPauseRenderManager<IAudioRenderContext> manager)
        {
            return new AudioPauseRenderEngine(manager);
        }

        /// <summary>
        /// Creates an <see cref="IAudioPauseRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override ITextToSpeechRenderEngine<IAudioRenderContext> CreateEngine(
            TextToSpeechRenderManager<IAudioRenderContext> manager)
        {
            return new TextToSpeechRenderEngine(manager);
        }
    }
}