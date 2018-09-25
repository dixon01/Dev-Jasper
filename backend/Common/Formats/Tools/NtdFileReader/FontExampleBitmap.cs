// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontExampleBitmap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontExampleBitmap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.Tools.NtdFileReader
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Fonts;

    /// <summary>
    /// Bitmap that shows all characters of the given font.
    /// </summary>
    public class FontExampleBitmap : IBitmap
    {
        private const int ColumnCount = 32;

        private const int Gap = 1;

        private readonly IFont font;

        private readonly List<List<IBitmap>> bitmaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontExampleBitmap"/> class.
        /// </summary>
        /// <param name="font">
        /// The font.
        /// </param>
        public FontExampleBitmap(IFont font)
        {
            this.font = font;
            var rows = (font.CharacterCount + ColumnCount - 1) / ColumnCount;
            this.bitmaps = new List<List<IBitmap>>(rows);
            for (int r = 0; r < rows; r++)
            {
                var maxX = -Gap;
                var row = new List<IBitmap>(ColumnCount);
                this.bitmaps.Add(row);
                for (int c = 0; c < ColumnCount; c++)
                {
                    var bitmap = font.GetCharacter((char)(0x20 + c + (r * ColumnCount)));
                    maxX += bitmap.Width + Gap;
                    row.Add(bitmap);
                }

                this.Width = Math.Max(maxX, this.Width);
            }

            this.Height = ((font.Height + Gap) * rows) - Gap;
        }

        /// <summary>
        /// Gets the width of the bitmap.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the bitmap.
        /// </summary>
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