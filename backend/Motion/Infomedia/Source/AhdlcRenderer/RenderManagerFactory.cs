// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer
{
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The factory for render managers for the AHDLC renderer.
    /// </summary>
    public class RenderManagerFactory : RenderManagerFactoryBase<IAhdlcRenderContext>
    {
        /// <summary>
        /// Creates an <see cref="ITextRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected override ITextRenderEngine<IAhdlcRenderContext> CreateEngine(
            TextRenderManager<IAhdlcRenderContext> manager)
        {
            return new TextRenderEngine(manager);
        }

        /// <summary>
        /// Creates an <see cref="IImageRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected override IImageRenderEngine<IAhdlcRenderContext> CreateEngine(
            ImageRenderManager<IAhdlcRenderContext> manager)
        {
            return new ImageRenderEngine(manager);
        }

        /// <summary>
        /// Creates an <see cref="IRectangleRenderEngine{TContext}"/> for the given manager.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <returns>
        /// The newly created engine or null if no rendering is required.
        /// </returns>
        protected override IRectangleRenderEngine<IAhdlcRenderContext> CreateEngine(
            RectangleRenderManager<IAhdlcRenderContext> manager)
        {
            return new RectangleRenderEngine(manager);
        }
    }
}