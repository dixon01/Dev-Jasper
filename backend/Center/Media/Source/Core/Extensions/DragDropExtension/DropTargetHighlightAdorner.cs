// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropTargetHighlightAdorner.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DropTargetHighlightAdorner.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// the drop target highlight adorner
    /// </summary>
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetHighlightAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">the adorned element</param>
        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// the render handler
        /// </summary>
        /// <param name="drawingContext">the drawing context</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var visualTargetItem = this.DropInfo.VisualTargetItem;
            if (visualTargetItem != null)
            {
                var rect = Rect.Empty;

                var treeviewitem = visualTargetItem as TreeViewItem;
                if (treeviewitem != null && VisualTreeHelper.GetChildrenCount(treeviewitem) > 0)
                {
                    var grid = VisualTreeHelper.GetChild(treeviewitem, 0) as Grid;
                    if (grid != null)
                    {
                        var descendant = VisualTreeHelper.GetDescendantBounds(treeviewitem);
                        rect = new Rect(treeviewitem.TranslatePoint(new Point(), this.AdornedElement), new Size(descendant.Width + 4, grid.RowDefinitions[0].ActualHeight));
                    }
                }

                if (rect.IsEmpty)
                {
                    rect = new Rect(visualTargetItem.TranslatePoint(new Point(), this.AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
                }

                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }
    }
}