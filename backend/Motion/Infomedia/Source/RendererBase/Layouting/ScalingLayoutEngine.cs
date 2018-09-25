// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScalingLayoutEngine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScalingLayoutEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Layout engine that scales all items until they fit
    /// into the given width of the <see cref="LayoutEngineBase{TItem}.Bounds"/>.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of <see cref="ILayoutItem"/> that is being laid out.
    /// </typeparam>
    internal class ScalingLayoutEngine<TItem> : LayoutEngineBase<TItem>
        where TItem : class, ILayoutItem, ISplittable<TItem>
    {
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
                // scaling is disabled if the width is not set
                base.Layout(lines);
                return;
            }

            int sizeFactor = 1000;
            int width = this.GetWidth(lines, sizeFactor * 0.001);

            for (int i = 0; width > this.Bounds.Width && i < 5; i++)
            {
                // calculate new size factor (an approximation to the expected width + ~5%)
                int oldFactor = sizeFactor;
                sizeFactor = sizeFactor * this.Bounds.Width / width;
                if (oldFactor - sizeFactor > 1)
                {
                    sizeFactor += ((oldFactor - sizeFactor) / 20) + 1;
                }

                width = this.GetWidth(lines, sizeFactor * 0.001);
            }

            base.Layout(lines);
        }

        private int GetWidth(IEnumerable<LayoutLine<TItem>> lines, double scaling)
        {
            var maxWidth = 0;
            foreach (var line in lines)
            {
                var width = 0;
                var gapAfter = 0;
                foreach (var item in line)
                {
                    width += gapAfter;
                    item.SetScaling(scaling);
                    width += item.Width;
                    gapAfter = item.HorizontalGapAfter;
                }

                maxWidth = Math.Max(maxWidth, width);
            }

            return maxWidth;
        }
    }
}