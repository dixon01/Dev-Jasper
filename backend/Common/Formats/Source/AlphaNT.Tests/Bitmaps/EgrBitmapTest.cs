// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EgrBitmapTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EgrBitmapTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Tests.Bitmaps
{
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Common;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="EgrBitmap"/>.
    /// </summary>
    [TestClass]
    public class EgrBitmapTest
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [TestMethod]
        [DeploymentItem("Bitmaps\\DB.EGR")]
        public void TestConstructor()
        {
            var target = new EgrBitmap("DB.EGR");
            Assert.AreEqual(23, target.Width);
            Assert.AreEqual(16, target.Height);

            TestLine(target, 0, " XXXXXXXXXXXXXXXXXXXXX ");
            TestLine(target, 1, "XXXXXXXXXXXXXXXXXXXXXXX");
            TestLine(target, 2, "XX                   XX");
            TestLine(target, 3, "XX XXXXXXXXXXXXXXXXX XX");
            TestLine(target, 4, "XX X      XX      XX XX");
            TestLine(target, 5, "XX XX  X   XX  XX  X XX");
            TestLine(target, 6, "XX XX  XX  XX  XX  X XX");
            TestLine(target, 7, "XX XX  XX  XX     XX XX");
            TestLine(target, 8, "XX XX  XX  XX     XX XX");
            TestLine(target, 9, "XX XX  XX  XX  XX  X XX");
            TestLine(target, 10, "XX XX  X   XX  XX  X XX");
            TestLine(target, 11, "XX X      XX      XX XX");
            TestLine(target, 12, "XX XXXXXXXXXXXXXXXXX XX");
            TestLine(target, 13, "XX                   XX");
            TestLine(target, 14, "XXXXXXXXXXXXXXXXXXXXXXX");
            TestLine(target, 15, " XXXXXXXXXXXXXXXXXXXXX ");
        }

        private static void TestLine(EgrBitmap target, int y, string expectation)
        {
            Assert.AreEqual(target.Width, expectation.Length);
            Assert.IsTrue(target.Height > y);

            for (int x = 0; x < expectation.Length; x++)
            {
                var expectedPixel = expectation[x] == ' ' ? Colors.Black : Colors.White;
                Assert.AreEqual(expectedPixel, target.GetPixel(x, y));
            }
        }
    }
}
