// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The layout manager can be used to lay out items according to given rules.
    /// Users of this class should:
    /// 1. Add lines using one of the AddLine() methods
    /// 2. Lay out the lines (and the items contained within the line) using any of the Layout() methods
    /// 3. Get the list of all items (assembled from all lines) using the <see cref="Items"/> property.
    /// Users may call <see cref="Clear"/> if they wish to reuse this manager.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of <see cref="ILayoutItem"/> that is being laid out.
    /// </typeparam>
    public class LayoutManager<TItem>
        where TItem : class, ILayoutItem, ISplittable<TItem>
    {
        private readonly List<LayoutLine<TItem>> lines = new List<LayoutLine<TItem>>();

        /// <summary>
        /// Gets the list of all items contained in this layout manager.
        /// If called before calling any of the Layout() methods, it will simply return all items
        /// ever added through <see cref="AddLine(System.Collections.Generic.IEnumerable{TItem})"/>.
        /// </summary>
        public IEnumerable<TItem> Items
        {
            get
            {
                foreach (var line in this.lines)
                {
                    foreach (var item in line)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a line of items to this manager.
        /// A line is always laid out on its own which will result in a "line break" after each line.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddLine(IEnumerable<TItem> items)
        {
            this.AddLine(items, null);
        }

        /// <summary>
        /// Adds a line of items to this manager.
        /// A line is always laid out on its own which will result in a "line break" after each line.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        public void AddLine(IEnumerable<TItem> items, HorizontalAlignment? horizontalAlignment)
        {
            var line = new LayoutLine<TItem>(horizontalAlignment);
            line.AddRange(items);
            this.lines.Add(line);
        }
        
        /// <summary>
        /// Does the actual layout according to the rules defined with the parameters.
        /// Horizontal alignment is <see cref="HorizontalAlignment.Left"/>.
        /// Vertical alignment is <see cref="VerticalAlignment.Top"/>.
        /// Text overflow is <see cref="TextOverflow.Wrap"/>.
        /// Baseline alignment is enabled.
        /// </summary>
        /// <param name="bounds">
        /// The bounds of the area into which the contents should fit.
        /// </param>
        public void Layout(Rectangle bounds)
        {
            this.Layout(bounds, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Does the actual layout according to the rules defined with the parameters.
        /// Vertical alignment is <see cref="VerticalAlignment.Top"/>.
        /// Text overflow is <see cref="TextOverflow.Wrap"/>.
        /// Baseline alignment is enabled.
        /// </summary>
        /// <param name="bounds">
        /// The bounds of the area into which the contents should fit.
        /// </param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment.
        /// </param>
        public void Layout(Rectangle bounds, HorizontalAlignment horizontalAlignment)
        {
            this.Layout(bounds, horizontalAlignment, VerticalAlignment.Top);
        }

        /// <summary>
        /// Does the actual layout according to the rules defined with the parameters.
        /// Text overflow is <see cref="TextOverflow.Wrap"/>.
        /// Baseline alignment is enabled.
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
        public void Layout(
            Rectangle bounds, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            this.Layout(bounds, horizontalAlignment, verticalAlignment, TextOverflow.Wrap);
        }

        /// <summary>
        /// Does the actual layout according to the rules defined with the parameters.
        /// Baseline alignment is enabled.
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
        /// <param name="overflow">
        /// The text overflow configuration.
        /// </param>
        public void Layout(
            Rectangle bounds,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            TextOverflow overflow)
        {
            this.Layout(bounds, horizontalAlignment, verticalAlignment, overflow, true);
        }

        /// <summary>
        /// Does the actual layout according to the rules defined with the parameters.
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
        /// <param name="overflow">
        /// The text overflow configuration.
        /// </param>
        /// <param name="baselineAlign">
        /// A flag indicating if text should always be aligned on the baseline.
        /// </param>
        public void Layout(
            Rectangle bounds,
            HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment,
            TextOverflow overflow,
            bool baselineAlign)
        {
            var engine = LayoutEngineBase<TItem>.Create(overflow);
            if (verticalAlignment == VerticalAlignment.Baseline && bounds.Height > 0)
            {
                verticalAlignment = VerticalAlignment.Top;
                baselineAlign = true;
            }

            engine.Prepare(bounds, horizontalAlignment, verticalAlignment, baselineAlign);
            engine.Layout(this.lines);
        }

        /// <summary>
        /// Clears this layout manager for later reuse.
        /// </summary>
        public void Clear()
        {
            this.lines.Clear();
        }
    }
}
