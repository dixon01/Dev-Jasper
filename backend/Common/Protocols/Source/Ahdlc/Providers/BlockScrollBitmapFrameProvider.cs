// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockScrollBitmapFrameProvider.cs" company="Gorba AG">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Source;

    /// <summary>
    /// Provider for block scroll bitmap frames (mode 0x04).
    /// </summary>
    public class BlockScrollBitmapFrameProvider : MonochromeBitmapFrameProviderBase
    {
        private readonly IMonochromePixelSource background;

        private readonly ScrollBlockList scrollBlocks;

        private readonly int intBackgroundBlocks = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockScrollBitmapFrameProvider"/> class.
        /// </summary>
        /// <param name="background">
        /// The background.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public BlockScrollBitmapFrameProvider(IMonochromePixelSource background, int width, int height)
            : this(background, width, height, Brightness.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockScrollBitmapFrameProvider"/> class.
        /// </summary>
        /// <param name="background">
        /// The pixel source.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="brightness">
        /// The brightness.
        /// </param>
        public BlockScrollBitmapFrameProvider(
            IMonochromePixelSource background, int width, int height, Brightness brightness)
        {
            if (width <= 0 || width > 0xFF)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height <= 0 || height > 0xFF)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            this.scrollBlocks = new ScrollBlockList(height);

            this.background = background;
            this.intBackgroundBlocks = this.CalculateBlockCount(background.Width);
            var setup = new SetupCommandFrame(DisplayMode.BlockScrollBitmap)
            { DataBlockCount = this.intBackgroundBlocks };
            setup.Data[5] = (byte)width; // D5 = width
            setup.Data[6] = (byte)height; // D6 = height
            setup.Data[7] = (byte)brightness; // D7 = brightness
            this.SetupCommand = setup;
        }

        /// <summary>
        /// Adds a scroll block to this provider.
        /// </summary>
        /// <param name="viewport">
        /// The viewport of the scroll block (the area on the screen that will be replaced by this scroll block).
        /// </param>
        /// <param name="scrollSpeed">
        /// The scroll speed to be used.
        /// </param>
        /// <param name="source">
        /// The pixel source for the scroll block.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If you try to add more than two scroll blocks.
        /// </exception>
        public void AddScrollBlock(Rectangle viewport, ScrollSpeed scrollSpeed, IMonochromePixelSource source)
        {
            if (this.scrollBlocks.Count >= 2)
            {
                throw new NotSupportedException("Can't add more than two scroll blocks");
            }

            if (this.background.Height != source.Height)
            {
                throw new ArgumentException("Scroll block needs to have the same height as the background");
            }

            if (viewport.Left < 0 || viewport.Left > 0xFF
                || viewport.Right < 0 || viewport.Right > 0xFF
                || viewport.Top < 0 || viewport.Top > 0xFF
                || viewport.Bottom < 0 || viewport.Bottom > 0xFF)
            {
                throw new ArgumentOutOfRangeException("viewport");
            }

            this.scrollBlocks.Add(source);

            var scrollBlock = new byte[6];
            scrollBlock[0] = (byte)viewport.Left;
            scrollBlock[1] = (byte)viewport.Top;
            scrollBlock[2] = (byte)viewport.Right;
            scrollBlock[3] = (byte)viewport.Bottom;
            LittleEndianConverter.SetInt16(scrollBlock, 4, (short)source.Width);
            if (this.scrollBlocks.Count == 2)
            {
                {
                    var lockCountValueBlock1 = this.SetupCommand.DataBlockCount - this.intBackgroundBlocks;
                    if (lockCountValueBlock1 < this.CalculateBlockCount(source.Width))
                    {
                        this.SetupCommand.DataBlockCount = this.intBackgroundBlocks;
                        this.SetupCommand.DataBlockCount += this.CalculateBlockCount(source.Width);
                    }
                }
            }
            else
            {
                this.SetupCommand.DataBlockCount += this.CalculateBlockCount(source.Width);
            }

            this.SetupCommand.ScrollBlocks.Add(new SetupCommandFrame.ScrollBlockInfo(scrollBlock, 0, 6));
            this.SetupCommand.Data[8] = (byte)this.scrollBlocks.Count;
        }

        /// <summary>
        /// Gets the output commands to be sent after the setup command.
        /// </summary>
        /// <returns>
        /// An enumeration over all output commands in the order they need to be sent.
        /// </returns>
        public override IEnumerable<OutputCommandFrame> GetOutputCommands()
        {
            if (this.scrollBlocks.Count == 0)
            {
                throw new NotSupportedException("At least one scroll block is required");
            }

            if (this.SetupCommand.DataBlockCount > 256)
            {
                throw new NotSupportedException(
                    "Number of output frames exceeds 256: " + this.SetupCommand.DataBlockCount);
            }

            byte blockNumber = 0;
            for (var x = 0; x < this.background.Width; x += 8)
            {
                yield return
                    new OutputCommandFrame { BlockNumber = blockNumber, Data = this.GetBitmapData(this.background, x) };
                blockNumber++;
            }

            for (var x = 0; x < this.scrollBlocks.Width; x += 8)
            {
                yield return
                    new OutputCommandFrame
                        {
                            BlockNumber = blockNumber,
                            Data = this.GetBitmapData(this.scrollBlocks, x)
                        };
                blockNumber++;
            }
        }

        private class ScrollBlockList : List<IMonochromePixelSource>, IMonochromePixelSource
        {
            public ScrollBlockList(int height)
            {
                this.Height = height;
            }

            public int Width
            {
                get
                {
                    var width = 0;
                    foreach (var scrollBlock in this)
                    {
                        width = Math.Max(width, scrollBlock.Width);
                    }

                    return width;
                }
            }

            public int Height { get; private set; }

            public bool GetPixel(int x, int y)
            {
                foreach (var scrollBlock in this)
                {
                    if (x < scrollBlock.Width && scrollBlock.GetPixel(x, y))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
