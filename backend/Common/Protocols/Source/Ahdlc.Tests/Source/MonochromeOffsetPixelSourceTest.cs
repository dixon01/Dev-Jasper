// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonochromeOffsetPixelSourceTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MonochromeOffsetPixelSourceTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Tests.Source
{
    using Gorba.Common.Protocols.Ahdlc.Source;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test for the <see cref="MonochromeOffsetPixelSource"/>.
    /// </summary>
    [TestClass]
    public class MonochromeOffsetPixelSourceTest
    {
        /// <summary>
        /// The constructor test.
        /// </summary>
        [TestMethod]
        public void ConstructorTest()
        {
            var target = new MonochromeOffsetPixelSource(200, 100, 15, 25, new TestPixelSource(30, 30));
            Assert.AreEqual(200, target.Width);
            Assert.AreEqual(100, target.Height);
        }

        /// <summary>
        /// Tests if the offset is calculated correctly.
        /// </summary>
        [TestMethod]
        public void GetPixelTest()
        {
            var target = new MonochromeOffsetPixelSource(200, 100, 15, 25, new TestPixelSource(30, 40));

            Assert.IsTrue(target.GetPixel(15, 25));
            Assert.IsTrue(target.GetPixel(17, 25));
            Assert.IsTrue(target.GetPixel(15, 27));
            Assert.IsTrue(target.GetPixel(17, 27));

            for (int x = 0; x < 200; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    Assert.IsFalse(target.GetPixel(x, y));
                }
            }

            for (int x = 0; x < 15; x++)
            {
                for (int y = 25; y < 100; y++)
                {
                    Assert.IsFalse(target.GetPixel(x, y));
                }
            }

            for (int x = 45; x < 200; x++)
            {
                for (int y = 25; y < 100; y++)
                {
                    Assert.IsFalse(target.GetPixel(x, y));
                }
            }
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
