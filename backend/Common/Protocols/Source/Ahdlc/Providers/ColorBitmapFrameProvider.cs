// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorBitmapFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ColorBitmapFrameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// Provider for color bitmaps (there is no mode identifier for this frame format).
    /// </summary>
    public class ColorBitmapFrameProvider : FrameProviderBase
    {
        private readonly IColorPixelSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBitmapFrameProvider"/> class.
        /// </summary>
        /// <param name="colorDepth">
        /// The color depth (usually 0).
        /// </param>
        /// <param name="source">
        /// The pixel source.
        /// </param>
        public ColorBitmapFrameProvider(int colorDepth, IColorPixelSource source)
        {
            if (source.Width > 0xFFFF)
            {
                throw new ArgumentOutOfRangeException("source", "Width > 0xFFFF");
            }

            if (source.Height > 0xFF)
            {
                // the header would support up to 0xFFFF pixels height, but the frame numbering only allows 0xFF
                throw new ArgumentOutOfRangeException("source", "Height > 0xFF");
            }

            if (colorDepth > 0xFF)
            {
                throw new ArgumentOutOfRangeException("colorDepth");
            }

            this.source = source;
            var setup = new SetupCommandFrame(DisplayMode.Color);
            setup.Data[1] = (byte)(source.Width >> 8); // D1,D2 = width
            setup.Data[2] = (byte)(source.Width & 0xFF);
            setup.Data[3] = (byte)(source.Height >> 8); // D3,D4 = height
            setup.Data[4] = (byte)(source.Height & 0xFF);
            setup.Data[5] = (byte)colorDepth; // D5 = color depth
            setup.Data[6] = 16; // D6 = colors per picture (default: 16)
            this.SetupCommand = setup;
        }

        /// <summary>
        /// Gets the output commands to be sent after the setup command.
        /// </summary>
        /// <returns>
        /// An enumeration over all output commands in the order they need to be sent.
        /// </returns>
        public override IEnumerable<OutputCommandFrame> GetOutputCommands()
        {
            // the order is reversed since the frames are transmitted bottom to top
            for (int y = this.source.Height - 1; y >= 0; y--)
            {
                yield return new OutputCommandFrame { BlockNumber = y, Data = this.GetBitmapData(y) };
            }
        }

        private byte[] GetBitmapData(int y)
        {
            var data = new byte[this.source.Width * 2];
            var offset = 0;
            for (int x = this.source.Width - 1; x >= 0; x--)
            {
                // we write the pixel values from right to left
                var pixel = this.source.GetPixel(x, y);

                // by masking the 4 least significant bits of each color component
                // we convert the 24 bit color to 12 bit color (3 x 4 bit)
                data[offset++] = (byte)(pixel.B >> 4);
                data[offset++] = (byte)((pixel.G & 0xF0) | (pixel.R >> 4));
            }

            return data;
        }
    }
}
