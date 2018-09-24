// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BitmapFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System;
    using System.Drawing;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// Factory for <see cref="IBitmap"/> objects used by renderers.
    /// </summary>
    public static class BitmapFactory
    {
        /// <summary>
        /// Creates a bitmap from the given file.
        /// Currently all standard Windows formats as well as Gorba formats EGF, EGL and EGR are supported.
        /// </summary>
        /// <param name="filename">
        /// The full filename of the bitmap.
        /// </param>
        /// <returns>
        /// The <see cref="IBitmap"/>.
        /// </returns>
        public static IBitmap CreateBitmap(string filename)
        {
            if (filename.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
            {
                return new EgfBitmap(filename);
            }

            if (filename.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase))
            {
                return new EglBitmap(filename);
            }

            if (filename.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase))
            {
                return new EgrBitmap(filename);
            }

            return new BitmapWrapper(new Bitmap(filename));
        }

        private class BitmapWrapper : IBitmap, IDisposable
        {
            private readonly Bitmap bitmap;

            public BitmapWrapper(Bitmap bitmap)
            {
                this.bitmap = bitmap;
                this.Width = bitmap.Width;
                this.Height = bitmap.Height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public IColor GetPixel(int x, int y)
            {
                var pixel = this.bitmap.GetPixel(x, y);
                if (pixel.A == 0)
                {
                    return Colors.Transparent;
                }

                return new SimpleColor(pixel.R, pixel.G, pixel.B);
            }

            public void Dispose()
            {
                this.bitmap.Dispose();
            }
        }
    }
}
