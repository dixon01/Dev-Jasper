// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.ImageList;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Video;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// Render manager factory implementation for DirectX.
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IDxDeviceRenderContext>
    {
        private readonly IDxDeviceRenderContext context;

        private readonly Rectangle viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerFactory"/> class.
        /// </summary>
        /// <param name="context">
        /// The render context.
        /// </param>
        /// <param name="viewport">
        /// The viewport of the device.
        /// </param>
        public RenderManagerFactory(IDxDeviceRenderContext context, Rectangle viewport)
        {
            this.context = context;
            this.viewport = viewport;
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
        protected override ITextRenderEngine<IDxDeviceRenderContext> CreateEngine(
            TextRenderManager<IDxDeviceRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            engine.Prepare(this.context, this.viewport);
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
        protected override IImageRenderEngine<IDxDeviceRenderContext> CreateEngine(
            ImageRenderManager<IDxDeviceRenderContext> manager)
        {
            var engine = new ImageRenderEngine(manager);
            engine.Prepare(this.context, this.viewport);
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
        protected override IVideoRenderEngine<IDxDeviceRenderContext> CreateEngine(
            VideoRenderManager<IDxDeviceRenderContext> manager)
        {
            var engine = new VideoRenderEngine(manager);
            engine.Prepare(this.context, this.viewport);
            return engine;
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
        protected override IImageListRenderEngine<IDxDeviceRenderContext> CreateEngine(
            ImageListRenderManager<IDxDeviceRenderContext> manager)
        {
            var engine = new ImageListRenderEngine(manager);
            engine.Prepare(this.context, this.viewport);
            return engine;
        }

        /// <summary>
        /// Creates an <see cref="IAnalogClockRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine.
        /// </returns>
        protected override IAnalogClockRenderEngine<IDxDeviceRenderContext> CreateEngine(
            AnalogClockRenderManager<IDxDeviceRenderContext> manager)
        {
            // special case: we don't need an engine for the analog clock item
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
        protected override IAnalogClockHandRenderEngine<IDxDeviceRenderContext> CreateEngine(
            AnalogClockHandRenderManager<IDxDeviceRenderContext> manager)
        {
            var engine = new AnalogClockHandRenderEngine(manager);
            engine.Prepare(this.context, this.viewport);
            return engine;
        }
    }
}
