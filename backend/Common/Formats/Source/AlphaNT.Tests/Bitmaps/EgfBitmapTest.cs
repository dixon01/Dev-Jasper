// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EgfBitmapTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EgfBitmapTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Tests.Bitmaps
{
    using System.Drawing;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="EgfBitmap"/>.
    /// </summary>
    [TestClass]
    public class EgfBitmapTest
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Bitmaps\\regenbogen3.bmp")]
        [DeploymentItem("Bitmaps\\REGENBOGEN3.EGF")]
        public void TestConstructor()
        {
            var bitmap = new Bitmap("regenbogen3.bmp");
            var target = new EgfBitmap("REGENBOGEN3.EGF");
            Assert.AreEqual(bitmap.Width, target.Width);
            Assert.AreEqual(bitmap.Height, target.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var bmpPixel = bitmap.GetPixel(x, y);
                    var egfPixel = target.GetPixel(x, y);
                    Assert.AreEqual(Get3BitColorValue(bmpPixel.R), egfPixel.R);
                    Assert.AreEqual(Get3BitColorValue(bmpPixel.G), egfPixel.G);
                    Assert.AreEqual(Get3BitColorValue(bmpPixel.B), egfPixel.B);
                }
            }
        }

        private static byte Get3BitColorValue(byte value)
        {
            return (byte)(((value * 224 / 256) + 16) / 32 * 32);
        }
    }
}