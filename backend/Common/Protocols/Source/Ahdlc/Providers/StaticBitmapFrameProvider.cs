// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticBitmapFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StaticBitmapFrameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// Provider for static bitmap frames (mode 0x00).
    /// </summary>
    public class StaticBitmapFrameProvider : MonochromeBitmapFrameProviderBase
    {
        private readonly IMonochromePixelSource pixelSource;

        private readonly int blockCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticBitmapFrameProvider"/> class.
        /// </summary>
        /// <param name="pixelSource">
        /// The pixel source.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public StaticBitmapFrameProvider(IMonochromePixelSource pixelSource, int height)
            : this(pixelSource, height, Brightness.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticBitmapFrameProvider"/> class.
        /// </summary>
        /// <param name="pixelSource">
        /// The pixel source.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="brightness">
        /// The brightness.
        /// </param>
        public StaticBitmapFrameProvider(IMonochromePixelSource pixelSource, int height, Brightness brightness)
        {
            if (height <= 0 || height > 0xFF)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            this.pixelSource = pixelSource;
            this.blockCount = this.CalculateBlockCount(pixelSource.Width);
            var setup = new SetupCommandFrame(DisplayMode.StaticBitmap) { DataBlockCount = this.blockCount };
            setup.Data[6] = (byte)height; // D6 = height
            setup.Data[7] = (byte)brightness; // D7 = brightness
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
            for (byte i = 0; i < this.blockCount; i++)
            {
                yield return
                    new OutputCommandFrame { BlockNumber = i, Data = this.GetBitmapData(this.pixelSource, i * 8) };
            }
        }
    }
}
