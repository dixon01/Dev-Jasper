// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrappingLayoutEngine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WrappingLayoutEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Layout engine that wraps all items at the given width from <see cref="LayoutEngineBase{TItem}.Bounds"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of <see cref="ILayoutItem"/> that is being laid out.
    /// </typeparam>
    internal class WrappingLayoutEngine<TItem> : LayoutEngineBase<TItem>
        where TItem : class, ILayoutItem, ISplittable<TItem>
    {
        private readonly bool scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappingLayoutEngine{TItem}"/> class.
        /// </summary>
        /// <param name="scale">
        /// A flag indicating if the outcome should also fit vertically (and horizontally) into the given bounds.
        /// If this is set to true, the algorithm will scale items (and re-wrap them) until they fit
        /// into the bounds.
        /// </param>
        public WrappingLayoutEngine(bool scale)
        {
            this.scale = scale;
        }

        /// <summary>
        /// Does the actual layout according to the rules defined in <see cref="LayoutEngineBase{TItem}.Prepare"/>.
        /// </summary>
        /// <param name="lines">
        /// The lines to layout.
        /// </param>
        public override void Layout(List<LayoutLine<TItem>> lines)
        {
            if (this.Bounds.Width == 0)
            {
                // wrapping is disabled if the width is not set
                base.Layout(lines);
                return;
            }

            List<LayoutLine<TItem>> wrapped = null;

            var scaling = 1.0;
            var lastValidScaling = 0.0;
            var lastInvalidScaling = 1.0;
            var lastScaling = 0.0;
            const int MaxLoop = 20;
            for (int i = 0; i < MaxLoop; i++)
            {
                lastScaling = scaling;
                wrapped = this.DoLayout(lines, scaling);

                if (!this.scale)
                {
                    lastValidScaling = scaling;
                    break;
                }

                var size = this.GetSize(wrapped);

                if (size.Width <= this.Bounds.Width && size.Height <= this.Bounds.Height)
                {
                    if (lastValidScaling < scaling)
                    {
                        lastValidScaling = scaling;
                    }
                }
                else
                {
                    lastInvalidScaling = scaling;
                }

                if ((size.Width == this.Bounds.Width
                    || (size.Width < this.Bounds.Width && scaling >= 1.0))
                    && (size.Height == this.Bounds.Height
                    || (size.Height < this.Bounds.Height && scaling >= 1.0)))
                {
                    break;
                }

                var additionalScalingX = 1.0 * this.Bounds.Width / size.Width;
                var additionalScalingY = 1.0 * this.Bounds.Height / size.Height;
                var additionalScaling = Math.Min(additionalScalingX, additionalScalingY);
                additionalScaling += (1 - additionalScaling) * (MaxLoop - 1 - i) * 0.04;
                scaling *= additionalScaling;

                if (scaling >= lastInvalidScaling)
                {
                    scaling = lastInvalidScaling * 0.99;
                }
            }

            // we really want to see if the scaling is the same, so that's ok!
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (lastValidScaling != lastScaling || wrapped == null)
            {
                wrapped = this.DoLayout(lines, lastValidScaling);
            }

            lines.Clear();
            lines.AddRange(wrapped);
        }

        private List<LayoutLine<TItem>> DoLayout(ICollection<LayoutLine<TItem>> lines, double scaling)
        {
            this.SetScaling(lines, scaling);

            var wrapped = new List<LayoutLine<TItem>>(lines.Count * 2);
            foreach (var line in lines)
            {
                this.WrapLine(line, wrapped);
            }

            base.Layout(wrapped);
            return wrapped;
        }

        private void SetScaling(IEnumerable<LayoutLine<TItem>> lines, double scaling)
        {
            foreach (var line in lines)
            {
                foreach (var item in line)
                {
                    item.SetScaling(scaling);
                }
            }
        }

        private Size GetSize(IEnumerable<LayoutLine<TItem>> lines)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;
            foreach (var line in lines)
            {
                foreach (var item in line)
                {
                    minX = Math.Min(minX, item.X);
                    maxX = Math.Max(maxX, item.X + item.Width);
                    minY = Math.Min(minY, item.Y);
                    maxY = Math.Max(maxY, item.Y + item.Height);
                }
            }

            return minY == int.MaxValue ? Size.Empty : new Size(maxX - minX, maxY - minY);
        }

        private void WrapLine(LayoutLine<TItem> line, ICollection<LayoutLine<TItem>> wrapped)
        {
            var outputLine = new LayoutLine<TItem>(line.HorizontalAlignment);
            wrapped.Add(outputLine);

            var x = 0;
            foreach (var item in line)
            {
                x += item.Width;

                var currentItem = item;
                while (x > this.Bounds.Width)
                {
                    TItem left;
                    TItem right;
                    if (currentItem.Split(this.Bounds.Width - x + currentItem.Width, out left, out right))
                    {
                        if (x != currentItem.Width && x - currentItem.Width + left.Width > this.Bounds.Width)
                        {
                            // the next item doesn't fit on this line anymore, let's try to put it on the next
                            outputLine = new LayoutLine<TItem>(line.HorizontalAlignment);
                            wrapped.Add(outputLine);
                            x = currentItem.Width;
                            continue;
                        }

                        outputLine.Add(left);
                        currentItem = right;
                    }
                    else if (x == currentItem.Width)
                    {
                        // there is only a single item available, we need to add it to the current line
                        break;
                    }
                    else
                    {
                        currentItem = left;
                    }

                    outputLine = new LayoutLine<TItem>(line.HorizontalAlignment);
                    wrapped.Add(outputLine);
                    x = currentItem.Width;
                }

                outputLine.Add(currentItem);
                x += currentItem.HorizontalGapAfter;
            }
        }
    }
}