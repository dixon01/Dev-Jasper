// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToSpeechRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextToSpeechRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AudioRenderer.Engine
{
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// Render engine for <see cref="TextToSpeechItem"/>.
    /// </summary>
    public class TextToSpeechRenderEngine :
        RenderEngineBase<TextToSpeechItem,
            ITextToSpeechRenderEngine<IAudioRenderContext>,
            TextToSpeechRenderManager<IAudioRenderContext>>,
        ITextToSpeechRenderEngine<IAudioRenderContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextToSpeechRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The render manager.
        /// </param>
        public TextToSpeechRenderEngine(TextToSpeechRenderManager<IAudioRenderContext> manager)
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
            context.GetPlaylist(this.Manager.Priority)
                   .AddSpeech(this.Manager.Voice, this.Manager.Value, this.Manager.Volume);
        }
    }
}