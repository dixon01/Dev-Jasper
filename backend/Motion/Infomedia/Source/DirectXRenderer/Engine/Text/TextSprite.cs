// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSprite.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSprite type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.RendererBase.Layouting;
    using Gorba.Motion.Infomedia.RendererBase.Manager;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// The text sprite.
    /// </summary>
    public class TextSprite : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TextRenderManager<IDxDeviceRenderContext> manager;

        private readonly IDxDeviceRenderContext context;

        private int currentWidth;
        private int currentHeight;

        private Font currentFont;
        private HorizontalAlignment currentAlign;
        private VerticalAlignment currentVAlign;
        private TextOverflow currentOverflow;

        private AlternationList<DxFormattedText, DxPart> alternatives;

        private int alternationInterval;

        private bool scroll;

        private int maxWidth; // widest altrernative
        private int sumPartWidth;

        private Sprite sprite;

        private DxTextFactory factory;

        private bool isRingscroll;

        private long scrollCounterOnTextSet;

        private Rectangle currentLocation;
        
        private long? persistentCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSprite"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public TextSprite(TextRenderManager<IDxDeviceRenderContext> manager, IDxDeviceRenderContext context)
        {
            this.manager = manager;
            this.context = context;

            if (context.Config.Text.TextMode == TextMode.FontSprite)
            {
                this.sprite = new Sprite(this.context.Device);
            }
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
            if (text.Equals(this.CurrentText)
                && this.currentWidth == this.manager.Width
                && this.currentHeight == this.manager.Height
                && this.currentFont == this.manager.Font
                && this.currentAlign == this.manager.Align
                && this.currentVAlign == this.manager.VAlign
                && this.currentOverflow == this.manager.Overflow)
            {
                return;
            }

            Logger.Trace("Changing text to {0}", text);
            this.ClearAlternatives();
            this.CurrentText = text;
            this.currentWidth = this.manager.Width;
            this.currentHeight = this.manager.Height;
            this.currentFont = this.manager.Font;
            this.currentAlign = this.manager.Align;
            this.currentVAlign = this.manager.VAlign;
            this.currentOverflow = this.manager.Overflow;
            this.scrollCounterOnTextSet = this.context.ScrollCounter;

            this.factory = new DxTextFactory(this.sprite, this.context) { BoldWeight = (int)FontWeight.Black };
            this.alternatives = this.factory.ParseAlternatives(text, this.manager.Font);
            this.scroll = this.manager.Overflow == TextOverflow.ScrollAlways
                          || this.manager.Overflow == TextOverflow.ScrollRing;
            this.isRingscroll = this.manager.Overflow == TextOverflow.ScrollRing;

            this.maxWidth = 0;
            this.sumPartWidth = 0;

            var interval = this.alternatives.Interval ?? this.context.Config.Text.AlternationInterval;
            this.alternationInterval = (int)interval.TotalMilliseconds;
            if (this.alternationInterval <= 0)
            {
                this.alternationInterval = 3000;
            }

            foreach (var alternative in this.alternatives)
            {
                this.UpdateAlternativeBounds(alternative);
            }

            Logger.Trace("New text bounds calculated");

            // Improved Persistent scroll
            if (this.manager.Overflow == TextOverflow.Scroll || this.manager.Overflow == TextOverflow.ScrollAlways
                || this.manager.Overflow == TextOverflow.ScrollRing)
            {
                this.persistentCounter = (long)this.context.PersistenceView.GetObject(this.manager.ElementId);
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
        /// <param name="renderContext">
        /// The context.
        /// </param>
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. No good fnction extraction left.")]
        public void Render(Rectangle deviceViewport, int alpha, IDxRenderContext renderContext)
        {
            if (this.alternatives == null || this.alternatives.Count == 0)
            {
                Logger.Trace("Text not drawn as it does not have any alternatives");
                return;
            }

            var alternationCounter = renderContext.ScrollCounter / this.alternationInterval;
            var altIndex = (int)(alternationCounter % this.alternatives.Count);
            var alternative = this.alternatives[altIndex];
            if (alternative.RenderParts.Count == 0)
            {
                // no text to draw
                return;
            }

            if (alternative.UpdateContent())
            {
                this.UpdateAlternativeBounds(alternative);
            }

            // not enough space in the box
            if (this.manager.Overflow == TextOverflow.Scroll && this.maxWidth > this.currentWidth)
            {
                this.scroll = true;
            }

            var oldViewport = this.SetViewport(deviceViewport);

            if (this.sprite != null)
            {
                this.sprite.Begin(SpriteFlags.SortTexture | SpriteFlags.AlphaBlend);
            }

            try
            {
                var rotationAngle = (float)(this.manager.Rotation * Math.PI / 180);
                var rotationCenter = new PointF(
                        this.manager.X + deviceViewport.X + (this.manager.Width / 2f),
                        this.manager.Y + deviceViewport.Y + (this.manager.Height / 2f));

                var offsetY = deviceViewport.Y + this.manager.Y;
                var offsetX = deviceViewport.X + this.manager.X + this.GetScrollOffset(alternative, renderContext);
                var loopOffsetX = this.CalculateLoopOffset(offsetX);

                // fill whole box if not ringscroll
                var rightLimit = deviceViewport.X + this.manager.X + this.currentWidth;
                while (loopOffsetX < rightLimit)
                {
                    foreach (var part in alternative.RenderParts)
                    {
                        if (part.Blink && !renderContext.BlinkOn)
                        {
                            continue;
                        }

                        if (part.X + loopOffsetX >= deviceViewport.Right
                            || part.Y + offsetY >= deviceViewport.Bottom
                            || part.X + part.Width + loopOffsetX <= deviceViewport.Left
                            || part.Y + part.Height + offsetY <= deviceViewport.Top)
                        {
                            // outside visible area
                            continue;
                        }

                        part.Render(loopOffsetX, offsetY, rotationCenter, rotationAngle, alpha, renderContext);
                        this.currentLocation.X = loopOffsetX;
                    }

                    loopOffsetX += this.sumPartWidth;

                    if (!this.isRingscroll)
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (this.sprite != null)
                {
                    this.sprite.End();
                }
            }

            if (oldViewport.HasValue)
            {
                this.context.Device.Viewport = oldViewport.Value;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.scroll)
            {
                MessageDispatcher.Instance.Broadcast(
                    new LastVisibleLocation { ItemId = this.manager.ItemId, Location = this.currentLocation });
                this.manager.UpdateItemPosition(this.currentLocation);
            }

            this.ClearAlternatives();
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

            if (this.alternatives == null)
            {
                return;
            }

            foreach (var alternative in this.alternatives)
            {
                foreach (var part in alternative.RenderParts)
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

            if (this.alternatives == null)
            {
                return;
            }

            foreach (var alternative in this.alternatives)
            {
                foreach (var part in alternative.RenderParts)
                {
                    part.OnLostDevice();
                }
            }
        }

        private static int DivisionAndRoof(int dividend, int divisor)
        {
            return ((dividend - 1) / divisor) + 1;
        }

        private int CalculateLoopOffset(int offsetX)
        {
            if (!this.isRingscroll)
            {
                return offsetX;
            }

            // move first to most left so filling is done only on the right side
            int loopOffsetX;
            if (offsetX > 0)
            {
                loopOffsetX = offsetX - (DivisionAndRoof(offsetX, this.sumPartWidth) * this.sumPartWidth);
            }
            else
            {
                loopOffsetX = offsetX;
            }

            return loopOffsetX;
        }

        private void ClearAlternatives()
        {
            if (this.alternatives == null)
            {
                return;
            }

            foreach (var alternative in this.alternatives)
            {
                alternative.Dispose();
            }

            this.alternatives = null;
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

            Viewport? oldViewport = this.context.Device.Viewport;

            var textViewport = new Rectangle(
                this.manager.X + deviceViewport.X,
                this.manager.Y + deviceViewport.Y,
                this.currentWidth,
                this.currentHeight);

            // cut off lower side if outside
            if (this.manager.X < 0)
            {
                textViewport.Width += this.manager.X;
                textViewport.X = deviceViewport.X;
            }

            if (this.manager.Y < 0)
            {
                textViewport.Height += this.manager.Y;
                textViewport.Y = deviceViewport.Y;
            }

            // cut off higher side if outside
            if (textViewport.Right > deviceViewport.Right)
            {
                textViewport.Width -= textViewport.Right - deviceViewport.Right;
            }

            if (textViewport.Bottom > deviceViewport.Bottom)
            {
                textViewport.Height -= textViewport.Bottom - deviceViewport.Bottom;
            }

            this.context.Device.Viewport = new Viewport
            {
                X = textViewport.X,
                Y = textViewport.Y,
                Width = textViewport.Width,
                Height = textViewport.Height,
                MinZ = -1f,
                MaxZ = 1f
            };
            return oldViewport;
        }

        private int GetScrollOffset(DxFormattedText alternative, IDxRenderContext renderContext)
        {
            if (!this.scroll)
            {
                return 0;
            }

            if (this.persistentCounter.HasValue)
            {
                this.scrollCounterOnTextSet = renderContext.ScrollCounter - this.persistentCounter.Value;
                this.persistentCounter = null; // only used persistence once
            }

            // start scrolling from start if text was set
            var counterDiff = renderContext.ScrollCounter - this.scrollCounterOnTextSet;
            var scrollOffset = counterDiff * this.manager.ScrollSpeed / 1000; // 1000 ms = 1 sec

            this.context.PersistenceView.UpdateObject(this.manager.ElementId, counterDiff);

            // wrap around after itemWidth + maxWidth
            int widthLimit;
            if (this.isRingscroll)
            {
                // ringscroll adds virtual space after text
                // the tail is a multiple of the part so the text will not jump on end
                var tailCount = this.currentWidth / this.maxWidth;
                widthLimit = this.maxWidth * (tailCount + 1);
            }
            else
            {
                widthLimit = this.currentWidth + this.maxWidth;
            }

            scrollOffset %= widthLimit;

            if (this.manager.ScrollSpeed < 0)
            {
                // enter from right, ringscroll starts with one row fully visible
                if (!this.isRingscroll)
                {
                    scrollOffset += this.currentWidth;
                }

                scrollOffset -= alternative.RenderParts[0].X; // compensate alignment offset
            }
            else
            {
                // enter from left, ringscroll starts with one row fully visible
                if (!this.isRingscroll)
                {
                    scrollOffset -= this.currentWidth;
                }

                // compensate alignment offset
                var lastPart = alternative.RenderParts[alternative.RenderParts.Count - 1];
                scrollOffset += this.currentWidth - lastPart.X - lastPart.Width;
            }

            return (int)scrollOffset;
        }

        private void UpdateAlternativeBounds(DxFormattedText alternative)
        {
            var layoutManager = new LayoutManager<DxPart>();

            foreach (var line in alternative.Lines)
            {
                foreach (var part in line.Parts)
                {
                    part.SetScaling(1.0);
                }

                layoutManager.AddLine(line.Parts, line.HorizontalAlignment);
            }

            layoutManager.Layout(
                new Rectangle(0, 0, this.manager.Width, this.manager.Height),
                this.manager.Align,
                alternative.VerticalAlignment ?? this.manager.VAlign,
                this.manager.Overflow);

            alternative.ClearRenderParts();
            alternative.RenderParts.AddRange(layoutManager.Items);

            if ((this.manager.Overflow != TextOverflow.Scroll
                 && this.manager.Overflow != TextOverflow.ScrollAlways
                 && this.manager.Overflow != TextOverflow.ScrollRing)
                || alternative.RenderParts.Count == 0)
            {
                return;
            }

            var minLeft = int.MaxValue;
            var maxRight = int.MinValue;
            this.sumPartWidth = 0;
            foreach (var part in alternative.RenderParts)
            {
                minLeft = Math.Min(minLeft, part.X);
                maxRight = Math.Max(maxRight, part.X + part.Width);
                this.sumPartWidth += part.Width;
            }

            this.maxWidth = Math.Max(this.maxWidth, maxRight - minLeft);

            if (this.maxWidth <= this.currentWidth && this.manager.Overflow == TextOverflow.Scroll)
            {
                return;
            }

            this.scroll = true;
            foreach (var part in alternative.RenderParts)
            {
                part.MoveTo(0, part.Y);
            }
        }
    }
}