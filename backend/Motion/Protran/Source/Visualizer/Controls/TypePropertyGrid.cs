// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypePropertyGrid.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypePropertyGrid type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    /// <summary>
    /// Advanced property grid that shows the type in a separate field above it.
    /// It also allows the actual property grid to be hidden/shown.
    /// </summary>
    public partial class TypePropertyGrid : UserControl
    {
        private readonly GlyphContorl glyphControl = new GlyphContorl();

        private int openPropertyGridSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypePropertyGrid"/> class.
        /// </summary>
        public TypePropertyGrid()
        {
            this.InitializeComponent();

            this.glyphControl.MinimumSize = this.glyphControl.MaximumSize = this.glyphControl.Size = new Size(12, 12);
            this.glyphControl.Click += this.OnGlyphControlClick;
        }

        /// <summary>
        /// Gets or sets the text of the label next to the type name.
        /// Default value is "Type:".
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("Type:")]
        public string LabelText
        {
            get
            {
                return this.label1.Text;
            }

            set
            {
                this.label1.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is collapsable.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Collapsable
        {
            get
            {
                return this.tableLayoutPanel1.ColumnCount == 3;
            }

            set
            {
                if (value == this.Collapsable)
                {
                    return;
                }

                if (value)
                {
                    this.ShowCollapsable();
                }
                else
                {
                    this.HideCollapsable();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is collapsed.
        /// This can only be set if <see cref="Collapsable"/> is set to true.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Collapsed
        {
            get
            {
                return this.Collapsable && !this.glyphControl.Opened;
            }

            set
            {
                if (value == this.Collapsed)
                {
                    return;
                }

                this.glyphControl.Opened = !value;
                this.UpdateCollapsed(value);
            }
        }

        /// <summary>
        /// Gets or sets the selected object of the property grid.
        /// </summary>
        public object SelectedObject
        {
            get
            {
                return this.propertyGrid1.SelectedObject;
            }

            set
            {
                if (value == null)
                {
                    this.textBoxType.Clear();
                }
                else
                {
                    this.textBoxType.Text = GetTypeName(value.GetType());
                }

                this.propertyGrid1.SelectedObject = value;
            }
        }

        /// <summary>
        /// Gets a C# like string representing a given type.
        /// </summary>
        /// <param name="type">
        /// The type for which you want the string represenation.
        /// </param>
        /// <returns>
        /// the string representation for the given type.
        /// </returns>
        public static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var sb = new StringBuilder();
            sb.Append(type.Name.Substring(0, type.Name.IndexOf('`')));
            sb.Append('<');
            foreach (var arg in type.GetGenericArguments())
            {
                sb.Append(GetTypeName(arg));
                sb.Append(',');
            }

            sb.Length--;
            sb.Append('>');
            return sb.ToString();
        }

        private void ShowCollapsable()
        {
            if (this.tableLayoutPanel1 == null || this.tableLayoutPanel1.Controls.Count == 0)
            {
                return;
            }

            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGrid1, 3);
            this.tableLayoutPanel1.SetColumn(this.label1, 1);
            this.tableLayoutPanel1.SetColumn(this.textBoxType, 2);
            this.tableLayoutPanel1.Controls.Add(this.glyphControl, 0, 0);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
        }

        private void HideCollapsable()
        {
            if (this.tableLayoutPanel1 == null || this.tableLayoutPanel1.Controls.Count == 0)
            {
                return;
            }

            this.tableLayoutPanel1.SuspendLayout();
            this.Collapsed = false;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.RemoveAt(2);
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGrid1, 2);
            this.tableLayoutPanel1.SetColumn(this.label1, 0);
            this.tableLayoutPanel1.SetColumn(this.textBoxType, 1);
            this.tableLayoutPanel1.Controls.Remove(this.glyphControl);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
        }

        private void UpdateCollapsed(bool collapsed)
        {
            this.SuspendLayout();
            if (this.openPropertyGridSize == 0)
            {
                this.openPropertyGridSize = this.propertyGrid1.Height;
            }

            this.propertyGrid1.Visible = !collapsed;
            int heightChange = this.openPropertyGridSize + this.propertyGrid1.Margin.Top;
            if (collapsed)
            {
                heightChange = -heightChange;
            }

            var oldHeight = this.Height;
            for (Control ctrl = this; this.Height == oldHeight; ctrl = ctrl.Parent)
            {
                ctrl.Height += heightChange;
            }

            this.ResumeLayout(true);
        }

        private void OnGlyphControlClick(object sender, EventArgs e)
        {
            this.glyphControl.Opened = !this.glyphControl.Opened;
            this.UpdateCollapsed(!this.glyphControl.Opened);
        }

        private class GlyphContorl : UserControl
        {
            private readonly VisualStyleRenderer renderer;

            private bool opened;

            public GlyphContorl()
            {
                this.Margin = new Padding(6, 8, 0, 0);
                this.opened = true;

                // Attention:
                // to see if the desired visual style is enabled or not,
                // we need to do the following checks:
                if (VisualStyleRenderer.IsSupported &&
                    VisualStyleRenderer.IsElementDefined(VisualStyleElement.TreeView.Glyph.Opened) &&
                    VisualStyleRenderer.IsElementDefined(VisualStyleElement.TreeView.Glyph.Closed))
                {
                    // I can really create this kind of renderer.
                    this.renderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                }
            }

            public bool Opened
            {
                get
                {
                    return this.opened;
                }

                set
                {
                    if (this.opened == value)
                    {
                        return;
                    }

                    this.opened = value;
                    if (this.renderer != null)
                    {
                        this.renderer.SetParameters(
                            this.opened
                                ? VisualStyleElement.TreeView.Glyph.Opened
                                : VisualStyleElement.TreeView.Glyph.Closed);
                    }

                    this.Invalidate();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var rect = new Rectangle(Point.Empty, this.Size);
                if (this.renderer != null)
                {
                    // I draw with the renderer having the specified visual style
                    this.renderer.DrawBackground(e.Graphics, rect);
                }
                else
                {
                    // otherwise I draw it manually
                    var g = e.Graphics;
                    rect.Width--;
                    rect.Height--;
                    rect.Width = rect.Width / 2 * 2;
                    rect.Height = rect.Height / 2 * 2;
                    g.DrawRectangle(Pens.Black, rect);

                    rect.Inflate(-2, -2);
                    int centerY = rect.Top + (rect.Height / 2);
                    g.DrawLine(Pens.Black, rect.Left, centerY, rect.Right, centerY);

                    if (!this.opened)
                    {
                        int centerX = rect.Left + (rect.Width / 2);
                        g.DrawLine(Pens.Black, centerX, rect.Top, centerX, rect.Bottom);
                    }
                }
            }
        }
    }
}
