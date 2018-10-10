// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest
{
    using DxRenderVideoTest.Engine;

    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Samples.DirectX.UtilityToolkit;

    /// <summary>
    /// Render manager factory implementation for DirectX.
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IDxRenderContext>
    {
        private readonly Framework framework;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerFactory"/> class.
        /// </summary>
        /// <param name="framework">
        /// The framework.
        /// </param>
        public RenderManagerFactory(Framework framework)
        {
            this.framework = framework;
        }

        /// <summary>
        /// Creates an <see cref="ITextRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override ITextRenderEngine<IDxRenderContext> CreateEngine(
            TextRenderManager<IDxRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            engine.Prepare(this.framework);
            return engine;
        }

        /// <summary>
        /// Creates an <see cref="IImageRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IImageRenderEngine<IDxRenderContext> CreateEngine(
            ImageRenderManager<IDxRenderContext> manager)
        {
            var engine = new ImageRenderEngine(manager);
            engine.Prepare(this.framework);
            return engine;
        }

        /// <summary>
        /// Creates an <see cref="IVideoRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IVideoRenderEngine<IDxRenderContext> CreateEngine(
            VideoRenderManager<IDxRenderContext> manager)
        {
            // TODO: implement
            return null;
        }

        /// <summary>
        /// Creates an <see cref="IImageListRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IImageListRenderEngine<IDxRenderContext> CreateEngine(
            ImageListRenderManager<IDxRenderContext> manager)
        {
            // TODO: implement
            return null;
        }

        /// <summary>
        /// Creates an <see cref="IAnalogClockHandRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IAnalogClockHandRenderEngine<IDxRenderContext> CreateEngine(
            AnalogClockRenderManager<IDxRenderContext>.Hand manager)
        {
            var engine = new AnalogClockHandRenderEngine(manager);
            engine.Prepare(this.framework);
            return engine;
        }
    }
}
