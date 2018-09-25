// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListSprite.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageListSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.ImageList
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image;
    using Gorba.Motion.Infomedia.RendererBase;
    using Gorba.Motion.Infomedia.RendererBase.Layouting;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// The image list sprite.
    /// </summary>
    public class ImageListSprite : IDisposable
    {
        private static readonly string ImageListMissing = ImageSprite.ResourcePrefix + "ImageListMissing";

        private readonly ImageListRenderManager<IDxDeviceRenderContext> manager;

        private readonly IDxDeviceRenderContext renderContext;

        private readonly List<RenderItem> items = new List<RenderItem>();

        private string[] currentImages;
        private string currentFallbackImage;

        private int currentImageWidth;
        private int currentImageHeight;

        private TextDirection currentDirection;
        private int currentHorizontalImageGap;
        private int currentVerticalImageGap;
        private Rectangle currentBounds;
        private HorizontalAlignment currentAlign;
        private TextOverflow currentOverflow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageListSprite"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public ImageListSprite(ImageListRenderManager<IDxDeviceRenderContext> manager, IDxDeviceRenderContext context)
        {
            this.manager = manager;
            this.renderContext = context;
        }

        /// <summary>
        /// Sets this sprite up for later rendering.
        /// This method should be called before every rendering.
        /// </summary>
        /// <param name="images">
        /// The list of images.
        /// </param>
        public void Setup(string[] images)
        {
            if (this.currentImages != images
                || this.currentFallbackImage != this.manager.FallbackImage
                || this.currentImageWidth != this.manager.ImageWidth
                || this.currentImageHeight != this.manager.ImageHeight)
            {
                this.currentImages = images;
                this.currentFallbackImage = this.manager.FallbackImage;
                this.currentImageWidth = this.manager.ImageWidth;
                this.currentImageHeight = this.manager.ImageHeight;

                this.DisposeSprites();

                if (images.Length == 0)
                {
                    return;
                }

                foreach (var image in images)
                {
                    var sprite = new ImageSprite(this.renderContext);
                    string text = null;

                    if (!sprite.Setup(image))
                    {
                        if (string.IsNullOrEmpty(this.manager.FallbackImage))
                        {
                            sprite.Setup(ImageListMissing);
                            text = image;
                        }
                        else if (!sprite.Setup(this.manager.FallbackImage))
                        {
                            sprite.Dispose();
                            continue;
                        }
                    }

                    this.items.Add(new RenderItem(sprite, text, this.GetSize(sprite)));
                }
            }
            else if (this.currentDirection == this.manager.Direction
                && this.currentHorizontalImageGap == this.manager.HorizontalImageGap
                && this.currentVerticalImageGap == this.manager.VerticalImageGap
                && this.currentBounds.Equals(this.manager.Bounds)
                && this.currentAlign == this.manager.Align
                && this.currentOverflow == this.manager.Overflow)
            {
                return;
            }

            this.currentDirection = this.manager.Direction;
            this.currentHorizontalImageGap = this.manager.HorizontalImageGap;
            this.currentVerticalImageGap = this.manager.VerticalImageGap;
            this.currentBounds = this.manager.Bounds;
            this.currentAlign = this.manager.Align;
            this.currentOverflow = this.manager.Overflow;

            this.CalculateBounds();
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public void Render(Rectangle bounds, int alpha, IDxDeviceRenderContext context)
        {
            foreach (var item in this.items)
            {
                if (item.Bounds.Bottom > bounds.Bottom)
                {
                    // this is the first item that runs out of the bounds,
                    // so we stop rendering anything else
                    return;
                }

                item.Render(alpha, context);
            }
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public void OnResetDevice()
        {
            this.items.ForEach(s => s.Sprite.OnResetDevice());
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public void OnLostDevice()
        {
            this.items.ForEach(s => s.Sprite.OnLostDevice());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.DisposeSprites();
        }

        private void CalculateBounds()
        {
            var layoutManager = new LayoutManager<LayoutItem>();
            var line = this.items;
            if (this.manager.Direction == TextDirection.RTL)
            {
                line = new List<RenderItem>(line);
                line.Reverse();
            }

            layoutManager.AddLine(
                new[] { new LayoutItem(line, this.manager.HorizontalImageGap, this.manager.VerticalImageGap) });
            layoutManager.Layout(
                this.manager.Bounds, this.manager.Align, VerticalAlignment.Top, this.manager.Overflow, false);
        }

        private Size GetSize(ImageSprite sprite)
        {
            const int MaxValue = 100000;

            var width = this.manager.ImageWidth;
            var height = this.manager.ImageHeight;

            if (width == 0 && height == 0)
            {
                return sprite.Size;
            }

            if (width == 0)
            {
                width = MaxValue;
            }
            else if (height == 0)
            {
                height = MaxValue;
            }
            else
            {
                return new Size(width, height);
            }

            var rect = RenderHelper.ApplyScaling(new Rectangle(0, 0, width, height), sprite.Size, ElementScaling.Scale);
            return rect.Size;
        }

        private void DisposeSprites()
        {
            this.items.ForEach(s => s.Sprite.Dispose());
            this.items.Clear();
        }

        private class LayoutItem : ILayoutItem, ISplittable<LayoutItem>
        {
            private readonly List<RenderItem> items;

            private readonly int verticalGap;

            public LayoutItem(List<RenderItem> items, int horizontalGap, int verticalGap)
            {
                this.items = items;
                this.HorizontalGapAfter = horizontalGap;
                this.verticalGap = verticalGap;

                foreach (var item in items)
                {
                    this.Width += item.Bounds.Width + horizontalGap;
                    this.Height = Math.Max(this.Height, item.Bounds.Height);
                }

                this.Ascent = this.Height;
                this.Height += verticalGap;

                if (this.Width > 0)
                {
                    this.Width -= horizontalGap;
                }
            }

            public int X { get; private set; }

            public int Y { get; private set; }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public int Ascent { get; private set; }

            public int HorizontalGapAfter { get; private set; }

            public void MoveTo(int x, int y)
            {
                this.X = x;
                this.Y = y;

                var posX = x;
                foreach (var item in this.items)
                {
                    var bounds = item.Bounds;
                    bounds.X = posX;
                    bounds.Y = y;
                    item.Bounds = bounds;
                    posX += bounds.Width + this.HorizontalGapAfter;
                }
            }

            public void SetScaling(double factor)
            {
                if (factor < 1.0)
                {
                    throw new NotSupportedException();
                }
            }

            public bool Split(int offset, out LayoutItem left, out LayoutItem right)
            {
                var x = 0;
                for (int i = 0; i < this.items.Count; i++)
                {
                    var item = this.items[i];
                    x += item.Bounds.Width;
                    if (x <= offset)
                    {
                        x += this.HorizontalGapAfter;
                        continue;
                    }

                    if (i == 0)
                    {
                        // can't split if the first item is already too big
                        if (this.items.Count == 1)
                        {
                            break;
                        }

                        i = 1;
                    }

                    var first = this.items.GetRange(0, i);
                    var second = this.items.GetRange(i, this.items.Count - i);

                    left = new LayoutItem(first, this.HorizontalGapAfter, this.verticalGap);
                    right = new LayoutItem(second, this.HorizontalGapAfter, this.verticalGap);
                    return true;
                }

                left = this;
                right = null;
                return false;
            }
        }

        private class RenderItem
        {
            private readonly string label;

            private IFontInfo fontInfo;

            public RenderItem(ImageSprite sprite, string label, Size size)
            {
                this.label = label;
                this.Sprite = sprite;
                this.Bounds = new Rectangle(Point.Empty, size);
            }

            public ImageSprite Sprite { get; private set; }

            public Rectangle Bounds { get; set; }

            public void Render(int alpha, IDxDeviceRenderContext context)
            {
                this.Sprite.Render(this.Bounds, ElementScaling.Stretch, alpha, context);

                if (this.label == null)
                {
                    return;
                }

                if (this.fontInfo == null)
                {
                    this.fontInfo = context.GetFontInfo(
                        24,
                        0,
                        FontWeight.Regular,
                        10,
                        false,
                        CharacterSet.Default,
                        Precision.Default,
                        FontQuality.Default,
                        PitchAndFamily.DefaultPitch,
                        "Arial");
                }

                this.fontInfo.Font.DrawText(null, this.label, this.Bounds.Left, this.Bounds.Bottom, Color.Black);
            }
        }
    }
}
