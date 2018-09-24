// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Text;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Visualizer.Data;

    /// <summary>
    /// Control that shows the steps of a telegram transformation.
    /// </summary>
    public partial class TransformationControl : UserControl, IVisualizationControl
    {
        private readonly Padding childMargin = new Padding(10, 10, 10, 25);

        private SideTab sideTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationControl"/> class.
        /// </summary>
        public TransformationControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control should redraw its surface using a secondary buffer to reduce or prevent flicker.
        /// </summary>
        /// <returns>
        /// true if the surface of the control should be drawn using double buffering; otherwise, false.
        /// </returns>
        protected override bool DoubleBuffered
        {
            get
            {
                return true;
            }

            // ReSharper disable ValueParameterNotUsed
            set
            {
                base.DoubleBuffered = true;
            }

            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// Populates this control with the given event args.
        /// This is used to fill the control when shown from the log view.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public virtual void Populate(TransformationChainEventArgs e)
        {
            this.SetTransformations(e.ChainName, e.Transformations);
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            var g = e.Graphics;
            int roundCorner = this.childMargin.Left;
            var pen = Pens.Black;
            var arrowPen = new Pen(Color.Black, 3);
            var arrowBrush = Brushes.Black;
            for (int i = 0; i < this.flowLayoutPanel1.Controls.Count; i++)
            {
                var child = this.flowLayoutPanel1.Controls[i];
                if (i > 0)
                {
                    int x = child.Width / 2;
                    int y = child.Top - (roundCorner / 2);
                    g.DrawLine(arrowPen, x, y - this.childMargin.Vertical + roundCorner, x, y - roundCorner);

                    y--;
                    var triangle = new[]
                        {
                            new Point(x - (roundCorner / 2), y - roundCorner), 
                            new Point(x + (roundCorner / 2), y - roundCorner),
                            new Point(x, y)
                        };
                    g.FillPolygon(arrowBrush, triangle);
                    g.DrawPolygon(pen, triangle);
                }

                var rect = child.Bounds;
                rect.Inflate(roundCorner / 2, roundCorner / 2);
                var path = new GraphicsPath();
                path.AddArc(rect.Left, rect.Top, roundCorner, roundCorner, 180, 90);
                path.AddArc(rect.Right - roundCorner, rect.Top, roundCorner, roundCorner, 270, 90);
                path.AddArc(rect.Right - roundCorner, rect.Bottom - roundCorner, roundCorner, roundCorner, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - roundCorner, roundCorner, roundCorner, 90, 90);
                path.CloseAllFigures();
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Clears this control. Call this method before adding new transformations.
        /// </summary>
        protected void Clear()
        {
            this.flowLayoutPanel1.Controls.Clear();
            if (this.sideTab != null)
            {
                this.sideTab.Description = string.Empty;
            }
        }

        /// <summary>
        /// Adds the given transformation information to this control.
        /// </summary>
        /// <param name="chainName">the name of the transformation chain </param>
        /// <param name="transformations">
        /// The transformations.
        /// </param>
        protected void SetTransformations(string chainName, IEnumerable<TransformationInfo> transformations)
        {
            int width = this.Width * 2 / 3;
            foreach (var transformation in transformations)
            {
                var inputControl = new TextBox { Text = this.InputToString(transformation.Input), ReadOnly = true, Width = width, Margin = this.childMargin };
                this.flowLayoutPanel1.Controls.Add(inputControl);

                if (transformation.Transformer == null)
                {
                    continue;
                }

                var transformerControl = new TypePropertyGrid();
                transformerControl.LabelText = "Transformation:";
                transformerControl.Size = new Size(width, 120);
                transformerControl.Margin = this.childMargin;
                this.flowLayoutPanel1.Controls.Add(transformerControl);

                transformerControl.Collapsable = true;
                transformerControl.Collapsed = true;
                if (transformation.Transformer.Config != null)
                {
                    transformerControl.SelectedObject = transformation.Transformer.Config;
                }
                else
                {
                    transformerControl.SelectedObject = transformation.Transformer;
                }
            }

            if (this.sideTab != null)
            {
                this.sideTab.Description = chainName;
            }
        }

        /// <summary>
        /// Converts the given transformation input to a string.
        /// </summary>
        /// <param name="value">
        /// The transformation input.
        /// </param>
        /// <returns>
        /// a string representing the provided value.
        /// </returns>
        protected virtual string InputToString(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            var buffer = value as byte[];
            if (buffer != null)
            {
                return '{' + BufferUtils.FromByteArrayToHexString(buffer) + '}';
            }

            var s = value as string;
            if (s != null)
            {
                return s;
            }

            if (!value.GetType().IsArray)
            {
                return value.ToString();
            }

            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var element in (IEnumerable)value)
            {
                sb.Append(this.InputToString(element)).Append(',');
            }

            sb.Length--;
            sb.Append(']');
            return sb.ToString();
        }

        private void FlowLayoutPanel1Layout(object sender, LayoutEventArgs e)
        {
            this.Invalidate(false);
        }
    }
}
