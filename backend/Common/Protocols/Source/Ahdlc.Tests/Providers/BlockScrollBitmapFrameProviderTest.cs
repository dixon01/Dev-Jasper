// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockScrollBitmapFrameProviderTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlockScrollBitmapFrameProviderTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Tests.Providers
{
    using System.Drawing;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Protocols.Ahdlc.Source;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for <see cref="BlockScrollBitmapFrameProvider"/>.
    /// </summary>
    [TestClass]
    public class BlockScrollBitmapFrameProviderTest
    {
        /// <summary>
        /// Tests the provider with one scroll block.
        /// </summary>
        [TestMethod]
        public void TestOneScrollBlock()
        {
            var background = new TestPixelSource(112, 16);
            var target = new BlockScrollBitmapFrameProvider(background, 112, 16, Brightness.Brightness1);
            target.AddScrollBlock(new Rectangle(27, 0, 86, 16), ScrollSpeed.Fast, new TestPixelSource(283, 16));

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);
            Assert.AreEqual(DisplayMode.BlockScrollBitmap, setup.Mode);
            Assert.AreEqual(50, setup.DataBlockCount);

            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(0, setup.Data[3]);
            Assert.AreEqual(0, setup.Data[4]);
            Assert.AreEqual(112, setup.Data[5]);
            Assert.AreEqual(16, setup.Data[6]);
            Assert.AreEqual(10, setup.Data[7]); // brightness
            Assert.AreEqual(1, setup.Data[8]);  // scroll block count

            Assert.AreEqual(1, setup.ScrollBlocks.Count);

            var scrollBlock = setup.ScrollBlocks[0];
            Assert.AreEqual(6, scrollBlock.Data.Length);
            Assert.AreEqual(27, scrollBlock.Data[0]);
            Assert.AreEqual(0, scrollBlock.Data[1]);
            Assert.AreEqual(113, scrollBlock.Data[2]);
            Assert.AreEqual(16, scrollBlock.Data[3]);
            Assert.AreEqual(0x01, scrollBlock.Data[4]); // 0x011B = 283
            Assert.AreEqual(0x1B, scrollBlock.Data[5]);

            var count = 0;
            foreach (var outputCommand in target.GetOutputCommands())
            {
                Assert.AreEqual(count, outputCommand.BlockNumber);
                count++;
            }

            // expected 50 frames (14 for background, 36 for scroll block)
            Assert.AreEqual(50, count);
        }

        /// <summary>
        /// Tests the provider with two different scroll blocks scroll block1 bigger scroll block2.
        /// </summary>
        [TestMethod]
        public void TestTwoScrollBlocksBlock1BiggerBlock2()
        {
            var background = new TestPixelSource(112, 16);
            var target = new BlockScrollBitmapFrameProvider(background, 112, 16, Brightness.Brightness2);
            target.AddScrollBlock(new Rectangle(27, 0, 86, 8), ScrollSpeed.Fast, new TestPixelSource(283, 16));
            target.AddScrollBlock(new Rectangle(15, 8, 91, 8), ScrollSpeed.Slow, new TestPixelSource(131, 16));

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);
            Assert.AreEqual(DisplayMode.BlockScrollBitmap, setup.Mode);
            Assert.AreEqual(50, setup.DataBlockCount);

            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(0, setup.Data[3]);
            Assert.AreEqual(0, setup.Data[4]);
            Assert.AreEqual(112, setup.Data[5]);
            Assert.AreEqual(16, setup.Data[6]);
            Assert.AreEqual(11, setup.Data[7]); // brightness
            Assert.AreEqual(2, setup.Data[8]);  // scroll block count

            Assert.AreEqual(2, setup.ScrollBlocks.Count);

            var scrollBlock = setup.ScrollBlocks[0];
            Assert.AreEqual(6, scrollBlock.Data.Length);
            Assert.AreEqual(27, scrollBlock.Data[0]);
            Assert.AreEqual(0, scrollBlock.Data[1]);
            Assert.AreEqual(113, scrollBlock.Data[2]);
            Assert.AreEqual(8, scrollBlock.Data[3]);
            Assert.AreEqual(0x01, scrollBlock.Data[4]); // 0x011B = 283
            Assert.AreEqual(0x1B, scrollBlock.Data[5]);

            scrollBlock = setup.ScrollBlocks[1];
            Assert.AreEqual(6, scrollBlock.Data.Length);
            Assert.AreEqual(15, scrollBlock.Data[0]);
            Assert.AreEqual(8, scrollBlock.Data[1]);
            Assert.AreEqual(106, scrollBlock.Data[2]);
            Assert.AreEqual(16, scrollBlock.Data[3]);
            Assert.AreEqual(0x00, scrollBlock.Data[4]); // 0x0083 = 131
            Assert.AreEqual(0x83, scrollBlock.Data[5]);

            var count = 0;
            foreach (var outputCommand in target.GetOutputCommands())
            {
                Assert.AreEqual(count, outputCommand.BlockNumber);
                count++;
            }

            // expected 50 frames:
            // 14 for background, 36 for scroll block 1 (since 36 is greater than 17 for scroll block 2)
            Assert.AreEqual(50, count);
        }

        /// <summary>
        /// Tests the provider with two different scroll blocks scroll block1 smaller scroll block2.
        /// </summary>
        [TestMethod]
        public void TestTwoScrollBlocksBlock1SmallerBlock2()
        {
            var background = new TestPixelSource(112, 16);
            var target = new BlockScrollBitmapFrameProvider(background, 112, 16, Brightness.Brightness2);
            target.AddScrollBlock(new Rectangle(27, 0, 86, 8), ScrollSpeed.Fast, new TestPixelSource(131, 16));
            target.AddScrollBlock(new Rectangle(15, 8, 91, 8), ScrollSpeed.Slow, new TestPixelSource(283, 16));

            var setup = target.SetupCommand;
            Assert.IsNotNull(setup);
            Assert.AreEqual(DisplayMode.BlockScrollBitmap, setup.Mode);
            Assert.AreEqual(50, setup.DataBlockCount);

            Assert.AreEqual(6, setup.Data.Length);
            Assert.AreEqual(0, setup.Data[3]);
            Assert.AreEqual(0, setup.Data[4]);
            Assert.AreEqual(112, setup.Data[5]);
            Assert.AreEqual(16, setup.Data[6]);
            Assert.AreEqual(11, setup.Data[7]); // brightness
            Assert.AreEqual(2, setup.Data[8]);  // scroll block count

            Assert.AreEqual(2, setup.ScrollBlocks.Count);

            var scrollBlock = setup.ScrollBlocks[0];
            Assert.AreEqual(6, scrollBlock.Data.Length);
            Assert.AreEqual(27, scrollBlock.Data[0]);
            Assert.AreEqual(0, scrollBlock.Data[1]);
            Assert.AreEqual(113, scrollBlock.Data[2]);
            Assert.AreEqual(8, scrollBlock.Data[3]);
            Assert.AreEqual(0x00, scrollBlock.Data[4]); // 0x0083 = 131
            Assert.AreEqual(0x83, scrollBlock.Data[5]);

            scrollBlock = setup.ScrollBlocks[1];
            Assert.AreEqual(6, scrollBlock.Data.Length);
            Assert.AreEqual(15, scrollBlock.Data[0]);
            Assert.AreEqual(8, scrollBlock.Data[1]);
            Assert.AreEqual(106, scrollBlock.Data[2]);
            Assert.AreEqual(16, scrollBlock.Data[3]);
            Assert.AreEqual(0x01, scrollBlock.Data[4]); // 0x011B = 283
            Assert.AreEqual(0x1B, scrollBlock.Data[5]);

            var count = 0;
            foreach (var outputCommand in target.GetOutputCommands())
            {
                Assert.AreEqual(count, outputCommand.BlockNumber);
                count++;
            }

            // expected 50 frames:
            // 14 for background, 36 for scroll block 2 (since 36 is greater than 17 for scroll block 1)
            Assert.AreEqual(50, count);
        }

        private class TestPixelSource : IMonochromePixelSource
        {
            public TestPixelSource(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public int Width { get; private set; }

            public int Height { get; private set; }

            public bool GetPixel(int x, int y)
            {
                return (x % 2) == (y % 2);
            }
        }
    }
}
