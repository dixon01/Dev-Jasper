// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageContentConversionFormat.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageContentConversionFormat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    /// <summary>
    /// Defines the format for an image conversion.
    /// </summary>
    public class ImageContentConversionFormat
    {
        /// <summary>
        /// The format for a time table.
        /// </summary>
        public static readonly ImageContentConversionFormat TimeTable = new ImageContentConversionFormat(1024, 1024);

        /// <summary>
        /// The format for the LCD display.
        /// </summary>
        public static readonly ImageContentConversionFormat Lcd = new ImageContentConversionFormat(120, 70);

        private ImageContentConversionFormat(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }
    }
}