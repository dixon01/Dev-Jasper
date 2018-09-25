// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EgfBitmap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EgfBitmap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Bitmaps
{
    using System;
    using System.IO;

    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// EGF bitmap class for reading color bitmaps.
    /// </summary>
    public class EgfBitmap : BitmapBase
    {
        private readonly IByteAccess data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EgfBitmap"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename from which to read the bitmap.
        /// </param>
        public EgfBitmap(string filename)
            : this(new BinaryFileReader(File.OpenRead(filename), true) { ExtendedFormat = true }, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgfBitmap"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader from which to read the bitmap.
        /// </param>
        /// <param name="closeReader">
        /// Flag indicating whether the <see cref="reader"/> should be closed
        /// before this method returns or throws an exception.
        /// </param>
        internal EgfBitmap(BinaryFileReader reader, bool closeReader)
        {
            if (!reader.ExtendedFormat)
            {
                throw new NotSupportedException("Trying to read an EGF bitmap from a non-extended format reader");
            }

            try
            {
                var x = reader.ReadLsbUInt16();
                var y = reader.ReadLsbUInt16();
                if ((x & 0x8000) == 0 || (y & 0x8000) == 0)
                {
                    throw new ArgumentException("Expected EGF color bitmap, but probably got EGL monochrome bitmap");
                }

                this.Width = x & 0x7FFF;
                this.Height = y & 0x7FFF;
                this.data = reader.ReadBytes(this.Width * this.Height * 2);
            }
            finally
            {
                if (closeReader)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the color of the pixel at the given position.
        /// </summary>
        /// <param name="x">
        /// The horizontal position in the bitmap (0 ... <see cref="BitmapBase.Width"/> - 1).
        /// </param>
        /// <param name="y">
        /// The vertical position in the bitmap (0 ... <see cref="BitmapBase.Height"/> - 1).
        /// </param>
        /// <returns>
        /// The <see cref="IColor"/> at the given position.
        /// </returns>
        public override IColor GetPixel(int x, int y)
        {
            var pos = (y * 2 * this.Width) + (x * 2);
            return new TwoByteColor(this.data[pos], this.data[pos + 1]);
        }
    }
}