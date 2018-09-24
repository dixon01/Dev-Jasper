// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Engine.Text
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.RendererBase.Text;
    using Gorba.Motion.Infomedia.SharpDXRendererTest.Config;

    using NLog;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Rectangle = System.Drawing.Rectangle;

    /// <summary>
    /// The text sprite.
    /// </summary>
    public class TextSprite : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TextRenderManager<IDxRenderContext> manager;

        private readonly Device device;

        private int currentWidth;

        private List<FormattedText<DxPart>> alternatives;

        private bool scroll;

        private int maxWidth;

        private Sprite sprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSprite"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public TextSprite(TextRenderManager<IDxRenderContext> manager, Device device)
        {
            this.manager = manager;
            this.device = device;

            if (ConfigService.Instance.Config.Text.TextMode == TextMode.FontSprite)
            {
                this.sprite = new Sprite(this.device);
            }

            this.alternatives = new List<FormattedText<DxPart>>(1);
        }

        /// <summary>
        /// Gets the current text.
        /// </summary>
        public string CurrentText { get; private set; }

        /// <summary>
        /// Sets this sprite up for later rendering.
        /// This method should be called before every rendering. 
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Setup(string text)
        {
            if (text.Equals(this.CurrentText) && this.currentWidth == this.manager.Width)
            {
                return;
            }

            Logger.Trace("Changing text to {0}", text);
            this.CurrentText = text;
            this.currentWidth = this.manager.Width;

            var factory = new DxTextFactory(this.device) { BoldWeight = (int)FontWeight.Black };
            this.alternatives = factory.ParseAlternatives(text, this.manager.Font);
            this.scroll = this.manager.Overflow == TextOverflow.ScrollAlways;
            this.maxWidth = 0;
            bool alignOnOriginalBaseline = this.manager.VAlign == VerticalAlignment.Baseline;

            foreach (var alternative in this.alternatives)
            {
                if (alternative.Parts.LastIndexOf(factory.NewLine) != alternative.Parts.Count - 1)
                {
                    // add a newline at the end to have a correct calculation
                    alternative.Parts.Add(factory.NewLine);
                }

                // we use integers instead of floats to have discrete results
                // like this we actually get to the right result faster than using floats
                int sizeFactor = 1000;
                int firstLineAscent = 0;
                int width = this.CalculateTextWidth(alternative, sizeFactor * 0.001f, ref firstLineAscent);
                this.scroll = this.scroll || (this.manager.Overflow == TextOverflow.Scroll && width > this.manager.Width);
                this.maxWidth = Math.Max(this.maxWidth, width);

                if (this.manager.Overflow != TextOverflow.Scale || this.manager.Width <= 0)
                {
                    continue;
                }

                for (int i = 0; width > this.manager.Width && i < 5; i++)
                {
                    // we only consider the original first line ascent 
                    // if VAlign is set to Baseline
                    int ascent = alignOnOriginalBaseline ? firstLineAscent : 0;

                    // calculate new size factor (an approximation to the expected width + ~5%)
                    int oldFactor = sizeFactor;
                    sizeFactor = sizeFactor * this.manager.Width / width;
                    if (oldFactor - sizeFactor > 1)
                    {
                        sizeFactor += ((oldFactor - sizeFactor) / 20) + 1;
                    }

                    width = this.CalculateTextWidth(alternative, sizeFactor * 0.001f, ref ascent);
                }
            }
        }

        /// <summary>
        /// Renders this sprite with a given alpha value.
        /// </summary>
        /// <param name="deviceViewport">
        /// The device viewport.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Render(Rectangle deviceViewport, int alpha, IDxRenderContext context)
        {
            if (this.alternatives == null || this.alternatives.Count == 0)
            {
                Logger.Trace("Text not drawn as it does not have any alternatives");
                return;
            }

            int altIndex = context.AlternationCounter % this.alternatives.Count;
            var alternative = this.alternatives[altIndex];
            if (alternative.Parts.Count == 0)
            {
                // no text to draw
                Logger.Trace("No text parts to draw");
                return;
            }

            var oldViewport = this.SetViewport(deviceViewport);

            if (this.sprite != null)
            {
                this.sprite.Begin(SpriteFlags.SortTexture | SpriteFlags.AlphaBlend);
            }

            int scrollOffset = 0;
            if (this.scroll)
            {
                scrollOffset = context.ScrollCounter * this.manager.ScrollSpeed / 1000; // 1000 ms = 1 sec
                scrollOffset %= this.currentWidth + this.maxWidth; // wrap around after itemWidth + maxWidth
                scrollOffset -= alternative.Parts[0].Bounds.X - this.manager.X; // compensate alignment offset

                if (this.manager.ScrollSpeed < 0)
                {
                    scrollOffset += this.currentWidth;
                }
                else
                {
                    scrollOffset -= this.maxWidth;
                }
            }

            foreach (var part in alternative.Parts)
            {
                if (part.Blink && !context.BlinkOn)
                {
                    continue;
                }

                int x = this.manager.X + scrollOffset - deviceViewport.X;
                int y = this.manager.Y - deviceViewport.Y;

                var partBounds = part.Bounds;
                if (partBounds.Left + x < deviceViewport.Right
                    && partBounds.Top + y < deviceViewport.Bottom
                    && partBounds.Right + x > deviceViewport.Left
                    && partBounds.Bottom + y > deviceViewport.Top)
                {
                    part.Render(this.sprite, x, y, alpha, context);
                }
            }

            if (this.sprite != null)
            {
                this.sprite.End();
            }

            if (oldViewport.HasValue)
            {
                this.device.Viewport = oldViewport.Value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var alternative in this.alternatives)
            {
                alternative.Dispose();
            }

            this.alternatives.Clear();
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.Dispose();
            this.sprite = null;
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public void OnResetDevice()
        {
            if (this.sprite != null)
            {
                this.sprite.OnResetDevice();
            }

            foreach (var alternative in this.alternatives)
            {
                foreach (var part in alternative.Parts)
                {
                    part.OnResetDevice();
                }
            }
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public void OnLostDevice()
        {
            if (this.sprite != null)
            {
                this.sprite.OnLostDevice();
            }

            foreach (var alternative in this.alternatives)
            {
                foreach (var part in alternative.Parts)
                {
                    part.OnLostDevice();
                }
            }
        }

        /// <summary>
        /// This method set the viewport if necessary and returns the previous viewport
        /// if it was changed.
        /// </summary>
        /// <param name="deviceViewport">the viewport of the device.</param>
        /// <returns>the previous viewport or null.</returns>
        private Viewport? SetViewport(Rectangle deviceViewport)
        {
            if (this.manager.Overflow != TextOverflow.Clip && !this.scroll)
            {
                return null;
            }

            Viewport? oldViewport = this.device.Viewport;

            var textViewport = new Rectangle(
                this.manager.X - deviceViewport.X,
                this.manager.Y - deviceViewport.Y,
                this.currentWidth,
                this.manager.Height);

            if (textViewport.X < 0)
            {
                textViewport.Width += textViewport.X;
                textViewport.X = 0;
            }

            if (textViewport.Y < 0)
            {
                textViewport.Height += textViewport.Y;
                textViewport.Y = 0;
            }

            this.device.Viewport = new Viewport
                                       {
                                           X = textViewport.X,
                                           Y = textViewport.Y,
                                           Width = textViewport.Width,
                                           Height = textViewport.Height,
                                           MinDepth = -1f,
                                           MaxDepth = 1f
                                       };
            return oldViewport;
        }

        private int CalculateTextWidth(FormattedText<DxPart> formattedText, float sizeFactor, ref int firstLineAscent)
        {
            int x = 0;
            int y = 0;
            int maxX = x;
            int height = -1;
            int lastNewLineIndex = -1;
            int maxAscent = -1;
            bool alignOnBaseline = this.manager.VAlign == VerticalAlignment.Baseline;

            for (int i = 0; i < formattedText.Parts.Count; i++)
            {
                var part = formattedText.Parts[i];
                if (part.IsNewLine)
                {
                    maxX = Math.Max(x, maxX);

                    // correct alignment
                    int alignOffset = 0;
                    if (this.manager.Align == HorizontalAlignment.Center)
                    {
                        alignOffset = (this.currentWidth - x) / 2;
                    }
                    else if (this.manager.Align == HorizontalAlignment.Right)
                    {
                        alignOffset = this.currentWidth - x;
                    }

                    if (alignOffset != 0)
                    {
                        // align all parts with the necessary offset (to make the text right or center aligned)
                        for (int j = lastNewLineIndex + 1; j <= i; j++)
                        {
                            var linePart = formattedText.Parts[j];
                            var bounds = linePart.Bounds;
                            bounds.X += alignOffset;
                            linePart.Bounds = bounds;
                        }
                    }

                    if (alignOnBaseline)
                    {
                        if (firstLineAscent > 0)
                        {
                            int ascentOffset = firstLineAscent - maxAscent;
                            maxAscent += ascentOffset;
                            height += ascentOffset;
                        }

                        // vertically align all parts so their baselines are at the same position
                        for (int j = lastNewLineIndex + 1; j <= i; j++)
                        {
                            var linePart = formattedText.Parts[j];
                            var bounds = linePart.Bounds;
                            bounds.Y += maxAscent - linePart.Ascent;
                            linePart.Bounds = bounds;
                        }
                    }

                    if (lastNewLineIndex == -1)
                    {
                        // report the max ascent of the first line to the caller
                        firstLineAscent = maxAscent;
                    }

                    x = 0;
                    y += height;
                    lastNewLineIndex = i;
                    height = 0;
                    maxAscent = 0;
                    continue;
                }

                var itemHeight = this.CalculatePartBounds(part, x, y, sizeFactor, ref alignOnBaseline);

                height = Math.Max(height, itemHeight);
                maxAscent = Math.Max(maxAscent, part.Ascent);
                x = part.Bounds.Right;
            }

            return maxX;
        }

        private int CalculatePartBounds(DxPart part, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            int height = part.UpdateBounds(this.sprite, x, y, sizeFactor, ref alignOnBaseline);

            var text = part as DxTextPartBase;
            if (text != null
                && (text.Font.Face != this.manager.Font.Face || text.Font.Height != this.manager.Font.Height))
            {
                // if we have different font faces or sizes, we MUST align, otherwise
                // it will look very ugly!
                alignOnBaseline = true;
            }

            return height;
        }
    }
}