// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// The TextRenderEngine.
    /// </summary>
    public class TextRenderEngine : RenderEngineBase<TextItem, ITextRenderEngine<IXnaRenderContext>, TextRenderManager<IXnaRenderContext>>,
          ITextRenderEngine<IXnaRenderContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        public TextRenderEngine(TextRenderManager<IXnaRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// This method is not implemented (at the moment).
        /// </exception>
        public override void Render(double alpha, IXnaRenderContext context)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The on create device.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected override void OnCreateDevice(GraphicsDevice device)
        {
            this.Release();
        }

        /// <summary>
        /// The on reset device.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        protected override void OnResetDevice(GraphicsDevice device)
        {
        }

        /// <summary>
        /// The release.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// This method is not implemented (at the moment).
        /// </exception>
        protected override void Release()
        {
            throw new System.NotImplementedException();
        }
    }
}
