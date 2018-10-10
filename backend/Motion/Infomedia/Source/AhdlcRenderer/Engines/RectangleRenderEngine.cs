// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleRenderEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleRenderEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System.Drawing;

    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    /// <summary>
    /// The <see cref="IRectangleRenderEngine{TContext}"/> implementation for AHDLC.
    /// </summary>
    public class RectangleRenderEngine :
        RenderEngineBase<RectangleItem,
            IRectangleRenderEngine<IAhdlcRenderContext>,
            RectangleRenderManager<IAhdlcRenderContext>>,
        IRectangleRenderEngine<IAhdlcRenderContext>
    {
        private string lastColorString;
        private IColor lastColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleRenderEngine"/> class.
        /// </summary>
        /// <param name="manager">
        /// The render manager.
        /// </param>
        public RectangleRenderEngine(RectangleRenderManager<IAhdlcRenderContext> manager)
            : base(manager)
        {
        }

        /// <summary>
        /// Renders the object represented by this engine.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        protected override void DoRender(IAhdlcRenderContext context)
        {
            var rectangle = this.CreateComponent<RectangleComponent>();
            var colorString = this.Manager.Color.NewValue;
            if (string.IsNullOrEmpty(colorString))
            {
                this.Logger.Warn("Got empty color", colorString);
                return;
            }

            var changed = !colorString.Equals(this.lastColorString);
            if (changed)
            {
                Color color;
                if (!ParserUtil.TryParseColor(colorString, out color))
                {
                    this.Logger.Warn("Couldn't parse color: {0}", colorString);
                    return;
                }

                this.lastColor = new SimpleColor(color.R, color.G, color.B);
            }

            rectangle.Color = this.lastColor;
            context.AddItem(rectangle, changed);
            this.lastColorString = colorString;
        }
    }
}