// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest
{
    using System.Drawing;

    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Image;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Video;

    using SharpDX.Direct3D9;

    /// <summary>
    /// Render manager factory implementation for DirectX.
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IDxRenderContext>
    {
        private readonly Device device;

        private readonly Rectangle viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerFactory"/> class.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="viewport">
        /// The viewport of the device.
        /// </param>
        public RenderManagerFactory(Device device, Rectangle viewport)
        {
            this.device = device;
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
        protected override ITextRenderEngine<IDxRenderContext> CreateEngine(
            TextRenderManager<IDxRenderContext> manager)
        {
            var engine = new TextRenderEngine(manager);
            engine.Prepare(this.device, this.viewport);
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
            engine.Prepare(this.device, this.viewport);
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
            var engine = new VideoRenderEngine(manager);
            engine.Prepare(this.device, this.viewport);
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
            engine.Prepare(this.device, this.viewport);
            return engine;
        }
    }
}
