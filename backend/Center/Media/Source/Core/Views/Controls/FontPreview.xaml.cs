// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontPreview.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for FontPreview.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Media.Imaging;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Fonts;

    using Brush = System.Drawing.Brush;
    using Color = System.Drawing.Color;
    using Colors = Gorba.Common.Formats.AlphaNT.Common.Colors;
    using FontFamily = System.Windows.Media.FontFamily;
    using Math = System.Math;
    using Pen = System.Drawing.Pen;

    /// <summary>
    /// Interaction logic for FontPreview.xaml
    /// </summary>
    public partial class FontPreview : INotifyPropertyChanged
    {
        /// <summary>
        /// The bitmap font property.
        /// </summary>
        public static readonly DependencyProperty BitmapFontProperty = DependencyProperty.Register(
            "BitmapFont",
            typeof(FontFile),
            typeof(FontPreview),
            new PropertyMetadata(default(FontFile)));

        /// <summary>
        /// The text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(FontPreview),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The windows font property.
        /// </summary>
        public static readonly DependencyProperty WindowsFontProperty = DependencyProperty.Register(
            "WindowsFont",
            typeof(FontFamily),
            typeof(FontPreview),
            new PropertyMetadata(new FontFamily()));

        private static readonly Brush AmberBrush = new SolidBrush(Color.FromArgb(0xFF, 0xC2, 0x00));
        private static readonly Pen AmberPen = new Pen(AmberBrush);

        private BitmapSource image;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontPreview"/> class.
        /// </summary>
        public FontPreview()
        {
            this.InitializeComponent();
            this.Loaded += this.OnPreviewLoaded;
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the windows font.
        /// </summary>
        public FontFamily WindowsFont
        {
            get
            {
                return (FontFamily)GetValue(WindowsFontProperty);
            }

            set
            {
                SetValue(WindowsFontProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public BitmapSource Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.image = value;
                this.OnPropertyChanged("Image");
            }
        }

        /// <summary>
        /// Gets or sets the bitmap font.
        /// </summary>
        public FontFile BitmapFont
        {
            get
            {
                return (FontFile)GetValue(BitmapFontProperty);
            }

            set
            {
                this.SetValue(BitmapFontProperty, value);
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPreviewLoaded(object sender, RoutedEventArgs e)
        {
            if (this.BitmapFont != null)
            {
                this.CreateBitmap(this.Text);
            }
        }

        private void CreateBitmap(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            const int SizeFactor = 3;
            var textBitmap = new TextBitmap(this.BitmapFont, text);
            var bmp = new Bitmap((textBitmap.Width * SizeFactor) + 1, (textBitmap.Height * SizeFactor) + 1);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
                for (int x = 0; x < textBitmap.Width; x++)
                {
                    for (int y = 0; y < textBitmap.Height; y++)
                    {
                        var pixel = textBitmap.GetPixel(x, y);
                        if (pixel == Colors.Transparent)
                        {
                            continue;
                        }

                        if (pixel == Colors.Black)
                        {
                            continue;
                        }

                        g.FillEllipse(
                            AmberBrush, (x * SizeFactor) + 1, (y * SizeFactor) + 1, SizeFactor - 2, SizeFactor - 2);
                        g.DrawEllipse(
                            AmberPen,
                            (x * SizeFactor) + 1,
                            (y * SizeFactor) + 1,
                            SizeFactor - 2,
                            SizeFactor - 2);
                    }
                }
            }

            this.Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private class TextBitmap : IBitmap
        {
            private const int Gap = 1;

            private const int ColumnCount = 20;
            private readonly List<List<IBitmap>> bitmaps;

            private readonly IFont font;

            public TextBitmap(IFont font, string text)
            {
                this.font = font;
                var rows = (text.Length + ColumnCount - 1) / ColumnCount;
                this.bitmaps = new List<List<IBitmap>>(rows);
                for (var r = 0; r < rows; r++)
                {
                    var maxX = -Gap;
                    var row = new List<IBitmap>();
                    this.bitmaps.Add(row);
                    for (var column = 0; column < ColumnCount; column++)
                    {
                        if (text.Length <= (column + (ColumnCount * r)))
                        {
                            break;
                        }

                        var bitmap = font.GetCharacter(text[(column + (ColumnCount * r))]);
                        maxX += bitmap.Width + Gap;
                        row.Add(bitmap);
                    }

                    this.Width = Math.Max(maxX, this.Width);
                }

                this.Height = ((font.Height + Gap) * rows) - Gap;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            /// <summary>
            /// Gets the color of the pixel at the given position.
            /// </summary>
            /// <param name="x">
            /// The horizontal position in the bitmap (0 ... <see cref="IBitmap.Width"/> - 1).
            /// </param>
            /// <param name="y">
            /// The vertical position in the bitmap (0 ... <see cref="IBitmap.Height"/> - 1).
            /// </param>
            /// <returns>
            /// The <see cref="IColor"/> at the given position.
            /// The returned value will always either be <see cref="Colors.Black"/> or <see cref="Colors.White"/>.
            /// </returns>
            public IColor GetPixel(int x, int y)
            {
                var rowIndex = y / (this.font.Height + Gap);
                if (rowIndex >= this.bitmaps.Count)
                {
                    return Colors.Black;
                }

                var relativeY = y - ((this.font.Height + Gap) * rowIndex);
                if (relativeY < 0 || relativeY >= this.font.Height)
                {
                    return Colors.Black;
                }

                var row = this.bitmaps[rowIndex];
                var offsetX = 0;
                foreach (var bitmap in row)
                {
                    if (offsetX > x)
                    {
                        return Colors.Black;
                    }

                    if (offsetX + bitmap.Width > x)
                    {
                        return bitmap.GetPixel(x - offsetX, relativeY);
                    }

                    offsetX += bitmap.Width + Gap;
                }

                return Colors.Black;
            }
        }
    }
}
