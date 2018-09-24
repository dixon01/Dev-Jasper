// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorSignRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColorSignRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Signs
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;

    /// <summary>
    /// Renderer for color signs.
    /// </summary>
    public class ColorSignRenderer : BitmapSignRendererBase
    {
        /// <summary>
        /// Renders the given components onto the sign.
        /// </summary>
        /// <param name="components">
        /// The components to render.
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <returns>
        /// The <see cref="IFrameProvider"/> that provides all frames to show the contents of the given components.
        /// </returns>
        public override IFrameProvider Render(ICollection<ComponentBase> components, IAhdlcRenderContext context)
        {
            var bitmap = new SimpleColorPixelSource(this.Width, this.Height);
            var renderer = new GraphicsRenderer(bitmap);
            foreach (var component in components)
            {
                var text = component as TextComponent;
                if (text != null)
                {
                    if ((text.Overflow == TextOverflow.Scroll || text.Overflow == TextOverflow.ScrollAlways)
                        && text.ScrollSpeed != 0)
                    {
                        this.Logger.Warn(
                            "Color sign doesn't support scrolling, but scroll speed is set to {0}", text.ScrollSpeed);
                    }

                    renderer.RenderText(text, context);
                    continue;
                }

                var image = component as ImageComponent;
                if (image != null)
                {
                    renderer.RenderImage(image);
                    continue;
                }

                var rectangle = component as RectangleComponent;
                if (rectangle != null)
                {
                    renderer.RenderRectangle(rectangle);
                }
            }

            return new ColorBitmapFrameProvider(0, bitmap);
        }
    }
}