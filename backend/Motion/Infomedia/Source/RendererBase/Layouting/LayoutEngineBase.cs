// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Base class for all layout engines.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of <see cref="ILayoutItem"/> that is being laid out.
    /// </typeparam>
    internal abstract class LayoutEngineBase<TItem>
        where TItem : class, ILayoutItem, ISplittable<TItem>
    {
        /// <summary>
        /// Gets the bounds of the area into which the contents should fit.
        /// </summary>
        protected Rectangle Bounds { get; private set; }

        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        protected HorizontalAlignment HorizontalAlignment { get; private set; }

        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        protected VerticalAlignment VerticalAlignment { get; private set; }

        /// <summary>
        /// Gets a value indicating whether text should always be aligned on the baseline.
        /// </summary>
        protected bool BaselineAlign { get; private set; }

        /// <summary>
        /// Creates a <see cref="LayoutEngineBase{TItem}"/> for the given overflow configuration.
        /// </summary>
        /// <param name="overflow">
        /// The overflow configuration.
        /// </param>
        /// <returns>
        /// An implementation of <see cref="LayoutEngineBase{TItem}"/> for the given config.
        /// </returns>
        public static LayoutEngineBase<TItem> Create(TextOverflow overflow)
        {
            switch (overflow)
            {
                case TextOverflow.Scale:
                    return new ScalingLayoutEngine<TItem>();
                case TextOverflow.Extend:
                case TextOverflow.Clip:
                case TextOverflow.Scroll:
                case TextOverflow.ScrollAlways:
                case TextOverflow.ScrollRing:
                    return new FixedLayoutEngine<TItem>();
                case TextOverflow.Wrap:
                    return new WrappingLayoutEngine<TItem>(false);
                case TextOverflow.WrapScale:
                    return new WrappingLayoutEngine<TItem>(true);
                default:
                    throw new ArgumentOutOfRangeException("overflow");
            }
        }

        /// <summary>
        /// Prepares this engine with all necessary parameters.
        /// </summary>
        /// <param name="bounds">
        /// The bounds of the area into which the contents should fit.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment.
        /// </param>
        /// <param name="baselineAlign">
        /// A flag indicating if text should always be aligned on the baseline.
        /// </param>
        public void Prepare(
            Rectangle bounds,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            bool baselineAlign)
        {
            this.Bounds = bounds;
            this.HorizontalAlignment = horizontalAlignment;
            this.VerticalAlignment = verticalAlignment;
            this.BaselineAlign = baselineAlign;
        }

        /// <summary>
        /// Does the actual layout according to the rules defined in <see cref="Prepare"/>.
        /// </summary>
        /// <param name="lines">
        /// The lines to layout.
        /// </param>
        public virtual void Layout(List<LayoutLine<TItem>> lines)
        {
            int y = this.Bounds.Y;
            int lastOffset = 0;
            foreach (var line in lines)
            {
                if (line.Count == 0)
                {
                    y += lastOffset;
                    continue;
                }

                var oldY = y;
                y = this.LayoutLine(line, y);
                this.ApplyHorizontalAlign(line);

                lastOffset = y - oldY;
            }

            this.ApplyVerticalAlign(lines, this.Bounds.Y, y);
        }

        private int LayoutLine(List<TItem> line, int offsetY)
        {
            var x = this.Bounds.X;
            var maxHeight = 0;
            var maxAscent = 0;
            var gapAfter = 0;
            foreach (var item in line)
            {
                x += gapAfter;
                maxHeight = Math.Max(maxHeight, item.Height);
                maxAscent = Math.Max(maxAscent, item.Ascent);
                item.MoveTo(x, offsetY);
                x += item.Width;
                gapAfter = item.HorizontalGapAfter;
            }

            var valign = this.BaselineAlign ? VerticalAlignment.Baseline : this.VerticalAlignment;

            if (valign == VerticalAlignment.Top)
            {
                return offsetY + maxHeight;
            }

            var maxY = offsetY;
            foreach (var item in line)
            {
                switch (valign)
                {
                    case VerticalAlignment.Middle:
                        item.MoveTo(item.X, offsetY + ((maxHeight - item.Height) / 2));
                        break;
                    case VerticalAlignment.Baseline:
                        item.MoveTo(item.X, offsetY + maxAscent - item.Ascent);
                        break;
                    case VerticalAlignment.Bottom:
                        item.MoveTo(item.X, offsetY + maxHeight - item.Height);
                        break;
                }

                maxY = Math.Max(maxY, item.Y + item.Height);
            }

            return maxY;
        }

        private void ApplyHorizontalAlign(LayoutLine<TItem> line)
        {
            if (line.Count == 0)
            {
                return;
            }

            var left = line[0].X;
            var rightItem = line[line.Count - 1];
            var right = rightItem.X + rightItem.Width;

            int offset;
            switch (line.HorizontalAlignment ?? this.HorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    offset = (this.Bounds.Width - right + left) / 2;
                    break;
                case HorizontalAlignment.Right:
                    offset = this.Bounds.Width - right + left;
                    break;
                default:
                    return;
            }

            if (offset == 0)
            {
                return;
            }

            foreach (var item in line)
            {
                item.MoveTo(item.X + offset, item.Y);
            }
        }

        private void ApplyVerticalAlign(List<LayoutLine<TItem>> lines, int top, int bottom)
        {
            int offset;
            switch (this.VerticalAlignment)
            {
                case VerticalAlignment.Middle:
                    offset = (this.Bounds.Height - bottom + top) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    offset = this.Bounds.Height - bottom + top;
                    break;
                case VerticalAlignment.Baseline:
                    if (lines.Count == 0 || lines[0].Count == 0)
                    {
                        return;
                    }

                    int maxAscent = 0;
                    foreach (var item in lines[0])
                    {
                        maxAscent = Math.Max(maxAscent, item.Ascent);
                    }

                    offset = -maxAscent;
                    break;
                default:
                    return;
            }

            if (offset == 0)
            {
                return;
            }

            foreach (var line in lines)
            {
                foreach (var item in line)
                {
                    item.MoveTo(item.X, item.Y + offset);
                }
            }
        }
    }
}