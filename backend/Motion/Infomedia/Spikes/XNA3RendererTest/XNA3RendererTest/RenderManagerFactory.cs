// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using System;

    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework;

    /// <summary>
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IXnaRenderContext>
    {
        private readonly GraphicsDeviceManager graphics;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerFactory"/> class.
        /// </summary>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public RenderManagerFactory(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        protected override ITextRenderEngine<IXnaRenderContext> CreateEngine(TextRenderManager<IXnaRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            engine.Prepare(this.graphics);
            return engine;
        }

        protected override IImageRenderEngine<IXnaRenderContext> CreateEngine(ImageRenderManager<IXnaRenderContext> manager)
        {
            var engine = new ImageRenderEngine(manager);
            engine.Prepare(this.graphics);
            return engine;
        }

        protected override IVideoRenderEngine<IXnaRenderContext> CreateEngine(VideoRenderManager<IXnaRenderContext> manager)
        {
            throw new NotImplementedException();
        }

        protected override IImageListRenderEngine<IXnaRenderContext> CreateEngine(ImageListRenderManager<IXnaRenderContext> manager)
        {
            throw new NotImplementedException();
        }

        protected override IAnalogClockHandRenderEngine<IXnaRenderContext> CreateEngine(AnalogClockRenderManager<IXnaRenderContext>.Hand manager)
        {
            throw new NotImplementedException();
        }
    }
}
