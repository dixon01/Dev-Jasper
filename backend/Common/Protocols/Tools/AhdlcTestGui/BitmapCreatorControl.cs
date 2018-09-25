// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapCreatorControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapCreatorControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// A control that allows to create bitmaps with some simple drawing tools.
    /// </summary>
    public partial class BitmapCreatorControl : UserControl
    {
        private int zoomFactor;

        private Size bitmapSize;

        private bool hasColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapCreatorControl"/> class.
        /// </summary>
        public BitmapCreatorControl()
        {
            this.InitializeComponent();
            this.comboBoxZoom.SelectedItem = "200%";
            this.BitmapSize = new Size(112, 16);
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap Bitmap { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the editor supports color.
        /// </summary>
        public bool HasColor
        {
            get
            {
                return this.hasColor;
            }

            set
            {
                if (this.hasColor == value)
                {
                    return;
                }

                this.hasColor = value;
                if (!value && this.DrawingColor != Color.Black)
                {
                    this.DrawingColor = Color.White;
                }
            }
        }

        /// <summary>
        /// Gets or sets the bitmap size.
        /// </summary>
        public Size BitmapSize
        {
            get
            {
                return this.bitmapSize;
            }

            set
            {
                if (this.bitmapSize.Equals(value))
                {
                    return;
                }

                this.bitmapSize = value;
                if (this.Bitmap != null)
                {
                    this.Bitmap.Dispose();
                    this.Bitmap = null;
                }

                if (this.bitmapSize.Width > 0 && this.bitmapSize.Height > 0)
                {
                    this.Bitmap = new Bitmap(this.bitmapSize.Width, this.bitmapSize.Height);
                    using (var g = Graphics.FromImage(this.Bitmap))
                    {
                        g.Clear(Color.Black);
                    }

                    this.UpdateDrawingPanelSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used for drawing.
        /// </summary>
        protected Color DrawingColor
        {
            get
            {
                return this.panelColor.BackColor;
            }

            set
            {
                this.panelColor.BackColor = value;
            }
        }

        private void ComboBoxZoomSelectedIndexChanged(object sender, EventArgs e)
        {
            var value = this.comboBoxZoom.Text.Substring(0, this.comboBoxZoom.Text.Length - 1);
            this.zoomFactor = int.Parse(value);
            this.UpdateDrawingPanelSize();
        }

        private void UpdateDrawingPanelSize()
        {
            this.panelDrawing.Size = new Size(
                this.bitmapSize.Width * this.zoomFactor / 100, this.bitmapSize.Height * this.zoomFactor / 100);
            this.panelDrawing.Invalidate();
        }

        private void PanelDrawingPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            if (this.Bitmap == null)
            {
                g.Clear(Color.Black);
                return;
            }

            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(
                this.Bitmap,
                new Rectangle(0, 0, this.panelDrawing.Width, this.panelDrawing.Height),
                0,
                0,
                this.Bitmap.Width,
                this.Bitmap.Height,
                GraphicsUnit.Pixel);
        }

        private void PanelDrawingMouseDown(object sender, MouseEventArgs e)
        {
            this.UpdatePixel(e);
        }

        private void PanelDrawingMouseMove(object sender, MouseEventArgs e)
        {
            this.UpdatePixel(e);
        }

        private void UpdatePixel(MouseEventArgs e)
        {
            if (this.Bitmap == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var x = e.X * 100 / this.zoomFactor;
            var y = e.Y * 100 / this.zoomFactor;
            if (x < 0 || x >= this.Bitmap.Width || y < 0 || y >= this.Bitmap.Height)
            {
                return;
            }

            this.Bitmap.SetPixel(x, y, this.DrawingColor);
            var invalid = Math.Max(2, this.zoomFactor / 25);
            this.panelDrawing.Invalidate(new Rectangle(e.X - (invalid / 2), e.Y - (invalid / 2), invalid, invalid));
        }

        private void PanelColorClick(object sender, EventArgs e)
        {
            if (!this.HasColor)
            {
                this.DrawingColor = this.DrawingColor == Color.Black ? Color.White : Color.Black;
                return;
            }

            if (this.colorDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.DrawingColor = this.colorDialog.Color;
        }

        private void TextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this.Bitmap == null || e.KeyCode != Keys.Enter)
            {
                return;
            }

            using (var g = Graphics.FromImage(this.Bitmap))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.Clear(Color.Black);
                using (var font = new Font(FontFamily.GenericSansSerif, this.BitmapSize.Height * 0.5f, FontStyle.Bold))
                {
                    using (var brush = new SolidBrush(this.DrawingColor))
                    {
                        g.DrawString(
                            this.textBox.Text,
                            font,
                            brush,
                            new RectangleF(PointF.Empty, this.BitmapSize),
                            new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center,
                                    FormatFlags = StringFormatFlags.NoWrap,
                                    Trimming = StringTrimming.EllipsisCharacter
                                });
                    }
                }
            }

            this.panelDrawing.Invalidate();
        }
    }
}
