// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSprite.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3RendererTest
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.Xna.Framework.Graphics;

    public class TextSprite : IDisposable
    {
        private readonly TextRenderManager<IXnaRenderContext> manager;

        private int currentWidth;

        private List<FormattedText<XnaPart>> alternatives;

        private bool scroll;

        private int maxWidth;

        private GraphicsDevice device;

        private SpriteBatch spriteBatch;

        public TextSprite(TextRenderManager<IXnaRenderContext> manager, GraphicsDevice device)
        {
            this.manager = manager;
            this.device = device;

            this.spriteBatch = new SpriteBatch(this.device);
            this.alternatives = new List<FormattedText<XnaPart>>(1);
        }

        public string CurrentText { get; private set; }


        public void Setup(string text)
        {
            if (text.Equals(this.CurrentText) && this.currentWidth == this.manager.Width)
            {
                return;
            }

            this.CurrentText = text;
            this.currentWidth = this.manager.Width;
             
            var factory = new XnaTextFactory { BoldWeight = (int)1/*FontWeight.Black*/ };
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

        public void Render(int alpha, IXnaRenderContext context)
        {
            if (this.alternatives == null || this.alternatives.Count == 0)
            {
                return;
            }

            int altIndex = context.AlternationCounter % this.alternatives.Count;
            var alternative = this.alternatives[altIndex];
            if (alternative.Parts.Count == 0)
            {
                // no text to draw
                return;
            }

            Viewport? oldViewport = null;
            if (this.manager.Overflow == TextOverflow.Clip || this.scroll)
            {
                oldViewport = this.device.Viewport;
                this.device.Viewport = new Viewport
                {
                    X = this.manager.X,
                    Y = this.manager.Y,
                    Width = this.currentWidth,
                    Height = this.manager.Height,
                    MinDepth = -1f,
                    MaxDepth = 1f
                };
            }

            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None);

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

                part.Render(this.spriteBatch, this.manager.X + scrollOffset, this.manager.Y, alpha, context);
            }

            this.spriteBatch.End();

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
            if (this.spriteBatch == null)
            {
                return;
            }

            foreach (var alternative in this.alternatives)
            {
                alternative.Dispose();
            }

            this.spriteBatch.Dispose();
            this.spriteBatch = null;
        }

        /// <summary>
        /// </summary>
        public void OnResetDevice()
        {
            if (this.spriteBatch == null)
            {
                return;
            }

            this.spriteBatch.Begin(); // TODO: To verify if correct

            foreach (var alternative in this.alternatives)
            {
                foreach (var part in alternative.Parts)
                {
                    part.OnResetDevice();
                }
            }
        }

        private int CalculateTextWidth(FormattedText<XnaPart> formattedText, float sizeFactor, ref int firstLineAscent)
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

        private int CalculatePartBounds(XnaPart part, int x, int y, float sizeFactor, ref bool alignOnBaseline)
        {
            int height = part.UpdateBounds(this.spriteBatch, x, y, sizeFactor, ref alignOnBaseline);

            var text = part as XnaTextPart;
            if (text != null)
            {
                if (text.Font.Face != this.manager.Font.Face || text.Font.Height != this.manager.Font.Height)
                {
                    // if we have different font faces or sizes, we MUST align, otherwise
                    // it will look very ugly!
                    alignOnBaseline = true;
                }
            }

            return height;
        }
    }
}
