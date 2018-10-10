// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicsRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicsRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Parts;

    /// <summary>
    /// Rendering component that can render <see cref="ComponentBase"/> objects to a graphics context.
    /// </summary>
    public class GraphicsRenderer
    {
        private readonly IGraphicsContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsRenderer"/> class.
        /// </summary>
        /// <param name="context">
        /// The context to which this renderer will render components.
        /// </param>
        public GraphicsRenderer(IGraphicsContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Renders a component on a graphics context.
        /// </summary>
        /// <param name="component">
        /// The component to render.
        /// </param>
        /// <param name="renderContext">
        /// The rendering context.
        /// </param>
        public void Render(ComponentBase component, IAhdlcRenderContext renderContext)
        {
            var text = component as TextComponent;
            if (text != null)
            {
                this.RenderText(text, renderContext);
                return;
            }

            var image = component as ImageComponent;
            if (image != null)
            {
                this.RenderImage(image);
                return;
            }

            var rect = component as RectangleComponent;
            if (rect != null)
            {
                this.RenderRectangle(rect);
            }
        }

        /// <summary>
        /// Renders the given <see cref="TextComponent"/> on a graphics context.
        /// </summary>
        /// <param name="text">
        /// The text to render.
        /// </param>
        /// <param name="renderContext">
        /// The rendering context.
        /// </param>
        public void RenderText(TextComponent text, IAhdlcRenderContext renderContext)
        {
            this.RenderText(0, 0, text, renderContext);
        }

        /// <summary>
        /// Renders the given <see cref="TextComponent"/> on a graphics context.
        /// </summary>
        /// <param name="offsetX">
        /// The x offset (positive: move the text to the right, negative: move it to the left).
        /// </param>
        /// <param name="offsetY">
        /// The y offset (positive: move the text down, negative: move it up).
        /// </param>
        /// <param name="text">
        /// The text to render.
        /// </param>
        /// <param name="renderContext">
        /// The rendering context.
        /// </param>
        public void RenderText(int offsetX, int offsetY, TextComponent text, IAhdlcRenderContext renderContext)
        {
            // we need to call GetLayoutParts first, otherwise at least the IsInverted property is not set
            var parts = text.GetLayoutParts(renderContext);

            var localContext = this.context;
            if (text.Overflow == TextOverflow.Clip)
            {
                localContext = new MaskingGraphicsContext(
                    localContext, new Rectangle(text.X + offsetX, text.Y + offsetY, text.Width, text.Height));
            }

            if (text.IsInverted)
            {
                var foreground = text.Font.GetColor();
                localContext = new InvertedGraphicsContext(
                    localContext,
                    new Rectangle(text.X + offsetX, text.Y + offsetY, text.Width, text.Height),
                    Colors.Black,
                    new SimpleColor(foreground.R, foreground.G, foreground.B));
            }

            foreach (var part in parts)
            {
                RenderTextPart(offsetX, offsetY, part, localContext);
            }
        }

        /// <summary>
        /// Renders the given <see cref="ImageComponent"/> on a graphics context.
        /// </summary>
        /// <param name="image">
        /// The image to render.
        /// </param>
        public void RenderImage(ImageComponent image)
        {
            this.context.DrawBitmap(image.X, image.Y, image.GetBitmap());
        }

        /// <summary>
        /// Renders the given <see cref="RectangleComponent"/> on a graphics context.
        /// </summary>
        /// <param name="rectangle">
        /// The rectangle to render.
        /// </param>
        public void RenderRectangle(RectangleComponent rectangle)
        {
            var bitmap = new RectangleBitmap(rectangle.Width, rectangle.Height, rectangle.Color);
            this.context.DrawBitmap(rectangle.X, rectangle.Y, bitmap);
        }

        private static void RenderTextPart(int offsetX, int offsetY, PartBase part, IGraphicsContext localContext)
        {
            var image = part as ImagePart;
            if (image != null)
            {
                localContext.DrawBitmap(image.X + offsetX, image.Y + offsetY, image.Bitmap);
                return;
            }

            var text = part as TextPart;
            if (text == null)
            {
                return;
            }

            var font = text.BitmapFont;
            var x = part.X + offsetX;
            var y = part.Y + offsetY;

            foreach (var c in text.BidiText)
            {
                var charBmp = font.GetCharacter(c);
                localContext.DrawBitmap(x, y, charBmp);
                x += charBmp.Width;
                if (!BidiUtility.IsCharacterArabHebrew(c, BidiUtility.AvailableCharacters.Arabic))
                {
                    x += text.CharSpacing;
                }
            }
        }

        /// <summary>
        /// Graphics context wrapper that only draws to the given viewport and skips everything outside the viewport.
        /// </summary>
        private class MaskingGraphicsContext : IGraphicsContext
        {
            private readonly IGraphicsContext context;

            private readonly Rectangle viewport;

            public MaskingGraphicsContext(IGraphicsContext context, Rectangle viewport)
            {
                this.context = context;
                this.viewport = viewport;
            }

            public void DrawBitmap(int offsetX, int offsetY, IBitmap bitmap)
            {
                this.context.DrawBitmap(
                    offsetX,
                    offsetY,
                    new MaskedBitmap(
                        bitmap,
                        new Rectangle(
                            this.viewport.X - offsetX,
                            this.viewport.Y - offsetY,
                            this.viewport.Width,
                            this.viewport.Height)));
            }
        }

        private class InvertedGraphicsContext : IGraphicsContext
        {
            private readonly IGraphicsContext context;

            private readonly IColor foregroundColor;

            private readonly IColor backgroundColor;

            public InvertedGraphicsContext(
                IGraphicsContext context, Rectangle bounds, IColor foregroundColor, IColor backgroundColor)
            {
                this.context = context;
                this.foregroundColor = foregroundColor;
                this.backgroundColor = backgroundColor;

                // fill bounds with background color (former foreground color)
                context.DrawBitmap(
                    bounds.X,
                    bounds.Y,
                    new RectangleBitmap(bounds.Width, bounds.Height, this.backgroundColor));
            }

            public void DrawBitmap(int offsetX, int offsetY, IBitmap bitmap)
            {
                this.context.DrawBitmap(
                    offsetX,
                    offsetY,
                    new InvertedBitmap(bitmap, this.foregroundColor, this.backgroundColor));
            }
        }

        /// <summary>
        /// Bitmap wrapper that only returns pixels within the given viewport and
        /// skips everything outside the viewport (i.e. returns transparent pixels).
        /// </summary>
        private class MaskedBitmap : IBitmap
        {
            private readonly IBitmap bitmap;

            private readonly Rectangle viewport;

            public MaskedBitmap(IBitmap bitmap, Rectangle viewport)
            {
                this.bitmap = bitmap;
                this.viewport = viewport;

                this.Width = bitmap.Width;
                this.Height = bitmap.Height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public IColor GetPixel(int x, int y)
            {
                if (x < this.viewport.Left || x >= this.viewport.Right || y < this.viewport.Top
                    || y >= this.viewport.Bottom)
                {
                    return Colors.Transparent;
                }

                return this.bitmap.GetPixel(x, y);
            }
        }

        private class RectangleBitmap : IBitmap
        {
            private readonly IColor color;

            public RectangleBitmap(int width, int height, IColor color)
            {
                this.color = color;
                this.Width = width;
                this.Height = height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public IColor GetPixel(int x, int y)
            {
                return this.color;
            }
        }

        private class InvertedBitmap : IBitmap
        {
            private readonly IBitmap bitmap;

            private readonly IColor foregroundColor;

            private readonly IColor backgroundColor;

            public InvertedBitmap(IBitmap bitmap, IColor foregroundColor, IColor backgroundColor)
            {
                this.bitmap = bitmap;
                this.foregroundColor = foregroundColor;
                this.backgroundColor = backgroundColor;
                this.Width = bitmap.Width;
                this.Height = bitmap.Height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public IColor GetPixel(int x, int y)
            {
                var pixel = this.bitmap.GetPixel(x, y);
                return pixel.Transparent || (pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
                           ? this.backgroundColor
                           : this.foregroundColor;
            }
        }
    }
}
