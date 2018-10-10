// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EglBitmap.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EglBitmap type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Bitmaps
{
    using System;
    using System.IO;

    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// EGL bitmap class for reading large size monochrome bitmaps.
    /// </summary>
    public class EglBitmap : BitmapBase
    {
        private readonly int bytesPerRow;

        private readonly IByteAccess data;

        /// <summary>
        /// Initializes a new instance of the <see cref="EglBitmap"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename from which to read the bitmap.
        /// </param>
        public EglBitmap(string filename)
            : this(new BinaryFileReader(File.OpenRead(filename), true) { ExtendedFormat = true }, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EglBitmap"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader from which to read the bitmap.
        /// </param>
        /// <param name="closeReader">
        /// Flag indicating whether the <see cref="reader"/> should be closed
        /// before this method returns or throws an exception.
        /// </param>
        internal EglBitmap(BinaryFileReader reader, bool closeReader)
        {
            if (!reader.ExtendedFormat)
            {
                throw new NotSupportedException("Trying to read an EGL bitmap from a non-extended format reader");
            }

            try
            {
                this.Width = reader.ReadLsbUInt16();
                this.Height = reader.ReadLsbUInt16();
                if (this.Width >= 0x8000 || this.Height >= 0x8000)
                {
                    throw new ArgumentException("Expected EGL monochrome bitmap, but probably got EGF color bitmap");
                }

                this.bytesPerRow = (this.Width + 7) / 8;
                this.data = reader.ReadBytes(this.bytesPerRow * this.Height);
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
        /// The returned value will always either be <see cref="Colors.Black"/> or <see cref="Colors.White"/>.
        /// </returns>
        public override IColor GetPixel(int x, int y)
        {
            var pos = (y * this.bytesPerRow) + (x / 8);
            return (this.data[pos] & (1 << (7 - (x % 8)))) == 0 ? Colors.Black : Colors.White;
        }
    }
}