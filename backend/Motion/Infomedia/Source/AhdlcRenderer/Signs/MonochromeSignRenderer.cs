// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonochromeSignRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MonochromeSignRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Signs
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Protocols.Ahdlc.Source;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;

    /// <summary>
    /// Sign renderer that can handle monochrome signs.
    /// </summary>
    public class MonochromeSignRenderer : BitmapSignRendererBase
    {
        private const int MaxScrollingAreas = 2;

        private Brightness brightness;

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
            var scrolling = this.GetScrollTexts(components, context);
            if (scrolling.Count == 0)
            {
                return this.RenderStaticBitmap(components, context);
            }

            return this.RenderBlockScroll(components, scrolling, context);
        }

        /// <summary>
        /// Configures this renderer.
        /// </summary>
        /// <param name="config">
        /// The sign configuration.
        /// </param>
        protected override void Configure(SignConfig config)
        {
            base.Configure(config);
            switch (config.Brightness)
            {
                case SignBrightness.Brightness1:
                    this.brightness = Brightness.Brightness1;
                    break;
                case SignBrightness.Brightness2:
                    this.brightness = Brightness.Brightness2;
                    break;
                case SignBrightness.Brightness3:
                    this.brightness = Brightness.Brightness3;
                    break;
                default:
                    this.brightness = Brightness.Default;
                    break;
            }
        }

        private List<ScrollText> GetScrollTexts(IEnumerable<ComponentBase> components, IAhdlcRenderContext context)
        {
            var scrolling = new List<ScrollText>();
            foreach (var component in components)
            {
                var text = component as TextComponent;
                if (text == null || text.ScrollSpeed == 0)
                {
                    continue;
                }

                Rectangle textBounds;
                if (text.Overflow == TextOverflow.ScrollAlways)
                {
                    textBounds = this.GetTextBounds(text, context);
                }
                else if (text.Overflow != TextOverflow.Scroll)
                {
                    continue;
                }
                else
                {
                    textBounds = this.GetTextBounds(text, context);
                    if (textBounds.Width <= text.Width)
                    {
                        continue;
                    }
                }

                var source = new SimpleMonochromePixelSource(textBounds.Width, this.Height);
                var renderer = new GraphicsRenderer(source);
                renderer.RenderText(-textBounds.X, 0, text, context); // TODO: use MonochromeOffsetPixelSource instead!
                scrolling.Add(new ScrollText(text, source));
            }

            return scrolling;
        }

        private Rectangle GetTextBounds(TextComponent text, IAhdlcRenderContext context)
        {
            var minX = text.X + text.Width;
            var maxX = text.X;
            foreach (var part in text.GetLayoutParts(context))
            {
                minX = Math.Min(minX, part.X);
                maxX = Math.Max(maxX, part.X + part.Width);
            }

            if (maxX < minX)
            {
                // return the default rectangle
                return new Rectangle(text.X, text.Y, text.Width, text.Height);
            }

            return new Rectangle(minX, text.Y, maxX - minX, text.Height);
        }

        private IFrameProvider RenderBlockScroll(
            IEnumerable<ComponentBase> components, List<ScrollText> scrolling, IAhdlcRenderContext context)
        {
            if (scrolling.Count > MaxScrollingAreas)
            {
                this.Logger.Warn(
                    "Layout has {0} scrolling texts, only {1} are supported; will only scroll the first {1}",
                    scrolling.Count,
                    MaxScrollingAreas);
                while (scrolling.Count > MaxScrollingAreas)
                {
                    scrolling.RemoveAt(MaxScrollingAreas);
                }
            }

            var background = new SimpleMonochromePixelSource(this.Width, this.Height);
            var renderer = new GraphicsRenderer(background);
            foreach (var component in components)
            {
                var text = component as TextComponent;
                if (text != null && scrolling.Find(s => object.ReferenceEquals(s.Text, text)) == null)
                {
                    // render only non-scrolling texts
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

            var provider = new BlockScrollBitmapFrameProvider(background, this.Width, this.Height, this.brightness);
            foreach (var scrollText in scrolling)
            {
                var text = scrollText.Text;
                var speed = this.GetScrollSpeed(text.ScrollSpeed);
                provider.AddScrollBlock(
                    new Rectangle(text.X, text.Y, text.Width, text.Height), speed, scrollText.Source);
            }

            return provider;
        }

        private ScrollSpeed GetScrollSpeed(int scrollSpeed)
        {
            // TODO: define proper mapping with approximate pixel/second values
            switch (Math.Abs(scrollSpeed))
            {
                case 1:
                    return ScrollSpeed.Slowest;
                case 2:
                    return ScrollSpeed.Slow;
                case 3:
                    return ScrollSpeed.Fast;
                default:
                    return ScrollSpeed.Fastest;
            }
        }

        private IFrameProvider RenderStaticBitmap(IEnumerable<ComponentBase> components, IAhdlcRenderContext context)
        {
            var bitmap = new SimpleMonochromePixelSource(this.Width, this.Height);
            var renderer = new GraphicsRenderer(bitmap);
            foreach (var component in components)
            {
                var text = component as TextComponent;
                if (text != null)
                {
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

            return new StaticBitmapFrameProvider(bitmap, this.Height, this.brightness);
        }

        private class ScrollText
        {
            public ScrollText(TextComponent text, IMonochromePixelSource source)
            {
                this.Text = text;
                this.Source = source;
            }

            public TextComponent Text { get; private set; }

            public IMonochromePixelSource Source { get; private set; }
        }
    }
}