// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Form that can display the contents of a <see cref="IBitmap"/>.
    /// </summary>
    public partial class BitmapForm : Form
    {
        private const int SizeFactor = 5;

        private static readonly Brush AmberBrush = new SolidBrush(Color.FromArgb(0xFF, 0xC2, 0x00));
        private static readonly Pen AmberPen = new Pen(AmberBrush);

        private static readonly Dictionary<int, Brush> Brushes = new Dictionary<int, Brush>();
        private static readonly Dictionary<int, Pen> Pens = new Dictionary<int, Pen>();

        private IBitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapForm"/> class.
        /// </summary>
        public BitmapForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the bitmap to be displayed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }

            set
            {
                if (this.bitmap == value)
                {
                    return;
                }

                var img = this.pictureBox.Image;
                if (img != null)
                {
                    this.pictureBox.Image = null;
                    img.Dispose();
                }

                this.bitmap = value;

                img = this.CreateBitmap();
                this.pictureBox.Image = img;
            }
        }

        /// <summary>
        /// Resizes this for to fit the displayed bitmap.
        /// </summary>
        public void ResizeToFit()
        {
            this.Width += this.pictureBox.ClientSize.Width - this.ClientSize.Width;
            this.Height += this.pictureBox.ClientSize.Height - this.ClientSize.Height;
        }

        private static void GetBrushAndPen(IColor color, out Brush brush, out Pen pen)
        {
            var key = (color.R << 16) | (color.G << 8) | color.B;
            if (Brushes.TryGetValue(key, out brush))
            {
                pen = Pens[key];
                return;
            }

            brush = new SolidBrush(Color.FromArgb(color.R, color.G, color.B));
            pen = new Pen(brush);
            Brushes.Add(key, brush);
            Pens.Add(key, pen);
        }

        private Bitmap CreateBitmap()
        {
            if (this.bitmap == null)
            {
                return null;
            }

            var bmp = new Bitmap((this.bitmap.Width * SizeFactor) + 1, (this.bitmap.Height * SizeFactor) + 1);
            var hasColor = this.bitmap is EgfBitmap;
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int x = 0; x < this.bitmap.Width; x++)
                {
                    for (int y = 0; y < this.bitmap.Height; y++)
                    {
                        var pixel = this.bitmap.GetPixel(x, y);
                        if (pixel == Colors.Transparent)
                        {
                            continue;
                        }

                        Brush brush;
                        Pen pen;
                        if (hasColor)
                        {
                            GetBrushAndPen(pixel, out brush, out pen);
                        }
                        else
                        {
                            if (pixel == Colors.Black)
                            {
                                continue;
                            }

                            brush = AmberBrush;
                            pen = AmberPen;
                        }

                        g.FillEllipse(
                            brush, (x * SizeFactor) + 1, (y * SizeFactor) + 1, SizeFactor - 2, SizeFactor - 2);
                        g.DrawEllipse(pen, (x * SizeFactor) + 1, (y * SizeFactor) + 1, SizeFactor - 2, SizeFactor - 2);
                    }
                }
            }

            return bmp;
        }
    }
}
