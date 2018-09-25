// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMarkerService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextMarkerService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    /// <summary>
    /// The text marker service used to show annotations on an AvalonEdit text editor.
    /// Based on code found at:
    /// <see cref="https://stackoverflow.com/questions/11149907/showing-invalid-xml-syntax-with-avalonedit/12639677"/>
    /// </summary>
    public class TextMarkerService : IBackgroundRenderer, IVisualLineTransformer
    {
        private readonly TextEditor textEditor;

        private readonly TextSegmentCollection<TextMarker> markers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMarkerService"/> class.
        /// </summary>
        /// <param name="textEditor">
        /// The text editor which will be managed by this service.
        /// </param>
        public TextMarkerService(TextEditor textEditor)
        {
            this.textEditor = textEditor;
            this.markers = new TextSegmentCollection<TextMarker>(textEditor.Document);
        }

        /// <summary>
        /// Gets the layer on which this background renderer should draw.
        /// </summary>
        public KnownLayer Layer
        {
            get
            {
                return KnownLayer.Selection;
            }
        }

        /// <summary>
        /// Creates a new text marker at the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="length">
        /// The length of the text being marked.
        /// </param>
        /// <param name="message">
        /// The message to be shown when hovering over the marked text.
        /// </param>
        public void Create(int offset, int length, string message)
        {
            var m = new TextMarker(offset, length);
            this.markers.Add(m);
            m.MarkerColor = Colors.Red;
            m.ToolTip = message;
            this.Redraw(m);
        }

        /// <summary>
        /// Gets all markers at the given text offset.
        /// </summary>
        /// <param name="offset">
        /// The text offset.
        /// </param>
        /// <returns>
        /// The list of all markers at the given offset.
        /// </returns>
        public IEnumerable<TextMarker> GetMarkersAtOffset(int offset)
        {
            return this.markers == null ? Enumerable.Empty<TextMarker>() : this.markers.FindSegmentsContaining(offset);
        }

        /// <summary>
        /// Clears all markers.
        /// </summary>
        public void Clear()
        {
            foreach (var m in this.markers.ToList())
            {
                this.Remove(m);
            }
        }

        void IBackgroundRenderer.Draw(TextView textView, DrawingContext drawingContext)
        {
            if (this.markers == null || !textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (var marker in this.markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (marker.BackgroundColor != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder { AlignToWholePixels = true, CornerRadius = 3 };
                    geoBuilder.AddSegment(textView, marker);
                    Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        Color color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }

                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                {
                    Point startPoint = rect.BottomLeft;
                    Point endPoint = rect.BottomRight;

                    var usedPen = new Pen(new SolidColorBrush(marker.MarkerColor), 1);
                    usedPen.Freeze();
                    var geometry = new StreamGeometry();

                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(startPoint, false, false);
                        ctx.PolyLineTo(CreatePoints(startPoint, endPoint), true, false);
                    }

                    geometry.Freeze();

                    drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
                    break;
                }
            }
        }

        void IVisualLineTransformer.Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
        }

        private static Point[] CreatePoints(Point start, Point end)
        {
            const double Offset = 2.5;

            int count = Math.Max((int)((end.X - start.X) / Offset) + 1, 4);

            var points = new Point[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = new Point(start.X + (i * Offset), start.Y - ((i + 1) % 2 == 0 ? Offset : 0));
            }

            return points;
        }

        private void Remove(TextMarker marker)
        {
            if (this.markers.Remove(marker))
            {
                this.Redraw(marker);
            }
        }

        private void Redraw(ISegment segment)
        {
            this.textEditor.TextArea.TextView.Redraw(segment);
        }

        /// <summary>
        /// The text marker.
        /// </summary>
        public sealed class TextMarker : TextSegment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TextMarker"/> class.
            /// </summary>
            /// <param name="startOffset">
            /// The start offset.
            /// </param>
            /// <param name="length">
            /// The length.
            /// </param>
            internal TextMarker(int startOffset, int length)
            {
                this.StartOffset = startOffset;
                this.Length = length;
            }

            /// <summary>
            /// Gets or sets the background color of the text behind the marker.
            /// </summary>
            public Color? BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the marker color.
            /// </summary>
            public Color MarkerColor { get; set; }

            /// <summary>
            /// Gets or sets the tool tip text.
            /// </summary>
            public string ToolTip { get; set; }
        }
    }
}
